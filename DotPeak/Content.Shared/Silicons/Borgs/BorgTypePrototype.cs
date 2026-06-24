// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Borgs.BorgTypePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Content.Shared.Radio;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Silicons.Borgs;

[Prototype(null, 1)]
public sealed class BorgTypePrototype : IPrototype
{
  private static readonly ProtoId<SoundCollectionPrototype> DefaultFootsteps = new ProtoId<SoundCollectionPrototype>("FootstepBorg");
  [DataField(null, false, 1, false, false, null)]
  public required EntProtoId DummyPrototype;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<RadioChannelPrototype>[] RadioChannels = Array.Empty<ProtoId<RadioChannelPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId[] DefaultModules = Array.Empty<EntProtoId>();

  [IdDataField(1, null)]
  public required string ID { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public int ExtraModuleCount { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? ModuleWhitelist { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<InventoryTemplatePrototype> InventoryTemplateId { get; set; } = (ProtoId<InventoryTemplatePrototype>) "borgShort";

  [DataField(null, false, 1, false, false, null)]
  public ComponentRegistry? AddComponents { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public string SpriteBodyState { get; set; } = "robot";

  [DataField(null, false, 1, false, false, null)]
  public string? SpriteBodyMovementState { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public string SpriteHasMindState { get; set; } = "robot_e";

  [DataField(null, false, 1, false, false, null)]
  public string SpriteNoMindState { get; set; } = "robot_e_r";

  [DataField(null, false, 1, false, false, null)]
  public string SpriteToggleLightState { get; set; } = "robot_l";

  [DataField(null, false, 1, false, false, null)]
  public string PetSuccessString { get; set; } = "petting-success-generic-cyborg";

  [DataField(null, false, 1, false, false, null)]
  public string PetFailureString { get; set; } = "petting-failure-generic-cyborg";

  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier FootstepCollection { get; set; } = (SoundSpecifier) new SoundCollectionSpecifier((string) BorgTypePrototype.DefaultFootsteps);
}
