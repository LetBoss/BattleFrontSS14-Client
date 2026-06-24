// Decompiled with JetBrains decompiler
// Type: Content.Shared.Teleportation.Systems.SharedTeleportLocationsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Teleportation.Components;
using Content.Shared.Timing;
using Content.Shared.UserInterface;
using Content.Shared.Warps;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared.Teleportation.Systems;

public abstract class SharedTeleportLocationsSystem : EntitySystem
{
  [Dependency]
  protected UseDelaySystem Delay;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private SharedTransformSystem _xform;
  protected const string TeleportDelay = "TeleportDelay";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<TeleportLocationsComponent, ActivatableUIOpenAttemptEvent>(new EntityEventRefHandler<TeleportLocationsComponent, ActivatableUIOpenAttemptEvent>(this.OnUiOpenAttempt));
    this.SubscribeLocalEvent<TeleportLocationsComponent, TeleportLocationDestinationMessage>(new EntityEventRefHandler<TeleportLocationsComponent, TeleportLocationDestinationMessage>(this.OnTeleportToLocationRequest));
  }

  private void OnUiOpenAttempt(
    Entity<TeleportLocationsComponent> ent,
    ref ActivatableUIOpenAttemptEvent args)
  {
    if (!this.Delay.IsDelayed((Entity<UseDelayComponent>) ent.Owner, "TeleportDelay"))
      return;
    args.Cancel();
  }

  protected virtual void OnTeleportToLocationRequest(
    Entity<TeleportLocationsComponent> ent,
    ref TeleportLocationDestinationMessage args)
  {
    EntityUid? entity;
    if (!this.TryGetEntity(args.NetEnt, out entity) || this.TerminatingOrDeleted(entity) || !this.HasComp<WarpPointComponent>(entity) || this.Delay.IsDelayed((Entity<UseDelayComponent>) ent.Owner, "TeleportDelay"))
      return;
    TeleportLocationsComponent comp = ent.Comp;
    EntityUid actor = args.Actor;
    TransformComponent xform = this.Transform(entity.Value);
    EntProtoId? teleportEffect = comp.TeleportEffect;
    this.SpawnAtPosition(teleportEffect.HasValue ? (string) teleportEffect.GetValueOrDefault() : (string) null, this.Transform(actor).Coordinates);
    this._xform.SetMapCoordinates(actor, this._xform.GetMapCoordinates(entity.Value, xform));
    teleportEffect = comp.TeleportEffect;
    this.SpawnAtPosition(teleportEffect.HasValue ? (string) teleportEffect.GetValueOrDefault() : (string) null, xform.Coordinates);
    this.Delay.TryResetDelay(ent.Owner, true, id: "TeleportDelay");
    if (!ent.Comp.CloseAfterTeleport)
      return;
    this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) TeleportLocationUiKey.Key);
  }
}
