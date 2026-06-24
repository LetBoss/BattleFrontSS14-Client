// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.Prototypes.XenoArchTriggerPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.Prototypes;

[Prototype(null, 1)]
public sealed class XenoArchTriggerPrototype : IPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public LocId Tip;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public ComponentRegistry Components = new ComponentRegistry();

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
