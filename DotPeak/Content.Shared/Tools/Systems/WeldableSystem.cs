// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tools.Systems.WeldableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Content.Shared.Tools.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using System;

#nullable enable
namespace Content.Shared.Tools.Systems;

public sealed class WeldableSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private SharedToolSystem _toolSystem;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedPhysicsSystem _physics;
  private Robust.Shared.GameObjects.EntityQuery<WeldableComponent> _query;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<WeldableComponent, InteractUsingEvent>(new ComponentEventHandler<WeldableComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<WeldableComponent, WeldFinishedEvent>(new ComponentEventHandler<WeldableComponent, WeldFinishedEvent>(this.OnWeldFinished));
    this.SubscribeLocalEvent<LayerChangeOnWeldComponent, WeldableChangedEvent>(new ComponentEventRefHandler<LayerChangeOnWeldComponent, WeldableChangedEvent>(this.OnWeldChanged));
    this.SubscribeLocalEvent<WeldableComponent, ExaminedEvent>(new ComponentEventHandler<WeldableComponent, ExaminedEvent>(this.OnExamine));
    this._query = this.GetEntityQuery<WeldableComponent>();
  }

  public bool IsWelded(EntityUid uid, WeldableComponent? component = null)
  {
    return this._query.Resolve(uid, ref component, false) && component.IsWelded;
  }

  private void OnExamine(EntityUid uid, WeldableComponent component, ExaminedEvent args)
  {
    if (!component.IsWelded || !component.WeldedExamineMessage.HasValue)
      return;
    ExaminedEvent examinedEvent = args;
    ILocalizationManager loc = this.Loc;
    LocId? weldedExamineMessage = component.WeldedExamineMessage;
    string valueOrDefault = weldedExamineMessage.HasValue ? (string) weldedExamineMessage.GetValueOrDefault() : (string) null;
    string text = loc.GetString(valueOrDefault);
    examinedEvent.PushText(text);
  }

  private void OnInteractUsing(EntityUid uid, WeldableComponent component, InteractUsingEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = this.TryWeld(uid, args.Used, args.User, component);
  }

  private bool CanWeld(EntityUid uid, EntityUid tool, EntityUid user, WeldableComponent? component = null)
  {
    if (!this._query.Resolve(uid, ref component))
      return false;
    WeldableAttemptEvent args = new WeldableAttemptEvent(user, tool);
    this.RaiseLocalEvent<WeldableAttemptEvent>(uid, args);
    return !args.Cancelled;
  }

  private bool TryWeld(EntityUid uid, EntityUid tool, EntityUid user, WeldableComponent? component = null)
  {
    if (!this._query.Resolve(uid, ref component) || !this.CanWeld(uid, tool, user, component) || !this._toolSystem.UseTool(tool, user, new EntityUid?(uid), (float) component.Time.Seconds, (string) component.WeldingQuality, (DoAfterEvent) new WeldFinishedEvent(), component.Fuel))
      return false;
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(16 /*0x10*/, 4);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" is ");
    logStringHandler.AppendFormatted(component.IsWelded ? "un" : "");
    logStringHandler.AppendLiteral("welding ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "target", "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" at ");
    logStringHandler.AppendFormatted<EntityCoordinates>(this.Transform(uid).Coordinates, "targetlocation", "Transform(uid).Coordinates");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
    return true;
  }

  private void OnWeldFinished(EntityUid uid, WeldableComponent component, WeldFinishedEvent args)
  {
    if (args.Cancelled || !args.Used.HasValue || !this.CanWeld(uid, args.Used.Value, args.User, component))
      return;
    this.SetWeldedState(uid, !component.IsWelded, component);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(8, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "user", "ToPrettyString(args.User)");
    logStringHandler.AppendLiteral(" ");
    logStringHandler.AppendFormatted(!component.IsWelded ? "un" : "");
    logStringHandler.AppendLiteral("welded ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "target", "ToPrettyString(uid)");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
  }

  private void OnWeldChanged(
    EntityUid uid,
    LayerChangeOnWeldComponent component,
    ref WeldableChangedEvent args)
  {
    FixturesComponent comp;
    if (!this.TryComp<FixturesComponent>(uid, out comp))
      return;
    foreach ((string str, Fixture fixture) in comp.Fixtures)
    {
      if (args.IsWelded)
      {
        if ((CollisionGroup) fixture.CollisionLayer == component.UnWeldedLayer)
          this._physics.SetCollisionLayer(uid, str, fixture, (int) component.WeldedLayer);
      }
      else if ((CollisionGroup) fixture.CollisionLayer == component.WeldedLayer)
        this._physics.SetCollisionLayer(uid, str, fixture, (int) component.UnWeldedLayer);
    }
  }

  private void UpdateAppearance(EntityUid uid, WeldableComponent? component = null)
  {
    if (!this._query.Resolve(uid, ref component))
      return;
    this._appearance.SetData(uid, (Enum) WeldableVisuals.IsWelded, (object) component.IsWelded);
  }

  public void SetWeldedState(EntityUid uid, bool state, WeldableComponent? component = null)
  {
    if (!this._query.Resolve(uid, ref component) || component.IsWelded == state)
      return;
    component.IsWelded = state;
    WeldableChangedEvent args = new WeldableChangedEvent(component.IsWelded);
    this.RaiseLocalEvent<WeldableChangedEvent>(uid, ref args);
    this.UpdateAppearance(uid, component);
    this.Dirty(uid, (IComponent) component);
  }

  public void SetWeldingTime(EntityUid uid, TimeSpan time, WeldableComponent? component = null)
  {
    if (!this._query.Resolve(uid, ref component) || component.Time.Equals(time))
      return;
    component.Time = time;
    this.Dirty(uid, (IComponent) component);
  }
}
