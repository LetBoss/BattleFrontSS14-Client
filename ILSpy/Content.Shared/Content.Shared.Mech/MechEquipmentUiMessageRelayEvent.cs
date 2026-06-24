using Robust.Shared.GameObjects;

namespace Content.Shared.Mech;

public sealed class MechEquipmentUiMessageRelayEvent : EntityEventArgs
{
	public MechEquipmentUiMessage Message;

	public MechEquipmentUiMessageRelayEvent(MechEquipmentUiMessage message)
	{
		Message = message;
	}
}
