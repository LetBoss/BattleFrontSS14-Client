// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.TacticalMap.TacticalMapUserBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.UserInterface;
using Content.Shared._RMC14.Areas;
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

public sealed class TacticalMapUserBui(EntityUid owner, Enum uiKey) : RMCPopOutBui<TacticalMapWindow>(owner, uiKey)
{
  [Dependency]
  private IPlayerManager _player;
  private static readonly ISawmill _logger = Logger.GetSawmill("tactical_map_settings");
  private bool _refreshed;
  private string? _currentMapName;

  protected override TacticalMapWindow? Window { get; set; }

  protected virtual void Open()
  {
    base.Open();
    EntityUid? nullable = new EntityUid?();
    TacticalMapUserComponent mapUserComponent;
    if (this.EntMan.TryGetComponent<TacticalMapUserComponent>(this.Owner, ref mapUserComponent) && mapUserComponent.Map.HasValue)
      nullable = new EntityUid?(mapUserComponent.Map.Value);
    this.Window = this.CreatePopOutableWindow<TacticalMapWindow>();
    if (nullable.HasValue)
      this.Window.SetMapEntity(this._currentMapName);
    TabContainer.SetTabTitle((Control) this.Window.Wrapper.MapTab, Loc.GetString("ui-tactical-map-tab-map"));
    TabContainer.SetTabVisible((Control) this.Window.Wrapper.MapTab, true);
    if (nullable.HasValue)
      this.Window.Wrapper.SetMapEntity(this._currentMapName);
    AreaGridComponent areaGridComponent;
    if (mapUserComponent != null && mapUserComponent.Map.HasValue && this.EntMan.TryGetComponent<AreaGridComponent>(mapUserComponent.Map.Value, ref areaGridComponent))
      this.Window.Wrapper.UpdateTexture(Entity<AreaGridComponent>.op_Implicit((mapUserComponent.Map.Value, areaGridComponent)));
    try
    {
      TacticalMapSettings settings = IoCManager.Resolve<TacticalMapSettingsManager>().LoadSettings(this._currentMapName);
      if (this._currentMapName != null)
        this.Window.Wrapper.LoadMapSpecificSettings(settings, this._currentMapName);
    }
    catch (Exception ex)
    {
      TacticalMapUserBui._logger.Error($"Failed to load tactical map user settings for map '{this._currentMapName}': {ex}");
    }
    this.Refresh();
    this.Window.Wrapper.SetupUpdateButton((Action<TacticalMapUpdateCanvasMsg>) (msg => this.SendPredictedMessage((BoundUserInterfaceMessage) msg)));
    this.Window.Wrapper.Map.OnQueenEyeMove += (Action<Vector2i>) (position => this.SendPredictedMessage((BoundUserInterfaceMessage) new TacticalMapQueenEyeMoveMsg(position)));
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
          TacticalMapUserBui._logger.Error($"Failed to save tactical map user settings during disposal for map '{this._currentMapName}': {ex}");
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
    this.UpdateTimestamps();
    this.Window.Wrapper.Map.Lines.Clear();
    TacticalMapLinesComponent componentOrNull1 = EntityManagerExt.GetComponentOrNull<TacticalMapLinesComponent>(this.EntMan, this.Owner);
    if (componentOrNull1 != null)
    {
      this.Window.Wrapper.Map.Lines.AddRange((IEnumerable<TacticalMapLine>) componentOrNull1.MarineLines);
      this.Window.Wrapper.Map.Lines.AddRange((IEnumerable<TacticalMapLine>) componentOrNull1.XenoLines);
    }
    if (this._refreshed)
      return;
    this.Window.Wrapper.Canvas.Lines.Clear();
    if (componentOrNull1 != null)
    {
      this.Window.Wrapper.Canvas.Lines.AddRange((IEnumerable<TacticalMapLine>) componentOrNull1.MarineLines);
      this.Window.Wrapper.Canvas.Lines.AddRange((IEnumerable<TacticalMapLine>) componentOrNull1.XenoLines);
    }
    TacticalMapUserComponent componentOrNull2 = EntityManagerExt.GetComponentOrNull<TacticalMapUserComponent>(this.EntMan, this.Owner);
    if ((componentOrNull2 != null ? (componentOrNull2.CanDraw ? 1 : 0) : 0) != 0)
    {
      TabContainer.SetTabTitle((Control) this.Window.Wrapper.CanvasTab, Loc.GetString("ui-tactical-map-tab-canvas"));
      TabContainer.SetTabVisible((Control) this.Window.Wrapper.CanvasTab, true);
    }
    else
      TabContainer.SetTabVisible((Control) this.Window.Wrapper.CanvasTab, false);
    this._refreshed = true;
  }

  private void UpdateBlips()
  {
    if (this.Window == null)
      return;
    TacticalMapUserComponent mapUserComponent;
    if (!this.EntMan.TryGetComponent<TacticalMapUserComponent>(this.Owner, ref mapUserComponent))
    {
      this.Window.Wrapper.UpdateBlips((TacticalMapBlip[]) null);
    }
    else
    {
      int length = mapUserComponent.MarineBlips.Count + mapUserComponent.XenoBlips.Count + mapUserComponent.XenoStructureBlips.Count;
      TacticalMapBlip[] blips = new TacticalMapBlip[length];
      int[] entityIds = new int[length];
      int index = 0;
      foreach ((int key, TacticalMapBlip tacticalMapBlip5) in mapUserComponent.MarineBlips)
      {
        int num = key;
        TacticalMapBlip tacticalMapBlip2 = tacticalMapBlip5;
        blips[index] = tacticalMapBlip2;
        entityIds[index] = num;
        ++index;
      }
      foreach ((key, tacticalMapBlip5) in mapUserComponent.XenoBlips)
      {
        int num = key;
        TacticalMapBlip tacticalMapBlip4 = tacticalMapBlip5;
        blips[index] = tacticalMapBlip4;
        entityIds[index] = num;
        ++index;
      }
      foreach ((key, tacticalMapBlip5) in mapUserComponent.XenoStructureBlips)
      {
        int num = key;
        TacticalMapBlip tacticalMapBlip6 = tacticalMapBlip5;
        blips[index] = tacticalMapBlip6;
        entityIds[index] = num;
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
      Dictionary<Vector2i, string> labels = new Dictionary<Vector2i, string>();
      foreach (KeyValuePair<Vector2i, string> marineLabel in componentOrNull.MarineLabels)
        labels[marineLabel.Key] = marineLabel.Value;
      foreach (KeyValuePair<Vector2i, string> xenoLabel in componentOrNull.XenoLabels)
        labels[xenoLabel.Key] = xenoLabel.Value;
      this.Window.Wrapper.Map.UpdateTacticalLabels(labels);
    }
    else
      this.Window.Wrapper.Map.UpdateTacticalLabels(new Dictionary<Vector2i, string>());
  }

  private void UpdateTimestamps()
  {
    TacticalMapUserComponent mapUserComponent;
    if (this.Window == null || !this.EntMan.TryGetComponent<TacticalMapUserComponent>(this.Owner, ref mapUserComponent))
      return;
    this.Window.Wrapper.LastUpdateAt = mapUserComponent.LastAnnounceAt;
    this.Window.Wrapper.NextUpdateAt = mapUserComponent.NextAnnounceAt;
  }
}
