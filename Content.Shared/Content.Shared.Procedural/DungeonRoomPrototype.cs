using System.Collections.Generic;
using Content.Shared.Maps;
using Content.Shared.Tag;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Procedural;

[Prototype(null, 1)]
public sealed class DungeonRoomPrototype : IPrototype
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<TagPrototype>> Tags = new List<ProtoId<TagPrototype>>();

	[DataField(null, false, 1, true, false, null)]
	public Vector2i Size;

	[DataField("atlas", false, 1, true, false, null)]
	public ResPath AtlasPath;

	[DataField(null, false, 1, true, false, null)]
	public Vector2i Offset;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<ContentTileDefinition>? IgnoreTile;

	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;
}
