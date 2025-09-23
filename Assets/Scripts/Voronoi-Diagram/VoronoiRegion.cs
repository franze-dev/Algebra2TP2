using CustomMath;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Face
{
    public List<Vec3> vertices;

    public Face(List<Vec3> vertices)
    {
        this.vertices = vertices;
    }
}

public class VoronoiRegion
{
    private Vec3 _site;
    private List<CustomPlane> _borders;
    private Bounds _bounds;

    private List<Vec3> _vertices;
    private List<Face> _faces;

    public Vec3 Site => _site;

    public List<Face> Faces => _faces;

    public VoronoiRegion(Bounds bounds, Vec3 site)
    {
        _borders = new List<CustomPlane>();

        _bounds = bounds;

        _site = site;

        InitVertices();
        InitFaces();
    }

    private void InitFaces()
    {
        _faces = new List<Face>();

        List<Vec3> topFace = new List<Vec3>();
        List<Vec3> bottomFace = new List<Vec3>();
        List<Vec3> leftFace = new List<Vec3>();
        List<Vec3> rightFace = new List<Vec3>();
        List<Vec3> frontFace = new List<Vec3>();
        List<Vec3> backFace = new List<Vec3>();

        bottomFace.Add(_vertices[0]);
        bottomFace.Add(_vertices[1]);
        bottomFace.Add(_vertices[3]);
        bottomFace.Add(_vertices[2]);

        topFace.Add(_vertices[4]);
        topFace.Add(_vertices[5]);
        topFace.Add(_vertices[7]);
        topFace.Add(_vertices[6]);

        leftFace.Add(_vertices[0]);
        leftFace.Add(_vertices[2]);
        leftFace.Add(_vertices[6]);
        leftFace.Add(_vertices[4]);

        rightFace.Add(_vertices[1]);
        rightFace.Add(_vertices[3]);
        rightFace.Add(_vertices[7]);
        rightFace.Add(_vertices[5]);

        frontFace.Add(_vertices[0]);
        frontFace.Add(_vertices[1]);
        frontFace.Add(_vertices[5]);
        frontFace.Add(_vertices[4]);

        backFace.Add(_vertices[2]);
        backFace.Add(_vertices[3]);
        backFace.Add(_vertices[7]);
        backFace.Add(_vertices[6]);

        _faces.Add(new(bottomFace));
        _faces.Add(new(topFace));
        _faces.Add(new(backFace));
        _faces.Add(new(frontFace));
        _faces.Add(new(rightFace));
        _faces.Add(new(leftFace));
    }

    private void InitVertices()
    {
        _vertices = new List<Vec3>();

        _vertices.Add(new(_bounds.min));
        _vertices.Add(new(_bounds.max.x, _bounds.min.y, _bounds.min.z));
        _vertices.Add(new(_bounds.min.x, _bounds.max.y, _bounds.min.z));
        _vertices.Add(new(_bounds.max.x, _bounds.max.y, _bounds.min.z));
        _vertices.Add(new(_bounds.min.x, _bounds.min.y, _bounds.max.z));
        _vertices.Add(new(_bounds.max.x, _bounds.min.y, _bounds.max.z));
        _vertices.Add(new(_bounds.min.x, _bounds.max.y, _bounds.max.z));
        _vertices.Add(new(_bounds.max));

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
        bool anyIn = false;
        bool anyOut = false;

        foreach (var v in _vertices)
        {
            if (plane.GetSide(v))
                anyIn = true;
            else
                anyOut = true;

            if (anyIn && anyOut)
                return true;
        }

        return false;

        //return Intersect(plane);
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
        {
            _borders.Add(border);
            ClipRegion(border);
        }
        else
            Debug.LogWarning("Cannot add border " + border + ". It does not intersect with the current region");
    }

    public void AddBorders(List<CustomPlane> borders)
    {
        foreach (var border in borders)
            AddBorder(border);
    }

    private List<Vec3> SortPoly(List<Vec3> poly, Vec3 normal)
    {
        Vec3 centroid = Vec3.zero;

        foreach (var point in poly)
            centroid += point;

        centroid /= poly.Count;

        Vec3 aX = Vec3.Cross(normal, Vec3.up);
        if (aX.sqrMagnitude < Mathf.Epsilon)
            aX = Vec3.Cross(normal, Vec3.right);

        aX.Normalize();

        Vec3 aY = Vec3.Cross(normal, Vec3.Cross(normal, aX));

        poly.Sort((a, b) =>
        {
            Vec3 dA = a - centroid;
            Vec3 dB = b - centroid;
            float angleA = Mathf.Atan2(Vec3.Dot(dA, aY), Vec3.Dot(dA, aX));
            float angleB = Mathf.Atan2(Vec3.Dot(dB, aY), Vec3.Dot(dB, aX));

            return angleA.CompareTo(angleB);
        });


        return poly;
    }

    private void ClipRegion(CustomPlane plane)
    {
        var newFaces = new List<Face>();
        var cap = new List<Vec3>();

        foreach (var face in _faces)
        {
            var clippedVerts = CollectIntersections(face.vertices, plane);

            if (clippedVerts != null && clippedVerts.Count >= 3)
            {
                var normal = Vec3.Cross(clippedVerts[1] - clippedVerts[0], clippedVerts[2] - clippedVerts[0]).normalized;
                var sorted = SortPoly(clippedVerts, normal);

                newFaces.Add(new(sorted));
            }

            var planeIntersects = GetPlaneIntersect(face.vertices, plane);
            if (planeIntersects != null && planeIntersects.Count > 0)
                cap.AddRange(planeIntersects);
        }

        _faces = newFaces;

        if (cap.Count > 0)
        {
            var distinct = cap.Distinct().ToList();

            for (var i = 0; i < distinct.Count; i++)
            {
                var p = distinct[i];
                float signedDist = Vec3.Dot(plane.normal, p) - plane.distance;

                distinct[i] = p - plane.normal * signedDist;
            }

            var sorted = SortPoly(distinct, plane.normal);

            _faces.Add(new(sorted));

            _vertices = new(sorted);
        }
    }

    private List<Vec3> CollectIntersections(List<Vec3> vertices, CustomPlane plane)
    {
        var res = new List<Vec3>();

        for (int i = 0; i < vertices.Count; i++)
        {
            var current = vertices[i];
            var next = vertices[(i + 1) % vertices.Count];

            bool currentInside = plane.GetSide(current);
            bool nextInside = plane.GetSide(next);

            if (currentInside)
                res.Add(current);

            if (currentInside != nextInside)
            {
                var intersection = IntersectEdgePlane(current, next, plane);
                res.Add(intersection);
            }
        }

        return res;
    }

    private List<Vec3> GetPlaneIntersect(List<Vec3> vertices, CustomPlane plane)
    {
        var res = new List<Vec3>();

        for (int i = 0; i < vertices.Count; i++)
        {
            var current = vertices[i];
            var next = vertices[(i + 1) % vertices.Count];

            bool currentOnPlane = Mathf.Abs(Vec3.Dot(plane.normal, current) - plane.distance) <= Mathf.Epsilon;

            if (currentOnPlane)
                res.Add(current);

            bool currentInside = plane.GetSide(current);
            bool nextInside = plane.GetSide(next);

            if (currentInside != nextInside)
            {
                var intersection = IntersectEdgePlane(current, next, plane);
                res.Add(intersection);
            }
        }

        return res;
    }

    private Face ClipFace(Face face, CustomPlane plane)
    {
        var vertices = face.vertices;

        var res = CollectIntersections(vertices, plane);

        if (res.Count >= 3)
        {
            res = SortPoly(res, plane.normal);
            return new Face(res);
        }

        return null;
    }

    private Vec3 IntersectEdgePlane(Vec3 a, Vec3 b, CustomPlane plane)
    {
        var dA = Vec3.Dot(plane.normal, a) - plane.distance;
        var dB = Vec3.Dot(plane.normal, b) - plane.distance;

        float denom = dA - dB;

        float t;

        if (Mathf.Abs(denom) < Mathf.Epsilon)
            // almost parallel
            t = 0.5f;
        else
            t = dA / denom;

        t = Mathf.Clamp01(t);

        return a + t * (b - a);
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

    public override string ToString()
    {
        string res = "";

        res += "Site: " + Site;
        res += " Borders amount: " + _borders.Count;

        return res;
    }
}
