using UnityEngine;
using CustomMath;
using System.Collections.Generic;

public class PointGeneration : MonoBehaviour
{
    [SerializeField] private GameObject pointPrefab;

    [SerializeField] private int N = 10;

    [SerializeField] private CustomTransform maxT;
    [SerializeField] private CustomTransform minT;

    private List<Vec3> points;

    private Vec3 max => (Vec3)maxT.position;
    private Vec3 min => (Vec3)minT.position;

    private void Start()
    {
        points = new List<Vec3>();

        GeneratePoints();

        SortPoints();

    }

    /// <summary>
    /// Sorts points from closest to furthest from the max point.
    /// </summary>
    private void SortPoints()
    {
        foreach (var point in points)
            Debug.Log(point.x + " " + point.y + " " + point.z);

        points.Sort((a, b) =>
        {
            float distA = (a - max).sqrMagnitude;
            float distB = (b - max).sqrMagnitude;
            return distA.CompareTo(distB);
        });

        Debug.Log("Sorted Points:");

        foreach (var point in points)
            Debug.Log(point.x + " " + point.y + " " + point.z);
    }

    private void GeneratePoints()
    {
        for (int i = 0; i < N; i++)
        {
            bool pointIsValid = false;

            Vec3 position;

            do
            {
                position = new Vec3(Random.Range(min.x, max.x),
                                    Random.Range(min.y, max.y),
                                    Random.Range(min.z, max.z));

                if (points.Contains(position))
                    pointIsValid = false;
                else
                    pointIsValid = true;

            } while (!pointIsValid);

            Instantiate(pointPrefab, (Vector3)position, (Quaternion)CustomQuaternion.Identity);
            points.Add(position);
        }

    }
}
