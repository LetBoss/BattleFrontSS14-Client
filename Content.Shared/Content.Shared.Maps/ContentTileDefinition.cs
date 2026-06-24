using System.Collections.Generic;
using Content.Shared.Tools;
using Robust.Shared.Audio;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;

namespace Content.Shared.Maps;

[Prototype("tile", 1)]
public sealed class ContentTileDefinition : IPrototype, IInheritingPrototype, ITileDefinition
{
	public static readonly ProtoId<ToolQualityPrototype> PryingToolQuality = ProtoId<ToolQualityPrototype>.op_Implicit("Prying");

	public const string SpaceID = "Space";

	[DataField(null, false, 1, false, false, null)]
	public float Mass = 800f;

	[DataField("thermalConductivity", false, 1, false, false, null)]
	public float ThermalConductivity = 0.04f;

	[DataField("heatCapacity", false, 1, false, false, null)]
	public float HeatCapacity = 0.0003f;

	[DataField("weather", false, 1, false, false, null)]
	public bool Weather;

	[DataField("indestructible", false, 1, false, false, null)]
	public bool Indestructible;

	[DataField(null, false, 1, false, false, null)]
	public bool CanDig;

	[DataField(null, false, 1, false, false, null)]
	public bool CanPlaceTunnel = true;

	[DataField(null, false, 1, false, false, null)]
	public bool WeedsSpreadable = true;

	[DataField(null, false, 1, false, false, null)]
	public bool SemiWeedable;

	[DataField(null, false, 1, false, false, null)]
	public bool BlockConstruction;

	[DataField(null, false, 1, false, false, null)]
	public bool BlockAnchoring;

	[DataField(null, false, 1, false, false, null)]
	public Color MinimapColor;

	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<ContentTileDefinition>), 1)]
	public string[]? Parents { get; private set; }

	[NeverPushInheritance]
	[AbstractDataField(1)]
	public bool Abstract { get; private set; }

	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;

	public ushort TileId { get; private set; }

	[DataField("name", false, 1, false, false, null)]
	public string Name { get; private set; } = "";

	[DataField("sprite", false, 1, false, false, null)]
	public ResPath? Sprite { get; private set; }

	[DataField("edgeSprites", false, 1, false, false, null)]
	public Dictionary<Direction, ResPath> EdgeSprites { get; private set; } = new Dictionary<Direction, ResPath>();

	[DataField("edgeSpritePriority", false, 1, false, false, null)]
	public int EdgeSpritePriority { get; private set; }

	[DataField("isSubfloor", false, 1, false, false, null)]
	public bool IsSubFloor { get; private set; }

	[DataField("baseTurf", false, 1, false, false, null)]
	public string BaseTurf { get; private set; } = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public PrototypeFlags<ToolQualityPrototype> DeconstructTools { get; set; } = new PrototypeFlags<ToolQualityPrototype>();

	public bool CanCrowbar => DeconstructTools.Contains(ProtoId<ToolQualityPrototype>.op_Implicit(PryingToolQuality));

	[DataField("footstepSounds", false, 1, false, false, null)]
	public SoundSpecifier? FootstepSounds { get; private set; }

	[DataField("barestepSounds", false, 1, false, false, null)]
	public SoundSpecifier? BarestepSounds { get; private set; } = (SoundSpecifier?)new SoundCollectionSpecifier("BarestepHard", (AudioParams?)null);

	[DataField("friction", false, 1, false, false, null)]
	public float Friction { get; set; } = 1f;

	[DataField("variants", false, 1, false, false, null)]
	public byte Variants { get; set; } = 1;

	[DataField(null, false, 1, false, false, null)]
	public bool AllowRotationMirror { get; set; }

	[DataField("placementVariants", false, 1, false, false, null)]
	public float[] PlacementVariants { get; set; } = new float[1] { 1f };

	[DataField("itemDrop", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string ItemDropPrototypeName { get; private set; } = "FloorTileItemSteel";

	[DataField("isSpace", false, 1, false, false, null)]
	public bool MapAtmosphere { get; private set; }

	[DataField("mobFriction", false, 1, false, false, null)]
	public float? MobFriction { get; private set; }

	[DataField("mobAcceleration", false, 1, false, false, null)]
	public float? MobAcceleration { get; private set; }

	[DataField("sturdy", false, 1, false, false, null)]
	public bool Sturdy { get; private set; } = true;

	public void AssignTileId(ushort id)
	{
		TileId = id;
	}
}
