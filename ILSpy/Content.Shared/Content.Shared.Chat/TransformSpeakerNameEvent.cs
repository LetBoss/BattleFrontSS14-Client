using Content.Shared.Inventory;
using Content.Shared.Speech;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared.Chat;

public sealed class TransformSpeakerNameEvent : EntityEventArgs, IInventoryRelayEvent
{
	public EntityUid Sender;

	public string VoiceName;

	public ProtoId<SpeechVerbPrototype>? SpeechVerb;

	public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

	public TransformSpeakerNameEvent(EntityUid sender, string name)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Sender = sender;
		VoiceName = name;
		SpeechVerb = null;
	}
}
