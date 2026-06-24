using System;
using System.Numerics;
using Robust.Shared.Maths;

namespace Robust.Shared.Physics;

[Serializable]
public struct Ray(Vector2 position, Vector2 direction) : IEquatable<Ray>
{
	public Vector2 Position = position;

	public Vector2 Direction = direction;

	public readonly bool Intersects(Box2 box, out float distance, out Vector2 hitPos)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		hitPos = Vector2.Zero;
		distance = 0f;
		float y = 0f;
		float y2 = float.MaxValue;
		if (MathF.Abs(Direction.X) < 1E-07f && (Position.X < MathF.Min(box.Left, box.Right) || Position.X > MathF.Max(box.Left, box.Right)))
		{
			return false;
		}
		float num = 1f / Direction.X;
		float num2 = (MathF.Min(box.Left, box.Right) - Position.X) * num;
		float num3 = (MathF.Max(box.Left, box.Right) - Position.X) * num;
		if (num2 > num3)
		{
			MathHelper.Swap(ref num2, ref num3);
		}
		y = MathF.Max(num2, y);
		y2 = MathF.Min(num3, y2);
		if (y > y2)
		{
			return false;
		}
		if (MathF.Abs(Direction.Y) < 1E-07f && (Position.Y < MathF.Min(box.Top, box.Bottom) || Position.Y > MathF.Max(box.Top, box.Bottom)))
		{
			return false;
		}
		float num4 = 1f / Direction.Y;
		float num5 = (MathF.Min(box.Top, box.Bottom) - Position.Y) * num4;
		float num6 = (MathF.Max(box.Top, box.Bottom) - Position.Y) * num4;
		if (num5 > num6)
		{
			MathHelper.Swap(ref num5, ref num6);
		}
		y = MathF.Max(num5, y);
		y2 = MathF.Min(num6, y2);
		if (y > y2)
		{
			return false;
		}
		hitPos = Position + Direction * y;
		distance = y;
		return true;
	}

	public readonly bool Equals(Ray other)
	{
		if (Position.Equals(other.Position))
		{
			return Direction.Equals(other.Direction);
		}
		return false;
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is Ray other)
		{
			return Equals(other);
		}
		return false;
	}

	public override readonly int GetHashCode()
	{
		return (Position.GetHashCode() * 397) ^ Direction.GetHashCode();
	}

	public static bool operator ==(Ray a, Ray b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(Ray a, Ray b)
	{
		return !(a == b);
	}
}
