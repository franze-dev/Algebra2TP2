using CustomMath;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiRegion
{
    private Vec3 _site;
    private List<CustomPlane> _borders;
    private Bounds _bounds;

    public Vec3 Site => _site;
    public List<CustomPlane> Borders => _borders;

    public Color Color;

    public VoronoiRegion(Bounds bounds, Vec3 site)
    {
        Color = new(Random.value, Random.value, Random.value);

        _borders = new List<CustomPlane>();

        _bounds = bounds;

        _site = site;

    }

    public bool GetSide(Vec3 point)
    {
        if (_bounds.Contains((Vector3)point))
        {
            foreach (var border in _borders)
            {
                if (!border.GetSide(point))
                    return false;
            }

            return true;
        }
        return false;
    }

    public bool ShouldAdd(CustomPlane plane, Vec3 mid)
    {
        if (!plane.GetSide(_site))
        {
            Debug.LogWarning("Tried to add a plane to the region of " + _site + " but the plane is not facing it.");
            return false;
        }

        if (_borders.Count == 0)
        {
            return true;
        }

        if (!GetSide(mid))
            return false;

        return Intersect(plane);
    }

    /// <summary>
    /// https://en.wikipedia.org/wiki/Plane%E2%80%93plane_intersection
    /// </summary>
    /// <param name="plane"></param>
    /// <returns></returns>
    private bool Intersect(CustomPlane plane)
    {
        // Does the plane intersect with any of the borders of this region?
        foreach (var border in _borders)
        {
            var dot = Vec3.Dot(plane.normal, border.normal);

            // Are planes parallel? If they are, do not count it
            if (Mathf.Abs(dot) > 0.999f)
                continue;

            return true;
        }

        return false;
    }

    public void AddBorder(CustomPlane border, Vec3 mid)
    {
        if (ShouldAdd(border, mid))
            _borders.Add(border);
    }

    public override string ToString()
    {
        string res = "";

        res += "Site: " + Site;
        res += " Borders amount: " + _borders.Count;

        return res;
    }
}
