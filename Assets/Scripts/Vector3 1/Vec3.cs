using UnityEngine;
using System;
namespace CustomMath
{
    [Serializable]
    public class Vec3 : IEquatable<Vec3>
    {
        #region Variables
        public float x;
        public float y;
        public float z;

        /// <summary>
        /// Vector magnitude squared. Useful for comparisons. It avoids an expensive square root calculation.
        /// </summary>
        public float sqrMagnitude { get => x * x + y * y + z * z; }
        /// <summary>
        /// This vector with a magnitude of 1.
        /// </summary>
        public Vec3 normalized { get => this / magnitude; }
        /// <summary>
        /// The length/norm of the vector.
        /// </summary>
        public float magnitude { get => Magnitude(this); }

        #endregion

        #region constants
        /// <summary>
        /// A small value that is used for floating point comparisons.
        /// </summary>
        public const float epsilon = 1e-05f;
        #endregion

        #region Default Values
        public static Vec3 zero { get { return new Vec3(0.0f, 0.0f, 0.0f); } }
        public static Vec3 one { get { return new Vec3(1.0f, 1.0f, 1.0f); } }
        public static Vec3 forward { get { return new Vec3(0.0f, 0.0f, 1.0f); } }
        public static Vec3 right { get { return new Vec3(1.0f, 0.0f, 0.0f); } }
        public static Vec3 up { get { return new Vec3(0.0f, 1.0f, 0.0f); } }
        public static Vec3 back { get { return -forward; } }
        public static Vec3 left { get { return -right; } }
        public static Vec3 down { get { return -up; } }
        public static Vec3 positiveInfinity { get { return new Vec3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity); } }
        public static Vec3 negativeInfinity { get { return new Vec3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity); } }
        #endregion                                                                                                                                                                               

        #region Constructors
        public Vec3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0.0f;
        }

        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vec3(Vec3 v3)
        {
            this.x = v3.x;
            this.y = v3.y;
            this.z = v3.z;
        }

        public Vec3(Vector3 v3)
        {
            this.x = v3.x;
            this.y = v3.y;
            this.z = v3.z;
        }

        public Vec3(Vector2 v2)
        {
            this.x = v2.x;
            this.y = v2.y;
            this.z = 0.0f;
        }
        #endregion

        #region Operators
        /// <summary>
        /// Calculculates if two vectors are equal using the squared magnitude of their difference.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Vec3 a, Vec3 b)
        {
            float diff_x = a.x - b.x;
            float diff_y = a.y - b.y;
            float diff_z = a.z - b.z;
            Vec3 diff = new Vec3(diff_x, diff_y, diff_z);
            return diff.sqrMagnitude < epsilon * epsilon;
        }

        public static bool operator !=(Vec3 a, Vec3 b)
        {
            return !(a == b);
        }

        public static Vec3 operator +(Vec3 a, Vec3 b)
        {
            return new Vec3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vec3 operator -(Vec3 a, Vec3 b)
        {
            return new Vec3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vec3 operator -(Vec3 v3)
        {
            return new Vec3(-v3.x, -v3.y, -v3.z);
        }

        /// <summary>
        /// Vector multiplied by a scalar. It can scale the vector up or down. (If the scalar is negative, the vector direction is reversed.)
        /// </summary>
        /// <param name="v3"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vec3 operator *(Vec3 v3, float scalar)
        {
            return new Vec3(v3.x * scalar, v3.y * scalar, v3.z * scalar);
        }
        public static Vec3 operator *(float scalar, Vec3 v3)
        {
            return v3 * scalar;
        }
        /// <summary>
        /// While not mathematically correct, it's useful for multiplying data saved in vectors.
        /// It'd be correct as long as they're not treated as actual vectors.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vec3 operator *(Vec3 a, Vec3 b)
        {
            return new Vec3(
                a.x * b.x,
                a.y * b.y,
                a.z * b.z
                );
        }
        /// <summary>
        /// While not mathematically correct, it's useful for dividing data saved in vectors.
        /// It'd be correct as long as they're not treated as actual vectors.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vec3 operator /(Vec3 a, Vec3 b)
        {
            return new Vec3(
                a.x / b.x,
                a.y / b.y,
                a.z / b.z
                );
        }

        /// <summary>
        /// Vector scaled down by a scalar. If the scalar is less than or equal to zero, the original vector is returned.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vec3 operator /(Vec3 a, float scalar)
        {
            if (scalar <= 0)
                return a;

            return new Vec3(a.x / scalar, a.y / scalar, a.z / scalar);
        }

        public static implicit operator Vector3(Vec3 a)
        {
            return new Vector3(a.x, a.y, a.z);
        }

        public static implicit operator Vector2(Vec3 a)
        {
            return new Vector2(a.x, a.y);
        }
        #endregion

        #region Functions
        public override string ToString()
        {
            return "X = " + x.ToString() + "   Y = " + y.ToString() + "   Z = " + z.ToString();
        }

        /// <summary>
        /// Returns the angle in degrees between from and to.
        /// https://www.mathworks.com/matlabcentral/answers/2092961-how-to-calculate-the-angle-between-two-3d-vectors
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float Angle(Vec3 from, Vec3 to)
        {
            var dot = Dot(from, to);

            if (dot == 0)
                return 90f;

            var cosTheta = dot / (Magnitude(from) * Magnitude(to));

            cosTheta = Mathf.Clamp(cosTheta, -1f, 1f);

            return Mathf.Acos(cosTheta) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Magnitude of a vector. It is always a positive/zero value.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float Magnitude(Vec3 vector)
        {
            return Magnitude(vector.x, vector.y, vector.z);
        }

        public static float Magnitude(float x, float y, float z)
        {
            return Mathf.Sqrt(x * x + y * y + z * z);
        }

        /// <summary>
        /// Returns a vector that is perpendicular to the two input vectors.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vec3 Cross(Vec3 a, Vec3 b)
        {
            return new Vec3(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
            );
        }

        /// <summary>
        /// Returns the distance between two points by creating a vector between them and returning its magnitude.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Distance(Vec3 a, Vec3 b)
        {
            float x = a.x - b.x;
            float y = a.y - b.y;
            float z = a.z - b.z;

            return Magnitude(x, y, z);
        }

        /// <summary>
        /// The euclidean dot product of two vectors. It is the product of the magnitudes of the two vectors and the cosine of the angle between them.
        /// Returns a positive value if the angle between the vectors is less than 90 degrees, 
        /// negative if it's greater than 90 degrees, 
        /// and zero if the vectors are orthogonal (perpendicular).
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Dot(Vec3 a, Vec3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vec3 Lerp(Vec3 a, Vec3 b, float t)
        {
            if (t < epsilon)
                return a;
            if (t >= 1f)
                return b;

            t = Mathf.Clamp01(t);

            return LerpUnclamped(a, b, t);
        }

        public static Vec3 LerpUnclamped(Vec3 a, Vec3 b, float t)
        {
            return new Vec3(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t
            );
        }

        /// <summary>
        /// Returns a vector that is made from the largest components of two vectors.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vec3 Max(Vec3 a, Vec3 b)
        {
            float newX = a.x > b.x ? a.x : b.x;
            float newY = a.y > b.y ? a.y : b.y;
            float newZ = a.z > b.z ? a.z : b.z;
            return new Vec3(newX, newY, newZ);
        }

        /// <summary>
        /// Returns a vector that is made from the smallest components of two vectors.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vec3 Min(Vec3 a, Vec3 b)
        {
            float newX = a.x < b.x ? a.x : b.x;
            float newY = a.y < b.y ? a.y : b.y;
            float newZ = a.z < b.z ? a.z : b.z;
            return new Vec3(newX, newY, newZ);
        }

        public void Set(float newX, float newY, float newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }
        public void Scale(Vec3 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
        }
        public void Normalize()
        {
            float mag = Magnitude(this);
            if (mag > epsilon)
            {
                x /= mag;
                y /= mag;
                z /= mag;
            }
            else
            {
                x = 0f;
                y = 0f;
                z = 0f;
            }
        }
        #endregion

        #region Internals
        public override bool Equals(object other)
        {
            if (!(other is Vec3)) return false;
            return Equals((Vec3)other);
        }

        public bool Equals(Vec3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }
        #endregion
    }
}