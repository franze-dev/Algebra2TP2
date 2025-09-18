using UnityEngine;

using CustomMath;

public class BoundsDraw : MonoBehaviour
{
    [SerializeField] Vec3 max;
    [SerializeField] Vec3 min;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((max + min) / 2, max - min);
    }
}
