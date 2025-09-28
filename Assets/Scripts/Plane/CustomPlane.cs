using UnityEngine;

namespace CustomMath
{
    public class CustomPlane
    {
        private Vec3 _normal;

        private float _distance;

        public Vec3 normal
        {
            get
            {
                return _normal;
            }
            set
            {
                _normal = value;
            }
        }

        public float distance
        {
            get
            {
                return _distance;
            }
            set
            {
                _distance = value;
            }
        }

        public CustomPlane(Vec3 normal, Vec3 point)
        {
            _normal = Vec3.Normalize(normal);
            _distance = -Vec3.Dot(_normal, point);
        }

        public bool GetSide(Vec3 point)
        {
            return Vec3.Dot(_normal, point) + _distance > 0f;
        }

        public Vec3 AnyPointOnPlane()
        {
            return -distance * _normal;
        }
        public Vec3 AnyPointOnPlane()
        {
            return -distance * _normal;
        }
    }
}

