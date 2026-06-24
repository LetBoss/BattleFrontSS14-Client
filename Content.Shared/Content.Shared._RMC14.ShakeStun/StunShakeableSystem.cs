using System;
using Content.Shared._RMC14.Stamina;
using Content.Shared._RMC14.Standing;
using Content.Shared._RMC14.Tackle;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.ShakeStun;

public sealed class StunShakeableSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogs;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private RMCStandingSystem _rmcStanding;

	[Dependency]
	private StatusEffectsSystem _statusEffects;

	[Dependency]
	private IGameTiming _timing;

	private static readonly ProtoId<StatusEffectPrototype> Stun = ProtoId<StatusEffectPrototype>.op_Implicit("Stun");

	private static readonly ProtoId<StatusEffectPrototype> KnockedDown = ProtoId<StatusEffectPrototype>.op_Implicit("KnockedDown");

	private static readonly ProtoId<StatusEffectPrototype> Unconscious = ProtoId<StatusEffectPrototype>.op_Implicit("Unconscious");

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<StunShakeableComponent, InteractHandEvent>((EntityEventRefHandler<StunShakeableComponent, InteractHandEvent>)OnStunShakeableInteractHand, new Type[1] { typeof(InteractionPopupSystem) }, (Type[])null);
	}

	private void OnStunShakeableInteractHand(Entity<StunShakeableComponent> ent, ref InteractHandEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid user = args.User;
		StunShakeableUserComponent shakeableUser = default(StunShakeableUserComponent);
		if (user == args.Target || !((EntitySystem)this).TryComp<StunShakeableUserComponent>(user, ref shakeableUser))
		{
			return;
		}
		EntityUid target = args.Target;
		RMCRestComponent rest = ((EntitySystem)this).CompOrNull<RMCRestComponent>(target);
		if (!_statusEffects.HasStatusEffect(target, ProtoId<StatusEffectPrototype>.op_Implicit(Stun)) && !_statusEffects.HasStatusEffect(target, ProtoId<StatusEffectPrototype>.op_Implicit(KnockedDown)) && !_statusEffects.HasStatusEffect(target, ProtoId<StatusEffectPrototype>.op_Implicit(Unconscious)) && !((EntitySystem)this).HasComp<TackledRecentlyByComponent>(target) && (rest == null || !rest.Resting))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		TimeSpan time = _timing.CurTime;
		if (time < shakeableUser.LastShake + shakeableUser.Cooldown)
		{
			return;
		}
		shakeableUser.LastShake = time;
		((EntitySystem)this).Dirty(user, (IComponent)(object)shakeableUser, (MetaDataComponent)null);
		RMCStaminaComponent stamina = default(RMCStaminaComponent);
		if (((EntitySystem)this).TryComp<RMCStaminaComponent>(Entity<StunShakeableComponent>.op_Implicit(ent), ref stamina) && stamina.Level >= 4)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-shake-awake-stamina", (ValueTuple<string, object>)("target", target)), target, user);
			return;
		}
		_rmcStanding.SetRest(Entity<RMCRestComponent>.op_Implicit(target), resting: false);
		_statusEffects.TryRemoveTime(target, ProtoId<StatusEffectPrototype>.op_Implicit(Stun), ent.Comp.DurationRemoved);
		_statusEffects.TryRemoveTime(target, ProtoId<StatusEffectPrototype>.op_Implicit(KnockedDown), ent.Comp.DurationRemoved);
		_statusEffects.TryRemoveTime(target, ProtoId<StatusEffectPrototype>.op_Implicit(Unconscious), ent.Comp.DurationRemoved);
		((EntitySystem)this).RemCompDeferred<TackledRecentlyByComponent>(target);
		string userPopup = base.Loc.GetString("rmc-shake-awake-user", (ValueTuple<string, object>)("target", target));
		_popup.PopupClient(userPopup, target, user);
		string targetPopup = base.Loc.GetString("rmc-shake-awake-target", (ValueTuple<string, object>)("user", user));
		_popup.PopupEntity(targetPopup, target, target);
		if (_net.IsServer)
		{
			_audio.PlayEntity(ent.Comp.ShakeSound, Filter.Pvs(target, 2f, (IEntityManager)null, (ISharedPlayerManager)null, (IConfigurationManager)null), target, false, (AudioParams?)null);
		}
		string othersPopup = base.Loc.GetString("rmc-shake-awake-others", (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("target", target));
		Filter others = Filter.PvsExcept(target, 2f, (IEntityManager)null).RemovePlayerByAttachedEntity(user);
		_popup.PopupEntity(othersPopup, target, others, recordReplay: true);
		ISharedAdminLogManager adminLogs = _adminLogs;
		LogStringHandler handler = new LogStringHandler(22, 2);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
		handler.AppendLiteral(" shook ");
		handler.AppendFormatted<EntityUid>(target, "target");
		handler.AppendLiteral(" out of a stun.");
		adminLogs.Add(LogType.RMCStunShake, ref handler);
	}
}
