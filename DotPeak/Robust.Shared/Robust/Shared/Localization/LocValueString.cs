// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Localization.LocValueString
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Robust.Shared.Localization;

public sealed record LocValueString(string Value) : LocValue<string>(Value)
{
  public override string Format(LocContext ctx) => this.Value;

  [CompilerGenerated]
  protected override bool PrintMembers(StringBuilder builder) => base.PrintMembers(builder);

  [CompilerGenerated]
  public override int GetHashCode() => base.GetHashCode();

  [CompilerGenerated]
  public sealed override bool Equals(LocValue<string>? other) => this.Equals((object) other);

  [CompilerGenerated]
  public bool Equals(LocValueString? other)
  {
    return (object) this == (object) other || base.Equals((LocValue<string>) other);
  }

  [CompilerGenerated]
  public void Deconstruct(out string Value) => Value = this.Value;
}
