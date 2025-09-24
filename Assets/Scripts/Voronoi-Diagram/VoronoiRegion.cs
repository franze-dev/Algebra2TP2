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

public class Line
{
    public Vec3 point;
    public Vec3 dir;

    public Line(Vec3 point, Vec3 dir)
    {
        this.point = point;
        this.dir = dir;
    }
}

public class Segment
{
    public Vec3 start;
    public Vec3 end;

    public Segment(Vec3 start, Vec3 end)
    {
        this.start = start;
        this.end = end;
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

    public List<Vec3> Vertices => _vertices;

    public List<CustomPlane> Borders => _borders;

    public Color Color;

    public VoronoiRegion(Bounds bounds, Vec3 site)
    {
        Color = new(Random.value, Random.value, Random.value);

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
        _vertices = new List<Vec3>
        {
            new(_bounds.min),
            new(_bounds.max.x, _bounds.min.y, _bounds.min.z),
            new(_bounds.min.x, _bounds.max.y, _bounds.min.z),
            new(_bounds.max.x, _bounds.max.y, _bounds.min.z),
            new(_bounds.min.x, _bounds.min.y, _bounds.max.z),
            new(_bounds.max.x, _bounds.min.y, _bounds.max.z),
            new(_bounds.min.x, _bounds.max.y, _bounds.max.z),
            new(_bounds.max)
        };

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
        if (Intersect(plane, out var myBorder))
        {
            //Check if they intersect in the bounds
            if (myBorder != null)
            {
                var intersect = GetIntersection(plane, myBorder);
                if (intersect != null)
                {
                    _vertices.Add(intersect.start);
                    _vertices.Add(intersect.end);
                    return true;
                }
                return false;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// https://discussions.unity.com/t/how-to-find-line-of-intersecting-planes/459131/2
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private Segment GetIntersection(CustomPlane a, CustomPlane b)
    {
        //Gets the line of intersection between a and b, checks if it intersects with the other borders twice.
        //If that's the case, a segment is made with those two intersections, and that's what's returned.

        Line line;

        var linePoint = Vec3.zero;
        var lineDir = Vec3.zero;

        lineDir = Vec3.Cross(a.normal, b.normal);

        var pointDir = Vec3.Cross(b.normal, lineDir);

        float numerator = Vec3.Dot(a.normal, pointDir);

        if (Mathf.Abs(numerator) > Mathf.Epsilon)
        {
            Vec3 dist = (a.normal * a.distance) - (b.normal * b.distance);
            float t = Vec3.Dot(a.normal, dist) / numerator;

            linePoint = (b.normal * b.distance) + t * pointDir;

            line = new(linePoint, lineDir);
        }
        else
            return null;

        //check if that linepoint is in the main box

        if (!_bounds.Contains(linePoint))
            return null;

        // Make the segment
        var boundsExtents = new Vec3(_bounds.extents);
        var boundsSize = new Vec3(_bounds.size);

        // Shoot a ray from under the bounds to detect colision
        Ray ray = new();

        var testDir = line.dir;

        if (line.dir.x < 0 ||
            line.dir.y < 0 ||
            line.dir.z < 0)
            testDir *= -1;

        ray.origin = line.point - (-testDir * boundsExtents);
        ray.direction = line.dir * boundsSize;

        float enter = 0f;

        List<Vec3> points = new List<Vec3>();

        foreach (var border in _borders)
        {
            if (border == a || border == b)
                continue;

            if (border.Raycast(ray, out enter))
            {
                Vec3 intersect = new(ray.GetPoint(enter));

                points.Add(intersect);
            }

            if (points.Count == 2)
                break;
        }

        if (points.Count == 0)
        {
            Debug.LogWarning("Could not make segment even after making the line.");
            return null;
        }

        return new(points[0], points[1]);

    }

    /// <summary>
    /// https://en.wikipedia.org/wiki/Plane%E2%80%93plane_intersection
    /// </summary>
    /// <param name="plane"></param>
    /// <returns></returns>
    private bool Intersect(CustomPlane plane, out CustomPlane myBorder)
    {
        if (!plane.GetSide(_site))
        {
            myBorder = null;
            Debug.LogWarning("Tried to add a plane to the region of " + _site + " but the plane is not facing it.");
            return false;
        }

        if (_borders.Count == 0)
        {
            myBorder = null;
            return true;
        }

        foreach (var border in _borders)
        {
            var dot = Vec3.Dot(plane.normal, border.normal);

            // Are planes parallel?
            // If they are coincident (same plane, so infinite intersections) I also don't want to count it,
            // for the sake of voronoi's logic.
            if (Mathf.Abs(dot) > 0.999f)
                continue;

            //Not parallel, they intersect.
            myBorder = border;
            return true;
        }

        myBorder = null;
        return false;
    }

    public void AddBorder(CustomPlane border)
    {
        if (ShouldAdd(border))
        {
            _borders.Add(border);
            //ClipRegion(border);
        }
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

    public override string ToString()
    {
        string res = "";

        res += "Site: " + Site;
        res += " Borders amount: " + _borders.Count;

        foreach (var border in _borders)
        {
            res += border.ToString();
            res += border.normal.ToString();
            res += border.distance.ToString();
        }

        return res;
    }
}
