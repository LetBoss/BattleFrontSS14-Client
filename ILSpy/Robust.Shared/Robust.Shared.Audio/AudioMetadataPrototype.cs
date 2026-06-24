using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Robust.Shared.Audio;

[Prototype("audioMetadata", 1)]
public sealed class AudioMetadataPrototype : IPrototype
{
	public const string ProtoName = "audioMetadata";

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan Length;

	[IdDataField(1, null)]
	public string ID { get; set; } = string.Empty;
}
