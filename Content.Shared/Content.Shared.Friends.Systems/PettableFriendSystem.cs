using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Friends.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Systems;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.Friends.Systems;

public sealed class PettableFriendSystem : EntitySystem
{
	[Dependency]
	private NpcFactionSystem _factionException;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private UseDelaySystem _useDelay;

	private EntityQuery<FactionExceptionComponent> _exceptionQuery;

	private EntityQuery<UseDelayComponent> _useDelayQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_exceptionQuery = ((EntitySystem)this).GetEntityQuery<FactionExceptionComponent>();
		_useDelayQuery = ((EntitySystem)this).GetEntityQuery<UseDelayComponent>();
		((EntitySystem)this).SubscribeLocalEvent<PettableFriendComponent, UseInHandEvent>((EntityEventRefHandler<PettableFriendComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PettableFriendComponent, GotRehydratedEvent>((EntityEventRefHandler<PettableFriendComponent, GotRehydratedEvent>)OnRehydrated, (Type[])null, (Type[])null);
	}

	private void OnUseInHand(Entity<PettableFriendComponent> ent, ref UseInHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		Entity<PettableFriendComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		PettableFriendComponent pettableFriendComponent = default(PettableFriendComponent);
		val.Deconstruct(ref val2, ref pettableFriendComponent);
		EntityUid uid = val2;
		PettableFriendComponent comp = pettableFriendComponent;
		EntityUid user = args.User;
		FactionExceptionComponent exceptionComp = default(FactionExceptionComponent);
		if (!((HandledEntityEventArgs)args).Handled && _exceptionQuery.TryComp(uid, ref exceptionComp))
		{
			(EntityUid, FactionExceptionComponent) exception = (uid, exceptionComp);
			UseDelayComponent useDelay = default(UseDelayComponent);
			if (!_factionException.IsIgnored(Entity<FactionExceptionComponent>.op_Implicit(exception), user))
			{
				_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(comp.SuccessString), (ValueTuple<string, object>)("target", uid)), user, user);
				_factionException.IgnoreEntity(Entity<FactionExceptionComponent>.op_Implicit(exception), Entity<FactionExceptionTrackerComponent>.op_Implicit(user));
				((HandledEntityEventArgs)args).Handled = true;
			}
			else if (!_useDelayQuery.TryComp(uid, ref useDelay) || _useDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((uid, useDelay)), checkDelayed: true))
			{
				_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(comp.FailureString), (ValueTuple<string, object>)("target", uid)), user, user);
			}
		}
	}

	private void OnRehydrated(Entity<PettableFriendComponent> ent, ref GotRehydratedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		FactionExceptionComponent comp = default(FactionExceptionComponent);
		if (((EntitySystem)this).TryComp<FactionExceptionComponent>(Entity<PettableFriendComponent>.op_Implicit(ent), ref comp))
		{
			_factionException.IgnoreEntities(Entity<FactionExceptionComponent>.op_Implicit(args.Target), comp.Ignored);
		}
	}
}
