// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ghost.SharedGhostSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Ghost;
using Content.Shared.Emoting;
using Content.Shared.Hands;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Ghost;

public abstract class SharedGhostSystem : EntitySystem
{
  [Dependency]
  protected SharedPopupSystem Popup;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<GhostComponent, UseAttemptEvent>(new ComponentEventHandler<GhostComponent, UseAttemptEvent>(this.OnAttempt));
    this.SubscribeLocalEvent<GhostComponent, InteractionAttemptEvent>(new EntityEventRefHandler<GhostComponent, InteractionAttemptEvent>(this.OnAttemptInteract));
    this.SubscribeLocalEvent<GhostComponent, EmoteAttemptEvent>(new ComponentEventHandler<GhostComponent, EmoteAttemptEvent>(this.OnAttempt));
    this.SubscribeLocalEvent<GhostComponent, DropAttemptEvent>(new ComponentEventHandler<GhostComponent, DropAttemptEvent>(this.OnAttempt));
    this.SubscribeLocalEvent<GhostComponent, PickupAttemptEvent>(new ComponentEventHandler<GhostComponent, PickupAttemptEvent>(this.OnAttempt));
  }

  private void OnAttemptInteract(Entity<GhostComponent> ent, ref InteractionAttemptEvent args)
  {
    if (ent.Comp.CanGhostInteract || this.HasComp<RMCIgnoreGhostInteractionLimitsComponent>(args.Target))
      return;
    args.Cancelled = true;
  }

  private void OnAttempt(EntityUid uid, GhostComponent component, CancellableEntityEventArgs args)
  {
    if (component.CanGhostInteract)
      return;
    args.Cancel();
  }

  public void SetTimeOfDeath(Entity<GhostComponent?> entity, TimeSpan value)
  {
    if (!this.Resolve<GhostComponent>((EntityUid) entity, ref entity.Comp) || entity.Comp.TimeOfDeath == value)
      return;
    entity.Comp.TimeOfDeath = value;
    this.Dirty<GhostComponent>(entity);
  }

  [Obsolete("Use the Entity<GhostComponent?> overload")]
  public void SetTimeOfDeath(EntityUid uid, TimeSpan value, GhostComponent? component)
  {
    this.SetTimeOfDeath((Entity<GhostComponent>) (uid, component), value);
  }

  public void SetCanReturnToBody(Entity<GhostComponent?> entity, bool value)
  {
    if (!this.Resolve<GhostComponent>((EntityUid) entity, ref entity.Comp) || entity.Comp.CanReturnToBody == value)
      return;
    entity.Comp.CanReturnToBody = value;
    this.Dirty<GhostComponent>(entity);
  }

  [Obsolete("Use the Entity<GhostComponent?> overload")]
  public void SetCanReturnToBody(EntityUid uid, bool value, GhostComponent? component = null)
  {
    this.SetCanReturnToBody((Entity<GhostComponent>) (uid, component), value);
  }

  [Obsolete("Use the Entity<GhostComponent?> overload")]
  public void SetCanReturnToBody(GhostComponent component, bool value)
  {
    this.SetCanReturnToBody((Entity<GhostComponent>) (component.Owner, component), value);
  }

  public void SetCanGhostInteract(Entity<GhostComponent?> entity, bool value)
  {
    if (!this.Resolve<GhostComponent>((EntityUid) entity, ref entity.Comp) || entity.Comp.CanGhostInteract == value)
      return;
    entity.Comp.CanGhostInteract = value;
    this.Dirty<GhostComponent>(entity);
  }
}
