using System;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Xenonids.GasToggle;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.AcidShroud;

public sealed class XenoAcidShroudSystem : EntitySystem
{
	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoAcidShroudComponent, XenoAcidShroudActionEvent>((EntityEventRefHandler<XenoAcidShroudComponent, XenoAcidShroudActionEvent>)OnAcidShroudAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoAcidShroudComponent, DoAfterAttemptEvent<XenoAcidShroudDoAfterEvent>>((EntityEventRefHandler<XenoAcidShroudComponent, DoAfterAttemptEvent<XenoAcidShroudDoAfterEvent>>)OnAcidShroudDoAfterAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoAcidShroudComponent, XenoAcidShroudDoAfterEvent>((EntityEventRefHandler<XenoAcidShroudComponent, XenoAcidShroudDoAfterEvent>)OnAcidShroudDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoAcidShroudComponent, XenoGasToggleActionEvent>((EntityEventRefHandler<XenoAcidShroudComponent, XenoGasToggleActionEvent>)OnToggleType, (Type[])null, (Type[])null);
	}

	private void OnAcidShroudAction(Entity<XenoAcidShroudComponent> ent, ref XenoAcidShroudActionEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		XenoAcidShroudDoAfterEvent ev = new XenoAcidShroudDoAfterEvent();
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoAcidShroudComponent>.op_Implicit(ent), ent.Comp.DoAfter, ev, Entity<XenoAcidShroudComponent>.op_Implicit(ent), Entity<ActionComponent>.op_Implicit(args.Action))
		{
			BreakOnMove = true
		};
		_doAfter.TryStartDoAfter(doAfter);
		_rmcActions.DisableSharedCooldownEvents(Entity<ActionSharedCooldownComponent>.op_Implicit(args.Action.Owner), Entity<XenoAcidShroudComponent>.op_Implicit(ent));
	}

	private void OnAcidShroudDoAfterAttempt(Entity<XenoAcidShroudComponent> ent, ref DoAfterAttemptEvent<XenoAcidShroudDoAfterEvent> args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.Event.Target;
		if (target.HasValue)
		{
			EntityUid action = target.GetValueOrDefault();
			ActionComponent actionComp = default(ActionComponent);
			if (((EntitySystem)this).HasComp<InstantActionComponent>(action) && ((EntitySystem)this).TryComp<ActionComponent>(action, ref actionComp) && !actionComp.Enabled)
			{
				_rmcActions.EnableSharedCooldownEvents(Entity<ActionSharedCooldownComponent>.op_Implicit(action), Entity<XenoAcidShroudComponent>.op_Implicit(ent));
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
	}

	private void OnAcidShroudDoAfter(Entity<XenoAcidShroudComponent> ent, ref XenoAcidShroudDoAfterEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.Target;
		if (target.HasValue)
		{
			EntityUid action = target.GetValueOrDefault();
			_rmcActions.EnableSharedCooldownEvents(Entity<ActionSharedCooldownComponent>.op_Implicit(action), Entity<XenoAcidShroudComponent>.op_Implicit(ent));
			if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
			{
				((HandledEntityEventArgs)args).Handled = true;
				EntityUid spawn = ((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(ent.Comp.Spawn), ent.Owner.ToCoordinates(), (ComponentRegistry)null);
				_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner), Entity<HiveMemberComponent>.op_Implicit(spawn));
				_rmcActions.ActivateSharedCooldown(Entity<ActionSharedCooldownComponent>.op_Implicit(action), Entity<XenoAcidShroudComponent>.op_Implicit(ent));
			}
		}
	}

	private void OnToggleType(Entity<XenoAcidShroudComponent> ent, ref XenoGasToggleActionEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Gases.Length != 0)
		{
			int index = Array.IndexOf(ent.Comp.Gases, ent.Comp.Spawn);
			index = ((index != -1 && index < ent.Comp.Gases.Length - 1) ? (index + 1) : 0);
			ent.Comp.Spawn = ent.Comp.Gases[index];
			((EntitySystem)this).Dirty<XenoAcidShroudComponent>(ent, (MetaDataComponent)null);
		}
	}
}
