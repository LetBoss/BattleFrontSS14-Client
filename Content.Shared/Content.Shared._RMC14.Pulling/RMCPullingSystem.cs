using System;
using System.Numerics;
using Content.Shared._RMC14.Fireman;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.ActionBlocker;
using Content.Shared.Buckle.Components;
using Content.Shared.Coordinates;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.MouseRotator;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.Weapons.Melee;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Pulling;

public sealed class RMCPullingSystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedMeleeWeaponSystem _melee;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private SharedXenoParasiteSystem _parasite;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private PullingSystem _pulling;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private StatusEffectsSystem _statusEffects;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private RotateToFaceSystem _rotateTo;

	private readonly SoundSpecifier _pullSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Effects/thudswoosh.ogg", (AudioParams?)null)
	{
		Params = ((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.05f)
	};

	private const string PullEffect = "CMEffectGrab";

	private EntityQuery<FiremanCarriableComponent> _firemanQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_firemanQuery = ((EntitySystem)this).GetEntityQuery<FiremanCarriableComponent>();
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, RMCGetPullTargetEvent>((EntityEventRefHandler<BuckleComponent, RMCGetPullTargetEvent>)OnGetPullTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, RMCPullToggleEvent>((EntityEventRefHandler<XenoComponent, RMCPullToggleEvent>)OnXenoPullToggle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParalyzeOnPullAttemptComponent, PullAttemptEvent>((EntityEventRefHandler<ParalyzeOnPullAttemptComponent, PullAttemptEvent>)OnParalyzeOnPullAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InfectOnPullAttemptComponent, PullAttemptEvent>((EntityEventRefHandler<InfectOnPullAttemptComponent, PullAttemptEvent>)OnInfectOnPullAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MeleeWeaponComponent, PullAttemptEvent>((EntityEventRefHandler<MeleeWeaponComponent, PullAttemptEvent>)OnMeleePullAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SlowOnPullComponent, PullStartedMessage>((EntityEventRefHandler<SlowOnPullComponent, PullStartedMessage>)OnSlowPullStarted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SlowOnPullComponent, PullStoppedMessage>((EntityEventRefHandler<SlowOnPullComponent, PullStoppedMessage>)OnSlowPullStopped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullingSlowedComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<PullingSlowedComponent, RefreshMovementSpeedModifiersEvent>)OnPullingSlowedMovementSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullWhitelistComponent, PullAttemptEvent>((EntityEventRefHandler<PullWhitelistComponent, PullAttemptEvent>)OnPullWhitelistPullAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockPullingDeadComponent, PullAttemptEvent>((EntityEventRefHandler<BlockPullingDeadComponent, PullAttemptEvent>)OnBlockDeadPullAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockPullingDeadComponent, PullStartedMessage>((EntityEventRefHandler<BlockPullingDeadComponent, PullStartedMessage>)OnBlockDeadPullStarted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockPullingDeadComponent, PullStoppedMessage>((EntityEventRefHandler<BlockPullingDeadComponent, PullStoppedMessage>)OnBlockDeadPullStopped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PreventPulledWhileAliveComponent, PullAttemptEvent>((EntityEventRefHandler<PreventPulledWhileAliveComponent, PullAttemptEvent>)OnPreventPulledWhileAliveAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PreventPulledWhileAliveComponent, PullStartedMessage>((EntityEventRefHandler<PreventPulledWhileAliveComponent, PullStartedMessage>)OnPreventPulledWhileAliveStart, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PreventPulledWhileAliveComponent, PullStoppedMessage>((EntityEventRefHandler<PreventPulledWhileAliveComponent, PullStoppedMessage>)OnPreventPulledWhileAliveStop, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullableComponent, PullStartedMessage>((EntityEventRefHandler<PullableComponent, PullStartedMessage>)OnPullAnimation, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullerComponent, PullStoppedMessage>((EntityEventRefHandler<PullerComponent, PullStoppedMessage>)OnPullerPullStopped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BeingPulledComponent, PullStoppedMessage>((EntityEventRefHandler<BeingPulledComponent, PullStoppedMessage>)OnBeingPulledPullStopped, (Type[])null, (Type[])null);
	}

	private void OnGetPullTarget(Entity<BuckleComponent> ent, ref RMCGetPullTargetEvent ev)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!(ent.Owner != ev.Target) && !((EntitySystem)this).HasComp<XenoComponent>(ev.User) && ((EntitySystem)this).HasComp<RMCRetargetBucklePullComponent>(ent.Comp.BuckledTo))
		{
			ev.Target = ent.Comp.BuckledTo.Value;
		}
	}

	private void OnParalyzeOnPullAttempt(Entity<ParalyzeOnPullAttemptComponent> ent, ref PullAttemptEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.PullerUid;
		EntityUid target = args.PulledUid;
		if (!(target != ent.Owner) && !((EntitySystem)this).HasComp<ParalyzeOnPullAttemptImmuneComponent>(user) && !_mobState.IsDead(Entity<ParalyzeOnPullAttemptComponent>.op_Implicit(ent)))
		{
			args.Cancelled = true;
			SoundSpecifier sound = ent.Comp.Sound;
			if (sound != null)
			{
				float pitch = _random.NextFloat(ent.Comp.MinPitch, ent.Comp.MaxPitch);
				SharedAudioSystem audio = _audio;
				EntityUid val = Entity<ParalyzeOnPullAttemptComponent>.op_Implicit(ent);
				EntityUid? val2 = user;
				AudioParams val3 = sound.Params;
				audio.PlayPredicted(sound, val, val2, (AudioParams?)((AudioParams)(ref val3)).WithPitchScale(pitch));
			}
			_stun.TryParalyze(user, ent.Comp.Duration, refresh: true);
			EntityUid puller = user;
			EntityUid pulled = target;
			string othersMessage = base.Loc.GetString("rmc-pull-paralyze-others", (ValueTuple<string, object>)("puller", puller), (ValueTuple<string, object>)("pulled", pulled));
			string selfMessage = base.Loc.GetString("rmc-pull-paralyze-self", (ValueTuple<string, object>)("puller", puller), (ValueTuple<string, object>)("pulled", pulled));
			_popup.PopupPredicted(selfMessage, othersMessage, puller, puller, PopupType.MediumCaution);
		}
	}

	private void OnInfectOnPullAttempt(Entity<InfectOnPullAttemptComponent> ent, ref PullAttemptEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.PullerUid;
		EntityUid target = args.PulledUid;
		XenoParasiteComponent paraComp = default(XenoParasiteComponent);
		if (!(target != ent.Owner) && !((EntitySystem)this).HasComp<InfectOnPullAttemptImmuneComponent>(user) && !_mobState.IsDead(Entity<InfectOnPullAttemptComponent>.op_Implicit(ent)) && ((EntitySystem)this).TryComp<XenoParasiteComponent>(target, ref paraComp))
		{
			Entity<XenoParasiteComponent> comp = Entity<XenoParasiteComponent>.op_Implicit((target, paraComp));
			args.Cancelled = true;
			if (_parasite.Infect(comp, user, popup: false, force: true))
			{
				EntityUid puller = user;
				EntityUid pulled = target;
				string othersMessage = base.Loc.GetString("rmc-pull-infect-others", (ValueTuple<string, object>)("puller", puller), (ValueTuple<string, object>)("pulled", pulled));
				string selfMessage = base.Loc.GetString("rmc-pull-infect-self", (ValueTuple<string, object>)("puller", puller), (ValueTuple<string, object>)("pulled", pulled));
				_popup.PopupPredicted(selfMessage, othersMessage, puller, puller, PopupType.MediumCaution);
			}
		}
	}

	private void OnSlowPullStarted(Entity<SlowOnPullComponent> ent, ref PullStartedMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Owner == args.PullerUid)
		{
			((EntitySystem)this).EnsureComp<PullingSlowedComponent>(args.PullerUid);
			_movementSpeed.RefreshMovementSpeedModifiers(args.PullerUid);
		}
	}

	private void OnSlowPullStopped(Entity<SlowOnPullComponent> ent, ref PullStoppedMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Owner == args.PullerUid)
		{
			((EntitySystem)this).RemComp<PullingSlowedComponent>(args.PullerUid);
			_movementSpeed.RefreshMovementSpeedModifiers(args.PullerUid);
		}
	}

	private void OnPullingSlowedMovementSpeed(Entity<PullingSlowedComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		PullerComponent puller = default(PullerComponent);
		SlowOnPullComponent slow = default(SlowOnPullComponent);
		if (((EntitySystem)this).HasComp<BypassInteractionChecksComponent>(Entity<PullingSlowedComponent>.op_Implicit(ent)) || !((EntitySystem)this).TryComp<PullerComponent>(Entity<PullingSlowedComponent>.op_Implicit(ent), ref puller) || !((EntitySystem)this).TryComp<SlowOnPullComponent>(Entity<PullingSlowedComponent>.op_Implicit(ent), ref slow) || !puller.Pulling.HasValue)
		{
			return;
		}
		PullSlowdownAttemptEvent ev = new PullSlowdownAttemptEvent(puller.Pulling.Value);
		((EntitySystem)this).RaiseLocalEvent<PullSlowdownAttemptEvent>(Entity<PullingSlowedComponent>.op_Implicit(ent), ref ev, false);
		if (ev.Cancelled)
		{
			return;
		}
		foreach (SlowOnPullComponent.SlowdownWhitelist slowdown in slow.Slowdowns)
		{
			if (_whitelist.IsWhitelistPass(slowdown.Whitelist, puller.Pulling.Value))
			{
				args.ModifySpeed(slowdown.Multiplier, slowdown.Multiplier);
				return;
			}
		}
		args.ModifySpeed(slow.Multiplier, slow.Multiplier);
	}

	private void OnPullWhitelistPullAttempt(Entity<PullWhitelistComponent> ent, ref PullAttemptEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !(ent.Owner == args.PulledUid) && !_whitelist.IsValid(ent.Comp.Whitelist, args.PulledUid))
		{
			_popup.PopupClient(base.Loc.GetString("cm-pull-whitelist-denied", (ValueTuple<string, object>)("name", args.PulledUid)), args.PulledUid, args.PullerUid);
			args.Cancelled = true;
		}
	}

	private void OnBlockDeadPullAttempt(Entity<BlockPullingDeadComponent> ent, ref PullAttemptEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !(ent.Owner == args.PulledUid) && !CanPullDead(Entity<BlockPullingDeadComponent>.op_Implicit(ent), args.PulledUid))
		{
			_popup.PopupClient(base.Loc.GetString("cm-pull-whitelist-denied-dead", (ValueTuple<string, object>)("name", args.PulledUid)), args.PulledUid, args.PullerUid);
			args.Cancelled = true;
		}
	}

	private void OnBlockDeadPullStarted(Entity<BlockPullingDeadComponent> ent, ref PullStartedMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Owner == args.PullerUid)
		{
			((EntitySystem)this).EnsureComp<BlockPullingDeadActiveComponent>(Entity<BlockPullingDeadComponent>.op_Implicit(ent));
		}
	}

	private void OnBlockDeadPullStopped(Entity<BlockPullingDeadComponent> ent, ref PullStoppedMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Owner == args.PullerUid)
		{
			((EntitySystem)this).RemCompDeferred<BlockPullingDeadActiveComponent>(Entity<BlockPullingDeadComponent>.op_Implicit(ent));
		}
	}

	private void OnPreventPulledWhileAliveAttempt(Entity<PreventPulledWhileAliveComponent> ent, ref PullAttemptEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.PulledUid != ent.Owner) && !CanPullPreventPulledWhileAlive(Entity<PreventPulledWhileAliveComponent>.op_Implicit((Entity<PreventPulledWhileAliveComponent>.op_Implicit(ent), Entity<PreventPulledWhileAliveComponent>.op_Implicit(ent))), args.PullerUid))
		{
			string msg = base.Loc.GetString("rmc-prevent-pull-alive", (ValueTuple<string, object>)("target", ent));
			_popup.PopupClient(msg, Entity<PreventPulledWhileAliveComponent>.op_Implicit(ent), args.PullerUid, PopupType.SmallCaution);
			args.Cancelled = true;
		}
	}

	private void OnMeleePullAttempt(Entity<MeleeWeaponComponent> ent, ref PullAttemptEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.PullerUid != ent.Owner) && ent.Comp.NextAttack > _timing.CurTime)
		{
			args.Cancelled = true;
		}
	}

	private void OnXenoPullToggle(Entity<XenoComponent> ent, ref RMCPullToggleEvent args)
	{
		args.Handled = true;
	}

	private void OnPreventPulledWhileAliveStart(Entity<PreventPulledWhileAliveComponent> ent, ref PullStartedMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.PulledUid != ent.Owner))
		{
			((EntitySystem)this).EnsureComp<ActivePreventPulledWhileAliveComponent>(Entity<PreventPulledWhileAliveComponent>.op_Implicit(ent));
		}
	}

	private void OnPreventPulledWhileAliveStop(Entity<PreventPulledWhileAliveComponent> ent, ref PullStoppedMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.PulledUid != ent.Owner))
		{
			((EntitySystem)this).RemCompDeferred<ActivePreventPulledWhileAliveComponent>(Entity<PreventPulledWhileAliveComponent>.op_Implicit(ent));
		}
	}

	private bool CanPullPreventPulledWhileAlive(Entity<PreventPulledWhileAliveComponent?> pulled, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PreventPulledWhileAliveComponent>(Entity<PreventPulledWhileAliveComponent>.op_Implicit(pulled), ref pulled.Comp, false))
		{
			return true;
		}
		if (!_mobState.IsAlive(Entity<PreventPulledWhileAliveComponent>.op_Implicit(pulled)))
		{
			return true;
		}
		if (!_whitelist.IsWhitelistPassOrNull(pulled.Comp.Whitelist, user))
		{
			return true;
		}
		foreach (ProtoId<StatusEffectPrototype> effect in pulled.Comp.ExceptEffects)
		{
			if (_statusEffects.HasStatusEffect(Entity<PreventPulledWhileAliveComponent>.op_Implicit(pulled), ProtoId<StatusEffectPrototype>.op_Implicit(effect)))
			{
				return true;
			}
		}
		return false;
	}

	public void TryStopUserPullIfPulling(EntityUid user, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		PullerComponent puller = default(PullerComponent);
		if (((EntitySystem)this).TryComp<PullerComponent>(user, ref puller))
		{
			EntityUid? pulling = puller.Pulling;
			PullableComponent pullable = default(PullableComponent);
			if (pulling.HasValue && !(pulling.GetValueOrDefault() != target) && ((EntitySystem)this).TryComp<PullableComponent>(puller.Pulling, ref pullable))
			{
				_pulling.TryStopPull(puller.Pulling.Value, pullable, user);
			}
		}
	}

	public void TryStopPullsOn(EntityUid puller)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		PullableComponent pullable = default(PullableComponent);
		if (((EntitySystem)this).TryComp<PullableComponent>(puller, ref pullable) && pullable.Puller.HasValue)
		{
			_pulling.TryStopPull(puller, pullable);
		}
	}

	public void TryStopAllPullsFromAndOn(EntityUid pullie)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		TryStopPullsOn(pullie);
		PullerComponent puller = default(PullerComponent);
		PullableComponent pullable2 = default(PullableComponent);
		if (((EntitySystem)this).TryComp<PullerComponent>(pullie, ref puller) && puller.Pulling.HasValue && ((EntitySystem)this).TryComp<PullableComponent>(puller.Pulling, ref pullable2))
		{
			_pulling.TryStopPull(puller.Pulling.Value, pullable2, pullie);
		}
	}

	private void OnPullAnimation(Entity<PullableComponent> ent, ref PullStartedMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.PulledUid != ent.Owner))
		{
			if (!_timing.ApplyingState)
			{
				((EntitySystem)this).EnsureComp<BeingPulledComponent>(Entity<PullableComponent>.op_Implicit(ent));
			}
			PlayPullEffect(args.PullerUid, args.PulledUid);
		}
	}

	private void OnBeingPulledPullStopped(Entity<BeingPulledComponent> ent, ref PullStoppedMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.PulledUid != ent.Owner) && !_timing.ApplyingState)
		{
			((EntitySystem)this).RemCompDeferred<BeingPulledComponent>(Entity<BeingPulledComponent>.op_Implicit(ent));
		}
	}

	private void OnPullerPullStopped(Entity<PullerComponent> ent, ref PullStoppedMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.PulledUid == ent.Owner) && !_timing.ApplyingState && !((EntitySystem)this).HasComp<MouseRotatorComponent>(Entity<PullerComponent>.op_Implicit(ent)))
		{
			((EntitySystem)this).RemCompDeferred<NoRotateOnMoveComponent>(Entity<PullerComponent>.op_Implicit(ent));
		}
	}

	public bool IsPulling(Entity<PullerComponent?> user, Entity<PullableComponent?> target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PullerComponent>(Entity<PullerComponent>.op_Implicit(user), ref user.Comp, false) || !((EntitySystem)this).Resolve<PullableComponent>(Entity<PullableComponent>.op_Implicit(target), ref target.Comp, false))
		{
			return false;
		}
		EntityUid? pulling = user.Comp.Pulling;
		EntityUid val = Entity<PullableComponent>.op_Implicit(target);
		if (!pulling.HasValue)
		{
			return false;
		}
		return pulling.GetValueOrDefault() == val;
	}

	public bool IsBeingPulled(Entity<PullableComponent?> target, out EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		user = default(EntityUid);
		if (!((EntitySystem)this).Resolve<PullableComponent>(Entity<PullableComponent>.op_Implicit(target), ref target.Comp, false))
		{
			return false;
		}
		EntityUid? puller = target.Comp.Puller;
		if (puller.HasValue)
		{
			EntityUid puller2 = puller.GetValueOrDefault();
			user = puller2;
		}
		return target.Comp.BeingPulled;
	}

	public void PlayPullEffect(EntityUid puller, EntityUid pulled)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.IsFirstTimePredicted)
		{
			TransformComponent userXform = ((EntitySystem)this).Transform(puller);
			Vector2 localPos = Vector2.Transform(_transform.GetWorldPosition(pulled), _transform.GetInvWorldMatrix(userXform));
			Angle val = userXform.LocalRotation;
			localPos = ((Angle)(ref val)).RotateVec(ref localPos);
			_melee.DoLunge(puller, puller, Angle.Zero, localPos, null);
			_audio.PlayPredicted(_pullSound, pulled, (EntityUid?)puller, (AudioParams?)null);
			EntityCoordinates val2 = pulled.ToCoordinates();
			val = default(Angle);
			((EntitySystem)this).PredictedSpawnAttachedTo("CMEffectGrab", val2, (ComponentRegistry)null, val);
		}
	}

	private bool CanPullDead(EntityUid puller, EntityUid pulled)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (!_mobState.IsDead(pulled))
		{
			return true;
		}
		if (((EntitySystem)this).HasComp<IgnoreBlockPullingDeadComponent>(pulled))
		{
			return true;
		}
		VictimInfectedComponent infect = default(VictimInfectedComponent);
		AllowPullWhileDeadAndInfectedComponent deadPull = default(AllowPullWhileDeadAndInfectedComponent);
		if (((EntitySystem)this).TryComp<VictimInfectedComponent>(pulled, ref infect) && ((EntitySystem)this).TryComp<AllowPullWhileDeadAndInfectedComponent>(pulled, ref deadPull) && infect.CurrentStage > deadPull.InfectionStageThreshold)
		{
			return true;
		}
		return false;
	}

	public EntityUid? TryRetargetPull(EntityUid user, EntityUid target)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		RMCGetPullTargetEvent ev = new RMCGetPullTargetEvent(user, target);
		((EntitySystem)this).RaiseLocalEvent<RMCGetPullTargetEvent>(target, ref ev, false);
		if (target == ev.Target)
		{
			return null;
		}
		return ev.Target;
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<BlockPullingDeadActiveComponent, PullerComponent> blockDeadActive = ((EntitySystem)this).EntityQueryEnumerator<BlockPullingDeadActiveComponent, PullerComponent>();
		EntityUid uid = default(EntityUid);
		BlockPullingDeadActiveComponent blockPullingDeadActiveComponent = default(BlockPullingDeadActiveComponent);
		PullerComponent puller = default(PullerComponent);
		PullableComponent pullable = default(PullableComponent);
		while (blockDeadActive.MoveNext(ref uid, ref blockPullingDeadActiveComponent, ref puller))
		{
			EntityUid? pulling = puller.Pulling;
			if (pulling.HasValue)
			{
				EntityUid pulling2 = pulling.GetValueOrDefault();
				if (((EntitySystem)this).TryComp<PullableComponent>(pulling2, ref pullable) && !CanPullDead(uid, pulling2))
				{
					_pulling.TryStopPull(pulling2, pullable, uid);
				}
			}
		}
		EntityQueryEnumerator<ActivePreventPulledWhileAliveComponent, PreventPulledWhileAliveComponent, PullableComponent> preventPulledWhileAlive = ((EntitySystem)this).EntityQueryEnumerator<ActivePreventPulledWhileAliveComponent, PreventPulledWhileAliveComponent, PullableComponent>();
		EntityUid uid2 = default(EntityUid);
		ActivePreventPulledWhileAliveComponent activePreventPulledWhileAliveComponent = default(ActivePreventPulledWhileAliveComponent);
		PreventPulledWhileAliveComponent prevent = default(PreventPulledWhileAliveComponent);
		PullableComponent pullable2 = default(PullableComponent);
		while (preventPulledWhileAlive.MoveNext(ref uid2, ref activePreventPulledWhileAliveComponent, ref prevent, ref pullable2))
		{
			EntityUid? pulling = pullable2.Puller;
			if (pulling.HasValue)
			{
				EntityUid puller2 = pulling.GetValueOrDefault();
				if (!CanPullPreventPulledWhileAlive(Entity<PreventPulledWhileAliveComponent>.op_Implicit((uid2, prevent)), puller2))
				{
					_pulling.TryStopPull(uid2, pullable2);
				}
			}
		}
		EntityQueryEnumerator<BeingPulledComponent, InputMoverComponent, PullableComponent> pulledQuery = ((EntitySystem)this).EntityQueryEnumerator<BeingPulledComponent, InputMoverComponent, PullableComponent>();
		EntityUid uid3 = default(EntityUid);
		BeingPulledComponent beingPulledComponent = default(BeingPulledComponent);
		InputMoverComponent input = default(InputMoverComponent);
		PullableComponent pullable3 = default(PullableComponent);
		while (pulledQuery.MoveNext(ref uid3, ref beingPulledComponent, ref input, ref pullable3))
		{
			if ((input.HeldMoveButtons & MoveButtons.AnyDirection) != MoveButtons.None && _actionBlocker.CanMove(uid3))
			{
				_pulling.TryStopPull(uid3, pullable3);
			}
		}
		EntityQueryEnumerator<BeingPulledComponent, PullableComponent> pullableQuery = ((EntitySystem)this).EntityQueryEnumerator<BeingPulledComponent, PullableComponent>();
		EntityUid uid4 = default(EntityUid);
		PullableComponent pullable4 = default(PullableComponent);
		FiremanCarriableComponent fireman = default(FiremanCarriableComponent);
		while (pullableQuery.MoveNext(ref uid4, ref beingPulledComponent, ref pullable4))
		{
			if (!pullable4.Puller.HasValue)
			{
				continue;
			}
			EntityUid puller3 = pullable4.Puller.Value;
			if (((EntitySystem)this).Exists(puller3) && (!_firemanQuery.TryComp(uid4, ref fireman) || !fireman.BeingCarried) && !((EntitySystem)this).HasComp<MouseRotatorComponent>(puller3))
			{
				if (!_timing.ApplyingState)
				{
					((EntitySystem)this).EnsureComp<NoRotateOnMoveComponent>(puller3);
				}
				Vector2 position = _transform.GetMapCoordinates(uid4, (TransformComponent)null).Position;
				Vector2 pullerCoords = _transform.GetMapCoordinates(puller3, (TransformComponent)null).Position;
				Angle val = DirectionExtensions.ToWorldAngle(position - pullerCoords);
				Angle angle = DirectionExtensions.ToAngle(((Angle)(ref val)).GetCardinalDir());
				_rotateTo.TryFaceAngle(puller3, angle);
			}
		}
	}
}
