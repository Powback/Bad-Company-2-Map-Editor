using UnityEngine;
using System.Collections;

public class MatrixHelper {
    
    	/// <summary>
    /// Extract translation from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Translation offset.
    /// </returns>
    public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix) {
        Vector3 translate;
        translate.x = matrix.m03;
        translate.y = matrix.m13;
        translate.z = matrix.m23;
        return translate;
    }
     
    /// <summary>
    /// Extract rotation quaternion from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Quaternion representation of rotation transform.
    /// </returns>

	public static Quaternion QuatFromMatrix(ref Matrix4x4 m) {
		Quaternion q = new Quaternion ();
		q.w = Mathf.Sqrt (Mathf.Max (0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
		q.x = Mathf.Sqrt (Mathf.Max (0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
		q.y = Mathf.Sqrt (Mathf.Max (0, 1 - m[0, 0] + m[1, 1] + m[2, 2])) / 2;
		q.z = Mathf.Sqrt (Mathf.Max (0, 1 - m[0, 0] - m[1, 1] - m[2, 2])) / 2;

		q.x *= Mathf.Sign (q.x * (m [2, 1] - m [1, 2]));
		q.y *= Mathf.Sign (q.y * (m [0, 2] - m [2, 0]));
		q.z *= Mathf.Sign (q.z * (m [1,0] - m [0,1]));
		return q;
	}
    public static Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix) {
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
     
    /// <summary>
    /// Extract scale from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Scale vector.
    /// </returns>
    public static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix) {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }
     
    /// <summary>
    /// Extract position, rotation and scale from TRS matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <param name="localPosition">Output position.</param>
    /// <param name="localRotation">Output rotation.</param>
    /// <param name="localScale">Output scale.</param>
    public static void DecomposeMatrix(ref Matrix4x4 matrix, out Vector3 localPosition, out Quaternion localRotation, out Vector3 localScale) {
        localPosition = ExtractTranslationFromMatrix(ref matrix);
        localRotation = ExtractRotationFromMatrix(ref matrix);
        localScale = ExtractScaleFromMatrix(ref matrix);
    }
     
    /// <summary>
    /// Set transform component from TRS matrix.
    /// </summary>
    /// <param name="transform">Transform component.</param>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    public static void SetTransformFromMatrix(Transform transform, ref Matrix4x4 matrix) {
        //transform.localPosition = ExtractTranslationFromMatrix(ref matrix);
        transform.localRotation = ExtractRotationFromMatrix(ref matrix);
        //transform.localScale = ExtractScaleFromMatrix(ref matrix);
		//transform.localRotation = QuatFromMatrix (ref matrix);
    }
     
     
    // EXTRAS!
     
    /// <summary>
    /// Identity quaternion.
    /// </summary>
    /// <remarks>
    /// <para>It is faster to access this variation than <c>Quaternion.identity</c>.</para>
    /// </remarks>
    public static readonly Quaternion IdentityQuaternion = Quaternion.identity;
    /// <summary>
    /// Identity matrix.
    /// </summary>
    /// <remarks>
    /// <para>It is faster to access this variation than <c>Matrix4x4.identity</c>.</para>
    /// </remarks>
    public static readonly Matrix4x4 IdentityMatrix = Matrix4x4.identity;
     
    /// <summary>
    /// Get translation matrix.
    /// </summary>
    /// <param name="offset">Translation offset.</param>
    /// <returns>
    /// The translation transform matrix.
    /// </returns>
    public static Matrix4x4 TranslationMatrix(Vector3 offset) {
        Matrix4x4 matrix = IdentityMatrix;
        matrix.m03 = offset.x;
        matrix.m13 = offset.y;
        matrix.m23 = offset.z;
        return matrix;
    }
}