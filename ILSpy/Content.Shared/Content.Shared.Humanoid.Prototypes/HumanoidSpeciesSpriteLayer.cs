using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Humanoid.Prototypes;

[Serializable]
[Prototype("humanoidBaseSprite", 1)]
[NetSerializable]
public sealed class HumanoidSpeciesSpriteLayer : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("baseSprite", false, 1, false, false, null)]
	public SpriteSpecifier? BaseSprite { get; private set; }

	[DataField("layerAlpha", false, 1, false, false, null)]
	public float LayerAlpha { get; private set; } = 1f;

	[DataField("allowsMarkings", false, 1, false, false, null)]
	public bool AllowsMarkings { get; private set; } = true;

	[DataField("matchSkin", false, 1, false, false, null)]
	public bool MatchSkin { get; private set; } = true;

	[DataField("markingsMatchSkin", false, 1, false, false, null)]
	public bool MarkingsMatchSkin { get; private set; }
}
