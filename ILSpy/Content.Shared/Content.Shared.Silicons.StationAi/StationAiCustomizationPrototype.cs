using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Silicons.StationAi;

[Prototype(null, 1)]
public sealed class StationAiCustomizationPrototype : IPrototype, IInheritingPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public LocId Name;

	[DataField(null, false, 1, true, false, null)]
	public Dictionary<string, PrototypeLayerData> LayerData = new Dictionary<string, PrototypeLayerData>();

	[DataField(null, false, 1, false, false, null)]
	public string PreviewKey = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier? PreviewBackground;

	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;

	[ViewVariables]
	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<StationAiCustomizationPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[ViewVariables]
	[NeverPushInheritance]
	[AbstractDataField(1)]
	public bool Abstract { get; private set; }
}
