using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Abduct;
using Content.Shared._RMC14.Xenonids.Tail_Lash;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Dislocate;

public sealed class XenoDislocateSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private RMCSlowSystem _slow;

	[Dependency]
	private SharedRMCMeleeWeaponSystem _rmcMelee;

	[Dependency]
	private StandingStateSystem _standing;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private RMCSizeStunSystem _sizeStun;

	[Dependency]
	private RMCObstacleSlammingSystem _obstacleSlamming;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoDislocateComponent, XenoDislocateActionEvent>((EntityEventRefHandler<XenoDislocateComponent, XenoDislocateActionEvent>)OnDislocateAction, (Type[])null, (Type[])null);
	}

	private void OnDislocateAction(Entity<XenoDislocateComponent> xeno, ref XenoDislocateActionEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_rmcActions.TryUseAction(args))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (_net.IsServer)
		{
			_audio.PlayPvs(xeno.Comp.Sound, Entity<XenoDislocateComponent>.op_Implicit(xeno), (AudioParams?)null);
		}
		EntityUid targetId = args.Target;
		_rmcPulling.TryStopAllPullsFromAndOn(targetId);
		bool isDebuffed = ((EntitySystem)this).HasComp<RMCSlowdownComponent>(targetId) || ((EntitySystem)this).HasComp<RMCSuperSlowdownComponent>(targetId) || ((EntitySystem)this).HasComp<RMCRootedComponent>(targetId) || ((EntitySystem)this).HasComp<StunnedComponent>(targetId) || _standing.IsDown(targetId);
		if (_damageable.TryChangeDamage(targetId, xeno.Comp.Damage, isDebuffed, interruptsDoAfters: true, null, Entity<XenoDislocateComponent>.op_Implicit(xeno), Entity<XenoDislocateComponent>.op_Implicit(xeno))?.GetTotal() > FixedPoint2.Zero)
		{
			Filter filter = Filter.Pvs(targetId, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
			_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { targetId }, filter);
		}
		if (isDebuffed)
		{
			_slow.TryRoot(targetId, _xeno.TryApplyXenoDebuffMultiplier(targetId, xeno.Comp.RootTime));
		}
		else if (_net.IsServer)
		{
			MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoDislocateComponent>.op_Implicit(xeno), (TransformComponent)null);
			_rmcMelee.DoLunge(Entity<XenoDislocateComponent>.op_Implicit(xeno), targetId);
			_obstacleSlamming.MakeImmune(targetId);
			_sizeStun.KnockBack(targetId, origin, xeno.Comp.FlingRange, xeno.Comp.FlingRange, 10f);
			((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.Effect), targetId.ToCoordinates(), (ComponentRegistry)null, default(Angle));
		}
		RefreshCooldowns(xeno);
	}

	private void RefreshCooldowns(Entity<XenoDislocateComponent> xeno)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity<ActionComponent> action in _actions.GetActions(Entity<XenoDislocateComponent>.op_Implicit(xeno)))
		{
			BaseActionEvent actionEvent = _actions.GetEvent(Entity<ActionComponent>.op_Implicit(action));
			if ((actionEvent is XenoAbductActionEvent || actionEvent is XenoTailLashActionEvent) && action.Comp.Cooldown.HasValue)
			{
				TimeSpan cooldownEnd = action.Comp.Cooldown.Value.End - xeno.Comp.CooldownReductionTime;
				if (cooldownEnd < action.Comp.Cooldown.Value.Start)
				{
					_actions.ClearCooldown(action.AsNullable());
				}
				else
				{
					_actions.SetCooldown(action.AsNullable(), action.Comp.Cooldown.Value.Start, cooldownEnd);
				}
			}
		}
	}
}
