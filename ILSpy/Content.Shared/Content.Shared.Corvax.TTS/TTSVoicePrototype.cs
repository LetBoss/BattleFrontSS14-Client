using Content.Shared.Humanoid;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Corvax.TTS;

[Prototype("ttsVoice", 1)]
public sealed class TTSVoicePrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("name", false, 1, false, false, null)]
	public string Name { get; private set; } = string.Empty;

	[DataField("sex", false, 1, true, false, null)]
	public Sex Sex { get; private set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("speaker", false, 1, true, false, null)]
	public string Speaker { get; private set; } = string.Empty;

	[DataField("roundStart", false, 1, false, false, null)]
	public bool RoundStart { get; private set; } = true;

	[DataField("sponsorOnly", false, 1, false, false, null)]
	public bool SponsorOnly { get; private set; }

	[DataField("requiresPermission", false, 1, false, false, null)]
	public string RequiresPermission { get; private set; } = string.Empty;
}
