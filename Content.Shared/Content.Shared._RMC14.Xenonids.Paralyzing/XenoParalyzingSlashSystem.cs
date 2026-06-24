using System;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Synth;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Jittering;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Paralyzing;

public sealed class XenoParalyzingSlashSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedJitteringSystem _jitter;

	[Dependency]
	private RMCDazedSystem _daze;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoParalyzingSlashComponent, XenoParalyzingSlashActionEvent>((EntityEventRefHandler<XenoParalyzingSlashComponent, XenoParalyzingSlashActionEvent>)OnXenoParalyzingSlashAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveParalyzingSlashComponent, MeleeHitEvent>((EntityEventRefHandler<XenoActiveParalyzingSlashComponent, MeleeHitEvent>)OnXenoParalyzingSlashHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveParalyzingSlashComponent, ComponentShutdown>((EntityEventRefHandler<XenoActiveParalyzingSlashComponent, ComponentShutdown>)OnXenoParalyzingSlashRemoved, (Type[])null, (Type[])null);
	}

	private void OnXenoParalyzingSlashAction(Entity<XenoParalyzingSlashComponent> xeno, ref XenoParalyzingSlashActionEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_rmcActions.TryUseAction(args))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		XenoActiveParalyzingSlashComponent active = ((EntitySystem)this).EnsureComp<XenoActiveParalyzingSlashComponent>(Entity<XenoParalyzingSlashComponent>.op_Implicit(xeno));
		active.ExpireAt = _timing.CurTime + xeno.Comp.ActiveDuration;
		active.ParalyzeDelay = xeno.Comp.StunDelay;
		active.ParalyzeDuration = xeno.Comp.StunDuration;
		active.DazeTime = xeno.Comp.DazeTime;
		((EntitySystem)this).Dirty(Entity<XenoParalyzingSlashComponent>.op_Implicit(xeno), (IComponent)(object)active, (MetaDataComponent)null);
		_popup.PopupClient(base.Loc.GetString("cm-xeno-paralyzing-slash-activate"), Entity<XenoParalyzingSlashComponent>.op_Implicit(xeno), Entity<XenoParalyzingSlashComponent>.op_Implicit(xeno));
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoParalyzingSlashActionEvent>(Entity<XenoParalyzingSlashComponent>.op_Implicit(xeno)))
		{
			_actions.SetToggled(action.AsNullable(), toggled: true);
		}
	}

	private void OnXenoParalyzingSlashRemoved(Entity<XenoActiveParalyzingSlashComponent> xeno, ref ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoParalyzingSlashActionEvent>(Entity<XenoActiveParalyzingSlashComponent>.op_Implicit(xeno)))
		{
			_actions.SetToggled(action.AsNullable(), toggled: false);
		}
	}

	private void OnXenoParalyzingSlashHit(Entity<XenoActiveParalyzingSlashComponent> xeno, ref MeleeHitEvent args)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsHit || args.HitEntities.Count == 0)
		{
			return;
		}
		foreach (EntityUid entity in args.HitEntities)
		{
			if (!_xeno.CanAbilityAttackTarget(Entity<XenoActiveParalyzingSlashComponent>.op_Implicit(xeno), entity) || ((EntitySystem)this).HasComp<VictimBeingParalyzedComponent>(entity) || ((EntitySystem)this).HasComp<XenoComponent>(entity))
			{
				continue;
			}
			if (((EntitySystem)this).HasComp<SynthComponent>(entity))
			{
				string immuneMsg = base.Loc.GetString("cm-xeno-paralyzing-slash-immune", (ValueTuple<string, object>)("target", entity));
				_popup.PopupEntity(immuneMsg, entity, entity, PopupType.SmallCaution);
				continue;
			}
			_daze.TryDaze(entity, xeno.Comp.DazeTime, refresh: true, null, stutter: true);
			_jitter.DoJitter(entity, xeno.Comp.ParalyzeDelay, refresh: true);
			if (!((EntitySystem)this).HasComp<XenoComponent>(entity))
			{
				VictimBeingParalyzedComponent victim = ((EntitySystem)this).EnsureComp<VictimBeingParalyzedComponent>(entity);
				victim.ParalyzeAt = _timing.CurTime + xeno.Comp.ParalyzeDelay;
				victim.ParalyzeDuration = xeno.Comp.ParalyzeDuration;
				((EntitySystem)this).Dirty(entity, (IComponent)(object)victim, (MetaDataComponent)null);
			}
			string message = base.Loc.GetString("cm-xeno-paralyzing-slash-hit", (ValueTuple<string, object>)("target", entity));
			if (_net.IsServer)
			{
				_popup.PopupEntity(message, entity, Entity<XenoActiveParalyzingSlashComponent>.op_Implicit(xeno));
			}
			((EntitySystem)this).RemCompDeferred<XenoActiveParalyzingSlashComponent>(Entity<XenoActiveParalyzingSlashComponent>.op_Implicit(xeno));
			break;
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		if (_net.IsServer)
		{
			EntityQueryEnumerator<XenoActiveParalyzingSlashComponent> activeQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoActiveParalyzingSlashComponent>();
			EntityUid uid = default(EntityUid);
			XenoActiveParalyzingSlashComponent active = default(XenoActiveParalyzingSlashComponent);
			while (activeQuery.MoveNext(ref uid, ref active))
			{
				if (!(active.ExpireAt > time))
				{
					((EntitySystem)this).RemCompDeferred<XenoActiveParalyzingSlashComponent>(uid);
					_popup.PopupEntity(base.Loc.GetString("cm-xeno-paralyzing-slash-expire"), uid, uid, PopupType.SmallCaution);
				}
			}
		}
		EntityQueryEnumerator<VictimBeingParalyzedComponent> victimQuery = ((EntitySystem)this).EntityQueryEnumerator<VictimBeingParalyzedComponent>();
		EntityUid uid2 = default(EntityUid);
		VictimBeingParalyzedComponent victim = default(VictimBeingParalyzedComponent);
		while (victimQuery.MoveNext(ref uid2, ref victim))
		{
			if (!(victim.ParalyzeAt > time))
			{
				((EntitySystem)this).RemCompDeferred<VictimBeingParalyzedComponent>(uid2);
				_stun.TryParalyze(uid2, victim.ParalyzeDuration, refresh: true);
			}
		}
	}
}
