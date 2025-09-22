using CustomMath;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiRegion
{
    private Vec3 _site;
    private List<CustomPlane> _borders;
    private Bounds _bounds;

    public Vec3 Site => _site;

    public VoronoiRegion(Bounds bounds, Vec3 site)
    {
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

    public bool ShouldAdd(CustomPlane plane)
    {
        return Intersect(plane);
    }

    /// <summary>
    /// https://en.wikipedia.org/wiki/Plane%E2%80%93plane_intersection
    /// </summary>
    /// <param name="plane"></param>
    /// <returns></returns>
    private bool Intersect(CustomPlane plane)
    {
        if (!plane.GetSide(_site))
        {
            Debug.LogWarning("Tried to add a plane to the region of " + _site + " but the plane is not facing it.");
            return false;
        }

        if (_borders.Count == 0)
            return true;

        foreach (var border in _borders)
        {
            var dot = Vec3.Dot(plane.normal, border.normal);

            // Are planes parallel?
            // If they are coincident (same plane, so infinite intersections) I also don't want to count it,
            // for the sake of voronoi's logic.
            if (Mathf.Abs(dot) > 0.999f)
                continue;

            //Not parallel, they intersect.
            return true;
        }

        return false;
    }

    public void AddBorder(CustomPlane border)
    {
        if (ShouldAdd(border))
            _borders.Add(border);
        else
            Debug.LogWarning("Cannot add border " + border + ". It does not intersect with the current region");
    }

    public void AddBorders(List<CustomPlane> borders)
    {
        foreach (var border in borders)
            AddBorder(border);
    }

    public bool IsPointInRegion(Vec3 point)
    {
        foreach (var border in _borders)
        {
            if (!border.GetSide(point))
                return false;
        }
        return true;
    }
}
