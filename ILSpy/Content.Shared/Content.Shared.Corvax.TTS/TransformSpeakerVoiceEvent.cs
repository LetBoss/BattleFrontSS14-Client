using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Corvax.TTS;

public sealed class TransformSpeakerVoiceEvent : EntityEventArgs, IInventoryRelayEvent
{
	public EntityUid Sender;

	public string? VoiceId;

	public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

	public TransformSpeakerVoiceEvent(EntityUid sender, string? voiceId)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Sender = sender;
		VoiceId = voiceId;
	}
}
