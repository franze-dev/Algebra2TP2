using CustomMath;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VoronoiRegion
{
    private Vec3 _site;
    private List<CustomPlane> _borders;
    private Bounds _bounds;

    public Vec3 Site => _site;
    public List<CustomPlane> Borders => _borders;

    public Color Color;
    public List<Vec3> _sortedPoints;

    public VoronoiRegion(Bounds bounds, Vec3 site, List<Vec3> points)
    {
        Color = new(Random.value, Random.value, Random.value);

        _borders = new List<CustomPlane>();

        _bounds = bounds;

        _site = site;
        this._sortedPoints = points;

        SortPoints();
    }

    private void SortPoints()
    {
        _sortedPoints.Sort((a, b) =>
        {
            float distA = Vec3.Distance(a, _site);
            float distB = Vec3.Distance(b, _site);
            return distA.CompareTo(distB);
        });
    }

    public bool GetSide(Vec3 point)
    {
        if (_bounds.Contains((Vector3)point))
        {
            foreach (var border in _borders)
            {
                if (border.GetSide(point) != border.GetSide(_site))
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
            return true;

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
            if (Mathf.Abs(dot) == 1)
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

    public bool BorderExists(CustomPlane bisector, Vec3 point)
    {
        foreach (var border in _borders)
        {
            var borderPoint = border.normal * border.distance;

            if (_borders.Contains(bisector) || Vec3.Distance(borderPoint, point) < Mathf.Epsilon)
                return true;
        }

        return false;
    }

    internal void Build()
    {
        foreach (var other in _sortedPoints)
        {
            if (_site == other)
                continue;

            var bisector = GetBisector(_site, other, out var mid);

            AddBorder(bisector, mid);
        }
    }

    private CustomPlane GetBisector(Vec3 site1, Vec3 site2, out Vec3 mid)
    {
        mid = (site1 + site2) * 0.5f;

        var normal = (site1 - site2).normalized;

        var plane = new CustomPlane(normal, mid);

        return plane;
    }
}
