// Decompiled with JetBrains decompiler
// Type: Content.Shared.Standing.StandingStateSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Hands.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Rotation;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using System;

#nullable enable
namespace Content.Shared.Standing;

public sealed class StandingStateSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPhysicsSystem _physics;
  private const int StandingCollisionLayer = 4;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<StandingStateComponent, AttemptMobCollideEvent>(new EntityEventRefHandler<StandingStateComponent, AttemptMobCollideEvent>(this.OnMobCollide));
    this.SubscribeLocalEvent<StandingStateComponent, AttemptMobTargetCollideEvent>(new EntityEventRefHandler<StandingStateComponent, AttemptMobTargetCollideEvent>(this.OnMobTargetCollide));
  }

  private void OnMobTargetCollide(
    Entity<StandingStateComponent> ent,
    ref AttemptMobTargetCollideEvent args)
  {
    if (ent.Comp.Standing)
      return;
    args.Cancelled = true;
  }

  private void OnMobCollide(Entity<StandingStateComponent> ent, ref AttemptMobCollideEvent args)
  {
    if (ent.Comp.Standing)
      return;
    args.Cancelled = true;
  }

  public bool IsDown(EntityUid uid, StandingStateComponent? standingState = null)
  {
    return this.Resolve<StandingStateComponent>(uid, ref standingState, false) && !standingState.Standing;
  }

  public bool Down(
    EntityUid uid,
    bool playSound = true,
    bool dropHeldItems = true,
    bool force = false,
    bool changeCollision = false,
    StandingStateComponent? standingState = null,
    AppearanceComponent? appearance = null,
    HandsComponent? hands = null)
  {
    if (!this.Resolve<StandingStateComponent>(uid, ref standingState, false))
      return false;
    this.Resolve<AppearanceComponent, HandsComponent>(uid, ref appearance, ref hands, false);
    if (!standingState.Standing)
      return true;
    if (dropHeldItems && hands != null)
    {
      DropHandItemsEvent args = new DropHandItemsEvent();
      this.RaiseLocalEvent<DropHandItemsEvent>(uid, ref args);
    }
    if (!force)
    {
      DownAttemptEvent args = new DownAttemptEvent();
      this.RaiseLocalEvent<DownAttemptEvent>(uid, args);
      if (args.Cancelled)
        return false;
    }
    standingState.Standing = false;
    this.Dirty(uid, (IComponent) standingState);
    this.RaiseLocalEvent<DownedEvent>(uid, new DownedEvent());
    this._appearance.SetData(uid, (Enum) RotationVisuals.RotationState, (object) RotationState.Horizontal, appearance);
    FixturesComponent comp;
    if (changeCollision && this.TryComp<FixturesComponent>(uid, out comp))
    {
      foreach ((string str, Fixture fixture) in comp.Fixtures)
      {
        if ((fixture.CollisionMask & 4) != 0)
        {
          standingState.ChangedFixtures.Add(str);
          this._physics.SetCollisionMask(uid, str, fixture, fixture.CollisionMask & -5, comp);
        }
      }
    }
    if (standingState.LifeStage <= ComponentLifeStage.Starting || !playSound)
      return true;
    this._audio.PlayPredicted(standingState.DownSound, uid, new EntityUid?(uid));
    return true;
  }

  public bool Stand(
    EntityUid uid,
    StandingStateComponent? standingState = null,
    AppearanceComponent? appearance = null,
    bool force = false)
  {
    if (!this.Resolve<StandingStateComponent>(uid, ref standingState, false))
      return false;
    this.Resolve<AppearanceComponent>(uid, ref appearance, false);
    if (standingState.Standing)
      return true;
    if (!force)
    {
      StandAttemptEvent args = new StandAttemptEvent();
      this.RaiseLocalEvent<StandAttemptEvent>(uid, args);
      if (args.Cancelled)
        return false;
    }
    standingState.Standing = true;
    this.Dirty(uid, (IComponent) standingState);
    this.RaiseLocalEvent<StoodEvent>(uid, new StoodEvent());
    this._appearance.SetData(uid, (Enum) RotationVisuals.RotationState, (object) RotationState.Vertical, appearance);
    FixturesComponent comp;
    if (this.TryComp<FixturesComponent>(uid, out comp))
    {
      foreach (string changedFixture in standingState.ChangedFixtures)
      {
        Fixture fixture;
        if (comp.Fixtures.TryGetValue(changedFixture, out fixture))
          this._physics.SetCollisionMask(uid, changedFixture, fixture, fixture.CollisionMask | 4, comp);
      }
    }
    standingState.ChangedFixtures.Clear();
    return true;
  }
}
