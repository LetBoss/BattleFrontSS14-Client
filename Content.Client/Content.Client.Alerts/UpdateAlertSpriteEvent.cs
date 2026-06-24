using Content.Shared.Alert;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Alerts;

[ByRefEvent]
public record struct UpdateAlertSpriteEvent
{
	public Entity<SpriteComponent> SpriteViewEnt;

	public EntityUid ViewerEnt;

	public AlertPrototype Alert;

	public UpdateAlertSpriteEvent(Entity<SpriteComponent> spriteViewEnt, EntityUid viewerEnt, AlertPrototype alert)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		SpriteViewEnt = spriteViewEnt;
		ViewerEnt = viewerEnt;
		Alert = alert;
	}
}
