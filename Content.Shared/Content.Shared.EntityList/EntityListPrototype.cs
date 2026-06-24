using System.Collections.Generic;
using System.Collections.Immutable;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Shared.EntityList;

[Prototype(null, 1)]
public sealed class EntityListPrototype : IPrototype
{
	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("entities", false, 1, false, false, typeof(PrototypeIdListSerializer<EntityPrototype>))]
	public ImmutableList<string> EntityIds { get; private set; } = ImmutableList<string>.Empty;

	public IEnumerable<EntityPrototype> Entities(IPrototypeManager? prototypeManager = null)
	{
		if (prototypeManager == null)
		{
			prototypeManager = IoCManager.Resolve<IPrototypeManager>();
		}
		foreach (string entityId in EntityIds)
		{
			yield return prototypeManager.Index<EntityPrototype>(entityId);
		}
	}
}
