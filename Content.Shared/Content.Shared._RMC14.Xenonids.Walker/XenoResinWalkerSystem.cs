using System;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Walker;

public sealed class XenoResinWalkerSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoResinWalkerComponent, XenoResinWalkerActionEvent>((EntityEventRefHandler<XenoResinWalkerComponent, XenoResinWalkerActionEvent>)OnXenoResinWalkerAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoResinWalkerComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<XenoResinWalkerComponent, RefreshMovementSpeedModifiersEvent>)OnXenoResinWalkerRefreshMovementSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).UpdatesAfter.Add(typeof(SharedPhysicsSystem));
	}

	private void OnXenoResinWalkerAction(Entity<XenoResinWalkerComponent> xeno, ref XenoResinWalkerActionEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || (!xeno.Comp.Active && !_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PlasmaCost)))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		xeno.Comp.Active = !xeno.Comp.Active;
		xeno.Comp.NextPlasmaUse = _timing.CurTime + xeno.Comp.PlasmaUseDelay;
		((EntitySystem)this).Dirty<XenoResinWalkerComponent>(xeno, (MetaDataComponent)null);
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoResinWalkerComponent>.op_Implicit(xeno));
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoResinWalkerActionEvent>(Entity<XenoResinWalkerComponent>.op_Implicit(xeno)))
		{
			_actions.SetToggled(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action))), xeno.Comp.Active);
		}
	}

	private void OnXenoResinWalkerRefreshMovementSpeed(Entity<XenoResinWalkerComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		AffectableByWeedsComponent affected = default(AffectableByWeedsComponent);
		if (ent.Comp.Active && ((EntitySystem)this).TryComp<AffectableByWeedsComponent>(Entity<XenoResinWalkerComponent>.op_Implicit(ent), ref affected) && affected.OnXenoWeeds)
		{
			args.ModifySpeed(ent.Comp.SpeedMultiplier, ent.Comp.SpeedMultiplier);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<XenoResinWalkerComponent> query = ((EntitySystem)this).EntityQueryEnumerator<XenoResinWalkerComponent>();
		EntityUid uid = default(EntityUid);
		XenoResinWalkerComponent walker = default(XenoResinWalkerComponent);
		while (query.MoveNext(ref uid, ref walker))
		{
			if (walker.Active && !(_timing.CurTime < walker.NextPlasmaUse))
			{
				walker.NextPlasmaUse = _timing.CurTime + walker.PlasmaUseDelay;
				if (!_xenoPlasma.TryRemovePlasma(Entity<XenoPlasmaComponent>.op_Implicit(uid), walker.PlasmaUpkeep))
				{
					walker.Active = false;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)walker, (MetaDataComponent)null);
					_movementSpeed.RefreshMovementSpeedModifiers(uid);
				}
			}
		}
	}
}
