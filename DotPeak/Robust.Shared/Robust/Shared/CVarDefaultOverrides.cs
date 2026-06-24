// Decompiled with JetBrains decompiler
// Type: Robust.Shared.CVarDefaultOverrides
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Configuration;

#nullable enable
namespace Robust.Shared;

internal static class CVarDefaultOverrides
{
  public static void OverrideClient(IConfigurationManager cfg)
  {
    CVarDefaultOverrides.OverrideShared(cfg);
  }

  public static void OverrideServer(IConfigurationManager cfg)
  {
    CVarDefaultOverrides.OverrideShared(cfg);
  }

  private static void OverrideShared(IConfigurationManager cfg)
  {
  }
}
