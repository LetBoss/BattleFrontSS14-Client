// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Configuration.CVarDef`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.Configuration;

public sealed class CVarDef<T> : CVarDef where T : notnull
{
  public T DefaultValue { get; }

  internal CVarDef(string name, T defaultValue, CVar flags, string? desc)
    : base(name, (object) defaultValue, flags, desc)
  {
    this.DefaultValue = defaultValue;
  }
}
