using System.Collections.Generic;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Shared.Chat.Prototypes;

[Prototype(null, 1)]
public sealed class EmoteSoundsPrototype : IPrototype, IInheritingPrototype
{
	[DataField("sound", false, 1, false, false, null)]
	[AlwaysPushInheritance]
	public SoundSpecifier? FallbackSound;

	[DataField("params", false, 1, false, false, null)]
	[AlwaysPushInheritance]
	public AudioParams? GeneralParams;

	[DataField(null, false, 1, false, false, null)]
	[AlwaysPushInheritance]
	public Dictionary<ProtoId<EmotePrototype>, SoundSpecifier> Sounds = new Dictionary<ProtoId<EmotePrototype>, SoundSpecifier>();

	[IdDataField(1, null)]
	public string ID { get; private set; }

	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<EmoteSoundsPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[AbstractDataField(1)]
	[NeverPushInheritance]
	public bool Abstract { get; private set; }
}
