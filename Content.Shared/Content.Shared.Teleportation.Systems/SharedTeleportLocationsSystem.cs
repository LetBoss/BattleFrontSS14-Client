using System;
using Content.Shared.Teleportation.Components;
using Content.Shared.Timing;
using Content.Shared.UserInterface;
using Content.Shared.Warps;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Teleportation.Systems;

public abstract class SharedTeleportLocationsSystem : EntitySystem
{
	[Dependency]
	protected UseDelaySystem Delay;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private SharedTransformSystem _xform;

	protected const string TeleportDelay = "TeleportDelay";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<TeleportLocationsComponent, ActivatableUIOpenAttemptEvent>((EntityEventRefHandler<TeleportLocationsComponent, ActivatableUIOpenAttemptEvent>)OnUiOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TeleportLocationsComponent, TeleportLocationDestinationMessage>((EntityEventRefHandler<TeleportLocationsComponent, TeleportLocationDestinationMessage>)OnTeleportToLocationRequest, (Type[])null, (Type[])null);
	}

	private void OnUiOpenAttempt(Entity<TeleportLocationsComponent> ent, ref ActivatableUIOpenAttemptEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (Delay.IsDelayed(Entity<UseDelayComponent>.op_Implicit(ent.Owner), "TeleportDelay"))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	protected virtual void OnTeleportToLocationRequest(Entity<TeleportLocationsComponent> ent, ref TeleportLocationDestinationMessage args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? telePointEnt = default(EntityUid?);
		if (((EntitySystem)this).TryGetEntity(args.NetEnt, ref telePointEnt) && !((EntitySystem)this).TerminatingOrDeleted(telePointEnt, (MetaDataComponent)null) && ((EntitySystem)this).HasComp<WarpPointComponent>(telePointEnt) && !Delay.IsDelayed(Entity<UseDelayComponent>.op_Implicit(ent.Owner), "TeleportDelay"))
		{
			TeleportLocationsComponent comp = ent.Comp;
			EntityUid originEnt = ((BaseBoundUserInterfaceEvent)args).Actor;
			TransformComponent telePointXForm = ((EntitySystem)this).Transform(telePointEnt.Value);
			EntProtoId? teleportEffect = comp.TeleportEffect;
			((EntitySystem)this).SpawnAtPosition(teleportEffect.HasValue ? EntProtoId.op_Implicit(teleportEffect.GetValueOrDefault()) : null, ((EntitySystem)this).Transform(originEnt).Coordinates, (ComponentRegistry)null);
			_xform.SetMapCoordinates(originEnt, _xform.GetMapCoordinates(telePointEnt.Value, telePointXForm));
			teleportEffect = comp.TeleportEffect;
			((EntitySystem)this).SpawnAtPosition(teleportEffect.HasValue ? EntProtoId.op_Implicit(teleportEffect.GetValueOrDefault()) : null, telePointXForm.Coordinates, (ComponentRegistry)null);
			Delay.TryResetDelay(ent.Owner, checkDelayed: true, null, "TeleportDelay");
			if (ent.Comp.CloseAfterTeleport)
			{
				_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)TeleportLocationUiKey.Key);
			}
		}
	}
}
