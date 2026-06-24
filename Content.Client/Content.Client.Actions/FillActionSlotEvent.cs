using Robust.Shared.GameObjects;

namespace Content.Client.Actions;

public sealed class FillActionSlotEvent : EntityEventArgs
{
	public EntityUid? Action;
}
