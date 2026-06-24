// Decompiled with JetBrains decompiler
// Type: Content.Shared.Doors.Systems.SharedTurnstileSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Systems;
using Content.Shared.Doors.Components;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.Doors.Systems;

public abstract class SharedTurnstileSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private AccessReaderSystem _accessReader;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private PullingSystem _pulling;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<TurnstileComponent, PreventCollideEvent>(new EntityEventRefHandler<TurnstileComponent, PreventCollideEvent>(this.OnPreventCollide));
    this.SubscribeLocalEvent<TurnstileComponent, StartCollideEvent>(new EntityEventRefHandler<TurnstileComponent, StartCollideEvent>(this.OnStartCollide));
    this.SubscribeLocalEvent<TurnstileComponent, EndCollideEvent>(new EntityEventRefHandler<TurnstileComponent, EndCollideEvent>(this.OnEndCollide));
  }

  private void OnPreventCollide(Entity<TurnstileComponent> ent, ref PreventCollideEvent args)
  {
    if (args.Cancelled || !args.OurFixture.Hard || !args.OtherFixture.Hard)
      return;
    if (ent.Comp.CollideExceptions.Contains(args.OtherEntity))
    {
      args.Cancelled = true;
    }
    else
    {
      EntityUid? nullable = this._pulling.GetPuller(args.OtherEntity);
      if (nullable.HasValue)
      {
        EntityUid valueOrDefault = nullable.GetValueOrDefault();
        if (ent.Comp.CollideExceptions.Contains(valueOrDefault))
        {
          ent.Comp.CollideExceptions.Add(args.OtherEntity);
          this.Dirty<TurnstileComponent>(ent);
          args.Cancelled = true;
          return;
        }
      }
      if (this._entityWhitelist.IsWhitelistFail(ent.Comp.ProcessWhitelist, args.OtherEntity))
        args.Cancelled = true;
      else if (this.CanPassDirection(ent, args.OtherEntity))
      {
        if (!this._accessReader.IsAllowed(args.OtherEntity, (EntityUid) ent))
          return;
        ent.Comp.CollideExceptions.Add(args.OtherEntity);
        nullable = this._pulling.GetPulling(args.OtherEntity);
        if (nullable.HasValue)
        {
          EntityUid valueOrDefault = nullable.GetValueOrDefault();
          ent.Comp.CollideExceptions.Add(valueOrDefault);
        }
        args.Cancelled = true;
        this.Dirty<TurnstileComponent>(ent);
      }
      else
      {
        if (!(this._timing.CurTime >= ent.Comp.NextResistTime))
          return;
        this._popup.PopupClient(this.Loc.GetString("turnstile-component-popup-resist", ("turnstile", (object) ent.Owner)), (EntityUid) ent, new EntityUid?(args.OtherEntity));
        ent.Comp.NextResistTime = this._timing.CurTime + TimeSpan.FromSeconds(0.1);
        this.Dirty<TurnstileComponent>(ent);
      }
    }
  }

  private void OnStartCollide(Entity<TurnstileComponent> ent, ref StartCollideEvent args)
  {
    if (!ent.Comp.CollideExceptions.Contains(args.OtherEntity))
    {
      if (!this.CanPassDirection(ent, args.OtherEntity) || this._accessReader.IsAllowed(args.OtherEntity, (EntityUid) ent))
        return;
      this._audio.PlayPredicted(ent.Comp.DenySound, (EntityUid) ent, new EntityUid?(args.OtherEntity));
      this.PlayAnimation((EntityUid) ent, ent.Comp.DenyState);
    }
    else
    {
      this.PlayAnimation((EntityUid) ent, ent.Comp.SpinState);
      this._audio.PlayPredicted(ent.Comp.TurnSound, (EntityUid) ent, new EntityUid?(args.OtherEntity));
    }
  }

  private void OnEndCollide(Entity<TurnstileComponent> ent, ref EndCollideEvent args)
  {
    if (args.OurFixture.Hard)
      return;
    ent.Comp.CollideExceptions.Remove(args.OtherEntity);
    this.Dirty<TurnstileComponent>(ent);
  }

  protected bool CanPassDirection(Entity<TurnstileComponent> ent, EntityUid other)
  {
    TransformComponent component1 = this.Transform((EntityUid) ent);
    TransformComponent component2 = this.Transform(other);
    (Vector2 WorldPosition, Angle WorldRotation) = this._transform.GetWorldPositionRotation(component1);
    Vector2 worldPosition = this._transform.GetWorldPosition(component2);
    double num = Math.Abs(Angle.op_Implicit(Angle.op_Subtraction(DirectionExtensions.ToAngle(WorldPosition - worldPosition), DirectionExtensions.ToAngle(((Angle) ref WorldRotation).ToWorldVec())))) % 6.2831854820251465;
    if (num > Math.PI)
      num = 6.2831854820251465 - num;
    return num < Math.PI / 4.0;
  }

  protected virtual void PlayAnimation(EntityUid uid, string stateId)
  {
  }
}
