using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Chat.Prototypes;

[Prototype(null, 1)]
public sealed class AutoEmotePrototype : IPrototype
{
	[DataField("emote", false, 1, true, false, typeof(PrototypeIdSerializer<EmotePrototype>))]
	public string EmoteId = string.Empty;

	[DataField("interval", false, 1, true, false, null)]
	public TimeSpan Interval;

	[DataField("chance", false, 1, false, false, null)]
	public float Chance = 1f;

	[DataField("withChat", false, 1, false, false, null)]
	public bool WithChat = true;

	[DataField("hiddenFromChatWindow", false, 1, false, false, null)]
	public bool HiddenFromChatWindow;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
