using CustomMath;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PointGeneration : MonoBehaviour
{
    [SerializeField] private int _pointsAmount = 0;

    [SerializeField] private CustomTransform _maxT;
    [SerializeField] private CustomTransform _minT;

    private VoronoiDiagram _diagram;

    private List<Vec3> _points;
    private List<CustomPlane> _planesToDraw;

    private Vec3 _max => _maxT.position;
    private Vec3 _min => _minT.position;

    [SerializeField] private Vec3 _testPoint;
    [SerializeField] private Vec3 _closestSite;
    [SerializeField] private List<Vec3> _sites;

    private void Start()
    {
        _planesToDraw = new();

        _points = new List<Vec3>();

        _sites = new List<Vec3>();

        GeneratePoints();
        SortPoints();

        _diagram = new(_points, _min, _max);

        DebugRegions();

        CheckRegions();
    }

    private void CheckRegions()
    {
        if (_diagram == null)
            return;
        _sites.Clear();
        foreach (var region in _diagram.Regions)
        {
            if (region.GetSide(_testPoint))
                _sites.Add(region.Site);
        }

        if (_sites.Count == 1)
            _closestSite = _sites[0];
        else
        {
            float dist;
            float prevDist = float.MaxValue;
            for (int i = 0; i < _sites.Count; i++)
            {
                dist = Vec3.Distance(_testPoint, _sites[i]);

                if (dist < prevDist)
                {
                    _closestSite = _sites[i];
                    prevDist = dist;
                }
            }
        }
    }

    private void OnValidate()
    {
        CheckRegions();
    }

    private void DebugRegions()
    {
        foreach (var region in _diagram.Regions)
        {
            Debug.Log("REGION: " + region.ToString());
        }
    }

    private void OnDrawGizmos()
    {
        if (_diagram == null)
            return;

        Gizmos.color = Color.yellow;

        //Bounding box normals
        foreach (var plane in _diagram.BoundsPlanes)
        {
            Gizmos.DrawRay(plane.normal * plane.distance, plane.normal * 10);
        }

        foreach (var region in _diagram.Regions)
        {
            Gizmos.color = Color.blue;
            _planesToDraw.AddRange(region.Borders);

            Gizmos.color = Color.black;
            Gizmos.DrawSphere(region.Site, 2f);

        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_testPoint, 3f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(_closestSite, 3f);
    }

    /// <summary>
    /// https://discussions.unity.com/t/how-to-debug-drawing-plane/72450
    /// </summary>
    /// <param name="plane"></param>
    private void DebugPlane(CustomPlane plane)
    {
        Vec3 v3;

        var normal = plane.normal;
        var position = plane.normal * plane.distance;

        if (normal.normalized != Vec3.forward)
            v3 = Vec3.Cross(normal, Vec3.forward).normalized * normal.magnitude;
        else
            v3 = Vec3.Cross(normal, Vec3.up).normalized * normal.magnitude; ;

        var corner0 = position + v3;
        var corner2 = position - v3;

        var q = CustomQuaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        var corner1 = position + v3;
        var corner3 = position - v3;

        Gizmos.DrawLine(corner0, corner2);
        Gizmos.DrawLine(corner1, corner3);
        Gizmos.DrawLine(corner0, corner1);
        Gizmos.DrawLine(corner1, corner2);
        Gizmos.DrawLine(corner2, corner3);
        Gizmos.DrawLine(corner3, corner0);
        Gizmos.DrawRay(position, normal);
    }

    /// <summary>
    /// Sorts points from closest to furthest from the max point.
    /// </summary>
    private void SortPoints()
    {
        _points.Sort((a, b) =>
        {
            float distA = Vec3.Distance(a, _max);
            float distB = Vec3.Distance(b, _max);
            return distA.CompareTo(distB);
        });
    }

    private void GeneratePoints()
    {
        for (int i = 0; i < _pointsAmount; i++)
        {
            bool pointIsValid = false;

            Vec3 position;

            do
            {
                position = new Vec3(Random.Range(_min.x, _max.x),
                                    Random.Range(_min.y, _max.y),
                                    Random.Range(_min.z, _max.z));

                if (_points.Contains(position))
                    pointIsValid = false;
                else
                    pointIsValid = true;

            } while (!pointIsValid);

            _points.Add(position);
        }

        _points.Add(_min);
        _points.Add(_max);

        _testPoint = new Vec3(Random.Range(_min.x, _max.x),
                              Random.Range(_min.y, _max.y),
                              Random.Range(_min.z, _max.z));
    }
}
