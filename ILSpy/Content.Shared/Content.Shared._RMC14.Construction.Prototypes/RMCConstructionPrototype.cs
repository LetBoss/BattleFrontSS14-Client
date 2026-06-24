using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Physics;
using Content.Shared.Stacks;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared._RMC14.Construction.Prototypes;

[Serializable]
[Prototype("rmcConstruction", 1)]
[NetSerializable]
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
	public EntProtoId<SkillDefinitionComponent> DelaySkill = EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillConstruction");

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
	public CollisionGroup? RestrictedCollisionGroup = CollisionGroup.Impassable;

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

	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<RMCConstructionPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[NeverPushInheritance]
	[AbstractDataField(1)]
	public bool Abstract { get; private set; }

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[AlwaysPushInheritance]
	[DataField(null, false, 1, true, false, null)]
	public string Name { get; set; }
}
