using UnityEngine;

using CustomMath;

public class BoundsDraw : MonoBehaviour
{
    [SerializeField] CustomTransform maxT;
    [SerializeField] CustomTransform minT;

    private Vec3 max => maxT.position;
    private Vec3 min => minT.position;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((max + min) / 2, max - min);
    }
}
