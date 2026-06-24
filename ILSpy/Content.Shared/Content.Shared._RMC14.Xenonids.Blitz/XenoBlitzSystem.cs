using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Stunnable;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Blitz;

public sealed class XenoBlitzSystem : EntitySystem
{
	[Dependency]
	private XenoPlasmaSystem _plasma;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private DamageableSystem _damage;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private MobStateSystem _mob;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private VanguardShieldSystem _vanguard;

	[Dependency]
	private SharedInteractionSystem _interact;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoBlitzComponent, XenoLeapActionEvent>((EntityEventRefHandler<XenoBlitzComponent, XenoLeapActionEvent>)OnLeapBlitz, new Type[1] { typeof(XenoLeapSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoBlitzComponent, XenoBlitzEvent>((EntityEventRefHandler<XenoBlitzComponent, XenoBlitzEvent>)OnAttackBlitz, (Type[])null, (Type[])null);
	}

	private void OnLeapBlitz(Entity<XenoBlitzComponent> xeno, ref XenoLeapActionEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || ((EntitySystem)this).HasComp<XenoLeapingComponent>(Entity<XenoBlitzComponent>.op_Implicit(xeno)))
		{
			return;
		}
		if (xeno.Comp.Dashed && xeno.Comp.SlashReady)
		{
			XenoBlitzEvent ev = default(XenoBlitzEvent);
			((EntitySystem)this).RaiseLocalEvent<XenoBlitzEvent>(Entity<XenoBlitzComponent>.op_Implicit(xeno), ref ev, false);
			((HandledEntityEventArgs)args).Handled = true;
		}
		else if (xeno.Comp.Dashed)
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
		else
		{
			XenoPlasmaComponent plasma = default(XenoPlasmaComponent);
			if (!((EntitySystem)this).TryComp<XenoPlasmaComponent>(Entity<XenoBlitzComponent>.op_Implicit(xeno), ref plasma) || !_plasma.HasPlasma(Entity<XenoPlasmaComponent>.op_Implicit((xeno.Owner, plasma)), xeno.Comp.PlasmaCost))
			{
				return;
			}
			xeno.Comp.Dashed = true;
			_actions.SetUseDelay(Entity<ActionComponent>.op_Implicit(args.Action.Owner), xeno.Comp.BaseUseDelay);
			xeno.Comp.FirstPartActivatedAt = _timing.CurTime;
			foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoLeapActionEvent>(Entity<XenoBlitzComponent>.op_Implicit(xeno)))
			{
				_actions.SetToggled(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action))), toggled: true);
			}
		}
		((EntitySystem)this).Dirty<XenoBlitzComponent>(xeno, (MetaDataComponent)null);
	}

	private void OnAttackBlitz(Entity<XenoBlitzComponent> xeno, ref XenoBlitzEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		xeno.Comp.Dashed = false;
		xeno.Comp.SlashReady = false;
		SetBlitzDelays(xeno);
		if (!_mob.IsAlive(Entity<XenoBlitzComponent>.op_Implicit(xeno)) || ((EntitySystem)this).HasComp<StunnedComponent>(Entity<XenoBlitzComponent>.op_Implicit(xeno)))
		{
			return;
		}
		XenoLeapAttemptEvent ev = default(XenoLeapAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoLeapAttemptEvent>(Entity<XenoBlitzComponent>.op_Implicit(xeno), ref ev, false);
		if (ev.Cancelled)
		{
			return;
		}
		((EntitySystem)this).EnsureComp<XenoSweepingComponent>(Entity<XenoBlitzComponent>.op_Implicit(xeno));
		int hits = 0;
		foreach (Entity<MobStateComponent> hit in _lookup.GetEntitiesInRange<MobStateComponent>(_transform.GetMapCoordinates(Entity<XenoBlitzComponent>.op_Implicit(xeno), (TransformComponent)null), xeno.Comp.Range, (LookupFlags)110))
		{
			if (!_xeno.CanAbilityAttackTarget(Entity<XenoBlitzComponent>.op_Implicit(xeno), Entity<MobStateComponent>.op_Implicit(hit)) || !_interact.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(xeno.Owner), Entity<TransformComponent>.op_Implicit(hit.Owner), xeno.Comp.Range))
			{
				continue;
			}
			hits++;
			if (_damage.TryChangeDamage(Entity<MobStateComponent>.op_Implicit(hit), _xeno.TryApplyXenoSlashDamageMultiplier(Entity<MobStateComponent>.op_Implicit(hit), xeno.Comp.Damage), ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoBlitzComponent>.op_Implicit(xeno), Entity<XenoBlitzComponent>.op_Implicit(xeno))?.GetTotal() > FixedPoint2.Zero)
			{
				Filter filter = Filter.Pvs(Entity<MobStateComponent>.op_Implicit(hit), 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
				_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { Entity<MobStateComponent>.op_Implicit(hit) }, filter);
			}
			if (_net.IsServer)
			{
				((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.Effect), hit.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
			}
		}
		if (_net.IsServer && hits > 0)
		{
			_audio.PlayPvs(xeno.Comp.Sound, Entity<XenoBlitzComponent>.op_Implicit(xeno), (AudioParams?)null);
		}
		if (hits >= xeno.Comp.HitsToRecharge)
		{
			_vanguard.RegenShield(Entity<XenoBlitzComponent>.op_Implicit(xeno));
		}
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoLeapActionEvent>(Entity<XenoBlitzComponent>.op_Implicit(xeno)))
		{
			_actions.SetToggled(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action))), toggled: false);
		}
		((EntitySystem)this).Dirty<XenoBlitzComponent>(xeno, (MetaDataComponent)null);
	}

	private void SetBlitzDelays(Entity<XenoBlitzComponent> xeno)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? bliz = null;
		using (IEnumerator<Entity<ActionComponent>> enumerator = _rmcActions.GetActionsWithEvent<XenoLeapActionEvent>(Entity<XenoBlitzComponent>.op_Implicit(xeno)).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				Entity<ActionComponent> action = enumerator.Current;
				bliz = Entity<ActionComponent>.op_Implicit(action);
			}
		}
		if (bliz.HasValue)
		{
			TimeSpan blitzCooldownTime = xeno.Comp.FinishedUseDelay - (_timing.CurTime - xeno.Comp.FirstPartActivatedAt);
			if (blitzCooldownTime < TimeSpan.Zero)
			{
				blitzCooldownTime = TimeSpan.Zero;
			}
			SharedActionsSystem actions = _actions;
			EntityUid? val = bliz;
			actions.SetUseDelay(val.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(val.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), blitzCooldownTime);
			SharedActionsSystem actions2 = _actions;
			val = bliz;
			actions2.SetCooldown(val.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(val.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), blitzCooldownTime);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoBlitzComponent> blitzes = ((EntitySystem)this).EntityQueryEnumerator<XenoBlitzComponent>();
		EntityUid uid = default(EntityUid);
		XenoBlitzComponent dash = default(XenoBlitzComponent);
		while (blitzes.MoveNext(ref uid, ref dash))
		{
			if (dash.Dashed)
			{
				if (!((EntitySystem)this).HasComp<XenoLeapingComponent>(uid) && !dash.SlashReady)
				{
					dash.SlashAroundAt = time + dash.SlashDashTime;
					dash.SlashReady = true;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)dash, (MetaDataComponent)null);
				}
				else if (dash.SlashReady && !(time < dash.SlashAroundAt))
				{
					XenoBlitzEvent ev = default(XenoBlitzEvent);
					((EntitySystem)this).RaiseLocalEvent<XenoBlitzEvent>(uid, ref ev, false);
				}
			}
		}
	}
}
