// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cloning.CloningSettingsPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Cloning;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class CloningSettingsPrototype : IPrototype, IInheritingPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public bool ForceCloning = true;
  [DataField(null, false, 1, false, false, null)]
  public SlotFlags? CopyEquipment = new SlotFlags?(SlotFlags.All);
  [DataField(null, false, 1, false, false, null)]
  public bool CopyInternalStorage = true;
  [DataField(null, false, 1, false, false, null)]
  public bool CopyImplants = true;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  [AlwaysPushInheritance]
  public HashSet<string> Components = new HashSet<string>();
  [DataField(null, false, 1, false, false, null)]
  [AlwaysPushInheritance]
  public HashSet<string> EventComponents = new HashSet<string>();

  [IdDataField(1, null)]
  public string ID { get; private set; }

  [ParentDataField(typeof (PrototypeIdArraySerializer<CloningSettingsPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [AbstractDataField(1)]
  [NeverPushInheritance]
  public bool Abstract { get; private set; }
}
