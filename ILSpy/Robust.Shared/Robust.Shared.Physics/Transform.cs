using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using Robust.Shared.Maths;

namespace Robust.Shared.Physics;

public struct Transform
{
	public static readonly Transform Empty;

	public Vector2 Position;

	public Quaternion2D Quaternion2D;

	public Transform(Vector2 position, Quaternion2D quat)
	{
		Position = position;
		Quaternion2D = quat;
	}

	public Transform(Vector2 position, float angle)
	{
		Position = position;
		Quaternion2D = new Quaternion2D(angle);
	}

	public Transform(float angle)
	{
		Position = Vector2.Zero;
		Quaternion2D = new Quaternion2D(angle);
	}

	public Transform(Vector2 position, Angle angle)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Position = position;
		Quaternion2D = new Quaternion2D(angle);
	}

	public static Vector2 InvTransformPoint(Transform t, Vector2 p)
	{
		float num = p.X - t.Position.X;
		float num2 = p.Y - t.Position.Y;
		return new Vector2(t.Quaternion2D.C * num + t.Quaternion2D.S * num2, (0f - t.Quaternion2D.S) * num + t.Quaternion2D.C * num2);
	}

	public static Vector2 Mul(in Transform transform, in Vector2 vector)
	{
		float x = transform.Quaternion2D.C * vector.X - transform.Quaternion2D.S * vector.Y + transform.Position.X;
		float y = transform.Quaternion2D.S * vector.X + transform.Quaternion2D.C * vector.Y + transform.Position.Y;
		return new Vector2(x, y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void MulSimd(in Transform transform, Vector128<float> x, Vector128<float> y, out Vector128<float> xOut, out Vector128<float> yOut)
	{
		Vector128<float> vector = Vector128.Create(transform.Quaternion2D.C);
		Vector128<float> vector2 = Vector128.Create(transform.Quaternion2D.S);
		xOut = vector * x - vector2 * y + Vector128.Create(transform.Position.X);
		yOut = vector2 * x + vector * y + Vector128.Create(transform.Position.Y);
	}

	public static Vector2 MulT(in Vector2[] A, in Vector2 v)
	{
		return new Vector2(v.X * A[0].X + v.Y * A[0].Y, v.X * A[1].X + v.Y * A[1].Y);
	}

	public static Vector2 MulT(in Transform T, in Vector2 v)
	{
		float num = v.X - T.Position.X;
		float num2 = v.Y - T.Position.Y;
		float x = T.Quaternion2D.C * num + T.Quaternion2D.S * num2;
		float y = (0f - T.Quaternion2D.S) * num + T.Quaternion2D.C * num2;
		return new Vector2(x, y);
	}

	public static Quaternion2D MulT(in Quaternion2D q, in Quaternion2D r)
	{
		Unsafe.SkipInit(out Quaternion2D result);
		result.S = q.C * r.S - q.S * r.C;
		result.C = q.C * r.C + q.S * r.S;
		return result;
	}

	public static Transform InvMulTransforms(in Transform A, in Transform B)
	{
		return new Transform(Quaternion2D.InvRotateVector(A.Quaternion2D, Vector2.Subtract(B.Position, A.Position)), Quaternion2D.InvMulRot(A.Quaternion2D, B.Quaternion2D));
	}

	public static Transform MulT(in Transform A, in Transform B)
	{
		return new Transform
		{
			Quaternion2D = MulT(in A.Quaternion2D, in B.Quaternion2D),
			Position = MulT(A.Quaternion2D, B.Position - A.Position)
		};
	}

	public static Vector2 MulT(Quaternion2D q, Vector2 v)
	{
		return new Vector2(q.C * v.X + q.S * v.Y, (0f - q.S) * v.X + q.C * v.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 Mul(in Quaternion2D quaternion2D, in Vector2 vector)
	{
		return new Vector2(quaternion2D.C * vector.X - quaternion2D.S * vector.Y, quaternion2D.S * vector.X + quaternion2D.C * vector.Y);
	}

	public static Vector2 Mul(Vector4 A, Vector2 v)
	{
		return new Vector2(A.X * v.X + A.Y * v.Y, A.Z * v.X + A.W * v.Y);
	}

	public static Vector2 Mul(in Vector2[] A, in Vector2 v)
	{
		return new Vector2(A[0].X * v.X + A[1].X * v.Y, A[0].Y * v.X + A[1].Y * v.Y);
	}

	public static Vector2 Mul(Matrix22 A, Vector2 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(A.EX.X * v.X + A.EY.X * v.Y, A.EX.Y * v.X + A.EY.Y * v.Y);
	}

	static Transform()
	{
		Empty = new Transform(0f);
	}
}
