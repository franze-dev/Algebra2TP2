using CustomMath;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    private void Start()
    {
        _planesToDraw = new();

        _points = new List<Vec3>();

        GeneratePoints();
        SortPoints();

        _diagram = new(_points, _min, _max);

        DebugRegions();
    }

    private void DebugRegions()
    {
        foreach (var region in _diagram.Regions)
        {
            Debug.Log("REGION: " + region.ToString());
            foreach (var face in region.Faces)
            {
                Debug.Log("Face vertices: " + face.vertices.Count);

                foreach (var v in face.vertices)
                    Debug.Log(v);
            }
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


        //Diagram debug
        //Gizmos.color = Color.red;

        //foreach (var region in _diagram.Regions)
        //{
        //    for (int i = 0; i < region.Vertices.Count; i++)
        //    {
        //        var v = region.Vertices[i];
        //        var vN = region.Vertices[(i + 1) % region.Vertices.Count];

        //        Gizmos.DrawSphere(v, 0.5f);

        //        Gizmos.DrawLine(v, vN);
        //    }
        //}


        //_planesToDraw.Clear();

        foreach (var region in _diagram.Regions)
        {
            Gizmos.color = Color.blue;
            _planesToDraw.AddRange(region.Borders);


            Gizmos.color = region.Color;
            Gizmos.DrawSphere(region.Site, 2f);

            foreach (var border in region.Borders)
                DebugPlane(border);
        }

        //_planesToDraw = _planesToDraw.Distinct().ToList();

        //foreach (var plane in _planesToDraw)
        //    DebugPlane(plane);
    }

    private void DebugPlane(CustomPlane plane)
    {
        Vec3 point = plane.normal * plane.distance;
        Vec3 scaledNormal = plane.normal * 3f;
        float size = 100f;

        Vector3 axis1 = Vector3.Cross(scaledNormal, Vector3.up);
        if (axis1.sqrMagnitude < 0.001f)
            axis1 = Vector3.Cross(scaledNormal, Vector3.right);

        axis1.Normalize();
        Vector3 axis2 = Vector3.Cross(scaledNormal, axis1).normalized;

        Vector3 corner0 = point + (axis1 + axis2) * size * 0.5f;
        Vector3 corner1 = point + (axis1 - axis2) * size * 0.5f;
        Vector3 corner2 = point + (-axis1 - axis2) * size * 0.5f;
        Vector3 corner3 = point + (-axis1 + axis2) * size * 0.5f;

        Gizmos.DrawLine(corner0, corner1);
        Gizmos.DrawLine(corner1, corner2);
        Gizmos.DrawLine(corner2, corner3);
        Gizmos.DrawLine(corner3, corner0);

        Gizmos.DrawLine(point, point + scaledNormal);
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
    }
}
