using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared._PUBG;

[Prototype(null, 1)]
public sealed class PubgFirstKillBannerPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("backgroundRsi", false, 1, true, false, null)]
	public ResPath BackgroundRsi { get; private set; }

	[DataField("backgroundState", false, 1, false, false, null)]
	public string BackgroundState { get; private set; } = "fk";

	[DataField("width", false, 1, false, false, null)]
	public int Width { get; private set; } = 400;

	[DataField("height", false, 1, false, false, null)]
	public int Height { get; private set; } = 200;
}
