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

        public CustomPlane flipped
        {
            get
            {
                return new CustomPlane(-_normal, 0f - _distance);
            }
        }

        public CustomPlane(Vec3 normal, Vec3 point)
        {
            _normal = Vec3.Normalize(normal);
            _distance = 0f - Vec3.Dot(_normal, point);
        }

        public CustomPlane(Vec3 normal, float distance)
        {
            _normal = Vec3.Normalize(normal);
            _distance = distance;
        }

        public CustomPlane(Vec3 a, Vec3 b, Vec3 c)
        {
            _normal = Vec3.Normalize(Vec3.Cross(b - a, c - a));
            _distance = 0f - Vec3.Dot(_normal, a);
        }
        public void SetNormalAndPosition(Vec3 normal, Vec3 point)
        {
            _normal = Vec3.Normalize(normal);
            _distance = 0f - Vec3.Dot(_normal, point);
        }
        public void Set3Points(Vec3 a, Vec3 b, Vec3 c)
        {
            _normal = Vec3.Normalize(Vec3.Cross(b - a, c - a));
            _distance = 0f - Vec3.Dot(_normal, a);
        }
        public void Flip()
        {
            _normal = -_normal;
            _distance = 0f - _distance;
        }

        public void Translate(Vec3 translation)
        {
            _distance += Vec3.Dot(_normal, translation);
        }

        public static CustomPlane Translate(CustomPlane customPlane, Vec3 translation)
        {
            return new CustomPlane(customPlane._normal, customPlane._distance += Vec3.Dot(customPlane._normal, translation));
        }

        public Vec3 ClosestPointOnPlane(Vec3 point)
        {
            float num = Vec3.Dot(_normal, point) + _distance;
            return point - _normal * num;
        }

        public float GetDistanceToPoint(Vec3 point)
        {
            return Vec3.Dot(_normal, point) + _distance;
        }

        public bool GetSide(Vec3 point)
        {
            return Vec3.Dot(_normal, point) + _distance > 0f;
        }

        public bool SameSide(Vec3 inPt0, Vec3 inPt1)
        {
            float distanceToPoint = GetDistanceToPoint(inPt0);
            float distanceToPoint2 = GetDistanceToPoint(inPt1);
            return (distanceToPoint > 0f && distanceToPoint2 > 0f) || (distanceToPoint <= 0f && distanceToPoint2 <= 0f);
        }

        public bool Raycast(Ray ray, out float enter)
        {
            Vec3 rayDir = new(ray.direction);
            Vec3 rayOri = new(ray.origin);

            float num = Vec3.Dot(rayDir, _normal);
            float num2 = 0f - Vec3.Dot(rayOri, _normal) - _distance;
            if (Mathf.Approximately(num, 0f))
            {
                enter = 0f;
                return false;
            }

            enter = num2 / num;
            return enter > 0f;
        }
    }

}

