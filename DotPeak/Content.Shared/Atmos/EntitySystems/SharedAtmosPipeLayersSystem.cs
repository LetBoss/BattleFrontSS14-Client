// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.EntitySystems.SharedAtmosPipeLayersSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.Components;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.SubFloor;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedAtmosPipeLayersSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private IPrototypeManager _protoManager;
  [Dependency]
  private SharedToolSystem _tool;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedPopupSystem _popup;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AtmosPipeLayersComponent, ExaminedEvent>(new EntityEventRefHandler<AtmosPipeLayersComponent, ExaminedEvent>((object) this, __methodptr(OnExamined)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AtmosPipeLayersComponent, GetVerbsEvent<Verb>>(new EntityEventRefHandler<AtmosPipeLayersComponent, GetVerbsEvent<Verb>>((object) this, __methodptr(OnGetVerb)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AtmosPipeLayersComponent, InteractUsingEvent>(new EntityEventRefHandler<AtmosPipeLayersComponent, InteractUsingEvent>((object) this, __methodptr(OnInteractUsing)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AtmosPipeLayersComponent, UseInHandEvent>(new EntityEventRefHandler<AtmosPipeLayersComponent, UseInHandEvent>((object) this, __methodptr(OnUseInHandEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AtmosPipeLayersComponent, TrySetNextPipeLayerCompletedEvent>(new EntityEventRefHandler<AtmosPipeLayersComponent, TrySetNextPipeLayerCompletedEvent>((object) this, __methodptr(OnSetNextPipeLayerCompleted)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AtmosPipeLayersComponent, TrySettingPipeLayerCompletedEvent>(new EntityEventRefHandler<AtmosPipeLayersComponent, TrySettingPipeLayerCompletedEvent>((object) this, __methodptr(OnSettingPipeLayerCompleted)), (Type[]) null, (Type[]) null);
  }

  private void OnExamined(Entity<AtmosPipeLayersComponent> ent, ref ExaminedEvent args)
  {
    string pipeLayerName = this.GetPipeLayerName(ent.Comp.CurrentPipeLayer);
    args.PushMarkup(this.Loc.GetString("atmos-pipe-layers-component-current-layer", ("layerName", (object) pipeLayerName)));
  }

  private void OnGetVerb(Entity<AtmosPipeLayersComponent> ent, ref GetVerbsEvent<Verb> args)
  {
    ToolQualityPrototype qualityPrototype;
    if (!args.CanAccess || !args.CanInteract || !args.CanComplexInteract || ent.Comp.NumberOfPipeLayers <= (byte) 1 || ent.Comp.PipeLayersLocked || !this._protoManager.TryIndex<ToolQualityPrototype>(ent.Comp.Tool, ref qualityPrototype))
      return;
    EntityUid user = args.User;
    SubFloorHideComponent floorHideComponent;
    if (this.TryComp<SubFloorHideComponent>(Entity<AtmosPipeLayersComponent>.op_Implicit(ent), ref floorHideComponent) && floorHideComponent.IsUnderCover)
    {
      Verb verb = new Verb()
      {
        Priority = 1,
        Category = VerbCategory.Adjust,
        Text = this.Loc.GetString("atmos-pipe-layers-component-pipes-are-covered"),
        Disabled = true,
        Impact = LogImpact.Low,
        DoContactInteraction = new bool?(true)
      };
      args.Verbs.Add(verb);
    }
    else if (!this.TryGetHeldTool(user, ent.Comp.Tool, out Entity<ToolComponent>? _))
    {
      Verb verb = new Verb()
      {
        Priority = 1,
        Category = VerbCategory.Adjust,
        Text = this.Loc.GetString("atmos-pipe-layers-component-tool-missing", ("toolName", (object) this.Loc.GetString(qualityPrototype.ToolName).ToLower())),
        Disabled = true,
        Impact = LogImpact.Low,
        DoContactInteraction = new bool?(true)
      };
      args.Verbs.Add(verb);
    }
    else
    {
      for (int index1 = 0; index1 < (int) ent.Comp.NumberOfPipeLayers; ++index1)
      {
        int index = index1;
        string str = this.Loc.GetString("atmos-pipe-layers-component-select-layer", ("layerName", (object) this.GetPipeLayerName((AtmosPipeLayer) index)));
        Verb verb = new Verb()
        {
          Priority = 1,
          Category = VerbCategory.Adjust,
          Text = str,
          Disabled = (AtmosPipeLayer) index == ent.Comp.CurrentPipeLayer,
          Impact = LogImpact.Low,
          DoContactInteraction = new bool?(true),
          Act = (Action) (() => this._tool.UseTool(Entity<ToolComponent>.op_Implicit(tool.Value), user, new EntityUid?(Entity<AtmosPipeLayersComponent>.op_Implicit(ent)), ent.Comp.Delay, (IEnumerable<string>) tool.Value.Comp.Qualities, (DoAfterEvent) new TrySettingPipeLayerCompletedEvent((AtmosPipeLayer) index)))
        };
        args.Verbs.Add(verb);
      }
    }
  }

  private void OnInteractUsing(Entity<AtmosPipeLayersComponent> ent, ref InteractUsingEvent args)
  {
    ToolComponent tool;
    if (ent.Comp.NumberOfPipeLayers <= (byte) 1 || ent.Comp.PipeLayersLocked || !this.TryComp<ToolComponent>(args.Used, ref tool) || !this._tool.HasQuality(args.Used, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.Tool), tool))
      return;
    SubFloorHideComponent floorHideComponent;
    if (this.TryComp<SubFloorHideComponent>(Entity<AtmosPipeLayersComponent>.op_Implicit(ent), ref floorHideComponent) && floorHideComponent.IsUnderCover)
      this._popup.PopupClient(this.Loc.GetString("atmos-pipe-layers-component-cannot-adjust-pipes"), Entity<AtmosPipeLayersComponent>.op_Implicit(ent), new EntityUid?(args.User));
    else
      this._tool.UseTool(args.Used, args.User, new EntityUid?(Entity<AtmosPipeLayersComponent>.op_Implicit(ent)), ent.Comp.Delay, (IEnumerable<string>) tool.Qualities, (DoAfterEvent) new TrySetNextPipeLayerCompletedEvent());
  }

  private void OnUseInHandEvent(Entity<AtmosPipeLayersComponent> ent, ref UseInHandEvent args)
  {
    if (ent.Comp.NumberOfPipeLayers <= (byte) 1 || ent.Comp.PipeLayersLocked)
      return;
    Entity<ToolComponent>? heldTool;
    if (!this.TryGetHeldTool(args.User, ent.Comp.Tool, out heldTool))
    {
      ToolQualityPrototype qualityPrototype;
      if (!this._protoManager.TryIndex<ToolQualityPrototype>(ent.Comp.Tool, ref qualityPrototype))
        return;
      this._popup.PopupClient(this.Loc.GetString("atmos-pipe-layers-component-tool-missing", ("toolName", (object) this.Loc.GetString(qualityPrototype.ToolName).ToLower())), Entity<AtmosPipeLayersComponent>.op_Implicit(ent), new EntityUid?(args.User));
    }
    else
      this._tool.UseTool(Entity<ToolComponent>.op_Implicit(heldTool.Value), args.User, new EntityUid?(Entity<AtmosPipeLayersComponent>.op_Implicit(ent)), ent.Comp.Delay, (IEnumerable<string>) heldTool.Value.Comp.Qualities, (DoAfterEvent) new TrySetNextPipeLayerCompletedEvent());
  }

  private void OnSetNextPipeLayerCompleted(
    Entity<AtmosPipeLayersComponent> ent,
    ref TrySetNextPipeLayerCompletedEvent args)
  {
    if (args.Cancelled)
      return;
    this.SetNextPipeLayer(ent, new EntityUid?(args.User), args.Used);
  }

  private void OnSettingPipeLayerCompleted(
    Entity<AtmosPipeLayersComponent> ent,
    ref TrySettingPipeLayerCompletedEvent args)
  {
    if (args.Cancelled)
      return;
    this.SetPipeLayer(ent, args.PipeLayer, new EntityUid?(args.User), args.Used);
  }

  public void SetNextPipeLayer(
    Entity<AtmosPipeLayersComponent> ent,
    EntityUid? user = null,
    EntityUid? used = null)
  {
    int layer = (int) (ent.Comp.CurrentPipeLayer + 1) % (int) ent.Comp.NumberOfPipeLayers;
    this.SetPipeLayer(ent, (AtmosPipeLayer) layer, user, used);
  }

  public virtual void SetPipeLayer(
    Entity<AtmosPipeLayersComponent> ent,
    AtmosPipeLayer layer,
    EntityUid? user = null,
    EntityUid? used = null)
  {
    if (ent.Comp.PipeLayersLocked)
      return;
    ent.Comp.CurrentPipeLayer = (AtmosPipeLayer) Math.Clamp((int) layer, 0, (int) ent.Comp.NumberOfPipeLayers - 1);
    this.Dirty<AtmosPipeLayersComponent>(ent, (MetaDataComponent) null);
    AppearanceComponent appearanceComponent;
    if (this.TryComp<AppearanceComponent>(Entity<AtmosPipeLayersComponent>.op_Implicit(ent), ref appearanceComponent))
    {
      string str;
      if (ent.Comp.SpriteRsiPaths.TryGetValue(ent.Comp.CurrentPipeLayer, out str))
        this._appearance.SetData(Entity<AtmosPipeLayersComponent>.op_Implicit(ent), (Enum) AtmosPipeLayerVisuals.Sprite, (object) str, appearanceComponent);
      if (ent.Comp.SpriteLayersRsiPaths.Count > 0)
      {
        Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
        foreach ((string key, Dictionary<AtmosPipeLayer, string> dictionary2) in ent.Comp.SpriteLayersRsiPaths)
        {
          if (dictionary2.TryGetValue(ent.Comp.CurrentPipeLayer, out str))
            dictionary1.TryAdd(key, str);
        }
        this._appearance.SetData(Entity<AtmosPipeLayersComponent>.op_Implicit(ent), (Enum) AtmosPipeLayerVisuals.SpriteLayers, (object) dictionary1, appearanceComponent);
      }
    }
    if (!user.HasValue)
      return;
    this._popup.PopupClient(this.Loc.GetString("atmos-pipe-layers-component-change-layer", ("layerName", (object) this.GetPipeLayerName(ent.Comp.CurrentPipeLayer))), Entity<AtmosPipeLayersComponent>.op_Implicit(ent), user);
  }

  public bool TryGetAlternativePrototype(
    AtmosPipeLayersComponent component,
    AtmosPipeLayer layer,
    out EntProtoId proto)
  {
    return component.AlternativePrototypes.TryGetValue(layer, out proto);
  }

  private bool TryGetHeldTool(
    EntityUid user,
    ProtoId<ToolQualityPrototype> toolQuality,
    [NotNullWhen(true)] out Entity<ToolComponent>? heldTool)
  {
    heldTool = new Entity<ToolComponent>?();
    foreach (EntityUid uid in this._hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit(user)))
    {
      ToolComponent tool;
      if (this.TryComp<ToolComponent>(uid, ref tool) && this._tool.HasQuality(uid, ProtoId<ToolQualityPrototype>.op_Implicit(toolQuality), tool))
      {
        heldTool = new Entity<ToolComponent>?(new Entity<ToolComponent>(uid, tool));
        return true;
      }
    }
    return false;
  }

  private string GetPipeLayerName(AtmosPipeLayer layer)
  {
    return this.Loc.GetString("atmos-pipe-layers-component-layer-" + layer.ToString().ToLower());
  }
}
