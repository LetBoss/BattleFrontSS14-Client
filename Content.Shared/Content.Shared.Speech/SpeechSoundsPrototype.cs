using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Speech;

[Prototype(null, 1)]
public sealed class SpeechSoundsPrototype : IPrototype
{
	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("variation", false, 1, false, false, null)]
	public float Variation { get; set; } = 0.1f;

	[DataField("saySound", false, 1, false, false, null)]
	public SoundSpecifier SaySound { get; set; } = (SoundSpecifier)new SoundPathSpecifier("/Audio/Voice/Talk/speak_2.ogg", (AudioParams?)null);

	[DataField("askSound", false, 1, false, false, null)]
	public SoundSpecifier AskSound { get; set; } = (SoundSpecifier)new SoundPathSpecifier("/Audio/Voice/Talk/speak_2_ask.ogg", (AudioParams?)null);

	[DataField("exclaimSound", false, 1, false, false, null)]
	public SoundSpecifier ExclaimSound { get; set; } = (SoundSpecifier)new SoundPathSpecifier("/Audio/Voice/Talk/speak_2_exclaim.ogg", (AudioParams?)null);
}
