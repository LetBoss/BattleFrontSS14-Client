using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Audio.Jukebox;

[Prototype(null, 1)]
public sealed class JukeboxPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public string Name = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public SoundPathSpecifier Path;

	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;
}
