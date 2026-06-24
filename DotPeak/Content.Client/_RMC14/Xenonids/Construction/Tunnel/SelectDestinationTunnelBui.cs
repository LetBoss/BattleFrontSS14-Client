// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Construction.Tunnel.SelectDestinationTunnelBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.TacticalMap;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.TacticalMap;
using Content.Shared._RMC14.Xenonids.Construction.Tunnel;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Construction.Tunnel;

public sealed class SelectDestinationTunnelBui : BoundUserInterface
{
  [Dependency]
  private IPlayerManager _player;
  private SelectDestinationTunnelWindow? _window;
  private NetEntity? _selectedTunnel;
  private Dictionary<string, NetEntity> _availableTunnels = new Dictionary<string, NetEntity>();
  private int? _currentTunnelNetEntityKey;
  private bool _showOnlyTunnels = true;
  private TunnelPathfindingConfig _pathfindingConfig = TunnelPathfindingConfig.Default;
  private readonly Dictionary<int, TunnelCacheEntry> _tunnelCache = new Dictionary<int, TunnelCacheEntry>();
  private readonly Dictionary<Vector2i, int> _positionToEntityCache = new Dictionary<Vector2i, int>();
  private readonly Dictionary<(Vector2i, Vector2i), double> _distanceCache = new Dictionary<(Vector2i, Vector2i), double>();
  private readonly Dictionary<(Vector2i, Vector2i), List<Vector2i>> _pathCache = new Dictionary<(Vector2i, Vector2i), List<Vector2i>>();
  private readonly List<TacticalMapBlip> _reusableBlipsList = new List<TacticalMapBlip>();
  private bool _cacheValid;
  private Vector2i? _cachedCurrentPos;
  private Vector2i? _cachedSelectedPos;

  public SelectDestinationTunnelBui(EntityUid ent, Enum key)
    : base(ent, key)
  {
    IoCManager.InjectDependencies<SelectDestinationTunnelBui>(this);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    if (!(state is SelectDestinationTunnelInterfaceState state1))
      return;
    this.Refresh(state1);
  }

  private void Refresh(SelectDestinationTunnelInterfaceState state)
  {
    NetEntity? selectedTunnel = this._selectedTunnel;
    int num = !this._availableTunnels.SequenceEqual<KeyValuePair<string, NetEntity>>((IEnumerable<KeyValuePair<string, NetEntity>>) state.HiveTunnels) ? 1 : 0;
    this._availableTunnels = state.HiveTunnels;
    if (num != 0)
      this.InvalidateCache();
    if (this._window == null)
    {
      this.Close();
    }
    else
    {
      this._window.SelectableTunnels.Clear();
      this.UpdateTunnelList(state);
      this.UpdateSelectedTunnel(selectedTunnel);
      this.UpdateTacticalMapDisplay();
      this.UpdateBlips();
    }
  }

  private void InvalidateCache()
  {
    this._tunnelCache.Clear();
    this._positionToEntityCache.Clear();
    this._distanceCache.Clear();
    this._pathCache.Clear();
    this._cacheValid = false;
    this._cachedCurrentPos = new Vector2i?();
    this._cachedSelectedPos = new Vector2i?();
  }

  private void UpdateTunnelList(SelectDestinationTunnelInterfaceState newState)
  {
    if (this._window == null)
      return;
    this._currentTunnelNetEntityKey = new int?();
    string currentTunnelName = (string) null;
    foreach (KeyValuePair<string, NetEntity> hiveTunnel in newState.HiveTunnels)
    {
      if (EntityUid.op_Equality(this.EntMan.GetEntity(hiveTunnel.Value), this.Owner))
      {
        currentTunnelName = hiveTunnel.Key;
        this._currentTunnelNetEntityKey = new int?(NetEntity.op_Explicit(hiveTunnel.Value));
      }
      else
        this._window.SelectableTunnels.Add(new ItemList.Item(this._window.SelectableTunnels)
        {
          Text = hiveTunnel.Key,
          Metadata = (object) hiveTunnel.Value
        });
    }
    this._window.UpdateCurrentTunnelDisplay(currentTunnelName);
  }

  private void UpdateSelectedTunnel(NetEntity? previouslySelectedTunnel)
  {
    if (this._window == null)
      return;
    if (previouslySelectedTunnel.HasValue && this._availableTunnels.ContainsValue(previouslySelectedTunnel.Value))
    {
      this._selectedTunnel = previouslySelectedTunnel;
      ((BaseButton) this._window.SelectButton).Disabled = false;
      this._window.UpdateSelectedTunnelDisplay(this.GetTunnelNameCached(this._selectedTunnel.Value));
    }
    else
    {
      this._selectedTunnel = new NetEntity?();
      ((BaseButton) this._window.SelectButton).Disabled = true;
      this._window.UpdateSelectedTunnelDisplay((string) null);
    }
  }

  private string? GetTunnelNameCached(NetEntity tunnel)
  {
    TunnelCacheEntry tunnelCacheEntry;
    return !this._tunnelCache.TryGetValue(NetEntity.op_Explicit(tunnel), out tunnelCacheEntry) ? this.GetTunnelName(tunnel) : tunnelCacheEntry.Name;
  }

  private string? GetTunnelName(NetEntity tunnel)
  {
    foreach (KeyValuePair<string, NetEntity> availableTunnel in this._availableTunnels)
    {
      if (NetEntity.op_Equality(availableTunnel.Value, tunnel))
        return availableTunnel.Key;
    }
    return (string) null;
  }

  private void UpdateTacticalMapDisplay()
  {
    if (this._window == null)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    TacticalMapUserComponent mapUserComponent;
    AreaGridComponent areaGridComponent;
    if (!localEntity.HasValue || !this.EntMan.TryGetComponent<TacticalMapUserComponent>(localEntity.GetValueOrDefault(), ref mapUserComponent) || !this.EntMan.TryGetComponent<AreaGridComponent>(mapUserComponent.Map, ref areaGridComponent))
      return;
    this._window.TacticalMapWrapper.UpdateTexture(Entity<AreaGridComponent>.op_Implicit((mapUserComponent.Map.Value, areaGridComponent)));
  }

  private void BuildTunnelCache(TacticalMapUserComponent user)
  {
    if (this._cacheValid)
      return;
    this._tunnelCache.Clear();
    this._positionToEntityCache.Clear();
    Dictionary<int, TacticalMapBlip>[] dictionaryArray = new Dictionary<int, TacticalMapBlip>[3]
    {
      user.XenoStructureBlips,
      user.XenoBlips,
      user.MarineBlips
    };
    foreach (Dictionary<int, TacticalMapBlip> dictionary in dictionaryArray)
    {
      foreach (KeyValuePair<int, TacticalMapBlip> keyValuePair in dictionary)
      {
        int key = keyValuePair.Key;
        TacticalMapBlip tacticalMapBlip = keyValuePair.Value;
        string tunnelNameByEntityId = this.GetTunnelNameByEntityId(key);
        if (tunnelNameByEntityId != null && this._availableTunnels.ContainsKey(tunnelNameByEntityId))
        {
          TunnelCacheEntry tunnelCacheEntry = new TunnelCacheEntry()
          {
            Position = tacticalMapBlip.Indices,
            Name = tunnelNameByEntityId,
            Entity = this._availableTunnels[tunnelNameByEntityId],
            EntityId = key
          };
          this._tunnelCache[key] = tunnelCacheEntry;
          this._positionToEntityCache[tacticalMapBlip.Indices] = key;
        }
      }
    }
    this._cacheValid = true;
  }

  private void UpdateBlips()
  {
    if (this._window != null)
    {
      EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
      TacticalMapUserComponent user;
      if (localEntity.HasValue && this.EntMan.TryGetComponent<TacticalMapUserComponent>(localEntity.GetValueOrDefault(), ref user))
      {
        this.BuildTunnelCache(user);
        this._reusableBlipsList.Clear();
        (Vector2i? nullable1, Vector2i? nullable2) = this.ProcessBlipCollections(user, this._selectedTunnel.HasValue ? new int?(NetEntity.op_Explicit(this._selectedTunnel.Value)) : new int?(), this._reusableBlipsList);
        this._window.TacticalMapWrapper.UpdateBlips(this._reusableBlipsList.ToArray());
        Vector2i? nullable3 = nullable1;
        Vector2i? nullable4 = this._cachedCurrentPos;
        if ((nullable3.HasValue == nullable4.HasValue ? (nullable3.HasValue ? (Vector2i.op_Inequality(nullable3.GetValueOrDefault(), nullable4.GetValueOrDefault()) ? 1 : 0) : 0) : 1) == 0)
        {
          nullable4 = nullable2;
          Vector2i? cachedSelectedPos = this._cachedSelectedPos;
          if ((nullable4.HasValue == cachedSelectedPos.HasValue ? (nullable4.HasValue ? (Vector2i.op_Inequality(nullable4.GetValueOrDefault(), cachedSelectedPos.GetValueOrDefault()) ? 1 : 0) : 0) : 1) == 0)
            return;
        }
        this.UpdateDirectionalArrow(nullable1, nullable2);
        this._cachedCurrentPos = nullable1;
        this._cachedSelectedPos = nullable2;
        return;
      }
    }
    this._window?.TacticalMapWrapper.UpdateBlips((TacticalMapBlip[]) null);
  }

  private void GetTunnelEntityIds(int? selectedTunnelKey, HashSet<int> output)
  {
    if (this._currentTunnelNetEntityKey.HasValue)
      output.Add(this._currentTunnelNetEntityKey.Value);
    foreach (NetEntity netEntity in this._availableTunnels.Values)
      output.Add(NetEntity.op_Explicit(netEntity));
    if (!selectedTunnelKey.HasValue)
      return;
    output.Add(selectedTunnelKey.Value);
  }

  private (Vector2i? currentPos, Vector2i? selectedPos) ProcessBlipCollections(
    TacticalMapUserComponent user,
    int? selectedTunnelKey,
    List<TacticalMapBlip> blipsList)
  {
    Vector2i? nullable1 = new Vector2i?();
    Vector2i? nullable2 = new Vector2i?();
    HashSet<int> intSet = new HashSet<int>(this._availableTunnels.Values.Select<NetEntity, int>((Func<NetEntity, int>) (t => NetEntity.op_Explicit(t))));
    if (this._currentTunnelNetEntityKey.HasValue)
      intSet.Add(this._currentTunnelNetEntityKey.Value);
    if (selectedTunnelKey.HasValue)
      intSet.Add(selectedTunnelKey.Value);
    Dictionary<int, TacticalMapBlip>[] dictionaryArray = new Dictionary<int, TacticalMapBlip>[3]
    {
      user.XenoStructureBlips,
      user.XenoBlips,
      user.MarineBlips
    };
    foreach (Dictionary<int, TacticalMapBlip> dictionary in dictionaryArray)
    {
      foreach ((int key, TacticalMapBlip tacticalMapBlip) in dictionary)
      {
        int entityId = key;
        TacticalMapBlip blip = tacticalMapBlip;
        if (!this._showOnlyTunnels || intSet.Contains(entityId))
        {
          blipsList.Add(this.HighlightBlip(blip, entityId, selectedTunnelKey));
          int? tunnelNetEntityKey = this._currentTunnelNetEntityKey;
          key = entityId;
          if (tunnelNetEntityKey.GetValueOrDefault() == key & tunnelNetEntityKey.HasValue && !nullable1.HasValue)
            nullable1 = new Vector2i?(blip.Indices);
          int? nullable3 = selectedTunnelKey;
          key = entityId;
          if (nullable3.GetValueOrDefault() == key & nullable3.HasValue && !nullable2.HasValue)
            nullable2 = new Vector2i?(blip.Indices);
        }
      }
    }
    return (nullable1, nullable2);
  }

  private List<Vector2i> GetAllTunnelPositions()
  {
    return this._tunnelCache.Values.Select<TunnelCacheEntry, Vector2i>((Func<TunnelCacheEntry, Vector2i>) (entry => entry.Position)).ToList<Vector2i>();
  }

  private List<Vector2i> FindTunnelPath(Vector2i startPos, Vector2i endPos)
  {
    (Vector2i, Vector2i) key = (startPos, endPos);
    List<Vector2i> tunnelPath1;
    if (this._pathCache.TryGetValue(key, out tunnelPath1))
      return tunnelPath1;
    List<Vector2i> allTunnelPositions = this.GetAllTunnelPositions();
    List<Vector2i> tunnelsOnDirectLine = this.GetTunnelsOnDirectLine(startPos, endPos, allTunnelPositions);
    List<Vector2i> tunnelPath2 = this.BuildOptimalPath(startPos, endPos, tunnelsOnDirectLine);
    this._pathCache[key] = tunnelPath2;
    return tunnelPath2;
  }

  private List<Vector2i> GetTunnelsOnDirectLine(
    Vector2i start,
    Vector2i end,
    List<Vector2i> allPositions)
  {
    return allPositions.Where<Vector2i>((Func<Vector2i, bool>) (pos => Vector2i.op_Inequality(pos, start) && Vector2i.op_Inequality(pos, end))).Where<Vector2i>((Func<Vector2i, bool>) (pos => this.IsDirectlyOnPath(start, end, pos, 30.0))).OrderBy<Vector2i, double>((Func<Vector2i, double>) (pos => this.CalculateProgressAlongPath(start, end, pos))).ToList<Vector2i>();
  }

  private bool IsDirectlyOnPath(Vector2i start, Vector2i end, Vector2i tunnel, double maxDistance)
  {
    if (this.CalculateDistanceFromLine(start, end, tunnel) > maxDistance)
      return false;
    double num1 = 50.0;
    double distance1 = this.CalculateDistance(start, tunnel);
    double distance2 = this.CalculateDistance(end, tunnel);
    double num2 = num1;
    if (distance1 < num2 || distance2 < num1)
      return false;
    double progressAlongPath = this.CalculateProgressAlongPath(start, end, tunnel);
    return progressAlongPath >= 0.15 && progressAlongPath <= 0.85;
  }

  private List<Vector2i> BuildOptimalPath(
    Vector2i start,
    Vector2i end,
    List<Vector2i> tunnelsOnLine)
  {
    List<Vector2i> path = new List<Vector2i>() { start };
    int num = 0;
    if (this._pathfindingConfig.MaxIntermediateTunnels <= 0)
    {
      path.Add(end);
      return path;
    }
    foreach (var data in tunnelsOnLine.Select(tunnel => new
    {
      Position = tunnel,
      Cost = this.CalculateTunnelCost(path.Last<Vector2i>(), tunnel, end)
    }).OrderBy(t => t.Cost).ToList())
    {
      if (num < this._pathfindingConfig.MaxIntermediateTunnels)
      {
        if (this.CalculateDistance(path.Last<Vector2i>(), data.Position) <= this._pathfindingConfig.MaxConnectionDistance)
        {
          path.Add(data.Position);
          ++num;
        }
      }
      else
        break;
    }
    path.Add(end);
    return path;
  }

  private double CalculateTunnelCost(Vector2i current, Vector2i tunnel, Vector2i destination)
  {
    double num1 = this.CalculateDistance(current, tunnel) * this._pathfindingConfig.DirectDistanceWeight;
    double num2 = this.CalculateDistance(tunnel, destination) * this._pathfindingConfig.DirectDistanceWeight;
    double tunnelHopPenalty = this._pathfindingConfig.TunnelHopPenalty;
    double distance = this.CalculateDistance(current, destination);
    double num3 = num2;
    double num4 = num1 + num3;
    double num5 = Math.Max(0.0, distance - num4);
    return num4 + tunnelHopPenalty - num5;
  }

  private double CalculateDistanceFromLine(Vector2i start, Vector2i end, Vector2i point)
  {
    Vector2 vector2_1 = new Vector2((float) (end.X - start.X), (float) (end.Y - start.Y));
    Vector2 vector2_2 = new Vector2((float) (point.X - start.X), (float) (point.Y - start.Y));
    if ((double) vector2_1.Length() < 0.001)
      return (double) Vector2.Distance(new Vector2((float) start.X, (float) start.Y), new Vector2((float) point.X, (float) point.Y));
    Vector2 vector2_3 = Vector2.Normalize(vector2_1);
    float num = Vector2.Dot(vector2_2, vector2_3);
    Vector2 vector2_4 = vector2_3 * num;
    return (double) Vector2.Distance(vector2_2, vector2_4);
  }

  private double CalculateProgressAlongPath(Vector2i start, Vector2i end, Vector2i point)
  {
    Vector2 vector2_1 = new Vector2((float) (end.X - start.X), (float) (end.Y - start.Y));
    Vector2 vector2_2 = new Vector2((float) (point.X - start.X), (float) (point.Y - start.Y));
    float num = vector2_1.LengthSquared();
    return (double) num < 0.001 ? 0.0 : (double) Vector2.Dot(vector2_2, vector2_1) / (double) num;
  }

  private double CalculateDistance(Vector2i pos1, Vector2i pos2)
  {
    (Vector2i, Vector2i) key = (pos1, pos2);
    double distance1;
    if (this._distanceCache.TryGetValue(key, out distance1))
      return distance1;
    double distance2 = Math.Sqrt(Math.Pow((double) (pos2.X - pos1.X), 2.0) + Math.Pow((double) (pos2.Y - pos1.Y), 2.0));
    this._distanceCache[key] = distance2;
    return distance2;
  }

  private string? GetTunnelNameByEntityId(int entityId)
  {
    TunnelCacheEntry tunnelCacheEntry;
    if (this._tunnelCache.TryGetValue(entityId, out tunnelCacheEntry))
      return tunnelCacheEntry.Name;
    foreach (KeyValuePair<string, NetEntity> availableTunnel in this._availableTunnels)
    {
      if (NetEntity.op_Explicit(availableTunnel.Value) == entityId)
        return availableTunnel.Key;
    }
    return (string) null;
  }

  private void UpdateDirectionalArrow(Vector2i? currentPosition, Vector2i? selectedPosition)
  {
    if (this._window == null)
      return;
    this._window.TacticalMapWrapper.Map.RemoveLinesByColor(Color.FromHex((ReadOnlySpan<char>) "#F535AA", new Color?()));
    if (!currentPosition.HasValue || !selectedPosition.HasValue)
      return;
    Vector2i? nullable1 = currentPosition;
    Vector2i? nullable2 = selectedPosition;
    if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (Vector2i.op_Inequality(nullable1.GetValueOrDefault(), nullable2.GetValueOrDefault()) ? 1 : 0) : 0) : 1) == 0)
      return;
    this._window.TacticalMapWrapper.Map.AddDashedTunnelPathClosest(this.FindTunnelPath(currentPosition.Value, selectedPosition.Value), Color.FromHex((ReadOnlySpan<char>) "#F535AA", new Color?()));
  }

  private TacticalMapBlip HighlightBlip(TacticalMapBlip blip, int entityId, int? selectedTunnelKey)
  {
    if (this._currentTunnelNetEntityKey.HasValue && entityId == this._currentTunnelNetEntityKey.Value)
      return blip with
      {
        Background = new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/map_blips.rsi"), "background"),
        HiveLeader = true
      };
    if (!selectedTunnelKey.HasValue || entityId != selectedTunnelKey.Value)
      return blip;
    return blip with
    {
      Background = new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/map_blips.rsi"), "background"),
      HiveLeader = true
    };
  }

  private void OnBlipClicked(Vector2i clickedIndices)
  {
    if (this._window == null)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    TacticalMapUserComponent user;
    if (!localEntity.HasValue || !this.EntMan.TryGetComponent<TacticalMapUserComponent>(localEntity.GetValueOrDefault(), ref user))
      return;
    int num;
    int? nullable = this._positionToEntityCache.TryGetValue(clickedIndices, out num) ? new int?(num) : this.FindEntityIdAtIndices(clickedIndices, user);
    if (!nullable.HasValue)
      return;
    string tunnelNameByEntityId = this.GetTunnelNameByEntityId(nullable.Value);
    NetEntity netEntity;
    if (tunnelNameByEntityId == null || !this._availableTunnels.TryGetValue(tunnelNameByEntityId, out netEntity) || this._currentTunnelNetEntityKey.HasValue && nullable.Value == this._currentTunnelNetEntityKey.Value)
      return;
    this._selectedTunnel = new NetEntity?(netEntity);
    ((BaseButton) this._window.SelectButton).Disabled = false;
    this._window.UpdateSelectedTunnelDisplay(tunnelNameByEntityId);
    this.UpdateBlips();
  }

  private void OnBlipRightClicked(Vector2i clickedIndices, string _)
  {
    if (this._window == null)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    TacticalMapUserComponent user;
    if (!localEntity.HasValue || !this.EntMan.TryGetComponent<TacticalMapUserComponent>(localEntity.GetValueOrDefault(), ref user))
      return;
    int num;
    int? nullable = this._positionToEntityCache.TryGetValue(clickedIndices, out num) ? new int?(num) : this.FindEntityIdAtIndices(clickedIndices, user);
    if (!nullable.HasValue)
      return;
    string tunnelNameByEntityId = this.GetTunnelNameByEntityId(nullable.Value);
    if (tunnelNameByEntityId == null || !this._availableTunnels.ContainsKey(tunnelNameByEntityId))
      return;
    Vector2 position = this._window.TacticalMapWrapper.Map.IndicesToPosition(clickedIndices);
    this._window.TacticalMapWrapper.Map.ShowTunnelInfo(clickedIndices, tunnelNameByEntityId, position);
    this._window.TacticalMapWrapper.Canvas.ShowTunnelInfo(clickedIndices, tunnelNameByEntityId, position);
  }

  private int? FindEntityIdAtIndices(Vector2i indices, TacticalMapUserComponent user)
  {
    foreach (KeyValuePair<int, TacticalMapBlip> xenoStructureBlip in user.XenoStructureBlips)
    {
      if (Vector2i.op_Equality(xenoStructureBlip.Value.Indices, indices))
        return new int?(xenoStructureBlip.Key);
    }
    foreach (KeyValuePair<int, TacticalMapBlip> xenoBlip in user.XenoBlips)
    {
      if (Vector2i.op_Equality(xenoBlip.Value.Indices, indices))
        return new int?(xenoBlip.Key);
    }
    foreach (KeyValuePair<int, TacticalMapBlip> marineBlip in user.MarineBlips)
    {
      if (Vector2i.op_Equality(marineBlip.Value.Indices, indices))
        return new int?(marineBlip.Key);
    }
    return new int?();
  }

  public void ConfigurePathfinding(TunnelPathfindingConfig config)
  {
    this._pathfindingConfig = config;
    this._distanceCache.Clear();
    this._pathCache.Clear();
  }

  protected virtual void Open()
  {
    base.Open();
    SelectDestinationTunnelWindow window = this._window;
    if (window != null && ((BaseWindow) window).IsOpen)
      return;
    this._window = BoundUserInterfaceExt.CreateWindow<SelectDestinationTunnelWindow>((BoundUserInterface) this);
    ((BaseButton) this._window.SelectButton).Disabled = true;
    this._window.SetBlipUpdateCallback((Action) (() => this.UpdateBlips()));
    TacticalMapWrapper tacticalMapWrapper = this._window.TacticalMapWrapper;
    TabContainer.SetTabVisible((Control) tacticalMapWrapper.CanvasTab, false);
    tacticalMapWrapper.Tabs.CurrentTab = 0;
    ((Control) tacticalMapWrapper.Map).MouseFilter = (Control.MouseFilterMode) 0;
    ((Control) tacticalMapWrapper.Canvas).MouseFilter = (Control.MouseFilterMode) 0;
    ((BaseButton) this._window.ShowOnlyTunnelsCheckbox).OnPressed += (Action<BaseButton.ButtonEventArgs>) (args =>
    {
      this._showOnlyTunnels = args.Button.Pressed;
      this.UpdateBlips();
    });
    this._window.SelectableTunnels.OnItemSelected += (Action<ItemList.ItemListSelectedEventArgs>) (args =>
    {
      ((BaseButton) this._window.SelectButton).Disabled = false;
      this._selectedTunnel = new NetEntity?((NetEntity) ((ItemList.ItemListEventArgs) args).ItemList[args.ItemIndex].Metadata);
      this._window.UpdateSelectedTunnelDisplay(this.GetTunnelNameCached(this._selectedTunnel.Value));
      this.UpdateBlips();
    });
    ((BaseButton) this._window.SelectButton).OnButtonDown += (Action<BaseButton.ButtonEventArgs>) (args =>
    {
      if (!this._selectedTunnel.HasValue)
      {
        args.Button.Disabled = true;
      }
      else
      {
        this.GoToTunnel(this._selectedTunnel.Value);
        this.Close();
      }
    });
    this._window.TacticalMapWrapper.Map.OnBlipClicked = new Action<Vector2i>(this.OnBlipClicked);
    this._window.TacticalMapWrapper.Canvas.OnBlipClicked = new Action<Vector2i>(this.OnBlipClicked);
    this._window.TacticalMapWrapper.Map.OnBlipRightClicked = new Action<Vector2i, string>(this.OnBlipRightClicked);
    this._window.TacticalMapWrapper.Canvas.OnBlipRightClicked = new Action<Vector2i, string>(this.OnBlipRightClicked);
  }

  private void GoToTunnel(NetEntity destinationTunnel)
  {
    this.SendMessage((BoundUserInterfaceMessage) new TraverseXenoTunnelMessage(destinationTunnel));
  }
}
