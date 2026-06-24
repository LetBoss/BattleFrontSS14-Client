using Robust.Shared.Audio;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weather;

[Prototype(null, 1)]
public sealed class WeatherPrototype : IPrototype
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("sprite", false, 1, true, false, null)]
	public SpriteSpecifier Sprite;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("color", false, 1, false, false, null)]
	public Color? Color;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("sound", false, 1, false, false, null)]
	public SoundSpecifier? Sound;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
