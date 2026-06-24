// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tools.EntitySystems.ToolOpenableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared.Tools.EntitySystems;

public sealed class ToolOpenableSystem : EntitySystem
{
  [Dependency]
  private SharedToolSystem _tool;
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ToolOpenableComponent, ComponentInit>(new EntityEventRefHandler<ToolOpenableComponent, ComponentInit>(this.OnInit));
    this.SubscribeLocalEvent<ToolOpenableComponent, ToolOpenableDoAfterEventToggleOpen>(new EntityEventRefHandler<ToolOpenableComponent, ToolOpenableDoAfterEventToggleOpen>(this.OnOpenableStateToggled));
    this.SubscribeLocalEvent<ToolOpenableComponent, InteractUsingEvent>(new EntityEventRefHandler<ToolOpenableComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<ToolOpenableComponent, ExaminedEvent>(new EntityEventRefHandler<ToolOpenableComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<ToolOpenableComponent, GetVerbsEvent<InteractionVerb>>(new EntityEventRefHandler<ToolOpenableComponent, GetVerbsEvent<InteractionVerb>>(this.OnGetVerb));
  }

  private void OnInit(Entity<ToolOpenableComponent> entity, ref ComponentInit args)
  {
    this.UpdateAppearance(entity);
    this.Dirty<ToolOpenableComponent>(entity);
  }

  private void OnInteractUsing(Entity<ToolOpenableComponent> entity, ref InteractUsingEvent args)
  {
    if (args.Handled || entity.Comp.VerbOnly || !this.TryOpenClose(entity, new EntityUid?(args.Used), args.User))
      return;
    args.Handled = true;
  }

  private bool TryOpenClose(
    Entity<ToolOpenableComponent> entity,
    EntityUid? toolToToggle,
    EntityUid user)
  {
    ProtoId<ToolQualityPrototype>? nullable1 = entity.Comp.IsOpen ? entity.Comp.CloseToolQualityNeeded : entity.Comp.OpenToolQualityNeeded;
    float num = entity.Comp.IsOpen ? entity.Comp.CloseTime : entity.Comp.OpenTime;
    ToolOpenableDoAfterEventToggleOpen afterEventToggleOpen = new ToolOpenableDoAfterEventToggleOpen();
    if (!toolToToggle.HasValue || !nullable1.HasValue)
      return false;
    SharedToolSystem tool1 = this._tool;
    EntityUid tool2 = toolToToggle.Value;
    EntityUid user1 = user;
    EntityUid? target = new EntityUid?((EntityUid) entity);
    double doAfterDelay = (double) num;
    ProtoId<ToolQualityPrototype>? nullable2 = nullable1;
    string valueOrDefault = nullable2.HasValue ? (string) nullable2.GetValueOrDefault() : (string) null;
    ToolOpenableDoAfterEventToggleOpen doAfterEv = afterEventToggleOpen;
    return tool1.UseTool(tool2, user1, target, (float) doAfterDelay, valueOrDefault, (DoAfterEvent) doAfterEv);
  }

  private void OnOpenableStateToggled(
    Entity<ToolOpenableComponent> entity,
    ref ToolOpenableDoAfterEventToggleOpen args)
  {
    if (args.Cancelled)
      return;
    this.ToggleState(entity);
  }

  private void ToggleState(Entity<ToolOpenableComponent> entity)
  {
    entity.Comp.IsOpen = !entity.Comp.IsOpen;
    this.UpdateAppearance(entity);
    this.Dirty<ToolOpenableComponent>(entity);
  }

  private string GetName(Entity<ToolOpenableComponent> entity)
  {
    return entity.Comp.Name == null ? (string) Identity.Name((EntityUid) entity, (IEntityManager) this.EntityManager) : this.Loc.GetString(entity.Comp.Name);
  }

  public bool IsOpen(EntityUid uid, ToolOpenableComponent? component = null)
  {
    return !this.Resolve<ToolOpenableComponent>(uid, ref component, false) || component.IsOpen;
  }

  private void UpdateAppearance(Entity<ToolOpenableComponent> entity)
  {
    this._appearance.SetData((EntityUid) entity, (Enum) ToolOpenableVisuals.ToolOpenableVisualState, (object) (ToolOpenableVisualState) (entity.Comp.IsOpen ? 0 : 1));
  }

  private void OnExamine(Entity<ToolOpenableComponent> entity, ref ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    string name = this.GetName(entity);
    string markup = !entity.Comp.IsOpen ? this.Loc.GetString("tool-openable-component-examine-closed", ("name", (object) name)) : this.Loc.GetString("tool-openable-component-examine-opened", ("name", (object) name));
    args.PushMarkup(markup);
  }

  private void OnGetVerb(
    Entity<ToolOpenableComponent> entity,
    ref GetVerbsEvent<InteractionVerb> args)
  {
    if (!args.CanInteract || !args.CanAccess || !entity.Comp.HasVerbs)
      return;
    EntityUid user = args.User;
    EntityUid? item = args.Using;
    string name = this.GetName(entity);
    InteractionVerb interactionVerb1 = new InteractionVerb();
    interactionVerb1.IconEntity = this.GetNetEntity(item);
    InteractionVerb interactionVerb2 = interactionVerb1;
    if (entity.Comp.IsOpen)
    {
      interactionVerb2.Text = interactionVerb2.Message = this.Loc.GetString("tool-openable-component-verb-close");
      ProtoId<ToolQualityPrototype>? toolQualityNeeded = entity.Comp.CloseToolQualityNeeded;
      if (toolQualityNeeded.HasValue)
      {
        if (item.HasValue)
        {
          SharedToolSystem tool = this._tool;
          EntityUid uid = item.Value;
          ProtoId<ToolQualityPrototype>? nullable = toolQualityNeeded;
          string valueOrDefault = nullable.HasValue ? (string) nullable.GetValueOrDefault() : (string) null;
          if (tool.HasQuality(uid, valueOrDefault))
            goto label_7;
        }
        interactionVerb2.Disabled = true;
        interactionVerb2.Message = this.Loc.GetString("tool-openable-component-verb-cant-close", ("name", (object) name));
      }
label_7:
      if (!toolQualityNeeded.HasValue)
        interactionVerb2.Act = (Action) (() => this.ToggleState(entity));
      else
        interactionVerb2.Act = (Action) (() => this.TryOpenClose(entity, item, user));
      args.Verbs.Add(interactionVerb2);
    }
    else
    {
      interactionVerb2.Text = interactionVerb2.Message = this.Loc.GetString("tool-openable-component-verb-open");
      ProtoId<ToolQualityPrototype>? toolQualityNeeded = entity.Comp.OpenToolQualityNeeded;
      if (!toolQualityNeeded.HasValue)
      {
        interactionVerb2.Act = (Action) (() => this.ToggleState(entity));
        args.Verbs.Add(interactionVerb2);
      }
      else
      {
        if (!item.HasValue)
          return;
        SharedToolSystem tool = this._tool;
        EntityUid uid = item.Value;
        ProtoId<ToolQualityPrototype>? nullable = toolQualityNeeded;
        string valueOrDefault = nullable.HasValue ? (string) nullable.GetValueOrDefault() : (string) null;
        if (!tool.HasQuality(uid, valueOrDefault))
          return;
        interactionVerb2.Act = (Action) (() => this.TryOpenClose(entity, item, user));
        args.Verbs.Add(interactionVerb2);
      }
    }
  }
}
