using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Robust.Shared.Map;

[Serializable]
[DataRecord]
[NetSerializable]
public readonly record struct MapCoordinates : ISpanFormattable, IFormattable
{
	public float X => Position.X;

	public float Y => Position.Y;

	public static readonly MapCoordinates Nullspace = new MapCoordinates(Vector2.Zero, MapId.Nullspace);

	public readonly Vector2 Position;

	public readonly MapId MapId;

	public MapCoordinates(Vector2 position, MapId mapId)
	{
		Position = position;
		MapId = mapId;
	}

	public MapCoordinates(float x, float y, MapId mapId)
		: this(new Vector2(x, y), mapId)
	{
	}

	public override string ToString()
	{
		return $"Map={MapId}, X={Position.X:N2}, Y={Position.Y:N2}";
	}

	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		return ToString();
	}

	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
	{
		Unsafe.SkipInit(out BufferInterpolatedStringHandler val);
		((BufferInterpolatedStringHandler)(ref val))._002Ector(12, 3, destination);
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral("Map=");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<MapId>(MapId);
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral(", X=");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<float>(Position.X, "N2");
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral(", Y=");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<float>(Position.Y, "N2");
		return FormatHelpers.TryFormatInto(destination, ref charsWritten, ref val);
	}

	public bool InRange(MapCoordinates otherCoords, float range)
	{
		if (otherCoords.MapId != MapId)
		{
			return false;
		}
		return (otherCoords.Position - Position).LengthSquared() < range * range;
	}

	public void Deconstruct(out float x, out float y)
	{
		x = X;
		y = Y;
	}

	public void Deconstruct(out MapId mapId, out float x, out float y)
	{
		mapId = MapId;
		x = X;
		y = Y;
	}

	public MapCoordinates Offset(Vector2 offset)
	{
		return new MapCoordinates(Position + offset, MapId);
	}

	public MapCoordinates Offset(float x, float y)
	{
		return Offset(new Vector2(x, y));
	}

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append("Position = ");
		builder.Append(Position.ToString());
		builder.Append(", MapId = ");
		builder.Append(MapId.ToString());
		builder.Append(", X = ");
		builder.Append(X.ToString());
		builder.Append(", Y = ");
		builder.Append(Y.ToString());
		return true;
	}
}
