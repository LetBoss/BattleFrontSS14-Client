// Decompiled with JetBrains decompiler
// Type: Content.Shared.Decals.DecalPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Decals;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class DecalPrototype : IPrototype, IInheritingPrototype, ICMSpecific
{
  [DataField("tags", false, 1, false, false, null)]
  public List<string> Tags = new List<string>();
  [DataField("showMenu", false, 1, false, false, null)]
  public bool ShowMenu = true;
  [DataField("snapCardinals", false, 1, false, false, null)]
  public bool SnapCardinals;
  [DataField(null, false, 1, false, false, null)]
  public bool DefaultCleanable;
  [DataField(null, false, 1, false, false, null)]
  public bool DefaultCustomColor;
  [DataField(null, false, 1, false, false, null)]
  public bool DefaultSnap = true;

  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("sprite", false, 1, false, false, null)]
  public SpriteSpecifier Sprite { get; private set; } = SpriteSpecifier.Invalid;

  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<DecalPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public bool IsCM { get; private set; }
}
