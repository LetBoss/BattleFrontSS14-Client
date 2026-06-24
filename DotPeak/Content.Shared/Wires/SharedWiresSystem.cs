// Decompiled with JetBrains decompiler
// Type: Content.Shared.Wires.SharedWiresSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Tools.Systems;
using Content.Shared.UserInterface;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using System;

#nullable enable
namespace Content.Shared.Wires;

public abstract class SharedWiresSystem : EntitySystem
{
  [Dependency]
  protected ISharedAdminLogManager AdminLogger;
  [Dependency]
  private ActivatableUISystem _activatableUI;
  [Dependency]
  protected SharedAppearanceSystem Appearance;
  [Dependency]
  protected SharedAudioSystem Audio;
  [Dependency]
  protected SharedToolSystem Tool;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<WiresPanelComponent, ComponentStartup>(new EntityEventRefHandler<WiresPanelComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<WiresPanelComponent, WirePanelDoAfterEvent>(new ComponentEventHandler<WiresPanelComponent, WirePanelDoAfterEvent>(this.OnPanelDoAfter));
    this.SubscribeLocalEvent<WiresPanelComponent, InteractUsingEvent>(new EntityEventRefHandler<WiresPanelComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<WiresPanelComponent, ExaminedEvent>(new ComponentEventHandler<WiresPanelComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<ActivatableUIRequiresPanelComponent, ActivatableUIOpenAttemptEvent>(new ComponentEventHandler<ActivatableUIRequiresPanelComponent, ActivatableUIOpenAttemptEvent>(this.OnAttemptOpenActivatableUI));
    this.SubscribeLocalEvent<ActivatableUIRequiresPanelComponent, PanelChangedEvent>(new ComponentEventRefHandler<ActivatableUIRequiresPanelComponent, PanelChangedEvent>(this.OnActivatableUIPanelChanged));
  }

  private void OnStartup(Entity<WiresPanelComponent> ent, ref ComponentStartup args)
  {
    this.UpdateAppearance((EntityUid) ent, (WiresPanelComponent) ent);
  }

  private void OnPanelDoAfter(EntityUid uid, WiresPanelComponent panel, WirePanelDoAfterEvent args)
  {
    if (args.Cancelled || !this.TogglePanel(uid, panel, !panel.Open, new EntityUid?(args.User)))
      return;
    ISharedAdminLogManager adminLogger = this.AdminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(30, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "user", "ToPrettyString(args.User)");
    logStringHandler.AppendLiteral(" screwed ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "target", "ToPrettyString(uid)");
    logStringHandler.AppendLiteral("'s maintenance panel ");
    logStringHandler.AppendFormatted(panel.Open ? "open" : "closed");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
    this.Audio.PlayPredicted(panel.Open ? panel.ScrewdriverOpenSound : panel.ScrewdriverCloseSound, uid, new EntityUid?(args.User));
    args.Handled = true;
  }

  private void OnInteractUsing(Entity<WiresPanelComponent> ent, ref InteractUsingEvent args)
  {
    if (!this.Tool.HasQuality(args.Used, (string) ent.Comp.OpeningTool) || !this.CanTogglePanel(ent, new EntityUid?(args.User)) || !this.Tool.UseTool(args.Used, args.User, new EntityUid?((EntityUid) ent), (float) ent.Comp.OpenDelay.TotalSeconds, (string) ent.Comp.OpeningTool, (DoAfterEvent) new WirePanelDoAfterEvent()))
      return;
    ISharedAdminLogManager adminLogger = this.AdminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(38, 4);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "user", "ToPrettyString(args.User)");
    logStringHandler.AppendLiteral(" is screwing ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) ent)), "target", "ToPrettyString(ent)");
    logStringHandler.AppendLiteral("'s ");
    logStringHandler.AppendFormatted(ent.Comp.Open ? "open" : "closed");
    logStringHandler.AppendLiteral(" maintenance panel at ");
    logStringHandler.AppendFormatted<EntityCoordinates>(this.Transform((EntityUid) ent).Coordinates, "targetlocation", "Transform(ent).Coordinates");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
    args.Handled = true;
  }

  private void OnExamine(EntityUid uid, WiresPanelComponent component, ExaminedEvent args)
  {
    using (args.PushGroup("WiresPanelComponent"))
    {
      if (!component.Open)
      {
        LocId? examineTextClosed = component.ExamineTextClosed;
        if (string.IsNullOrEmpty(examineTextClosed.HasValue ? (string) examineTextClosed.GetValueOrDefault() : (string) null))
          return;
        ExaminedEvent examinedEvent = args;
        ILocalizationManager loc = this.Loc;
        examineTextClosed = component.ExamineTextClosed;
        string valueOrDefault = examineTextClosed.HasValue ? (string) examineTextClosed.GetValueOrDefault() : (string) null;
        string markup = loc.GetString(valueOrDefault);
        examinedEvent.PushMarkup(markup);
      }
      else
      {
        LocId? examineTextOpen = component.ExamineTextOpen;
        if (!string.IsNullOrEmpty(examineTextOpen.HasValue ? (string) examineTextOpen.GetValueOrDefault() : (string) null))
        {
          ExaminedEvent examinedEvent = args;
          ILocalizationManager loc = this.Loc;
          examineTextOpen = component.ExamineTextOpen;
          string valueOrDefault = examineTextOpen.HasValue ? (string) examineTextOpen.GetValueOrDefault() : (string) null;
          string markup = loc.GetString(valueOrDefault);
          examinedEvent.PushMarkup(markup);
        }
        WiresPanelSecurityComponent comp;
        if (!this.TryComp<WiresPanelSecurityComponent>(uid, out comp) || comp.Examine == null)
          return;
        args.PushMarkup(this.Loc.GetString(comp.Examine));
      }
    }
  }

  public void ChangePanelVisibility(EntityUid uid, WiresPanelComponent component, bool visible)
  {
    component.Visible = visible;
    this.UpdateAppearance(uid, component);
    this.Dirty(uid, (IComponent) component);
  }

  protected void UpdateAppearance(EntityUid uid, WiresPanelComponent panel)
  {
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>(uid, out comp))
      return;
    this.Appearance.SetData(uid, (Enum) WiresVisuals.MaintenancePanelState, (object) (bool) (!panel.Open ? 0 : (panel.Visible ? 1 : 0)), comp);
  }

  public bool TogglePanel(
    EntityUid uid,
    WiresPanelComponent component,
    bool open,
    EntityUid? user = null)
  {
    if (!this.CanTogglePanel((Entity<WiresPanelComponent>) (uid, component), user))
      return false;
    component.Open = open;
    this.UpdateAppearance(uid, component);
    this.Dirty(uid, (IComponent) component);
    PanelChangedEvent args = new PanelChangedEvent(component.Open);
    this.RaiseLocalEvent<PanelChangedEvent>(uid, ref args);
    return true;
  }

  public bool CanTogglePanel(Entity<WiresPanelComponent> ent, EntityUid? user)
  {
    AttemptChangePanelEvent args = new AttemptChangePanelEvent(ent.Comp.Open, user);
    this.RaiseLocalEvent<AttemptChangePanelEvent>((EntityUid) ent, ref args);
    return !args.Cancelled;
  }

  public bool IsPanelOpen(Entity<WiresPanelComponent?> entity, EntityUid? tool = null)
  {
    if (!this.Resolve<WiresPanelComponent>((EntityUid) entity, ref entity.Comp, false))
      return true;
    if (tool.HasValue)
    {
      PanelOverrideEvent args = new PanelOverrideEvent();
      this.RaiseLocalEvent<PanelOverrideEvent>(tool.Value, ref args);
      if (args.Allowed)
        return true;
    }
    WiresPanelSecurityComponent comp;
    return (!this.TryComp<WiresPanelSecurityComponent>((EntityUid) entity, out comp) || comp.WiresAccessible) && entity.Comp.Open;
  }

  private void OnAttemptOpenActivatableUI(
    EntityUid uid,
    ActivatableUIRequiresPanelComponent component,
    ActivatableUIOpenAttemptEvent args)
  {
    WiresPanelComponent comp;
    if (args.Cancelled || !this.TryComp<WiresPanelComponent>(uid, out comp) || component.RequireOpen == comp.Open)
      return;
    args.Cancel();
  }

  private void OnActivatableUIPanelChanged(
    EntityUid uid,
    ActivatableUIRequiresPanelComponent component,
    ref PanelChangedEvent args)
  {
    if (args.Open == component.RequireOpen)
      return;
    this._activatableUI.CloseAll(uid);
  }
}
