// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Configuration.CVarDef
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.Configuration;

public abstract class CVarDef
{
  public object DefaultValue { get; }

  public CVar Flags { get; }

  public string Name { get; }

  public string? Desc { get; }

  private protected CVarDef(string name, object defaultValue, CVar flags, string? desc)
  {
    this.Name = name;
    this.DefaultValue = defaultValue;
    this.Flags = flags;
    this.Desc = desc;
  }

  public static CVarDef<T> Create<T>(string name, T defaultValue, CVar flag = CVar.NONE, string? desc = null) where T : notnull
  {
    return new CVarDef<T>(name, defaultValue, flag, desc);
  }
}
