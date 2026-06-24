using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Robust.Shared.Utility;

[Serializable]
[NetSerializable]
public readonly record struct MarkupParameter
{
	public string? StringValue { get; init; }

	public long? LongValue { get; init; }

	public Color? ColorValue { get; init; }

	public MarkupParameter(string? StringValue = null, long? LongValue = null, Color? ColorValue = null)
	{
		this.StringValue = StringValue;
		this.LongValue = LongValue;
		this.ColorValue = ColorValue;
	}

	public MarkupParameter(string? stringValue)
		: this(stringValue, null, null)
	{
	}

	public MarkupParameter(long? longValue)
		: this(null, longValue)
	{
	}

	public MarkupParameter(Color? colorValue)
		: this(null, null, colorValue)
	{
	}

	public bool TryGetString([NotNullWhen(true)] out string? value)
	{
		value = StringValue;
		return StringValue != null;
	}

	public bool TryGetLong([NotNullWhen(true)] out long? value)
	{
		value = LongValue;
		return LongValue.HasValue;
	}

	public bool TryGetColor([NotNullWhen(true)] out Color? value)
	{
		value = ColorValue;
		return ColorValue.HasValue;
	}

	public override string ToString()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		if (StringValue != null)
		{
			return "=\"" + StringValue + "\"";
		}
		if (LongValue.HasValue)
		{
			return LongValue?.ToString().Insert(0, "=") ?? "";
		}
		Color? colorValue = ColorValue;
		object obj;
		Color val;
		if (!colorValue.HasValue)
		{
			obj = null;
		}
		else
		{
			val = colorValue.GetValueOrDefault();
			obj = ((Color)(ref val)).Name();
		}
		if (obj != null)
		{
			val = ColorValue.Value;
			return ((Color)(ref val)).Name().Insert(0, "=");
		}
		colorValue = ColorValue;
		object obj2;
		if (!colorValue.HasValue)
		{
			obj2 = null;
		}
		else
		{
			val = colorValue.GetValueOrDefault();
			obj2 = ((Color)(ref val)).ToHex().Insert(0, "=");
		}
		if (obj2 == null)
		{
			obj2 = "";
		}
		return (string)obj2;
	}

	public bool Equals(MarkupParameter? other)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (!other.HasValue)
		{
			return false;
		}
		bool num = (StringValue == other.Value.StringValue) & (LongValue == other.Value.LongValue);
		Color? colorValue = ColorValue;
		Color? colorValue2 = other.Value.ColorValue;
		return num & (colorValue.HasValue == colorValue2.HasValue && (!colorValue.HasValue || colorValue.GetValueOrDefault() == colorValue2.GetValueOrDefault()));
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(StringValue, LongValue, ColorValue);
	}

	[CompilerGenerated]
	public void Deconstruct(out string? StringValue, out long? LongValue, out Color? ColorValue)
	{
		StringValue = this.StringValue;
		LongValue = this.LongValue;
		ColorValue = this.ColorValue;
	}
}
