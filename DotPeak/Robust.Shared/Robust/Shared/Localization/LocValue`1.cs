// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Localization.LocValue`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.Localization;

public abstract record LocValue<T> : ILocValue
{
  public T Value { get; init; }

  object? ILocValue.Value => (object) this.Value;

  protected LocValue(T val) => this.Value = val;

  public abstract string Format(LocContext ctx);
}
