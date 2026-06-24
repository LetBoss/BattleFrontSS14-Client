using System.Collections.Generic;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

namespace Robust.Shared.Prototypes;

[Prototype(null, 1)]
public sealed class EntityCategoryPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public string? Name;

	[DataField(null, false, 1, false, false, null)]
	public string? Description;

	[DataField(null, false, 1, false, false, null)]
	public string? Suffix;

	[DataField(null, false, 1, false, false, null)]
	public bool HideSpawnMenu;

	[DataField(null, false, 1, false, false, typeof(CustomHashSetSerializer<string, ComponentNameSerializer>))]
	public HashSet<string>? Components;

	[DataField(null, false, 1, false, false, null)]
	public bool Inheritable = true;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
