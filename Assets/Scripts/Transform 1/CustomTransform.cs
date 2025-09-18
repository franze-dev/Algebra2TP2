using UnityEngine;

namespace CustomMath
{
    /// <summary>
    /// https://developer.unigine.com/en/docs/latest/code/fundamentals/matrix_transformations/index?implementationLanguage=cpp
    /// https://ingmec.ual.es/~jlblanco/papers/jlblanco2010geometry3D_techrep.pdf
    /// https://learnopengl.com/Getting-started/Transformations
    /// </summary>
    public class CustomTransform : MonoBehaviour
    {
        public Vec3 localPosition;
        public Vec3 eulerRotation;
        public Vec3 localScale = new Vec3(1, 1, 1);
        public CustomQuaternion localRotation;

        public CustomTransform parent;

        private void Awake()
        {
            localRotation = CustomQuaternion.Euler(eulerRotation);

            if (parent == null && transform.parent != null)
            {
                var parentTransform = transform.parent.GetComponent<CustomTransform>();
                if (parentTransform != null)
                    SetParent(parentTransform);
            }

            if (transform.position != Vec3.zero)
                localPosition = new(transform.position);
        }

        private void OnValidate()
        {
            localRotation = CustomQuaternion.Euler(eulerRotation);

        }

        public Vec3 position
        {
            get
            {
                return parent != null ?
                    parent.localToWorldMatrix.MultiplyPoint(localPosition):
                    localPosition;
            }
            set
            {
                localPosition = parent != null ?
                    parent.worldToLocalMatrix.MultiplyPoint(value) :
                    value;
            }
        }

        public CustomQuaternion rotation
        {
            get
            {
                return parent != null ?
                    parent.rotation * localRotation : localRotation;
            }
            set
            {
                localRotation = parent != null ?
                    parent.rotation.Inverse() * value : value;
            }
        }


        public CustomMatrix4x4 worldToLocalMatrix
        {
            get
            {
                return localToWorldMatrix.Inverse();
            }
        }

        /// <summary>
        /// Convert a point from local space to world space.
        /// 
        /// </summary>
        public CustomMatrix4x4 localToWorldMatrix
        {
            get
            {
                var local = CustomMatrix4x4.TRS(localPosition, localRotation, localScale);
                return parent != null ? parent.localToWorldMatrix * local : local;
            }
        }

        public Vec3 lossyScale
        {
            get
            {
                return parent != null ? parent.lossyScale * localScale : localScale;
            }
        }

        /// <summary>
        /// https://learnopengl.com/Getting-started/Transformations
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="worldPositionStays"></param>
        public void SetParent(CustomTransform parent, bool worldPositionStays = true)
        {
            if (worldPositionStays)
            {
                localPosition = position;
                localRotation = rotation;
                localScale = lossyScale;
            }

            this.parent = parent;
        }

        public Vec3 TransformPoint(Vec3 position)
        {
            return localToWorldMatrix.MultiplyPoint(position);
        }

        public Vec3 InverseTransformPoint(Vec3 position)
        {
            return worldToLocalMatrix.MultiplyPoint(position);
        }
    }

}

