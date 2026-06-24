using System;
using Content.Shared._RMC14.Actions;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Stun;

public sealed class RMCDazedSystem : EntitySystem
{
	[Dependency]
	private SharedChargesSystem _charges;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private StatusEffectsSystem _statusEffect;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private ISharedPlayerManager _playerManager;

	[Dependency]
	private SharedStutteringSystem _stutter;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCDazedComponent, DazedEvent>((EntityEventRefHandler<RMCDazedComponent, DazedEvent>)OnDazed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDazedComponent, ComponentShutdown>((EntityEventRefHandler<RMCDazedComponent, ComponentShutdown>)OnDazedEnd, (Type[])null, (Type[])null);
	}

	private void OnDazed(Entity<RMCDazedComponent> ent, ref DazedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		EntityUid val = default(EntityUid);
		ActionComponent actionComponent = default(ActionComponent);
		RMCDazeableActionComponent rMCDazeableActionComponent = default(RMCDazeableActionComponent);
		foreach (Entity<ActionComponent> action in _actions.GetActions(Entity<RMCDazedComponent>.op_Implicit(ent)))
		{
			action.Deconstruct(ref val, ref actionComponent);
			EntityUid actionId = val;
			if (((EntitySystem)this).TryComp<RMCDazeableActionComponent>(actionId, ref rMCDazeableActionComponent))
			{
				_actions.SetEnabled(Entity<ActionComponent>.op_Implicit(actionId), enabled: false);
				if (((EntitySystem)this).HasComp<LimitedChargesComponent>(actionId))
				{
					_charges.SetCharges(Entity<LimitedChargesComponent>.op_Implicit(actionId), 0);
				}
			}
		}
	}

	private void OnDazedEnd(Entity<RMCDazedComponent> ent, ref ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid val = default(EntityUid);
		ActionComponent actionComponent = default(ActionComponent);
		RMCDazeableActionComponent rMCDazeableActionComponent = default(RMCDazeableActionComponent);
		foreach (Entity<ActionComponent> action in _actions.GetActions(Entity<RMCDazedComponent>.op_Implicit(ent)))
		{
			action.Deconstruct(ref val, ref actionComponent);
			EntityUid actionId = val;
			if (((EntitySystem)this).TryComp<RMCDazeableActionComponent>(actionId, ref rMCDazeableActionComponent))
			{
				_actions.SetEnabled(Entity<ActionComponent>.op_Implicit(actionId), enabled: true);
				_charges.ResetCharges(Entity<LimitedChargesComponent>.op_Implicit(actionId));
			}
		}
		ICommonSession session = default(ICommonSession);
		if (_net.IsServer && _playerManager.TryGetSessionByEntity(ent.Owner, ref session))
		{
			DazedComponentShutdownEvent ev = new DazedComponentShutdownEvent();
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)ev, session.Channel);
		}
	}

	public bool TryDaze(EntityUid uid, TimeSpan time, bool refresh = false, StatusEffectsComponent? status = null, bool stutter = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return false;
		}
		if (time <= TimeSpan.Zero)
		{
			return false;
		}
		if (_statusEffect.TryAddStatusEffect<RMCDazedComponent>(uid, "Dazed", time, refresh, status, false))
		{
			if (stutter)
			{
				_stutter.DoStutter(uid, time, refresh: true);
			}
			DazedEvent ev = new DazedEvent(time);
			((EntitySystem)this).RaiseLocalEvent<DazedEvent>(uid, ref ev, false);
			return true;
		}
		return false;
	}
}
