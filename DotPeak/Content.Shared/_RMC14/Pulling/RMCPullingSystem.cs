// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Pulling.RMCPullingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System.Numerics;

#nullable enable
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
  private readonly SoundSpecifier _pullSound;
  private const string PullEffect = "CMEffectGrab";
  private Robust.Shared.GameObjects.EntityQuery<FiremanCarriableComponent> _firemanQuery;

  public override void Initialize()
  {
    this._firemanQuery = this.GetEntityQuery<FiremanCarriableComponent>();
    this.SubscribeLocalEvent<BuckleComponent, RMCGetPullTargetEvent>(new EntityEventRefHandler<BuckleComponent, RMCGetPullTargetEvent>(this.OnGetPullTarget));
    this.SubscribeLocalEvent<XenoComponent, RMCPullToggleEvent>(new EntityEventRefHandler<XenoComponent, RMCPullToggleEvent>(this.OnXenoPullToggle));
    this.SubscribeLocalEvent<ParalyzeOnPullAttemptComponent, PullAttemptEvent>(new EntityEventRefHandler<ParalyzeOnPullAttemptComponent, PullAttemptEvent>(this.OnParalyzeOnPullAttempt));
    this.SubscribeLocalEvent<InfectOnPullAttemptComponent, PullAttemptEvent>(new EntityEventRefHandler<InfectOnPullAttemptComponent, PullAttemptEvent>(this.OnInfectOnPullAttempt));
    this.SubscribeLocalEvent<MeleeWeaponComponent, PullAttemptEvent>(new EntityEventRefHandler<MeleeWeaponComponent, PullAttemptEvent>(this.OnMeleePullAttempt));
    this.SubscribeLocalEvent<SlowOnPullComponent, PullStartedMessage>(new EntityEventRefHandler<SlowOnPullComponent, PullStartedMessage>(this.OnSlowPullStarted));
    this.SubscribeLocalEvent<SlowOnPullComponent, PullStoppedMessage>(new EntityEventRefHandler<SlowOnPullComponent, PullStoppedMessage>(this.OnSlowPullStopped));
    this.SubscribeLocalEvent<PullingSlowedComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<PullingSlowedComponent, RefreshMovementSpeedModifiersEvent>(this.OnPullingSlowedMovementSpeed));
    this.SubscribeLocalEvent<PullWhitelistComponent, PullAttemptEvent>(new EntityEventRefHandler<PullWhitelistComponent, PullAttemptEvent>(this.OnPullWhitelistPullAttempt));
    this.SubscribeLocalEvent<BlockPullingDeadComponent, PullAttemptEvent>(new EntityEventRefHandler<BlockPullingDeadComponent, PullAttemptEvent>(this.OnBlockDeadPullAttempt));
    this.SubscribeLocalEvent<BlockPullingDeadComponent, PullStartedMessage>(new EntityEventRefHandler<BlockPullingDeadComponent, PullStartedMessage>(this.OnBlockDeadPullStarted));
    this.SubscribeLocalEvent<BlockPullingDeadComponent, PullStoppedMessage>(new EntityEventRefHandler<BlockPullingDeadComponent, PullStoppedMessage>(this.OnBlockDeadPullStopped));
    this.SubscribeLocalEvent<PreventPulledWhileAliveComponent, PullAttemptEvent>(new EntityEventRefHandler<PreventPulledWhileAliveComponent, PullAttemptEvent>(this.OnPreventPulledWhileAliveAttempt));
    this.SubscribeLocalEvent<PreventPulledWhileAliveComponent, PullStartedMessage>(new EntityEventRefHandler<PreventPulledWhileAliveComponent, PullStartedMessage>(this.OnPreventPulledWhileAliveStart));
    this.SubscribeLocalEvent<PreventPulledWhileAliveComponent, PullStoppedMessage>(new EntityEventRefHandler<PreventPulledWhileAliveComponent, PullStoppedMessage>(this.OnPreventPulledWhileAliveStop));
    this.SubscribeLocalEvent<PullableComponent, PullStartedMessage>(new EntityEventRefHandler<PullableComponent, PullStartedMessage>(this.OnPullAnimation));
    this.SubscribeLocalEvent<PullerComponent, PullStoppedMessage>(new EntityEventRefHandler<PullerComponent, PullStoppedMessage>(this.OnPullerPullStopped));
    this.SubscribeLocalEvent<BeingPulledComponent, PullStoppedMessage>(new EntityEventRefHandler<BeingPulledComponent, PullStoppedMessage>(this.OnBeingPulledPullStopped));
  }

  private void OnGetPullTarget(Entity<BuckleComponent> ent, ref RMCGetPullTargetEvent ev)
  {
    if (ent.Owner != ev.Target || this.HasComp<XenoComponent>(ev.User) || !this.HasComp<RMCRetargetBucklePullComponent>(ent.Comp.BuckledTo))
      return;
    ev.Target = ent.Comp.BuckledTo.Value;
  }

  private void OnParalyzeOnPullAttempt(
    Entity<ParalyzeOnPullAttemptComponent> ent,
    ref PullAttemptEvent args)
  {
    EntityUid pullerUid = args.PullerUid;
    EntityUid pulledUid = args.PulledUid;
    if (pulledUid != ent.Owner || this.HasComp<ParalyzeOnPullAttemptImmuneComponent>(pullerUid) || this._mobState.IsDead((EntityUid) ent))
      return;
    args.Cancelled = true;
    SoundSpecifier sound = ent.Comp.Sound;
    if (sound != null)
    {
      float pitch = this._random.NextFloat(ent.Comp.MinPitch, ent.Comp.MaxPitch);
      this._audio.PlayPredicted(sound, (EntityUid) ent, new EntityUid?(pullerUid), new AudioParams?(sound.Params.WithPitchScale(pitch)));
    }
    this._stun.TryParalyze(pullerUid, ent.Comp.Duration, true);
    EntityUid uid = pullerUid;
    EntityUid entityUid = pulledUid;
    string othersMessage = this.Loc.GetString("rmc-pull-paralyze-others", ("puller", (object) uid), ("pulled", (object) entityUid));
    this._popup.PopupPredicted(this.Loc.GetString("rmc-pull-paralyze-self", ("puller", (object) uid), ("pulled", (object) entityUid)), othersMessage, uid, new EntityUid?(uid), PopupType.MediumCaution);
  }

  private void OnInfectOnPullAttempt(
    Entity<InfectOnPullAttemptComponent> ent,
    ref PullAttemptEvent args)
  {
    EntityUid pullerUid = args.PullerUid;
    EntityUid pulledUid = args.PulledUid;
    XenoParasiteComponent comp;
    if (pulledUid != ent.Owner || this.HasComp<InfectOnPullAttemptImmuneComponent>(pullerUid) || this._mobState.IsDead((EntityUid) ent) || !this.TryComp<XenoParasiteComponent>(pulledUid, out comp))
      return;
    Entity<XenoParasiteComponent> parasite = (Entity<XenoParasiteComponent>) (pulledUid, comp);
    args.Cancelled = true;
    if (!this._parasite.Infect(parasite, pullerUid, false, true))
      return;
    EntityUid uid = pullerUid;
    EntityUid entityUid = pulledUid;
    string othersMessage = this.Loc.GetString("rmc-pull-infect-others", ("puller", (object) uid), ("pulled", (object) entityUid));
    this._popup.PopupPredicted(this.Loc.GetString("rmc-pull-infect-self", ("puller", (object) uid), ("pulled", (object) entityUid)), othersMessage, uid, new EntityUid?(uid), PopupType.MediumCaution);
  }

  private void OnSlowPullStarted(Entity<SlowOnPullComponent> ent, ref PullStartedMessage args)
  {
    if (!(ent.Owner == args.PullerUid))
      return;
    this.EnsureComp<PullingSlowedComponent>(args.PullerUid);
    this._movementSpeed.RefreshMovementSpeedModifiers(args.PullerUid);
  }

  private void OnSlowPullStopped(Entity<SlowOnPullComponent> ent, ref PullStoppedMessage args)
  {
    if (!(ent.Owner == args.PullerUid))
      return;
    this.RemComp<PullingSlowedComponent>(args.PullerUid);
    this._movementSpeed.RefreshMovementSpeedModifiers(args.PullerUid);
  }

  private void OnPullingSlowedMovementSpeed(
    Entity<PullingSlowedComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    PullerComponent comp1;
    SlowOnPullComponent comp2;
    if (this.HasComp<BypassInteractionChecksComponent>((EntityUid) ent) || !this.TryComp<PullerComponent>((EntityUid) ent, out comp1) || !this.TryComp<SlowOnPullComponent>((EntityUid) ent, out comp2) || !comp1.Pulling.HasValue)
      return;
    PullSlowdownAttemptEvent args1 = new PullSlowdownAttemptEvent(comp1.Pulling.Value);
    this.RaiseLocalEvent<PullSlowdownAttemptEvent>((EntityUid) ent, ref args1);
    if (args1.Cancelled)
      return;
    foreach (SlowOnPullComponent.SlowdownWhitelist slowdown in comp2.Slowdowns)
    {
      if (this._whitelist.IsWhitelistPass(slowdown.Whitelist, comp1.Pulling.Value))
      {
        args.ModifySpeed(slowdown.Multiplier, slowdown.Multiplier);
        return;
      }
    }
    args.ModifySpeed(comp2.Multiplier, comp2.Multiplier);
  }

  private void OnPullWhitelistPullAttempt(
    Entity<PullWhitelistComponent> ent,
    ref PullAttemptEvent args)
  {
    if (args.Cancelled || ent.Owner == args.PulledUid || this._whitelist.IsValid(ent.Comp.Whitelist, args.PulledUid))
      return;
    this._popup.PopupClient(this.Loc.GetString("cm-pull-whitelist-denied", ("name", (object) args.PulledUid)), args.PulledUid, new EntityUid?(args.PullerUid));
    args.Cancelled = true;
  }

  private void OnBlockDeadPullAttempt(
    Entity<BlockPullingDeadComponent> ent,
    ref PullAttemptEvent args)
  {
    if (args.Cancelled || ent.Owner == args.PulledUid || this.CanPullDead((EntityUid) ent, args.PulledUid))
      return;
    this._popup.PopupClient(this.Loc.GetString("cm-pull-whitelist-denied-dead", ("name", (object) args.PulledUid)), args.PulledUid, new EntityUid?(args.PullerUid));
    args.Cancelled = true;
  }

  private void OnBlockDeadPullStarted(
    Entity<BlockPullingDeadComponent> ent,
    ref PullStartedMessage args)
  {
    if (!(ent.Owner == args.PullerUid))
      return;
    this.EnsureComp<BlockPullingDeadActiveComponent>((EntityUid) ent);
  }

  private void OnBlockDeadPullStopped(
    Entity<BlockPullingDeadComponent> ent,
    ref PullStoppedMessage args)
  {
    if (!(ent.Owner == args.PullerUid))
      return;
    this.RemCompDeferred<BlockPullingDeadActiveComponent>((EntityUid) ent);
  }

  private void OnPreventPulledWhileAliveAttempt(
    Entity<PreventPulledWhileAliveComponent> ent,
    ref PullAttemptEvent args)
  {
    if (args.PulledUid != ent.Owner || this.CanPullPreventPulledWhileAlive((Entity<PreventPulledWhileAliveComponent>) ((EntityUid) ent, (PreventPulledWhileAliveComponent) ent), args.PullerUid))
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-prevent-pull-alive", ("target", (object) ent)), (EntityUid) ent, new EntityUid?(args.PullerUid), PopupType.SmallCaution);
    args.Cancelled = true;
  }

  private void OnMeleePullAttempt(Entity<MeleeWeaponComponent> ent, ref PullAttemptEvent args)
  {
    if (args.PullerUid != ent.Owner || !(ent.Comp.NextAttack > this._timing.CurTime))
      return;
    args.Cancelled = true;
  }

  private void OnXenoPullToggle(Entity<XenoComponent> ent, ref RMCPullToggleEvent args)
  {
    args.Handled = true;
  }

  private void OnPreventPulledWhileAliveStart(
    Entity<PreventPulledWhileAliveComponent> ent,
    ref PullStartedMessage args)
  {
    if (args.PulledUid != ent.Owner)
      return;
    this.EnsureComp<ActivePreventPulledWhileAliveComponent>((EntityUid) ent);
  }

  private void OnPreventPulledWhileAliveStop(
    Entity<PreventPulledWhileAliveComponent> ent,
    ref PullStoppedMessage args)
  {
    if (args.PulledUid != ent.Owner)
      return;
    this.RemCompDeferred<ActivePreventPulledWhileAliveComponent>((EntityUid) ent);
  }

  private bool CanPullPreventPulledWhileAlive(
    Entity<PreventPulledWhileAliveComponent?> pulled,
    EntityUid user)
  {
    if (!this.Resolve<PreventPulledWhileAliveComponent>((EntityUid) pulled, ref pulled.Comp, false) || !this._mobState.IsAlive((EntityUid) pulled) || !this._whitelist.IsWhitelistPassOrNull(pulled.Comp.Whitelist, user))
      return true;
    foreach (ProtoId<StatusEffectPrototype> exceptEffect in pulled.Comp.ExceptEffects)
    {
      if (this._statusEffects.HasStatusEffect((EntityUid) pulled, (string) exceptEffect))
        return true;
    }
    return false;
  }

  public void TryStopUserPullIfPulling(EntityUid user, EntityUid target)
  {
    PullerComponent comp1;
    if (!this.TryComp<PullerComponent>(user, out comp1))
      return;
    EntityUid? pulling = comp1.Pulling;
    EntityUid entityUid = target;
    PullableComponent comp2;
    if ((pulling.HasValue ? (pulling.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0 || !this.TryComp<PullableComponent>(comp1.Pulling, out comp2))
      return;
    this._pulling.TryStopPull(comp1.Pulling.Value, comp2, new EntityUid?(user));
  }

  public void TryStopPullsOn(EntityUid puller)
  {
    PullableComponent comp;
    if (!this.TryComp<PullableComponent>(puller, out comp) || !comp.Puller.HasValue)
      return;
    this._pulling.TryStopPull(puller, comp);
  }

  public void TryStopAllPullsFromAndOn(EntityUid pullie)
  {
    this.TryStopPullsOn(pullie);
    PullerComponent comp1;
    PullableComponent comp2;
    if (!this.TryComp<PullerComponent>(pullie, out comp1) || !comp1.Pulling.HasValue || !this.TryComp<PullableComponent>(comp1.Pulling, out comp2))
      return;
    this._pulling.TryStopPull(comp1.Pulling.Value, comp2, new EntityUid?(pullie));
  }

  private void OnPullAnimation(Entity<PullableComponent> ent, ref PullStartedMessage args)
  {
    if (args.PulledUid != ent.Owner)
      return;
    if (!this._timing.ApplyingState)
      this.EnsureComp<BeingPulledComponent>((EntityUid) ent);
    this.PlayPullEffect(args.PullerUid, args.PulledUid);
  }

  private void OnBeingPulledPullStopped(
    Entity<BeingPulledComponent> ent,
    ref PullStoppedMessage args)
  {
    if (args.PulledUid != ent.Owner || this._timing.ApplyingState)
      return;
    this.RemCompDeferred<BeingPulledComponent>((EntityUid) ent);
  }

  private void OnPullerPullStopped(Entity<PullerComponent> ent, ref PullStoppedMessage args)
  {
    if (args.PulledUid == ent.Owner || this._timing.ApplyingState || this.HasComp<MouseRotatorComponent>((EntityUid) ent))
      return;
    this.RemCompDeferred<NoRotateOnMoveComponent>((EntityUid) ent);
  }

  public bool IsPulling(Entity<PullerComponent?> user, Entity<PullableComponent?> target)
  {
    if (!this.Resolve<PullerComponent>((EntityUid) user, ref user.Comp, false) || !this.Resolve<PullableComponent>((EntityUid) target, ref target.Comp, false))
      return false;
    EntityUid? pulling = user.Comp.Pulling;
    EntityUid entityUid = (EntityUid) target;
    return pulling.HasValue && pulling.GetValueOrDefault() == entityUid;
  }

  public bool IsBeingPulled(Entity<PullableComponent?> target, out EntityUid user)
  {
    user = new EntityUid();
    if (!this.Resolve<PullableComponent>((EntityUid) target, ref target.Comp, false))
      return false;
    EntityUid? puller = target.Comp.Puller;
    if (puller.HasValue)
    {
      EntityUid valueOrDefault = puller.GetValueOrDefault();
      user = valueOrDefault;
    }
    return target.Comp.BeingPulled;
  }

  public void PlayPullEffect(EntityUid puller, EntityUid pulled)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    TransformComponent component = this.Transform(puller);
    Vector2 localPos = Vector2.Transform(this._transform.GetWorldPosition(pulled), this._transform.GetInvWorldMatrix(component));
    Angle localRotation = component.LocalRotation;
    localPos = ((Angle) ref localRotation).RotateVec(ref localPos);
    this._melee.DoLunge(puller, puller, Angle.Zero, localPos, (string) null);
    this._audio.PlayPredicted(this._pullSound, pulled, new EntityUid?(puller));
    this.PredictedSpawnAttachedTo("CMEffectGrab", pulled.ToCoordinates(), rotation: new Angle());
  }

  private bool CanPullDead(EntityUid puller, EntityUid pulled)
  {
    VictimInfectedComponent comp1;
    AllowPullWhileDeadAndInfectedComponent comp2;
    return !this._mobState.IsDead(pulled) || this.HasComp<IgnoreBlockPullingDeadComponent>(pulled) || this.TryComp<VictimInfectedComponent>(pulled, out comp1) && this.TryComp<AllowPullWhileDeadAndInfectedComponent>(pulled, out comp2) && comp1.CurrentStage > comp2.InfectionStageThreshold;
  }

  public EntityUid? TryRetargetPull(EntityUid user, EntityUid target)
  {
    RMCGetPullTargetEvent args = new RMCGetPullTargetEvent(user, target);
    this.RaiseLocalEvent<RMCGetPullTargetEvent>(target, ref args);
    return target == args.Target ? new EntityUid?() : new EntityUid?(args.Target);
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<BlockPullingDeadActiveComponent, PullerComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<BlockPullingDeadActiveComponent, PullerComponent>();
    EntityUid uid1;
    PullerComponent comp2_1;
    EntityUid? nullable;
    while (entityQueryEnumerator1.MoveNext(out uid1, out BlockPullingDeadActiveComponent _, out comp2_1))
    {
      nullable = comp2_1.Pulling;
      if (nullable.HasValue)
      {
        EntityUid valueOrDefault = nullable.GetValueOrDefault();
        PullableComponent comp;
        if (this.TryComp<PullableComponent>(valueOrDefault, out comp) && !this.CanPullDead(uid1, valueOrDefault))
          this._pulling.TryStopPull(valueOrDefault, comp, new EntityUid?(uid1));
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActivePreventPulledWhileAliveComponent, PreventPulledWhileAliveComponent, PullableComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<ActivePreventPulledWhileAliveComponent, PreventPulledWhileAliveComponent, PullableComponent>();
    EntityUid uid2;
    PreventPulledWhileAliveComponent comp2_2;
    PullableComponent comp3_1;
    while (entityQueryEnumerator2.MoveNext(out uid2, out ActivePreventPulledWhileAliveComponent _, out comp2_2, out comp3_1))
    {
      nullable = comp3_1.Puller;
      if (nullable.HasValue)
      {
        EntityUid valueOrDefault = nullable.GetValueOrDefault();
        if (!this.CanPullPreventPulledWhileAlive((Entity<PreventPulledWhileAliveComponent>) (uid2, comp2_2), valueOrDefault))
        {
          PullingSystem pulling = this._pulling;
          EntityUid pullableUid = uid2;
          PullableComponent pullable = comp3_1;
          nullable = new EntityUid?();
          EntityUid? user = nullable;
          pulling.TryStopPull(pullableUid, pullable, user);
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<BeingPulledComponent, InputMoverComponent, PullableComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<BeingPulledComponent, InputMoverComponent, PullableComponent>();
    EntityUid uid3;
    BeingPulledComponent comp1;
    InputMoverComponent comp2_3;
    PullableComponent comp3_2;
    while (entityQueryEnumerator3.MoveNext(out uid3, out comp1, out comp2_3, out comp3_2))
    {
      if ((comp2_3.HeldMoveButtons & MoveButtons.AnyDirection) != MoveButtons.None && this._actionBlocker.CanMove(uid3))
      {
        PullingSystem pulling = this._pulling;
        EntityUid pullableUid = uid3;
        PullableComponent pullable = comp3_2;
        nullable = new EntityUid?();
        EntityUid? user = nullable;
        pulling.TryStopPull(pullableUid, pullable, user);
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<BeingPulledComponent, PullableComponent> entityQueryEnumerator4 = this.EntityQueryEnumerator<BeingPulledComponent, PullableComponent>();
    EntityUid uid4;
    PullableComponent comp2_4;
    while (entityQueryEnumerator4.MoveNext(out uid4, out comp1, out comp2_4))
    {
      if (comp2_4.Puller.HasValue)
      {
        EntityUid entityUid = comp2_4.Puller.Value;
        FiremanCarriableComponent component;
        if (this.Exists(entityUid) && (!this._firemanQuery.TryComp(uid4, out component) || !component.BeingCarried) && !this.HasComp<MouseRotatorComponent>(entityUid))
        {
          if (!this._timing.ApplyingState)
            this.EnsureComp<NoRotateOnMoveComponent>(entityUid);
          Angle worldAngle = DirectionExtensions.ToWorldAngle(this._transform.GetMapCoordinates(uid4).Position - this._transform.GetMapCoordinates(entityUid).Position);
          Angle angle = DirectionExtensions.ToAngle(((Angle) ref worldAngle).GetCardinalDir());
          this._rotateTo.TryFaceAngle(entityUid, angle);
        }
      }
    }
  }

  public RMCPullingSystem()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Effects/thudswoosh.ogg");
    soundPathSpecifier.Params = AudioParams.Default.WithVariation(new float?(0.05f));
    this._pullSound = (SoundSpecifier) soundPathSpecifier;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }
}
