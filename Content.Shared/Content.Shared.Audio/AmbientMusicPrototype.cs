using Content.Shared.Random.Rules;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Audio;

[Prototype(null, 1)]
public sealed class AmbientMusicPrototype : IPrototype
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("priority", false, 1, false, false, null)]
	public int Priority;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("interruptable", false, 1, false, false, null)]
	public bool Interruptable;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("fadeIn", false, 1, false, false, null)]
	public bool FadeIn;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("sound", false, 1, true, false, null)]
	public SoundSpecifier Sound;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("rules", false, 1, true, false, typeof(PrototypeIdSerializer<RulesPrototype>))]
	public string Rules = string.Empty;

	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;
}
