using System.Collections.Generic;
using Content.Shared.Research.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Lathe.Prototypes;

[Prototype(null, 1)]
public sealed class LatheRecipePackPrototype : IPrototype, IInheritingPrototype
{
	[DataField(null, false, 1, true, false, null)]
	[AlwaysPushInheritance]
	public HashSet<ProtoId<LatheRecipePrototype>> Recipes = new HashSet<ProtoId<LatheRecipePrototype>>();

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<LatheRecipePackPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[NeverPushInheritance]
	[AbstractDataField(1)]
	public bool Abstract { get; private set; }
}
