using UnityEngine;
using CustomMath;
using System.Collections.Generic;

public class PointGeneration : MonoBehaviour
{
    [SerializeField] private GameObject _pointPrefab;

    [SerializeField] private int _pointsAmount = 10;

    [SerializeField] private CustomTransform _maxT;
    [SerializeField] private CustomTransform _minT;

    private List<Vec3> _points;

    private Vec3 _max => _maxT.position;
    private Vec3 _min => _minT.position;

    private void Start()
    {
        _points = new List<Vec3>();

        GeneratePoints();
        SortPoints();

        CreateBisectors();
    }

    private void CreateBisectors()
    {
        foreach (var point in _points)
        {

        }
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

        foreach (var point in _points)
        {
            Debug.Log(point);
        }
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

            GameObject pointGO = Instantiate(_pointPrefab, (Vector3)position, (Quaternion)CustomQuaternion.Identity, transform.parent);

            pointGO.GetComponent<CustomTransform>().localScale = new Vec3(2, 2, 2);

            _points.Add(position);
        }

    }
}
