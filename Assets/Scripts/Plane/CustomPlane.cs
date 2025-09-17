using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CustomMath
{
public class CustomPlane : MonoBehaviour
{
    internal const int size = 16;

    private Vector3 m_Normal;

    private float m_Distance;

    //
    // Resumen:
    //     Normal vector of the plane.
    public Vector3 normal
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return m_Normal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            m_Normal = value;
        }
    }

    //
    // Resumen:
    //     The distance measured from the Plane to the origin, along the Plane's normal.
    public float distance
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return m_Distance;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            m_Distance = value;
        }
    }

    //
    // Resumen:
    //     Returns a copy of the plane that faces in the opposite direction.
    public Plane flipped
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return new Plane(-m_Normal, 0f - m_Distance);
        }
    }

    //
    // Resumen:
    //     Creates a plane.
    //
    // Parámetros:
    //   inNormal:
    //
    //   inPoint:
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Plane(Vector3 inNormal, Vector3 inPoint)
    {
        m_Normal = Vector3.Normalize(inNormal);
        m_Distance = 0f - Vector3.Dot(m_Normal, inPoint);
    }

    //
    // Resumen:
    //     Creates a plane.
    //
    // Parámetros:
    //   inNormal:
    //
    //   d:
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Plane(Vector3 inNormal, float d)
    {
        m_Normal = Vector3.Normalize(inNormal);
        m_Distance = d;
    }

    //
    // Resumen:
    //     Creates a plane.
    //
    // Parámetros:
    //   a:
    //
    //   b:
    //
    //   c:
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Plane(Vector3 a, Vector3 b, Vector3 c)
    {
        m_Normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
        m_Distance = 0f - Vector3.Dot(m_Normal, a);
    }

    //
    // Resumen:
    //     Sets a plane using a point that lies within it along with a normal to orient
    //     it.
    //
    // Parámetros:
    //   inNormal:
    //     The plane's normal vector.
    //
    //   inPoint:
    //     A point that lies on the plane.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetNormalAndPosition(Vector3 inNormal, Vector3 inPoint)
    {
        m_Normal = Vector3.Normalize(inNormal);
        m_Distance = 0f - Vector3.Dot(m_Normal, inPoint);
    }

    //
    // Resumen:
    //     Sets a plane using three points that lie within it. The points go around clockwise
    //     as you look down on the top surface of the plane.
    //
    // Parámetros:
    //   a:
    //     First point in clockwise order.
    //
    //   b:
    //     Second point in clockwise order.
    //
    //   c:
    //     Third point in clockwise order.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set3Points(Vector3 a, Vector3 b, Vector3 c)
    {
        m_Normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
        m_Distance = 0f - Vector3.Dot(m_Normal, a);
    }

    //
    // Resumen:
    //     Makes the plane face in the opposite direction.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Flip()
    {
        m_Normal = -m_Normal;
        m_Distance = 0f - m_Distance;
    }

    //
    // Resumen:
    //     Moves the plane in space by the translation vector.
    //
    // Parámetros:
    //   translation:
    //     The offset in space to move the plane with.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Translate(Vector3 translation)
    {
        m_Distance += Vector3.Dot(m_Normal, translation);
    }

    //
    // Resumen:
    //     Returns a copy of the given plane that is moved in space by the given translation.
    //
    //
    // Parámetros:
    //   plane:
    //     The plane to move in space.
    //
    //   translation:
    //     The offset in space to move the plane with.
    //
    // Devuelve:
    //     The translated plane.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Plane Translate(Plane plane, Vector3 translation)
    {
        return new Plane(plane.m_Normal, plane.m_Distance += Vector3.Dot(plane.m_Normal, translation));
    }

    //
    // Resumen:
    //     For a given point returns the closest point on the plane.
    //
    // Parámetros:
    //   point:
    //     The point to project onto the plane.
    //
    // Devuelve:
    //     A point on the plane that is closest to point.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 ClosestPointOnPlane(Vector3 point)
    {
        float num = Vector3.Dot(m_Normal, point) + m_Distance;
        return point - m_Normal * num;
    }

    //
    // Resumen:
    //     Returns a signed distance from plane to point.
    //
    // Parámetros:
    //   point:
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float GetDistanceToPoint(Vector3 point)
    {
        return Vector3.Dot(m_Normal, point) + m_Distance;
    }

    //
    // Resumen:
    //     Is a point on the positive side of the plane?
    //
    // Parámetros:
    //   point:
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GetSide(Vector3 point)
    {
        return Vector3.Dot(m_Normal, point) + m_Distance > 0f;
    }

    //
    // Resumen:
    //     Are two points on the same side of the plane?
    //
    // Parámetros:
    //   inPt0:
    //
    //   inPt1:
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool SameSide(Vector3 inPt0, Vector3 inPt1)
    {
        float distanceToPoint = GetDistanceToPoint(inPt0);
        float distanceToPoint2 = GetDistanceToPoint(inPt1);
        return (distanceToPoint > 0f && distanceToPoint2 > 0f) || (distanceToPoint <= 0f && distanceToPoint2 <= 0f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Raycast(Ray ray, out float enter)
    {
        float num = Vector3.Dot(ray.direction, m_Normal);
        float num2 = 0f - Vector3.Dot(ray.origin, m_Normal) - m_Distance;
        if (Mathf.Approximately(num, 0f))
        {
            enter = 0f;
            return false;
        }

        enter = num2 / num;
        return enter > 0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
        return ToString(null, null);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(string format)
    {
        return ToString(format, null);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(string format, IFormatProvider formatProvider)
    {
        if (string.IsNullOrEmpty(format))
        {
            format = "F2";
        }

        if (formatProvider == null)
        {
            formatProvider = CultureInfo.InvariantCulture.NumberFormat;
        }

        return UnityString.Format("(normal:{0}, distance:{1})", m_Normal.ToString(format, formatProvider), m_Distance.ToString(format, formatProvider));
    }
}

}

