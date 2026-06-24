using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Robust.Shared.Map;

[Serializable]
public readonly struct Tile : IEquatable<Tile>, ISpanFormattable, IFormattable
{
	public readonly int TypeId;

	public readonly byte Flags;

	public readonly byte Variant;

	public readonly byte RotationMirroring;

	public static readonly Tile Empty = new Tile(0, 0, 0, 0);

	public bool IsEmpty => TypeId == 0;

	public Tile(int typeId, byte flags = 0, byte variant = 0, byte rotationMirroring = 0)
	{
		TypeId = typeId;
		Flags = flags;
		Variant = variant;
		RotationMirroring = rotationMirroring;
	}

	public static byte DirectionToByte(Direction direction, bool throwIfDiagonal = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected I4, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected I4, but got Unknown
		switch ((int)direction)
		{
		case 0:
			return 0;
		case 2:
			return 1;
		case 4:
			return 2;
		case 6:
			return 3;
		default:
			if (throwIfDiagonal)
			{
				throw new InvalidEnumArgumentException("direction", (int)direction, typeof(Direction));
			}
			return 0;
		}
	}

	public static bool operator ==(Tile a, Tile b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(Tile a, Tile b)
	{
		return !a.Equals(b);
	}

	public override string ToString()
	{
		return $"Tile {TypeId}, {Flags}, {Variant}, {RotationMirroring}";
	}

	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		return ToString();
	}

	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
	{
		Unsafe.SkipInit(out BufferInterpolatedStringHandler val);
		((BufferInterpolatedStringHandler)(ref val))._002Ector(9, 3, destination);
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral("Tile ");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<int>(TypeId);
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral(", ");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<byte>(Flags);
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral(", ");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<byte>(Variant);
		return FormatHelpers.TryFormatInto(destination, ref charsWritten, ref val);
	}

	public bool Equals(Tile other)
	{
		if (TypeId == other.TypeId && Flags == other.Flags && Variant == other.Variant)
		{
			return RotationMirroring == other.RotationMirroring;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is Tile other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (TypeId.GetHashCode() * 397) ^ Flags.GetHashCode() ^ Variant.GetHashCode() ^ RotationMirroring.GetHashCode();
	}

	public Tile WithVariant(byte variant)
	{
		return new Tile(TypeId, Flags, variant, 0);
	}

	public Tile WithFlag(byte flag)
	{
		return new Tile(TypeId, flag, Variant, 0);
	}
}
