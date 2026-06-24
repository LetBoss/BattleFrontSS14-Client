using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Maths;

namespace Content.Client.Rendering.Profiled08b;

internal readonly struct RMCDrawSkewSlice91dac4(byte Grid, byte CellX, byte CellY, byte SizePercent, Color Color) : IEquatable<RMCDrawSkewSlice91dac4>
{
	public byte Grid { get; init; }

	public byte CellX { get; init; }

	public byte CellY { get; init; }

	public byte SizePercent { get; init; }

	public Color Color { get; init; }

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("RMCWeaponProfileOverlayState");
		stringBuilder.Append(" { ");
		if (_mc6e4480cb1ad(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool _mc6e4480cb1ad(StringBuilder builder)
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		builder.Append("Grid = ");
		builder.Append(Grid.ToString());
		builder.Append(", CellX = ");
		builder.Append(CellX.ToString());
		builder.Append(", CellY = ");
		builder.Append(CellY.ToString());
		builder.Append(", SizePercent = ");
		builder.Append(SizePercent.ToString());
		builder.Append(", Color = ");
		builder.Append(((object)Color/*cast due to constrained. prefix*/).ToString());
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(RMCDrawSkewSlice91dac4 left, RMCDrawSkewSlice91dac4 right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(RMCDrawSkewSlice91dac4 left, RMCDrawSkewSlice91dac4 right)
	{
		return left.Equals(right);
	}

	[CompilerGenerated]
	public override int GetHashCode()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		return (((EqualityComparer<byte>.Default.GetHashCode(_f0a36d90eccfa) * -1521134295 + EqualityComparer<byte>.Default.GetHashCode(_f6e2c17bcd7f1)) * -1521134295 + EqualityComparer<byte>.Default.GetHashCode(_f5a32a6054187)) * -1521134295 + EqualityComparer<byte>.Default.GetHashCode(_fdd755d252529)) * -1521134295 + EqualityComparer<Color>.Default.GetHashCode(_f8f8d79c1e6b4);
	}

	[CompilerGenerated]
	public override bool Equals(object obj)
	{
		if (obj is RMCDrawSkewSlice91dac4)
		{
			return Equals((RMCDrawSkewSlice91dac4)obj);
		}
		return false;
	}

	[CompilerGenerated]
	public bool Equals(RMCDrawSkewSlice91dac4 other)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (EqualityComparer<byte>.Default.Equals(_f0a36d90eccfa, other._f0a36d90eccfa) && EqualityComparer<byte>.Default.Equals(_f6e2c17bcd7f1, other._f6e2c17bcd7f1) && EqualityComparer<byte>.Default.Equals(_f5a32a6054187, other._f5a32a6054187) && EqualityComparer<byte>.Default.Equals(_fdd755d252529, other._fdd755d252529))
		{
			return EqualityComparer<Color>.Default.Equals(_f8f8d79c1e6b4, other._f8f8d79c1e6b4);
		}
		return false;
	}

	[CompilerGenerated]
	public void Deconstruct(out byte Grid, out byte CellX, out byte CellY, out byte SizePercent, out Color Color)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Grid = this.Grid;
		CellX = this.CellX;
		CellY = this.CellY;
		SizePercent = this.SizePercent;
		Color = this.Color;
	}
}
