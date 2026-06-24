using System;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Remotes.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Remotes.EntitySystems;

public abstract class SharedDoorRemoteSystem : EntitySystem
{
	[Dependency]
	protected SharedPopupSystem Popup;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<DoorRemoteComponent, UseInHandEvent>((EntityEventRefHandler<DoorRemoteComponent, UseInHandEvent>)OnInHandActivation, (Type[])null, (Type[])null);
	}

	private void OnInHandActivation(Entity<DoorRemoteComponent> entity, ref UseInHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		string switchMessageId;
		switch (entity.Comp.Mode)
		{
		case OperatingMode.OpenClose:
			entity.Comp.Mode = OperatingMode.ToggleBolts;
			switchMessageId = "door-remote-switch-state-toggle-bolts";
			break;
		case OperatingMode.ToggleBolts:
			entity.Comp.Mode = OperatingMode.ToggleEmergencyAccess;
			switchMessageId = "door-remote-switch-state-toggle-emergency-access";
			break;
		case OperatingMode.ToggleEmergencyAccess:
			entity.Comp.Mode = OperatingMode.OpenClose;
			switchMessageId = "door-remote-switch-state-open-close";
			break;
		default:
			throw new InvalidOperationException($"{"DoorRemoteComponent"} had invalid mode {entity.Comp.Mode}");
		}
		((EntitySystem)this).Dirty<DoorRemoteComponent>(entity, (MetaDataComponent)null);
		Popup.PopupClient(base.Loc.GetString(switchMessageId), Entity<DoorRemoteComponent>.op_Implicit(entity), args.User);
	}
}
