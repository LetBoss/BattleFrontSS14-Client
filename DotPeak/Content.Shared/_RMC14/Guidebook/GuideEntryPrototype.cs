// Decompiled with JetBrains decompiler
// Type: Content.Shared.Guidebook.GuideEntryPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

#nullable enable
namespace Content.Shared.Guidebook;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class GuideEntryPrototype : GuideEntry, IPrototype, IInheritingPrototype, ICMSpecific
{
  public string ID => this.Id;

  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<GuideEntryPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public bool IsCM { get; private set; }
}
