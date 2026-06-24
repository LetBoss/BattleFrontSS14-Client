// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weather.WeatherPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

#nullable enable
namespace Content.Shared.Weather;

[Prototype(null, 1)]
public sealed class WeatherPrototype : IPrototype
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("sprite", false, 1, true, false, null)]
  public SpriteSpecifier Sprite;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("color", false, 1, false, false, null)]
  public Robust.Shared.Maths.Color? Color;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("sound", false, 1, false, false, null)]
  public SoundSpecifier? Sound;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
