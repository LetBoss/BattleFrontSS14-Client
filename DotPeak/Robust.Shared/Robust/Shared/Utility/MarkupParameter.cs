// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.MarkupParameter
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Utility;

[NetSerializable]
[Serializable]
public readonly record struct MarkupParameter(
  string? StringValue = null,
  long? LongValue = null,
  Color? ColorValue = null)
{
  public MarkupParameter(string? stringValue)
    : this(stringValue, new long?(), new Color?())
  {
  }

  public MarkupParameter(long? longValue)
    : this((string) null, longValue, new Color?())
  {
  }

  public MarkupParameter(Color? colorValue)
    : this((string) null, new long?(), colorValue)
  {
  }

  public bool TryGetString([NotNullWhen(true)] out string? value)
  {
    value = this.StringValue;
    return this.StringValue != null;
  }

  public bool TryGetLong([NotNullWhen(true)] out long? value)
  {
    value = this.LongValue;
    return this.LongValue.HasValue;
  }

  public bool TryGetColor([NotNullWhen(true)] out Color? value)
  {
    value = this.ColorValue;
    return this.ColorValue.HasValue;
  }

  public override string ToString()
  {
    if (this.StringValue != null)
      return $"=\"{this.StringValue}\"";
    long? longValue = this.LongValue;
    if (longValue.HasValue)
    {
      longValue = this.LongValue;
      ref long? local = ref longValue;
      return (local.HasValue ? local.GetValueOrDefault().ToString().Insert(0, "=") : (string) null) ?? "";
    }
    Color? colorValue = this.ColorValue;
    ref Color? local1 = ref colorValue;
    Color valueOrDefault;
    string str1;
    if (!local1.HasValue)
    {
      str1 = (string) null;
    }
    else
    {
      valueOrDefault = local1.GetValueOrDefault();
      str1 = ((Color) ref valueOrDefault).Name();
    }
    if (str1 != null)
    {
      colorValue = this.ColorValue;
      valueOrDefault = colorValue.Value;
      return ((Color) ref valueOrDefault).Name().Insert(0, "=");
    }
    colorValue = this.ColorValue;
    ref Color? local2 = ref colorValue;
    string str2;
    if (!local2.HasValue)
    {
      str2 = (string) null;
    }
    else
    {
      valueOrDefault = local2.GetValueOrDefault();
      str2 = ((Color) ref valueOrDefault).ToHex().Insert(0, "=");
    }
    return str2 ?? "";
  }

  public bool Equals(MarkupParameter? other)
  {
    if (!other.HasValue)
      return false;
    string stringValue1 = this.StringValue;
    MarkupParameter markupParameter = other.Value;
    string stringValue2 = markupParameter.StringValue;
    int num1 = stringValue1 == stringValue2 ? 1 : 0;
    long? longValue1 = this.LongValue;
    markupParameter = other.Value;
    long? longValue2 = markupParameter.LongValue;
    int num2 = longValue1.GetValueOrDefault() == longValue2.GetValueOrDefault() & longValue1.HasValue == longValue2.HasValue ? 1 : 0;
    int num3 = num1 & num2;
    Color? colorValue1 = this.ColorValue;
    markupParameter = other.Value;
    Color? colorValue2 = markupParameter.ColorValue;
    int num4 = colorValue1.HasValue == colorValue2.HasValue ? (colorValue1.HasValue ? (Color.op_Equality(colorValue1.GetValueOrDefault(), colorValue2.GetValueOrDefault()) ? 1 : 0) : 1) : 0;
    return (num3 & num4) != 0;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine<string, long?, Color?>(this.StringValue, this.LongValue, this.ColorValue);
  }
}
