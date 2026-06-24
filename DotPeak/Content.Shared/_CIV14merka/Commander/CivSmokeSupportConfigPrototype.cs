// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Commander.CivSmokeSupportConfigPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared._CIV14merka.Commander;

[Prototype(null, 1)]
public sealed class CivSmokeSupportConfigPrototype : IPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public int PuffCount = 8;
  [DataField(null, false, 1, false, false, null)]
  public float PuffSpacing = 2f;
  [DataField(null, false, 1, false, false, null)]
  public int PuffRange = 3;
  [DataField(null, false, 1, false, false, null)]
  public float SmokeDuration = 60f;
  [DataField(null, false, 1, false, false, null)]
  public float DelaySeconds = 3f;
  [DataField(null, false, 1, false, false, null)]
  public int CooldownSeconds = 60;
  [DataField(null, false, 1, false, false, null)]
  public int BypassCost;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId SmokeEntity = (EntProtoId) "CivCommanderSmokePuff";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? WarningSound;
  [DataField(null, false, 1, false, false, null)]
  public string WarningPopup = "ДЫМ!";

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
