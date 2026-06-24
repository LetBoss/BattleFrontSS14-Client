// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.TacticalMap.TacticalMapComputerBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.UserInterface;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.TacticalMap;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.TacticalMap;

public sealed class TacticalMapComputerBui(EntityUid owner, Enum uiKey) : 
  RMCPopOutBui<TacticalMapWindow>(owner, uiKey)
{
  [Dependency]
  private IPlayerManager _player;
  private bool _refreshed;
  private string? _currentMapName;

  protected override TacticalMapWindow? Window { get; set; }

  protected virtual void Open()
  {
    base.Open();
    TacticalMapComputerComponent componentOrNull = EntityManagerExt.GetComponentOrNull<TacticalMapComputerComponent>(this.EntMan, this.Owner);
    if (componentOrNull != null && componentOrNull.Map.HasValue)
    {
      EntityUid entityUid = componentOrNull.Map.Value;
    }
    this.Window = this.CreatePopOutableWindow<TacticalMapWindow>();
    if (this._currentMapName != null)
      this.Window.SetMapEntity(this._currentMapName);
    TabContainer.SetTabTitle((Control) this.Window.Wrapper.MapTab, Loc.GetString("ui-tactical-map-tab-map"));
    TabContainer.SetTabVisible((Control) this.Window.Wrapper.MapTab, true);
    if (componentOrNull != null)
    {
      EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
      if (localEntity.HasValue)
      {
        EntityUid valueOrDefault = localEntity.GetValueOrDefault();
        if (this.EntMan.System<SkillsSystem>().HasSkill(Entity<SkillsComponent>.op_Implicit(valueOrDefault), componentOrNull.Skill, componentOrNull.SkillLevel))
        {
          TabContainer.SetTabTitle((Control) this.Window.Wrapper.CanvasTab, Loc.GetString("ui-tactical-map-tab-canvas"));
          TabContainer.SetTabVisible((Control) this.Window.Wrapper.CanvasTab, true);
          goto label_9;
        }
      }
    }
    TabContainer.SetTabVisible((Control) this.Window.Wrapper.CanvasTab, false);
label_9:
    AreaGridComponent areaGridComponent;
    if (componentOrNull != null && this.EntMan.TryGetComponent<AreaGridComponent>(componentOrNull.Map, ref areaGridComponent))
      this.Window.Wrapper.UpdateTexture(Entity<AreaGridComponent>.op_Implicit((componentOrNull.Map.Value, areaGridComponent)));
    if (this._currentMapName != null)
      this.Window.Wrapper.SetMapEntity(this._currentMapName);
    try
    {
      TacticalMapSettings settings = IoCManager.Resolve<TacticalMapSettingsManager>().LoadSettings(this._currentMapName);
      if (this._currentMapName != null)
        this.Window.Wrapper.LoadMapSpecificSettings(settings, this._currentMapName);
    }
    catch (Exception ex)
    {
      Logger.GetSawmill("tactical_map_settings").Error($"Failed to load tactical map settings for map '{this._currentMapName}': {ex}");
    }
    this.Refresh();
    ((BaseButton) this.Window.Wrapper.UpdateCanvasButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new TacticalMapUpdateCanvasMsg(this.Window.Wrapper.Canvas.Lines, this.Window.Wrapper.Canvas.TacticalLabels)));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    if (!(state is TacticalMapBuiState tacticalMapBuiState))
      return;
    this._currentMapName = tacticalMapBuiState.MapName;
    this.Window?.SetMapEntity(this._currentMapName);
    this.Window?.Wrapper.SetMapEntity(this._currentMapName);
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      if (this.Window?.Wrapper != null)
      {
        try
        {
          IoCManager.Resolve<TacticalMapSettingsManager>().SaveSettings(this.Window.Wrapper.GetCurrentSettings() with
          {
            WindowSize = new Vector2(((Control) this.Window).SetSize.X, ((Control) this.Window).SetSize.Y),
            WindowPosition = new Vector2(((Control) this.Window).Position.X, ((Control) this.Window).Position.Y)
          }, this._currentMapName);
        }
        catch (Exception ex)
        {
          Logger.GetSawmill("tactical_map_settings").Error($"Failed to save tactical map settings during disposal for map '{this._currentMapName}': {ex}");
        }
      }
    }
    base.Dispose(disposing);
  }

  public void Refresh()
  {
    if (this.Window == null)
      return;
    this.Window.Wrapper.SetLineLimit(this.EntMan.System<TacticalMapSystem>().LineLimit);
    this.UpdateBlips();
    this.UpdateLabels();
    TacticalMapComputerComponent computerComponent;
    if (this.EntMan.TryGetComponent<TacticalMapComputerComponent>(this.Owner, ref computerComponent))
    {
      this.Window.Wrapper.LastUpdateAt = computerComponent.LastAnnounceAt;
      this.Window.Wrapper.NextUpdateAt = computerComponent.NextAnnounceAt;
    }
    this.Window.Wrapper.Map.Lines.Clear();
    TacticalMapLinesComponent componentOrNull = EntityManagerExt.GetComponentOrNull<TacticalMapLinesComponent>(this.EntMan, this.Owner);
    if (componentOrNull != null)
      this.Window.Wrapper.Map.Lines.AddRange((IEnumerable<TacticalMapLine>) componentOrNull.MarineLines);
    if (this._refreshed)
      return;
    if (componentOrNull != null)
      this.Window.Wrapper.Canvas.Lines.AddRange((IEnumerable<TacticalMapLine>) componentOrNull.MarineLines);
    this._refreshed = true;
  }

  private void UpdateBlips()
  {
    if (this.Window == null)
      return;
    TacticalMapComputerComponent computerComponent;
    if (!this.EntMan.TryGetComponent<TacticalMapComputerComponent>(this.Owner, ref computerComponent))
    {
      this.Window.Wrapper.UpdateBlips((TacticalMapBlip[]) null);
    }
    else
    {
      TacticalMapBlip[] blips = new TacticalMapBlip[computerComponent.Blips.Count];
      int[] entityIds = new int[computerComponent.Blips.Count];
      int index = 0;
      foreach ((int key, TacticalMapBlip tacticalMapBlip) in computerComponent.Blips)
      {
        blips[index] = tacticalMapBlip;
        entityIds[index] = key;
        ++index;
      }
      this.Window.Wrapper.UpdateBlips(blips, entityIds);
      EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
      int? nullable;
      if (!localEntity.HasValue)
      {
        nullable = new int?();
      }
      else
      {
        IEntityManager entMan = this.EntMan;
        localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
        EntityUid entityUid = localEntity.Value;
        nullable = new int?(NetEntity.op_Explicit(entMan.GetNetEntity(entityUid, (MetaDataComponent) null)));
      }
      int? entityId = nullable;
      this.Window.Wrapper.Map.SetLocalPlayerEntityId(entityId);
      this.Window.Wrapper.Canvas.SetLocalPlayerEntityId(entityId);
    }
  }

  private void UpdateLabels()
  {
    if (this.Window == null)
      return;
    TacticalMapLabelsComponent componentOrNull = EntityManagerExt.GetComponentOrNull<TacticalMapLabelsComponent>(this.EntMan, this.Owner);
    if (componentOrNull != null)
    {
      this.Window.Wrapper.Map.UpdateTacticalLabels(componentOrNull.MarineLabels);
      if (this._refreshed)
        return;
      this.Window.Wrapper.Canvas.UpdateTacticalLabels(componentOrNull.MarineLabels);
    }
    else
    {
      this.Window.Wrapper.Map.UpdateTacticalLabels(new Dictionary<Vector2i, string>());
      if (this._refreshed)
        return;
      this.Window.Wrapper.Canvas.UpdateTacticalLabels(new Dictionary<Vector2i, string>());
    }
  }
}
