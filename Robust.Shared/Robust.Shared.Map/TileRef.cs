using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Robust.Shared.Map;

public readonly struct TileRef : IEquatable<TileRef>, ISpanFormattable, IFormattable
{
	public readonly EntityUid GridUid;

	public readonly Vector2i GridIndices;

	public readonly Tile Tile;

	public static TileRef Zero => new TileRef(EntityUid.Invalid, Vector2i.Zero, Tile.Empty);

	public int X => GridIndices.X;

	public int Y => GridIndices.Y;

	internal TileRef(EntityUid gridUid, int xIndex, int yIndex, Tile tile)
		: this(gridUid, new Vector2i(xIndex, yIndex), tile)
	{
	}//IL_0004: Unknown result type (might be due to invalid IL or missing references)


	internal TileRef(EntityUid gridUid, Vector2i gridIndices, Tile tile)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		GridUid = gridUid;
		GridIndices = gridIndices;
		Tile = tile;
	}

	public override string ToString()
	{
		return $"TileRef: {X},{Y} ({Tile})";
	}

	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		return ToString();
	}

	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
	{
		Unsafe.SkipInit(out BufferInterpolatedStringHandler val);
		((BufferInterpolatedStringHandler)(ref val))._002Ector(13, 3, destination);
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral("TileRef: ");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<int>(X);
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral(",");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<int>(Y);
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral(" (");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<Tile>(Tile);
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral(")");
		return FormatHelpers.TryFormatInto(destination, ref charsWritten, ref val);
	}

	public bool Equals(TileRef other)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (GridUid.Equals(other.GridUid) && ((Vector2i)(ref GridIndices)).Equals(other.GridIndices))
		{
			return Tile.Equals(other.Tile);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is TileRef other)
		{
			return Equals(other);
		}
		return false;
	}

	public static bool operator ==(TileRef a, TileRef b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(TileRef a, TileRef b)
	{
		return !a.Equals(b);
	}

	public override int GetHashCode()
	{
		return (((GridUid.GetHashCode() * 397) ^ ((object)Unsafe.As<Vector2i, Vector2i>(ref GridIndices)/*cast due to constrained. prefix*/).GetHashCode()) * 397) ^ Tile.GetHashCode();
	}
}
