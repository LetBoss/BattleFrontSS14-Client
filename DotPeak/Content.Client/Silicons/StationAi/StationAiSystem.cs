// Decompiled with JetBrains decompiler
// Type: Content.Client.Silicons.StationAi.StationAiSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Doors.Components;
using Content.Shared.Electrocution;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Light.Components;
using Content.Shared.Silicons.StationAi;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client.Silicons.StationAi;

public sealed class StationAiSystem : SharedStationAiSystem
{
  private readonly ResPath _aiActionsRsi = new ResPath("/Textures/Interface/Actions/actions_ai.rsi");
  [Dependency]
  private IOverlayManager _overlayMgr;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;
  private StationAiOverlay? _overlay;

  private void InitializeAirlock()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DoorBoltComponent, GetStationAiRadialEvent>(new EntityEventRefHandler<DoorBoltComponent, GetStationAiRadialEvent>((object) this, __methodptr(OnDoorBoltGetRadial)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AirlockComponent, GetStationAiRadialEvent>(new EntityEventRefHandler<AirlockComponent, GetStationAiRadialEvent>((object) this, __methodptr(OnEmergencyAccessGetRadial)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ElectrifiedComponent, GetStationAiRadialEvent>(new EntityEventRefHandler<ElectrifiedComponent, GetStationAiRadialEvent>((object) this, __methodptr(OnDoorElectrifiedGetRadial)), (Type[]) null, (Type[]) null);
  }

  private void OnDoorBoltGetRadial(Entity<DoorBoltComponent> ent, ref GetStationAiRadialEvent args)
  {
    args.Actions.Add(new StationAiRadial()
    {
      Sprite = ent.Comp.BoltsDown ? (SpriteSpecifier) new SpriteSpecifier.Rsi(this._aiActionsRsi, "unbolt_door") : (SpriteSpecifier) new SpriteSpecifier.Rsi(this._aiActionsRsi, "bolt_door"),
      Tooltip = ent.Comp.BoltsDown ? this.Loc.GetString("bolt-open") : this.Loc.GetString("bolt-close"),
      Event = (BaseStationAiAction) new StationAiBoltEvent()
      {
        Bolted = !ent.Comp.BoltsDown
      }
    });
  }

  private void OnEmergencyAccessGetRadial(
    Entity<AirlockComponent> ent,
    ref GetStationAiRadialEvent args)
  {
    args.Actions.Add(new StationAiRadial()
    {
      Sprite = ent.Comp.EmergencyAccess ? (SpriteSpecifier) new SpriteSpecifier.Rsi(this._aiActionsRsi, "emergency_off") : (SpriteSpecifier) new SpriteSpecifier.Rsi(this._aiActionsRsi, "emergency_on"),
      Tooltip = ent.Comp.EmergencyAccess ? this.Loc.GetString("emergency-access-off") : this.Loc.GetString("emergency-access-on"),
      Event = (BaseStationAiAction) new StationAiEmergencyAccessEvent()
      {
        EmergencyAccess = !ent.Comp.EmergencyAccess
      }
    });
  }

  private void OnDoorElectrifiedGetRadial(
    Entity<ElectrifiedComponent> ent,
    ref GetStationAiRadialEvent args)
  {
    args.Actions.Add(new StationAiRadial()
    {
      Sprite = ent.Comp.Enabled ? (SpriteSpecifier) new SpriteSpecifier.Rsi(this._aiActionsRsi, "door_overcharge_off") : (SpriteSpecifier) new SpriteSpecifier.Rsi(this._aiActionsRsi, "door_overcharge_on"),
      Tooltip = ent.Comp.Enabled ? this.Loc.GetString("electrify-door-off") : this.Loc.GetString("electrify-door-on"),
      Event = (BaseStationAiAction) new StationAiElectrifiedEvent()
      {
        Electrified = !ent.Comp.Enabled
      }
    });
  }

  public override void Initialize()
  {
    base.Initialize();
    this.InitializeAirlock();
    this.InitializePowerToggle();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StationAiOverlayComponent, LocalPlayerAttachedEvent>(new EntityEventRefHandler<StationAiOverlayComponent, LocalPlayerAttachedEvent>((object) this, __methodptr(OnAiAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StationAiOverlayComponent, LocalPlayerDetachedEvent>(new EntityEventRefHandler<StationAiOverlayComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnAiDetached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StationAiOverlayComponent, ComponentInit>(new EntityEventRefHandler<StationAiOverlayComponent, ComponentInit>((object) this, __methodptr(OnAiOverlayInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StationAiOverlayComponent, ComponentRemove>(new EntityEventRefHandler<StationAiOverlayComponent, ComponentRemove>((object) this, __methodptr(OnAiOverlayRemove)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StationAiCoreComponent, AppearanceChangeEvent>(new EntityEventRefHandler<StationAiCoreComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnAiOverlayInit(Entity<StationAiOverlayComponent> ent, ref ComponentInit args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid owner = ent.Owner;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(localEntity.GetValueOrDefault(), owner) ? 1 : 0) : 1) != 0)
      return;
    this.AddOverlay();
  }

  private void OnAiOverlayRemove(Entity<StationAiOverlayComponent> ent, ref ComponentRemove args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid owner = ent.Owner;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(localEntity.GetValueOrDefault(), owner) ? 1 : 0) : 1) != 0)
      return;
    this.RemoveOverlay();
  }

  private void AddOverlay()
  {
    if (this._overlay != null)
      return;
    this._overlay = new StationAiOverlay();
    this._overlayMgr.AddOverlay((Overlay) this._overlay);
  }

  private void RemoveOverlay()
  {
    if (this._overlay == null)
      return;
    this._overlayMgr.RemoveOverlay((Overlay) this._overlay);
    this._overlay = (StationAiOverlay) null;
  }

  private void OnAiAttached(
    Entity<StationAiOverlayComponent> ent,
    ref LocalPlayerAttachedEvent args)
  {
    this.AddOverlay();
  }

  private void OnAiDetached(
    Entity<StationAiOverlayComponent> ent,
    ref LocalPlayerDetachedEvent args)
  {
    this.RemoveOverlay();
  }

  private void OnAppearanceChange(
    Entity<StationAiCoreComponent> entity,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    PrototypeLayerData prototypeLayerData;
    if (this._appearance.TryGetData<PrototypeLayerData>(entity.Owner, (Enum) StationAiVisualState.Key, ref prototypeLayerData, args.Component))
      this._sprite.LayerSetData(Entity<SpriteComponent>.op_Implicit((entity.Owner, args.Sprite)), (Enum) StationAiVisualState.Key, prototypeLayerData);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((entity.Owner, args.Sprite)), (Enum) StationAiVisualState.Key, prototypeLayerData != null);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlayMgr.RemoveOverlay<StationAiOverlay>();
  }

  private void InitializePowerToggle()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemTogglePointLightComponent, GetStationAiRadialEvent>(new EntityEventRefHandler<ItemTogglePointLightComponent, GetStationAiRadialEvent>((object) this, __methodptr(OnLightGetRadial)), (Type[]) null, (Type[]) null);
  }

  private void OnLightGetRadial(
    Entity<ItemTogglePointLightComponent> ent,
    ref GetStationAiRadialEvent args)
  {
    ItemToggleComponent itemToggleComponent;
    if (!this.TryComp<ItemToggleComponent>(ent.Owner, ref itemToggleComponent))
      return;
    args.Actions.Add(new StationAiRadial()
    {
      Tooltip = this.Loc.GetString("toggle-light"),
      Sprite = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/light.svg.192dpi.png")),
      Event = (BaseStationAiAction) new StationAiLightEvent()
      {
        Enabled = !itemToggleComponent.Activated
      }
    });
  }
}
