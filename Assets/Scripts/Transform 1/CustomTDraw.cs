using CustomMath;
using UnityEngine;

public class CustomTDraw : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Color color = Color.blue;
    [SerializeField] private CustomTransform customTransform;

    private void Awake()
    {
        if (customTransform == null)
        {
            customTransform = GetComponent<CustomTransform>();
            if (customTransform == null)
                Debug.LogError("CustomTransform component not found.");
        }
    }

    private void OnDrawGizmos()
    {
        if (mesh != null)
        {
            Gizmos.color = color;

            Quaternion rotation = new(customTransform.rotation.x, 
                                      customTransform.rotation.y, 
                                      customTransform.rotation.z, 
                                      customTransform.rotation.w);

            Gizmos.DrawMesh(mesh, (Vector3)customTransform.position, rotation, (Vector3)customTransform.lossyScale);
        }
        else
            Debug.Log("Mesh not assigned in CustomTDraw component.");
    }
}
