using System.Collections.Generic;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural;

[Prototype(null, 1)]
public sealed class DungeonPresetPrototype : IPrototype
{
	[DataField("roomPacks", false, 1, true, false, null)]
	public List<Box2i> RoomPacks = new List<Box2i>();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
