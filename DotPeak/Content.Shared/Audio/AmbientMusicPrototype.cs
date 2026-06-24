// Decompiled with JetBrains decompiler
// Type: Content.Shared.Audio.AmbientMusicPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Random.Rules;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

#nullable enable
namespace Content.Shared.Audio;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class AmbientMusicPrototype : IPrototype
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("priority", false, 1, false, false, null)]
  public int Priority;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("interruptable", false, 1, false, false, null)]
  public bool Interruptable;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("fadeIn", false, 1, false, false, null)]
  public bool FadeIn;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("sound", false, 1, true, false, null)]
  public SoundSpecifier Sound;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("rules", false, 1, true, false, typeof (PrototypeIdSerializer<RulesPrototype>))]
  public string Rules = string.Empty;

  [IdDataField(1, null)]
  public string ID { get; private set; } = string.Empty;
}
