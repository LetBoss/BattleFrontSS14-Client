using System;
using Content.Shared.CombatMode;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared.Throwing;

public sealed class CatchableSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private ThrownItemSystem _thrown;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	private EntityQuery<HandsComponent> _handsQuery;

	private EntityQuery<CombatModeComponent> _combatModeQuery;

	public override void Initialize()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CatchableComponent, ThrowDoHitEvent>((EntityEventRefHandler<CatchableComponent, ThrowDoHitEvent>)OnDoHit, (Type[])null, (Type[])null);
		_handsQuery = ((EntitySystem)this).GetEntityQuery<HandsComponent>();
		_combatModeQuery = ((EntitySystem)this).GetEntityQuery<CombatModeComponent>();
	}

	private void OnDoHit(Entity<CatchableComponent> ent, ref ThrowDoHitEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent handsComp = default(HandsComponent);
		CombatModeComponent combatModeComp = default(CombatModeComponent);
		if (!_handsQuery.TryGetComponent(args.Target, ref handsComp) || (ent.Comp.RequireCombatMode && (!_combatModeQuery.TryComp(args.Target, ref combatModeComp) || !combatModeComp.IsInCombatMode)) || !_whitelist.IsWhitelistPassOrNull(ent.Comp.CatcherWhitelist, args.Target))
		{
			return;
		}
		CatchAttemptEvent attemptEv = new CatchAttemptEvent(ent.Owner, ent.Comp.CatchChance);
		((EntitySystem)this).RaiseLocalEvent<CatchAttemptEvent>(args.Target, ref attemptEv, false);
		if (!attemptEv.Cancelled && !(new System.Random(HashCode.Combine((int)_timing.CurTick.Value, ((EntitySystem)this).GetNetEntity(Entity<CatchableComponent>.op_Implicit(ent), (MetaDataComponent)null).Id)).NextDouble() >= (double)ent.Comp.CatchChance) && _hands.TryPickupAnyHand(args.Target, ent.Owner, checkActionBlocker: true, animateUser: false, animate: false, handsComp))
		{
			_thrown.StopThrow(ent.Owner, args.Component);
			if (!_net.IsClient)
			{
				string selfMessage = base.Loc.GetString("catchable-component-success-self", (ValueTuple<string, object>)("item", ent.Owner), (ValueTuple<string, object>)("catcher", Identity.Entity(args.Target, (IEntityManager)(object)base.EntityManager)));
				string othersMessage = base.Loc.GetString("catchable-component-success-others", (ValueTuple<string, object>)("item", ent.Owner), (ValueTuple<string, object>)("catcher", Identity.Entity(args.Target, (IEntityManager)(object)base.EntityManager)));
				_popup.PopupEntity(selfMessage, args.Target, args.Target);
				_popup.PopupEntity(othersMessage, args.Target, Filter.PvsExcept(args.Target, 2f, (IEntityManager)null), recordReplay: true);
				_audio.PlayPvs(ent.Comp.CatchSuccessSound, args.Target, (AudioParams?)null);
			}
		}
	}
}
