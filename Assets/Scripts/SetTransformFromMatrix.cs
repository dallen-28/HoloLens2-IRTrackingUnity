using System;
using UnityEngine;

public class SetTransformFromMatrix : MonoBehaviour
{
    public Vector4 column1;
    public Vector4 column2;
    public Vector4 column3;
    public Vector4 column4;

    public Matrix4x4 targetMatrix;

    void Awake()
    {
        targetMatrix = Matrix4x4.identity;

        targetMatrix.SetColumn(0, column1);
        targetMatrix.SetColumn(1, column2);
        targetMatrix.SetColumn(2, column3);
        targetMatrix.SetColumn(3, column4);

        //targetMatrix = MatrixExtensions.FlipTransformRightLeft(targetMatrix);


        this.transform.SetPositionAndRotation(targetMatrix.ExtractPosition(), targetMatrix.ExtractRotation());
    }
}
