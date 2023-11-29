using UnityEngine;

public static class MatrixExtensions
{
    public static Matrix4x4 FlipTransformRightLeft(Matrix4x4 matr)
    {
        matr.m20 = matr.m20 * -1.0f;
        matr.m02 = matr.m02 * -1.0f;
        matr.m21 = matr.m21 * -1.0f;
        matr.m12 = matr.m12 * -1.0f;
        matr.m23 = matr.m23 * -1.0f;
        return matr;
    }
    public static Quaternion ExtractRotation(this Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;

        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;

        return Quaternion.LookRotation(forward, upwards);
    }
    public static Vector3 ExtractPosition(this Matrix4x4 matrix)
    {
        Vector3 position;
        position.x = matrix.m03;
        position.y = matrix.m13;
        position.z = matrix.m23;
        return position;
    }
}
