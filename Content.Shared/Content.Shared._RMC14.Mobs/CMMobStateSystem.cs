using System;
using Content.Shared._RMC14.Sprite;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Mobs;

public sealed class CMMobStateSystem : EntitySystem
{
	[Dependency]
	private IConsoleHost _host;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedRMCSpriteSystem _rmcSprite;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MobStateActionsComponent, CMGhostActionEvent>((EntityEventRefHandler<MobStateActionsComponent, CMGhostActionEvent>)OnMobStateActionsGhost, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCMobStateDrawDepthComponent, GetDrawDepthEvent>((EntityEventRefHandler<RMCMobStateDrawDepthComponent, GetDrawDepthEvent>)OnMobStateDrawDepth, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCMobStateDrawDepthComponent, MobStateChangedEvent>((EntityEventRefHandler<RMCMobStateDrawDepthComponent, MobStateChangedEvent>)OnMobStateChanged, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<MobStateActionsComponent>(((EntitySystem)this).Subs, (object)CMMobStateActionsUI.Key, (BuiEventSubscriber<MobStateActionsComponent>)delegate(Subscriber<MobStateActionsComponent> subs)
		{
			subs.Event<CMGhostActionBuiMsg>((EntityEventRefHandler<MobStateActionsComponent, CMGhostActionBuiMsg>)OnGhostActionBuiMsg);
		});
	}

	private void OnMobStateActionsGhost(Entity<MobStateActionsComponent> ent, ref CMGhostActionEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		if (_mobState.IsDead(Entity<MobStateActionsComponent>.op_Implicit(ent)))
		{
			ActorComponent actor = default(ActorComponent);
			if (_net.IsServer && ((EntitySystem)this).TryComp<ActorComponent>(Entity<MobStateActionsComponent>.op_Implicit(ent), ref actor))
			{
				_host.ExecuteCommand(actor.PlayerSession, "ghost");
			}
		}
		else
		{
			((HandledEntityEventArgs)args).Handled = true;
			_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)CMMobStateActionsUI.Key, (EntityUid?)Entity<MobStateActionsComponent>.op_Implicit(ent), false);
		}
	}

	private void OnMobStateDrawDepth(Entity<RMCMobStateDrawDepthComponent> ent, ref GetDrawDepthEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		MobStateComponent mobState = default(MobStateComponent);
		if (((EntitySystem)this).TryComp<MobStateComponent>(Entity<RMCMobStateDrawDepthComponent>.op_Implicit(ent), ref mobState) && args.DrawDepth == ent.Comp.Default && ent.Comp.DrawDepths.TryGetValue(mobState.CurrentState, out var depth))
		{
			args.DrawDepth = depth;
		}
	}

	private void OnMobStateChanged(Entity<RMCMobStateDrawDepthComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_rmcSprite.UpdateDrawDepth(Entity<RMCMobStateDrawDepthComponent>.op_Implicit(ent));
	}

	private void OnGhostActionBuiMsg(Entity<MobStateActionsComponent> ent, ref CMGhostActionBuiMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? entity = default(EntityUid?);
		if (!_mobState.IsIncapacitated(Entity<MobStateActionsComponent>.op_Implicit(ent)) || !((EntitySystem)this).TryGetEntity(((BoundUserInterfaceMessage)args).Entity, ref entity))
		{
			return;
		}
		EntityUid? val = entity;
		EntityUid actor = ((BaseBoundUserInterfaceEvent)args).Actor;
		if (val.HasValue && !(val.GetValueOrDefault() != actor))
		{
			_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)CMMobStateActionsUI.Key);
			ActorComponent actor2 = default(ActorComponent);
			if (_net.IsServer && ((EntitySystem)this).TryComp<ActorComponent>(((BaseBoundUserInterfaceEvent)args).Actor, ref actor2))
			{
				_host.ExecuteCommand(actor2.PlayerSession, "ghost");
			}
		}
	}
}
