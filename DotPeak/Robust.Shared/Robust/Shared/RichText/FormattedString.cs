// Decompiled with JetBrains decompiler
// Type: Robust.Shared.RichText.FormattedString
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Robust.Shared.RichText;

public struct FormattedString : IEquatable<FormattedString>, ISelfSerialize
{
  public static readonly FormattedString Empty = new FormattedString("");
  public readonly string Markup;

  [Obsolete("Do not construct FormattedString directly")]
  public FormattedString()
  {
    throw new NotSupportedException("Do not construct FormattedString directly");
  }

  private FormattedString(string markup) => this.Markup = markup;

  public static FormattedString FromMarkup(string markup)
  {
    return FormattedMessage.ValidMarkup(markup) ? new FormattedString(markup) : throw new ArgumentException("Invalid markup string");
  }

  public static FormattedString FromMarkupPermissive(string markup)
  {
    return (FormattedString) FormattedMessage.FromMarkupPermissive(markup);
  }

  public static FormattedString FromPlainText(string plainText)
  {
    return new FormattedString(FormattedMessage.EscapeText(plainText));
  }

  public static explicit operator FormattedString(FormattedMessage message)
  {
    return new FormattedString(message.ToMarkup());
  }

  public static explicit operator FormattedMessage(FormattedString str)
  {
    return FormattedMessage.FromMarkupOrThrow(str.Markup);
  }

  public static explicit operator string(FormattedString str) => str.Markup;

  public readonly bool Equals(FormattedString other) => other.Markup == this.Markup;

  public override readonly bool Equals(object? obj)
  {
    return obj is FormattedString other && this.Equals(other);
  }

  public override readonly int GetHashCode() => this.Markup.GetHashCode();

  public static bool operator ==(FormattedString left, FormattedString right) => left.Equals(right);

  public static bool operator !=(FormattedString left, FormattedString right)
  {
    return !left.Equals(right);
  }

  void ISelfSerialize.Deserialize(string value) => this = FormattedString.FromMarkup(value);

  readonly string ISelfSerialize.Serialize() => this.Markup;
}
