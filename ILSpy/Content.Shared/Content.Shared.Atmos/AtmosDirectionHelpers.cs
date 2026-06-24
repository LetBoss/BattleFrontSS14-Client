using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;

namespace Content.Shared.Atmos;

public static class AtmosDirectionHelpers
{
	public static AtmosDirection GetOpposite(this AtmosDirection direction)
	{
		return direction switch
		{
			AtmosDirection.North => AtmosDirection.South, 
			AtmosDirection.South => AtmosDirection.North, 
			AtmosDirection.East => AtmosDirection.West, 
			AtmosDirection.West => AtmosDirection.East, 
			AtmosDirection.NorthEast => AtmosDirection.SouthWest, 
			AtmosDirection.NorthWest => AtmosDirection.SouthEast, 
			AtmosDirection.SouthEast => AtmosDirection.NorthWest, 
			AtmosDirection.SouthWest => AtmosDirection.NorthEast, 
			_ => throw new ArgumentOutOfRangeException("direction"), 
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ToOppositeIndex(this int index)
	{
		return index ^ 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static AtmosDirection ToOppositeDir(this int index)
	{
		return (AtmosDirection)(1 << (index ^ 1));
	}

	public static Direction ToDirection(this AtmosDirection direction)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		return (Direction)(direction switch
		{
			AtmosDirection.North => 4, 
			AtmosDirection.South => 0, 
			AtmosDirection.East => 2, 
			AtmosDirection.West => 6, 
			AtmosDirection.NorthEast => 3, 
			AtmosDirection.NorthWest => 5, 
			AtmosDirection.SouthEast => 1, 
			AtmosDirection.SouthWest => 7, 
			AtmosDirection.Invalid => -1, 
			_ => throw new ArgumentOutOfRangeException("direction"), 
		});
	}

	public static AtmosDirection ToAtmosDirection(this Direction direction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected I4, but got Unknown
		return (direction - -1) switch
		{
			5 => AtmosDirection.North, 
			1 => AtmosDirection.South, 
			3 => AtmosDirection.East, 
			7 => AtmosDirection.West, 
			4 => AtmosDirection.NorthEast, 
			6 => AtmosDirection.NorthWest, 
			2 => AtmosDirection.SouthEast, 
			8 => AtmosDirection.SouthWest, 
			0 => AtmosDirection.Invalid, 
			_ => throw new ArgumentOutOfRangeException("direction"), 
		};
	}

	public static Angle ToAngle(this AtmosDirection direction)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		return (Angle)(direction switch
		{
			AtmosDirection.South => Angle.Zero, 
			AtmosDirection.East => new Angle(1.5707963705062866), 
			AtmosDirection.North => new Angle(Math.PI), 
			AtmosDirection.West => new Angle(-1.5707963705062866), 
			AtmosDirection.NorthEast => new Angle(Math.PI * 3.0 / 4.0), 
			AtmosDirection.NorthWest => new Angle(Math.PI * -3.0 / 4.0), 
			AtmosDirection.SouthWest => new Angle(-0.7853981852531433), 
			AtmosDirection.SouthEast => new Angle(0.7853981852531433), 
			_ => throw new ArgumentOutOfRangeException("direction", $"It was {direction}."), 
		});
	}

	public static AtmosDirection ToAtmosDirectionCardinal(this Angle angle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return ((Angle)(ref angle)).GetCardinalDir().ToAtmosDirection();
	}

	public static AtmosDirection ToAtmosDirection(this Angle angle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return ((Angle)(ref angle)).GetDir().ToAtmosDirection();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ToIndex(this AtmosDirection direction)
	{
		return BitOperations.Log2((uint)direction);
	}

	public static AtmosDirection WithFlag(this AtmosDirection direction, AtmosDirection other)
	{
		return direction | other;
	}

	public static AtmosDirection WithoutFlag(this AtmosDirection direction, AtmosDirection other)
	{
		return direction & ~other;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsFlagSet(this AtmosDirection direction, AtmosDirection other)
	{
		return (direction & other) == other;
	}

	public static Vector2i CardinalToIntVec(this AtmosDirection dir)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		return (Vector2i)(dir switch
		{
			AtmosDirection.North => new Vector2i(0, 1), 
			AtmosDirection.East => new Vector2i(1, 0), 
			AtmosDirection.South => new Vector2i(0, -1), 
			AtmosDirection.West => new Vector2i(-1, 0), 
			_ => throw new ArgumentException($"Direction dir {dir} is not a cardinal direction", "dir"), 
		});
	}

	public static Vector2i Offset(this Vector2i pos, AtmosDirection dir)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return pos + dir.CardinalToIntVec();
	}
}
