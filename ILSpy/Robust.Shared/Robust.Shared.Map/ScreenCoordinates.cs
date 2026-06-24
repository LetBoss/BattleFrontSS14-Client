using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Robust.Shared.Map;

[Serializable]
[NetSerializable]
public readonly struct ScreenCoordinates : IEquatable<ScreenCoordinates>, ISpanFormattable, IFormattable
{
	public static readonly ScreenCoordinates Invalid;

	public readonly Vector2 Position;

	public readonly WindowId Window;

	public float X => Position.X;

	public float Y => Position.Y;

	public bool IsValid => Window != WindowId.Invalid;

	public ScreenCoordinates(Vector2 position, WindowId window)
	{
		Position = position;
		Window = window;
	}

	public ScreenCoordinates(float x, float y, WindowId window)
	{
		Position = new Vector2(x, y);
		Window = window;
	}

	public override string ToString()
	{
		return $"({Position.X}, {Position.Y}, W{Window.Value})";
	}

	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		return ToString();
	}

	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
	{
		Unsafe.SkipInit(out BufferInterpolatedStringHandler val);
		((BufferInterpolatedStringHandler)(ref val))._002Ector(7, 3, destination);
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral("(");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<float>(Position.X);
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral(", ");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<float>(Position.Y);
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral(", W");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<int>(Window.Value);
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral(")");
		return FormatHelpers.TryFormatInto(destination, ref charsWritten, ref val);
	}

	public bool Equals(ScreenCoordinates other)
	{
		if (Position.Equals(other.Position))
		{
			return Window == other.Window;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is ScreenCoordinates other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Position, Window);
	}

	public static bool operator ==(ScreenCoordinates a, ScreenCoordinates b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(ScreenCoordinates a, ScreenCoordinates b)
	{
		return !a.Equals(b);
	}

	public void Deconstruct(out Vector2 pos, out WindowId window)
	{
		pos = Position;
		window = Window;
	}

	static ScreenCoordinates()
	{
		Invalid = new ScreenCoordinates(Vector2.Zero, WindowId.Invalid);
	}
}
