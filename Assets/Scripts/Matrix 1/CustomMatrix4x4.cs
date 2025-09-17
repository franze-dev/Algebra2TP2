using System;
using UnityEngine;

namespace CustomMath
{
    public class CustomMatrix4x4
    {
        public float m00;
        public float m10;
        public float m20;
        public float m30;
        public float m01;
        public float m11;
        public float m21;
        public float m31;
        public float m02;
        public float m12;
        public float m22;
        public float m32;
        public float m03;
        public float m13;
        public float m23;
        public float m33;
        private static readonly CustomMatrix4x4 zeroMatrix = new CustomMatrix4x4(new Vector4(0f, 0f, 0f, 0f),
                                                                                 new Vector4(0f, 0f, 0f, 0f),
                                                                                 new Vector4(0f, 0f, 0f, 0f),
                                                                                 new Vector4(0f, 0f, 0f, 0f));

        public CustomQuaternion rotation => GetRotation();
        public Vec3 lossyScale => GetLossyScale();

        public bool isIdentity => IsIdentity();

        public float this[int row, int column]
        {
            get
            {
                return this[row + column * 4];
            }
            set
            {
                this[row + column * 4] = value;
            }
        }

        public float this[int index]
        {
            get
            {
                return index switch
                {
                    0 => m00,
                    1 => m10,
                    2 => m20,
                    3 => m30,
                    4 => m01,
                    5 => m11,
                    6 => m21,
                    7 => m31,
                    8 => m02,
                    9 => m12,
                    10 => m22,
                    11 => m32,
                    12 => m03,
                    13 => m13,
                    14 => m23,
                    15 => m33,
                    _ => throw new IndexOutOfRangeException(),
                };
            }
            set
            {
                switch (index)
                {
                    case 0:
                        m00 = value;
                        break;
                    case 1:
                        m10 = value;
                        break;
                    case 2:
                        m20 = value;
                        break;
                    case 3:
                        m30 = value;
                        break;
                    case 4:
                        m01 = value;
                        break;
                    case 5:
                        m11 = value;
                        break;
                    case 6:
                        m21 = value;
                        break;
                    case 7:
                        m31 = value;
                        break;
                    case 8:
                        m02 = value;
                        break;
                    case 9:
                        m12 = value;
                        break;
                    case 10:
                        m22 = value;
                        break;
                    case 11:
                        m32 = value;
                        break;
                    case 12:
                        m03 = value;
                        break;
                    case 13:
                        m13 = value;
                        break;
                    case 14:
                        m23 = value;
                        break;
                    case 15:
                        m33 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
        public Vector4 GetColumn(int index)
        {
            return index switch
            {
                0 => new Vector4(m00, m10, m20, m30),
                1 => new Vector4(m01, m11, m21, m31),
                2 => new Vector4(m02, m12, m22, m32),
                3 => new Vector4(m03, m13, m23, m33),
                _ => throw new IndexOutOfRangeException("Invalid column index!"),
            };
        }

        public static CustomMatrix4x4 zero => zeroMatrix;
        public static CustomMatrix4x4 identity => new CustomMatrix4x4(new Vector4(1f, 0f, 0f, 0f),
                                                                      new Vector4(0f, 1f, 0f, 0f),
                                                                      new Vector4(0f, 0f, 1f, 0f),
                                                                      new Vector4(0f, 0f, 0f, 1f));

        /// <summary>
        /// Returns the rotation component of the matrix as a quaternion.
        /// https://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/
        /// </summary>
        /// <returns></returns>
        private CustomQuaternion GetRotation()
        {
            float x;
            float y;
            float z;
            float w;

            //diagonal elements (the sum of these must not be zero)
            float diagonal = m00 + m11 + m22;

            // check which major diagonal element has the greatest value
            // if the diagonal is greater than zero then w is the largest
            if (diagonal > 0)
            {
                float S = Mathf.Sqrt(diagonal + 1f) * 2;
                w = 0.25f * S;
                x = (m21 - m12) / S;
                y = (m02 - m20) / S;
                z = (m10 - m01) / S;
            }
            // otherwise we need to figure out which of x, y, or z is the largest
            else if ((m00 > m11) && (m00 > m22))
            {
                float S = Mathf.Sqrt(1f + m00 - m11 - m22) * 2;
                w = (m21 - m12) / S;
                x = 0.25f * S;
                y = (m01 + m10) / S;
                z = (m02 + m20) / S;
            }
            else if (m11 > m22)
            {
                float S = Mathf.Sqrt(1f + m11 - m00 - m22) * 2;
                w = (m02 - m20) / S;
                x = (m01 + m10) / S;
                y = 0.25f * S;
                z = (m12 + m21) / S;
            }
            else
            {
                float S = Mathf.Sqrt(1f + m22 - m00 - m11) * 2;
                w = (m10 - m01) / S;
                x = (m02 + m20) / S;
                y = (m12 + m21) / S;
                z = 0.25f * S;
            }

            return new CustomQuaternion(x, y, z, w).normalized;
        }

        private Vec3 GetLossyScale()
        {
            return new Vec3(
                new Vec3(m00, m10, m20).magnitude,
                new Vec3(m01, m11, m21).magnitude,
                new Vec3(m02, m12, m22).magnitude
            );
        }

        private bool IsIdentity()
        {
            return this == identity;
        }

        public bool ValidTRS()
        {
            Vec3 scale = lossyScale;

            return scale.x > Mathf.Epsilon && scale.y > Mathf.Epsilon && scale.z > Mathf.Epsilon;
        }

        /// <summary>
        /// https://learnopengl.com/Getting-started/Transformations
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="q"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static CustomMatrix4x4 TRS(Vec3 pos, CustomQuaternion q, Vec3 s)
        {
            var scale = Scale(s);

            var rotation = Rotate(q);

            var transform = Translate(pos);

            return transform * rotation * scale;
        }
        public void SetTRS(Vec3 pos, CustomQuaternion q, Vec3 s)
        {
            var mat = TRS(pos, q, s);

            this.Set(mat);
        }

        private void Set(CustomMatrix4x4 mat)
        {
            for (int i = 0; i < 16; i++)
                this[i] = mat[i];
        }

        public CustomMatrix4x4(Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3)
        {
            m00 = column0.x;
            m01 = column1.x;
            m02 = column2.x;
            m03 = column3.x;

            m10 = column0.y;
            m11 = column1.y;
            m12 = column2.y;
            m13 = column3.y;

            m20 = column0.z;
            m21 = column1.z;
            m22 = column2.z;
            m23 = column3.z;

            m30 = column0.w;
            m31 = column1.w;
            m32 = column2.w;
            m33 = column3.w;
        }

        public CustomMatrix4x4()
        {
            var identity = CustomMatrix4x4.identity;
            Set(identity);
        }

        public static CustomMatrix4x4 operator *(CustomMatrix4x4 lhs, CustomMatrix4x4 rhs)
        {
            CustomMatrix4x4 result = new();
            result.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30;
            result.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31;
            result.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32;
            result.m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33;

            result.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30;
            result.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31;
            result.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32;
            result.m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33;

            result.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30;
            result.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31;
            result.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32;
            result.m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33;

            result.m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30;
            result.m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31;
            result.m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32;
            result.m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33;

            return result;
        }

        public static Vector4 operator *(CustomMatrix4x4 lhs, Vector4 vector)
        {
            Vector4 result = default(Vector4);
            result.x = lhs.m00 * vector.x + lhs.m01 * vector.y + lhs.m02 * vector.z + lhs.m03 * vector.w;
            result.y = lhs.m10 * vector.x + lhs.m11 * vector.y + lhs.m12 * vector.z + lhs.m13 * vector.w;
            result.z = lhs.m20 * vector.x + lhs.m21 * vector.y + lhs.m22 * vector.z + lhs.m23 * vector.w;
            result.w = lhs.m30 * vector.x + lhs.m31 * vector.y + lhs.m32 * vector.z + lhs.m33 * vector.w;
            return result;
        }

        public static bool operator ==(CustomMatrix4x4 lhs, CustomMatrix4x4 rhs)
        {
            return lhs.GetColumn(0) == rhs.GetColumn(0) &&
                   lhs.GetColumn(1) == rhs.GetColumn(1) &&
                   lhs.GetColumn(2) == rhs.GetColumn(2) &&
                   lhs.GetColumn(3) == rhs.GetColumn(3);
        }

        public static bool operator !=(CustomMatrix4x4 lhs, CustomMatrix4x4 rhs)
        {
            return !(lhs == rhs);
        }

        public static explicit operator Matrix4x4(CustomMatrix4x4 v)
        {
            throw new NotImplementedException();
        }

        public Vec3 GetPosition()
        {
            return new Vec3(m03, m13, m23);
        }

        public void SetColumn(int index, Vector4 column)
        {
            this[0, index] = column.x;
            this[1, index] = column.y;
            this[2, index] = column.z;
            this[3, index] = column.w;
        }

        public void SetRow(int index, Vector4 row)
        {
            this[index, 0] = row.x;
            this[index, 1] = row.y;
            this[index, 2] = row.z;
            this[index, 3] = row.w;
        }

        public Vec3 MultiplyPoint(Vec3 point)
        {
            float x = m00 * point.x + m01 * point.y + m02 * point.z + m03;
            float y = m10 * point.x + m11 * point.y + m12 * point.z + m13;
            float z = m20 * point.x + m21 * point.y + m22 * point.z + m23;
            float w = m30 * point.x + m31 * point.y + m32 * point.z + m33;

            if (Mathf.Abs(w) > Mathf.Epsilon)
                return new Vec3(x / w, y / w, z / w);

            return new Vec3(x, y, z);
        }

        public Vec3 MultiplyPoint3x4(Vec3 point)
        {
            var vec = MultiplyVector(point);

            vec.x += m03;
            vec.y += m13;
            vec.z += m23;

            return vec;
        }

        public Vec3 MultiplyVector(Vec3 point)
        {
            float x = m00 * point.x + m01 * point.y + m02 * point.z;
            float y = m10 * point.x + m11 * point.y + m12 * point.z;
            float z = m20 * point.x + m21 * point.y + m22 * point.z;
            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Returns a matrix that scales by the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static CustomMatrix4x4 Scale(Vec3 vector)
        {
            var m = identity;

            m.m00 = vector.x;
            m.m11 = vector.y;
            m.m22 = vector.z;

            return m;
        }

        /// <summary>
        /// Returns a matrix that translates by the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static CustomMatrix4x4 Translate(Vec3 vector)
        {
            var m = identity;

            m.m03 = vector.x;
            m.m13 = vector.y;
            m.m23 = vector.z;
            return m;
        }

        /// <summary>
        /// Returns a matrix that rotates by the given quaternion.
        /// https://ingmec.ual.es/~jlblanco/papers/jlblanco2010geometry3D_techrep.pdf
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static CustomMatrix4x4 Rotate(CustomQuaternion q)
        {
            q.Normalize();

            var xx = q.x * q.x;
            var yy = q.y * q.y;
            var zz = q.z * q.z;
            var xy = q.x * q.y;
            var xz = q.x * q.z;
            var yz = q.y * q.z;
            var wx = q.w * q.x;
            var wy = q.w * q.y;
            var wz = q.w * q.z;
            var ww = q.w * q.w;

            var res = identity;

            res.m00 = ww + xx - yy - zz;
            res.m01 = 2 * (xy - wz);
            res.m02 = 2 * (xz + wy);

            res.m10 = 2 * (xy + wz);
            res.m11 = ww - xx + yy - zz;
            res.m12 = 2 * (yz - wx);

            res.m20 = 2 * (xz - wy);
            res.m21 = 2 * (yz + wx);
            res.m22 = ww - xx - yy + zz;

            return res;
        }

        /// <summary>
        /// https://stackoverflow.com/questions/1148309/inverting-a-4x4-matrix
        /// https://rodolphe-vaillant.fr/entry/7/c-code-for-4x4-matrix-inversion
        /// </summary>
        /// <returns></returns>
        public CustomMatrix4x4 Inverse()
        {

            CustomMatrix4x4 inv = new();
            CustomMatrix4x4 m = this;

            float det;
            int i;

            inv.m00 = m.m11 * m.m22 * m.m33 -
                     m.m11 * m.m32 * m.m23 -
                     m.m12 * m.m21 * m.m33 +
                     m.m12 * m.m31 * m.m23 +
                     m.m13 * m.m21 * m.m32 -
                     m.m13 * m.m31 * m.m22;

            inv.m01 = -m.m01 * m.m22 * m.m33 +
                      m.m01 * m.m32 * m.m23 +
                      m.m02 * m.m21 * m.m33 -
                      m.m02 * m.m31 * m.m23 -
                      m.m03 * m.m21 * m.m32 +
                      m.m03 * m.m31 * m.m22;

            inv.m02 = m.m01 * m.m12 * m.m33 -
                     m.m01 * m.m32 * m.m13 -
                     m.m02 * m.m11 * m.m33 +
                     m.m02 * m.m31 * m.m13 +
                     m.m03 * m.m11 * m.m32 -
                     m.m03 * m.m31 * m.m12;

            inv.m03 = -m.m01 * m.m12 * m.m23 +
                       m.m01 * m.m22 * m.m13 +
                       m.m02 * m.m11 * m.m23 -
                       m.m02 * m.m21 * m.m13 -
                       m.m03 * m.m11 * m.m22 +
                       m.m03 * m.m21 * m.m12;

            inv.m10 = -m.m10 * m.m22 * m.m33 +
                      m.m10 * m.m32 * m.m23 +
                      m.m12 * m.m20 * m.m33 -
                      m.m12 * m.m30 * m.m23 -
                      m.m13 * m.m20 * m.m32 +
                      m.m13 * m.m30 * m.m22;

            inv.m11 = m.m00 * m.m22 * m.m33 -
                     m.m00 * m.m32 * m.m23 -
                     m.m02 * m.m20 * m.m33 +
                     m.m02 * m.m30 * m.m23 +
                     m.m03 * m.m20 * m.m32 -
                     m.m03 * m.m30 * m.m22;

            inv.m12 = -m.m00 * m.m12 * m.m33 +
                      m.m00 * m.m32 * m.m13 +
                      m.m02 * m.m10 * m.m33 -
                      m.m02 * m.m30 * m.m13 -
                      m.m03 * m.m10 * m.m32 +
                      m.m03 * m.m30 * m.m12;

            inv.m13 = m.m00 * m.m12 * m.m23 -
                      m.m00 * m.m22 * m.m13 -
                      m.m02 * m.m10 * m.m23 +
                      m.m02 * m.m20 * m.m13 +
                      m.m03 * m.m10 * m.m22 -
                      m.m03 * m.m20 * m.m12;

            inv.m20 = m.m10 * m.m21 * m.m33 -
                     m.m10 * m.m31 * m.m23 -
                     m.m11 * m.m20 * m.m33 +
                     m.m11 * m.m30 * m.m23 +
                     m.m13 * m.m20 * m.m31 -
                     m.m13 * m.m30 * m.m21;

            inv.m21 = -m.m00 * m.m21 * m.m33 +
                      m.m00 * m.m31 * m.m23 +
                      m.m01 * m.m20 * m.m33 -
                      m.m01 * m.m30 * m.m23 -
                      m.m03 * m.m20 * m.m31 +
                      m.m03 * m.m30 * m.m21;

            inv.m22 = m.m00 * m.m11 * m.m33 -
                      m.m00 * m.m31 * m.m13 -
                      m.m01 * m.m10 * m.m33 +
                      m.m01 * m.m30 * m.m13 +
                      m.m03 * m.m10 * m.m31 -
                      m.m03 * m.m30 * m.m11;

            inv.m23 = -m.m00 * m.m11 * m.m23 +
                       m.m00 * m.m21 * m.m13 +
                       m.m01 * m.m10 * m.m23 -
                       m.m01 * m.m20 * m.m13 -
                       m.m03 * m.m10 * m.m21 +
                       m.m03 * m.m20 * m.m11;

            inv.m30 = -m.m10 * m.m21 * m.m32 +
                      m.m10 * m.m31 * m.m22 +
                      m.m11 * m.m20 * m.m32 -
                      m.m11 * m.m30 * m.m22 -
                      m.m12 * m.m20 * m.m31 +
                      m.m12 * m.m30 * m.m21;

            inv.m31 = m.m00 * m.m21 * m.m32 -
                     m.m00 * m.m31 * m.m22 -
                     m.m01 * m.m20 * m.m32 +
                     m.m01 * m.m30 * m.m22 +
                     m.m02 * m.m20 * m.m31 -
                     m.m02 * m.m30 * m.m21;

            inv.m32 = -m.m00 * m.m11 * m.m32 +
                       m.m00 * m.m31 * m.m12 +
                       m.m01 * m.m10 * m.m32 -
                       m.m01 * m.m30 * m.m12 -
                       m.m02 * m.m10 * m.m31 +
                       m.m02 * m.m30 * m.m11;

            inv.m33 = m.m00 * m.m11 * m.m22 -
                      m.m00 * m.m21 * m.m12 -
                      m.m01 * m.m10 * m.m22 +
                      m.m01 * m.m20 * m.m12 +
                      m.m02 * m.m10 * m.m21 -
                      m.m02 * m.m20 * m.m11;

            det = m.m00 * inv.m00 + m.m10 * inv[4] + m.m20 * inv[8] + m.m30 * inv[12];

            if (det == 0)
                return m;

            det = 1.0f / det;

            for (i = 0; i < 16; i++)
                inv[i] *= det;


            return inv;
        }
    }
}