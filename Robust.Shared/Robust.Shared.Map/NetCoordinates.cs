using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Robust.Shared.Map;

[Serializable]
[NetSerializable]
public readonly struct NetCoordinates : IEquatable<NetCoordinates>, ISpanFormattable, IFormattable
{
	public static readonly NetCoordinates Invalid;

	public readonly NetEntity NetEntity;

	public readonly Vector2 Position;

	public float X => Position.X;

	public float Y => Position.Y;

	public NetCoordinates(NetEntity netEntity, Vector2 position)
	{
		NetEntity = netEntity;
		Position = position;
	}

	public NetCoordinates(NetEntity netEntity, float x, float y)
	{
		NetEntity = netEntity;
		Position = new Vector2(x, y);
	}

	public bool Equals(NetCoordinates other)
	{
		if (NetEntity.Equals(other.NetEntity))
		{
			return Position.Equals(other.Position);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is NetCoordinates other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(NetEntity, Position);
	}

	public void Deconstruct(out NetEntity entId, out Vector2 localPos)
	{
		entId = NetEntity;
		localPos = Position;
	}

	public override string ToString()
	{
		return $"NetEntity={NetEntity}, X={Position.X:N2}, Y={Position.Y:N2}";
	}

	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		return ToString();
	}

	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
	{
		Unsafe.SkipInit(out BufferInterpolatedStringHandler val);
		((BufferInterpolatedStringHandler)(ref val))._002Ector(18, 3, destination);
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral("NetEntity=");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<NetEntity>(NetEntity);
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral(", X=");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<float>(Position.X, "N2");
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral(", Y=");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<float>(Position.Y, "N2");
		return FormatHelpers.TryFormatInto(destination, ref charsWritten, ref val);
	}

	static NetCoordinates()
	{
		Invalid = new NetCoordinates(NetEntity.Invalid, Vector2.Zero);
	}
}
