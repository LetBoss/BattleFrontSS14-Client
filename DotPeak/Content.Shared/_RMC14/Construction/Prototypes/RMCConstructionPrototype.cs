// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Construction.Prototypes.RMCConstructionPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Physics;
using Content.Shared.Stacks;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Construction.Prototypes;

[Robust.Shared.Prototypes.Prototype("rmcConstruction", 1)]
[NetSerializable]
[Serializable]
public sealed class RMCConstructionPrototype : IPrototype, IInheritingPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public bool IsDivider;
  [AlwaysPushInheritance]
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier? Icon;
  [AlwaysPushInheritance]
  [DataField(null, false, 1, false, false, null)]
  public bool NoRotate;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<RMCConstructionPrototype>[]? Listed;
  [AlwaysPushInheritance]
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId<SkillDefinitionComponent>? Skill;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId<SkillDefinitionComponent> DelaySkill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillConstruction";
  [AlwaysPushInheritance]
  [DataField(null, false, 1, false, false, null)]
  public int RequiredSkillLevel = 1;
  [AlwaysPushInheritance]
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DoAfterTime = TimeSpan.Zero;
  [AlwaysPushInheritance]
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DoAfterTimeMin = TimeSpan.Zero;
  [AlwaysPushInheritance]
  [DataField(null, false, 1, false, false, null)]
  public CollisionGroup? RestrictedCollisionGroup = new CollisionGroup?(CollisionGroup.Impassable);
  [AlwaysPushInheritance]
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<TagPrototype>[]? RestrictedTags;
  [AlwaysPushInheritance]
  [DataField(null, false, 1, false, false, null)]
  public bool IgnoreBuildRestrictions;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Prototype;
  [AlwaysPushInheritance]
  [DataField(null, false, 1, false, false, null)]
  public int? MaterialCost;
  [DataField(null, false, 1, false, false, null)]
  public int Amount = 1;
  [DataField(null, false, 1, false, false, null)]
  public HashSet<int>? StackAmounts;
  [DataField(null, false, 1, false, false, null)]
  public bool PlaceInFront;
  [AlwaysPushInheritance]
  [DataField(null, false, 1, false, false, null)]
  public string? SideId;
  [AlwaysPushInheritance]
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<StackPrototype>? FillMaterial;
  [AlwaysPushInheritance]
  [DataField(null, false, 1, false, false, null)]
  public bool RequiresFobZone;

  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<RMCConstructionPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [AlwaysPushInheritance]
  [DataField(null, false, 1, true, false, null)]
  public string Name { get; set; }
}
