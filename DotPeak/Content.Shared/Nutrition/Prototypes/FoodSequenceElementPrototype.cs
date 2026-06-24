// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Prototypes.FoodSequenceElementPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Tag;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared.Nutrition.Prototypes;

[Prototype(null, 1)]
public sealed class FoodSequenceElementPrototype : IPrototype
{
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public List<SpriteSpecifier> Sprites { get; private set; } = new List<SpriteSpecifier>();

  [DataField(null, false, 1, false, false, null)]
  public Vector2 Scale { get; private set; } = Vector2.One;

  [DataField(null, false, 1, false, false, null)]
  public LocId? Name { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public bool Final { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<TagPrototype>> Tags { get; set; } = new List<ProtoId<TagPrototype>>();
}
