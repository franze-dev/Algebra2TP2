using System;
using UnityEngine;

namespace CustomMath
{
    public class CustomQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        private static readonly CustomQuaternion _identity = new(0f, 0f, 0f, 1f);

        public static CustomQuaternion Identity => _identity;

        public const float kEpsilon = 1E-06f;

        public CustomQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public CustomQuaternion(Vec3 v, float w)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = w;
        }

        public Vec3 eulerAngles
        {
            get => MakePositive(ToEulerRad(this) * Mathf.Rad2Deg);
            set
            {
                CustomQuaternion q = FromEulerRad(value * Mathf.Deg2Rad);

                Set(q.x, q.y, q.z, q.w);
            }
        }

        public CustomQuaternion normalized => Normalize(this);

        /// <summary>
        /// Finds the shortest rotation from one vector to another.
        /// https://stackoverflow.com/questions/1171849/finding-quaternion-representing-the-rotation-from-one-vector-to-another
        /// </summary>
        /// <param name="fromDirection"></param>
        /// <param name="toDirection"></param>
        /// <returns></returns>
        public static CustomQuaternion FromToRotation(Vec3 fromDirection, Vec3 toDirection)
        {
            var cross = Vec3.Cross(fromDirection, toDirection);

            if (cross.sqrMagnitude < kEpsilon)
            {
                // Vectors are parallel
                if (Vec3.Dot(fromDirection, toDirection) > 0f)
                    //Same direction
                    return Identity;
                // Opposite direction
                return AngleAxis(180f, fromDirection.normalized);
            }

            var w = Mathf.Sqrt(fromDirection.sqrMagnitude * toDirection.sqrMagnitude) + Vec3.Dot(fromDirection, toDirection);

            return Normalize(new(cross, w));
        }

        /// <summary>
        /// Inverts a rotation.
        /// https://www.mathworks.com/help/aeroblks/quaternioninverse.html
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static CustomQuaternion Inverse(CustomQuaternion rotation)
        {
            var sqrMag = Dot(rotation, rotation);

            if (sqrMag < kEpsilon)
                return Identity;

            return new(
                -rotation.x / sqrMag,
                -rotation.y / sqrMag,
                -rotation.z / sqrMag,
                rotation.w / sqrMag
            );
        }

        /// <summary>
        /// Inverts this rotation.
        /// </summary>
        /// <returns></returns>
        public CustomQuaternion Inverse()
        {
            return Inverse(this);
        }

        public static CustomQuaternion Slerp(CustomQuaternion a, CustomQuaternion b, float t)
        {
            t = Mathf.Clamp01(t);

            return SlerpUnclamped(a, b, t);
        }

        /// <summary>
        /// This interpolates between quaternions a and b by t.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static CustomQuaternion SlerpUnclamped(CustomQuaternion a, CustomQuaternion b, float t)
        {
            var nA = a.normalized;
            var nB = b.normalized;

            if (Dot(nA, nB) < 0f)
                // shortest interpolation path
                nB = new(-nB.x, -nB.y, -nB.z, -nB.w);

            var angle = Angle(a, b) * Mathf.Deg2Rad;
            var sin = Mathf.Sin(angle);

            if (sin < kEpsilon)
                // if angle is too small, use linear interpolation to avoid division by zero
                return LerpUnclamped(a, b, t);

            var resA = Mathf.Sin((1 - t) * angle) / sin;
            var resB = Mathf.Sin(t * angle) / sin;

            return new(
                nA.x * resA + nB.x * resB,
                nA.y * resA + nB.y * resB,
                nA.z * resA + nB.z * resB,
                nA.w * resA + nB.w * resB
            );
        }

        public static CustomQuaternion Lerp(CustomQuaternion a, CustomQuaternion b, float t)
        {
            t = Mathf.Clamp01(t);
            return LerpUnclamped(a, b, t);
        }

        /// <summary>
        /// Linearly interpolates between two quaternions by t.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static CustomQuaternion LerpUnclamped(CustomQuaternion a, CustomQuaternion b, float t)
        {
            var lerpT = (1 - t);

            var q = new CustomQuaternion(
                lerpT * a.x + t * b.x,
                lerpT * a.y + t * b.y,
                lerpT * a.z + t * b.z,
                lerpT * a.w + t * b.w
            );

            return Normalize(q);
        }

        /// <summary>
        /// Receives euler angles in radians and returns the corresponding quaternion.
        /// https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
        /// </summary>
        /// <param name="euler"></param>
        /// <returns></returns>
        private static CustomQuaternion FromEulerRad(Vec3 euler)
        {
            var roll = euler.z;
            var pitch = euler.x;
            var yaw = euler.y;

            float cr = Mathf.Cos(roll * 0.5f);
            float sr = Mathf.Sin(roll * 0.5f);
            float cp = Mathf.Cos(pitch * 0.5f);
            float sp = Mathf.Sin(pitch * 0.5f);
            float cy = Mathf.Cos(yaw * 0.5f);
            float sy = Mathf.Sin(yaw * 0.5f);

            float w = cr * cp * cy + sr * sp * sy;
            float x = sr * cp * cy - cr * sp * sy;
            float y = cr * sp * cy + sr * cp * sy;
            float z = cr * cp * sy - sr * sp * cy;

            return Normalize(new(x, y, z, w));
        }

        /// <summary>
        /// Receives a quaternion and returns the corresponding euler angles in radians.
        /// https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        private static Vec3 ToEulerRad(CustomQuaternion rotation)
        {
            CustomQuaternion q = rotation.normalized;

            var roll = Mathf.Atan2(2f * (q.w * q.x + q.y * q.z), 1f - 2f * (q.x * q.x + q.y * q.y));

            var pitch = Mathf.Asin(2 * (q.w * q.y) - q.x * q.z);

            var yaw = Mathf.Atan2(2f * (q.w * q.z + q.x * q.y), 1f - 2f * (q.y * q.y + q.z * q.z));

            return new(pitch, yaw, roll);
        }

        /// <summary>
        /// It creates a rotation which rotates angle degrees around axis.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static CustomQuaternion AngleAxis(float angle, Vec3 axis)
        {
            if (axis.sqrMagnitude < kEpsilon)
                return Identity;

            axis = axis.normalized;

            float half = (angle * Mathf.Deg2Rad) * 0.5f;

            float sin = Mathf.Sin(half);
            float cos = Mathf.Cos(half);

            return new CustomQuaternion(
                axis.x * sin,
                axis.y * sin,
                axis.z * sin,
                cos
                );
        }

        public static CustomQuaternion LookRotation(Vec3 forward)
        {
            return LookRotation(forward, Vec3.up);
        }

        /// <summary>
        /// This constructs a quaternion representing a rotation that looks in the 'forward' direction.
        /// </summary>
        /// <param name="forward">This vector must be orthogonal to the 'upwards' vector</param>
        /// <param name="upwards">This vector must be orthogonal to the 'forward' vector</param>
        /// <returns></returns>
        public static CustomQuaternion LookRotation(Vec3 forward, Vec3 upwards)
        {
            var fw = forward.normalized;
            var up = upwards.normalized;
            var right = Vec3.Cross(up, fw).normalized;

            CustomMatrix4x4 m = new();

            m.SetColumn(0, new Vector4(right.x, right.y, right.z, 0));
            m.SetColumn(1, new Vector4(up.x,    up.y,    up.z,    0));
            m.SetColumn(2, new Vector4(fw.x,    fw.y,    fw.z,    0));
            m.SetColumn(3, new Vector4(0,       0,       0,       1));

            CustomQuaternion q = m.rotation;

            return q;
        }

        public void Set(float newX, float newY, float newZ, float newW)
        {
            x = newX;
            y = newY;
            z = newZ;
            w = newW;
        }

        public static CustomQuaternion operator *(CustomQuaternion q1, CustomQuaternion q2)
        {
            return new CustomQuaternion(
                q1.w * q2.x + q1.x * q2.w + q1.y * q2.z - q1.z * q2.y,
                q1.w * q2.y + q1.y * q2.w + q1.z * q2.x - q1.x * q2.z,
                q1.w * q2.z + q1.z * q2.w + q1.x * q2.y - q1.y * q2.x,
                q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z);
        }

        // Rotates a point by a quaternion rotation
        public static Vec3 operator *(CustomQuaternion rotation, Vec3 point)
        {
            Vec3 u = new(rotation.x, rotation.y, rotation.z);
            float w = rotation.w;

            Vec3 uv = Vec3.Cross(u, point);
            Vec3 uuv = Vec3.Cross(u, uv);

            return point + (uv * (2f * w)) + (uuv * 2f);
        }

        private static bool IsEqualUsingDot(float dot)
        {
            return dot > 0.999999f || dot < -0.999999f;
        }

        public static bool operator ==(CustomQuaternion lhs, CustomQuaternion rhs)
        {
            return IsEqualUsingDot(Dot(lhs, rhs));
        }

        public static bool operator !=(CustomQuaternion lhs, CustomQuaternion rhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator Quaternion(CustomQuaternion a)
        {
            return new Quaternion(a.x, a.y, a.z, a.w);
        }

        /// <summary>
        /// If it returns 1 the quaternions are equal, 
        /// if it returns -1 they are opposite.
        /// If it returns 0 they are orthogonal.
        /// https://www.cs.ucdavis.edu/~amenta/3dphoto/quaternion.pdf
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Dot(CustomQuaternion a, CustomQuaternion b)
        {
            return a.x * b.x +
                   a.y * b.y +
                   a.z * b.z +
                   a.w * b.w;
        }

        public void SetLookRotation(Vec3 view)
        {
            Vec3 up = Vec3.up;
            SetLookRotation(view, up);
        }

        public void SetLookRotation(Vec3 view, Vec3 up)
        {
            CustomQuaternion q = LookRotation(view, up);
            Set(q.x, q.y, q.z, q.w);
        }

        public static float Angle(CustomQuaternion a, CustomQuaternion b)
        {
            float dot = Mathf.Clamp01(Mathf.Abs(Dot(a, b)));

            return IsEqualUsingDot(dot) ?
                0f :
                (Mathf.Acos(dot) * 2f * Mathf.Rad2Deg); ;
        }

        private static Vec3 MakePositive(Vec3 euler)
        {
            float min = kEpsilon;
            float max = 360f + kEpsilon;
            if (euler.x < min)
                euler.x += 360f;
            else if (euler.x > max)
                euler.x -= 360f;

            if (euler.y < min)
                euler.y += 360f;
            else if (euler.y > max)
                euler.y -= 360f;

            if (euler.z < min)
                euler.z += 360f;
            else if (euler.z > max)
                euler.z -= 360f;

            return euler;
        }

        public static CustomQuaternion Euler(Vec3 euler)
        {
            return FromEulerRad(euler * Mathf.Deg2Rad);
        }

        public void SetFromToRotation(Vec3 fromDirection, Vec3 toDirection)
        {
            CustomQuaternion q = FromToRotation(fromDirection, toDirection);
            Set(q.x, q.y, q.z, q.w);
        }

        public static CustomQuaternion RotateTowards(CustomQuaternion from, CustomQuaternion to, float maxDegreesDelta)
        {
            float angle = Angle(from, to);
            if (angle == 0f)
            {
                return to;
            }

            return SlerpUnclamped(from, to, Mathf.Min(1f, maxDegreesDelta / angle));
        }

        public static CustomQuaternion Normalize(CustomQuaternion q)
        {
            float mag = Mathf.Sqrt(Dot(q, q));
            if (mag < Mathf.Epsilon)
            {
                return Identity;
            }

            return new CustomQuaternion(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
        }

        public void Normalize()
        {
            CustomQuaternion q = Normalize(this);
            Set(q.x, q.y, q.z, q.w);
        }
    }
}
