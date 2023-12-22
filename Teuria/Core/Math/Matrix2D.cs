using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Teuria;

public struct Matrix2D : IEquatable<Matrix2D>
{
    public float M11; // x scale
    public float M12; // rot

    public float M21; // rot
    public float M22; // y scale

    public float M31; // x
    public float M32; // y
    private static Matrix2D identity = new(1f, 0f, 0f, 1f, 0f, 0f);

    public Matrix2D(float m11, float m12, float m21, float m22, float m31, float m32) 
    {
        M11 = m11; M21 = m21; M31 = m31;
        M12 = m12; M22 = m22; M32 = m32;
    }

    public static Matrix2D Identity => identity;

    public Vector2 Translation 
    {
        readonly get => new(M31, M32);
        set { M31 = value.X; M32 = value.Y; } 
    }

    public float Rotation 
    {
        readonly get => (float)Math.Atan2(M21, M11);
        set 
        {
            var cos = (float)Math.Cos(value);
            var sin = (float)Math.Sin(value);

            M11 = cos;
            M12 = sin;
            M21 = -sin;
            M22 = cos;
        }
    }

    public float RotationDegrees 
    {
        readonly get => Rotation * MathUtils.Degrees;
        set => Rotation = value * MathUtils.Radians;
    }

    public Vector2 Scale 
    {
        readonly get => new(M11, M22);
        set { M11 = value.X; M22 = value.Y; }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D Add(Matrix2D m1, Matrix2D m2)
    {
        m1.M11 += m2.M11;
        m1.M12 += m2.M12;

        m1.M21 += m2.M21;
        m1.M22 += m2.M22;

        m1.M31 += m2.M31;
        m1.M32 += m2.M32;

        return m1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Add(ref Matrix2D m1, ref Matrix2D m2, out Matrix2D result)
    {
        result.M11 = m1.M11 += m2.M11;
        result.M12 = m1.M12 += m2.M12;
        result.M21 = m1.M21 += m2.M21;
        result.M22 = m1.M22 += m2.M22;
        result.M31 = m1.M31 += m2.M31;
        result.M32 = m1.M32 += m2.M32;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D Subtract(Matrix2D m1, Matrix2D m2)
    {
        m1.M11 -= m2.M11;
        m1.M12 -= m2.M12;

        m1.M21 -= m2.M21;
        m1.M22 -= m2.M22;

        m1.M31 -= m2.M31;
        m1.M32 -= m2.M32;

        return m1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Substract(ref Matrix2D m1, ref Matrix2D m2, out Matrix2D result)
    {
        result.M11 = m1.M11 -= m2.M11;
        result.M12 = m1.M12 -= m2.M12;
        result.M21 = m1.M21 -= m2.M21;
        result.M22 = m1.M22 -= m2.M22;
        result.M31 = m1.M31 -= m2.M31;
        result.M32 = m1.M32 -= m2.M32;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D Multiply(Matrix2D m1, Matrix2D m2)
    {
        var m11 = m1.M11 * m2.M11 + m1.M12 * m2.M21;
        var m12 = m1.M11 * m2.M12 + m1.M12 * m2.M22;

        var m21 = m1.M21 * m2.M11 + m1.M22 * m2.M21;
        var m22 = m1.M21 * m2.M12 + m1.M22 * m2.M22;

        var m31 = m1.M31 * m2.M11 + m1.M32 * m2.M21 + m2.M31;
        var m32 = m1.M31 * m2.M12 + m1.M32 * m2.M22 + m2.M32;

        m1.M11 = m11;
        m1.M12 = m12;

        m1.M21 = m21;
        m1.M22 = m22;

        m1.M31 = m31;
        m1.M32 = m32;

        return m1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply(ref Matrix2D m1, ref Matrix2D m2, out Matrix2D result)
    {
        var m11 = m1.M11 * m2.M11 + m1.M12 * m2.M21;
        var m12 = m1.M11 * m2.M12 + m1.M12 * m2.M22;

        var m21 = m1.M21 * m2.M11 + m1.M22 * m2.M21;
        var m22 = m1.M21 * m2.M12 + m1.M22 * m2.M22;

        var m31 = m1.M31 * m2.M11 + m1.M32 * m2.M21 + m2.M31;
        var m32 = m1.M31 * m2.M12 + m1.M32 * m2.M22 + m2.M32;

        result.M11 = m11;
        result.M12 = m12;

        result.M21 = m21;
        result.M22 = m22;

        result.M31 = m31;
        result.M32 = m32;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D Multiply(Matrix2D m1, float scale)
    {
        m1.M11 *= scale;
        m1.M12 *= scale;
        m1.M21 *= scale;
        m1.M22 *= scale;
        m1.M31 *= scale;
        m1.M32 *= scale;
        return m1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply(ref Matrix2D m1, float scale, out Matrix2D result)
    {
        result.M11 = m1.M11 *= scale;
        result.M12 = m1.M12 *= scale;
        result.M21 = m1.M21 *= scale;
        result.M22 = m1.M22 *= scale;
        result.M31 = m1.M31 *= scale;
        result.M32 = m1.M32 *= scale;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D Divide(Matrix2D m1, Matrix2D m2)
    {
        m1.M11 = m1.M11 / m2.M11;
        m1.M12 = m1.M12 / m2.M12;

        m1.M21 = m1.M21 / m2.M21;
        m1.M22 = m1.M22 / m2.M22;

        m1.M31 = m1.M31 / m2.M31;
        m1.M32 = m1.M32 / m2.M32;
        return m1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Divide(ref Matrix2D m1, ref Matrix2D m2, out Matrix2D result)
    {
        result.M11 = m1.M11 / m2.M11;
        result.M12 = m1.M12 / m2.M12;

        result.M21 = m1.M21 / m2.M21;
        result.M22 = m1.M22 / m2.M22;

        result.M31 = m1.M31 / m2.M31;
        result.M32 = m1.M32 / m2.M32;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D Divide(Matrix2D m1, float divider)
    {
        float num = 1f / divider;
        m1.M11 = m1.M11 * num;
        m1.M12 = m1.M12 * num;

        m1.M21 = m1.M21 * num;
        m1.M22 = m1.M22 * num;

        m1.M31 = m1.M31 * num;
        m1.M32 = m1.M32 * num;

        return m1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Divide(ref Matrix2D m1, float divider, out Matrix2D result)
    {
        float num = 1f / divider;
        result.M11 = m1.M11 * num;
        result.M12 = m1.M12 * num;

        result.M21 = m1.M21 * num;
        result.M22 = m1.M22 * num;

        result.M31 = m1.M31 * num;
        result.M32 = m1.M32 * num;
    }

#region Translation
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D CreateTranslation(Vector2 position) 
    {
        CreateTranslation(ref position, out Matrix2D result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateTranslation(ref Vector2 position, out Matrix2D result) 
    {
        result = new Matrix2D(1, 0, 0, 1, position.X, position.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D CreateTranslation(float x, float y) 
    {
        CreateTranslation(x, y, out Matrix2D result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateTranslation(float x, float y, out Matrix2D result) 
    {
        result = new Matrix2D(1, 0, 0, 1, x, y);
    }
#endregion

#region Rotation
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D CreateRotation(float radians) 
    {
        CreateRotation(radians, out Matrix2D result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotation(float radians, out Matrix2D result) 
    {
        result = identity;

        var cos = (float)Math.Cos(radians);
        var sin = (float)Math.Sin(radians);

        result.M11 = cos;
        result.M12 = sin;
        result.M21 = -sin;
        result.M22 = cos;
    }
#endregion

#region Scale
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D CreateScale(Vector2 scale) 
    {
        CreateScale(ref scale, out Matrix2D result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateScale(ref Vector2 scale, out Matrix2D result) 
    {
        result.M11 = scale.X;
        result.M12 = 0;

        result.M21 = 0;
        result.M22 = scale.Y;

        result.M31 = 0;
        result.M32 = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D CreateScale(float scale) 
    {
        CreateScale(scale, scale, out Matrix2D result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateScale(float scale, out Matrix2D result) 
    {
        CreateScale(scale, scale, out result);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D CreateScale(float xScale, float yScale) 
    {
        CreateScale(xScale, yScale, out Matrix2D result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateScale(float xScale, float yScale, out Matrix2D result) 
    {
        result.M11 = xScale;
        result.M12 = 0;

        result.M21 = 0;
        result.M22 = yScale;

        result.M31 = 0;
        result.M32 = 0;
    }
#endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly float Determinant() => M11 * M22 - M12 * M21;    


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Invert(ref Matrix2D matrix, out Matrix2D result) 
    {
        var det = 1f/  matrix.Determinant();

        result.M11 = matrix.M22 * det;
        result.M12 = -matrix.M12 * det;

        result.M21 = -matrix.M21 * det;
        result.M22 = matrix.M11 * det;

        result.M31 = (matrix.M32 * matrix.M21 - matrix.M31 * matrix.M22) * det;
        result.M32 = -(matrix.M32 * matrix.M11 - matrix.M31 * matrix.M12) * det;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D Lerp(Matrix2D m1, Matrix2D m2, float delta)
    {
        m1.M11 += (m2.M11 - m1.M11) * delta;
        m1.M12 += (m2.M12 - m1.M12) * delta;

        m1.M21 += (m2.M21 - m1.M21) * delta;
        m1.M22 += (m2.M22 - m1.M22) * delta;

        m1.M31 += (m2.M31 - m1.M31) * delta;
        m1.M32 += (m2.M32 - m1.M32) * delta;
        return m1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Lerp(ref Matrix2D m1, ref Matrix2D m2, float delta, out Matrix2D result)
    {
        result.M11 = m1.M11 + ((m2.M11 - m1.M11) * delta);
        result.M12 = m1.M12 + ((m2.M12 - m1.M12) * delta);

        result.M21 = m1.M21 + ((m2.M21 - m1.M21) * delta);
        result.M22 = m1.M22 + ((m2.M22 - m1.M22) * delta);

        result.M31 = m1.M31 + ((m2.M31 - m1.M31) * delta);
        result.M32 = m1.M32 + ((m2.M32 - m1.M32) * delta);
    }

    public static Matrix2D Transpose(Matrix2D mat) 
    {
        Transpose(ref mat, out Matrix2D result);
        return result;
    }


    public static void Transpose(ref Matrix2D mat, out Matrix2D result) 
    {
        result.M11 = mat.M11;
        result.M12 = mat.M21;

        result.M21 = mat.M12;
        result.M22 = mat.M22;

        result.M31 = result.M32 = 0;
    }

    public override int GetHashCode()
    {
        return M11.GetHashCode() +
            M12.GetHashCode() +
            M21.GetHashCode() +
            M22.GetHashCode() +
            M31.GetHashCode() +
            M32.GetHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D operator +(Matrix2D m1, Matrix2D m2)
    {
        m1.M11 += m2.M11;
        m1.M12 += m2.M12;

        m1.M21 += m2.M21;
        m1.M22 += m2.M22;

        m1.M31 += m2.M31;
        m1.M32 += m2.M32;
        return m1;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D operator -(Matrix2D m1, Matrix2D m2)
    {
        m1.M11 -= m2.M11;
        m1.M12 -= m2.M12;

        m1.M21 -= m2.M21;
        m1.M22 -= m2.M22;

        m1.M31 -= m2.M31;
        m1.M32 -= m2.M32;
        return m1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D operator -(Matrix2D mat)
    {
        mat.M11 = -mat.M11;
        mat.M12 = -mat.M12;

        mat.M21 = -mat.M21;
        mat.M22 = -mat.M22;

        mat.M31 = -mat.M31;
        mat.M32 = -mat.M32;
        return mat;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D operator *(Matrix2D m1, Matrix2D m2)
    {
        var m11 = m1.M11 * m2.M11 + m1.M12 * m2.M21;
        var m12 = m1.M11 * m2.M12 + m1.M12 * m2.M22;

        var m21 = m1.M21 * m2.M11 + m1.M22 * m2.M21;
        var m22 = m1.M21 * m2.M12 + m1.M22 * m2.M22;

        var m31 = m1.M31 * m2.M11 + m1.M32 * m2.M21 + m2.M31;
        var m32 = m1.M31 * m2.M12 + m1.M32 * m2.M22 + m2.M32;

        m1.M11 = m11;
        m1.M12 = m12;

        m1.M21 = m21;
        m1.M22 = m22;

        m1.M31 = m31;
        m1.M32 = m32;

        return m1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D operator /(Matrix2D mat, float divider)
    {
        float num = 1f / divider;
        mat.M11 = mat.M11 * num;
        mat.M12 = mat.M12 * num;

        mat.M21 = mat.M21 * num;
        mat.M22 = mat.M22 * num;

        mat.M31 = mat.M31 * num;
        mat.M32 = mat.M32 * num;
        return mat;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D operator /(Matrix2D m1, Matrix2D m2)
    {
        m1.M11 = m1.M11 / m2.M11;
        m1.M12 = m1.M12 / m2.M12;

        m1.M21 = m1.M21 / m2.M21;
        m1.M22 = m1.M22 / m2.M22;

        m1.M31 = m1.M31 / m2.M31;
        m1.M32 = m1.M32 / m2.M32;
        return m1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D operator *(Matrix2D matrix, float scale)
    {
        matrix.M11 = matrix.M11 * scale;
        matrix.M12 = matrix.M12 * scale;

        matrix.M21 = matrix.M21 * scale;
        matrix.M22 = matrix.M22 * scale;

        matrix.M31 = matrix.M31 * scale;
        matrix.M32 = matrix.M32 * scale;
        return matrix;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Matrix2D m1, Matrix2D m2)
    {
        return (
            m1.M11 == m2.M11 &&
            m1.M12 == m2.M12 &&
            m1.M21 == m2.M21 &&
            m1.M22 == m2.M22 &&
            m1.M31 == m2.M31 &&
            m1.M32 == m2.M32
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Matrix2D m1, Matrix2D m2)
    {
        return (
            m1.M11 != m2.M11 ||
            m1.M12 != m2.M12 ||
            m1.M21 != m2.M21 ||
            m1.M22 != m2.M22 ||
            m1.M31 != m2.M31 ||
            m1.M32 != m2.M32
        );
    }


    public static implicit operator Matrix(Matrix2D mat) 
    {
        return new Matrix(
            mat.M11, mat.M12, 0, 0,
            mat.M21, mat.M22, 0, 0,
            0, 0, 1, 0,
            mat.M31, mat.M32, 0, 1
        );
    }

    public readonly bool Equals(Matrix2D other)
    {
        return (
            M11 == other.M11 &&
            M12 == other.M12 &&
            M21 == other.M21 &&
            M22 == other.M22 &&
            M31 == other.M31 &&
            M32 == other.M32
        );
    }

    public override readonly string ToString()
    {
        return "{ M11:" + M11 + " M12:" + M12 + " }"
                + " { M21:" + M21 + " M22:" + M22 + " }"
                + " { M31:" + M31 + " M32:" + M32 + " }";
    }

    public override readonly bool Equals(object? obj)
    {
        if (obj is Matrix2D mat)
            return Equals(mat);
        return false;
    }
}