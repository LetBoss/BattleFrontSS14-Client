using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<SelectDestinationTunnelBui>(this);
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		if (state is SelectDestinationTunnelInterfaceState state2)
		{
			Refresh(state2);
		}
	}

	private void Refresh(SelectDestinationTunnelInterfaceState state)
	{
		NetEntity? selectedTunnel = _selectedTunnel;
		bool num = !_availableTunnels.SequenceEqual(state.HiveTunnels);
		_availableTunnels = state.HiveTunnels;
		if (num)
		{
			InvalidateCache();
		}
		if (_window == null)
		{
			((BoundUserInterface)this).Close();
			return;
		}
		_window.SelectableTunnels.Clear();
		UpdateTunnelList(state);
		UpdateSelectedTunnel(selectedTunnel);
		UpdateTacticalMapDisplay();
		UpdateBlips();
	}

	private void InvalidateCache()
	{
		_tunnelCache.Clear();
		_positionToEntityCache.Clear();
		_distanceCache.Clear();
		_pathCache.Clear();
		_cacheValid = false;
		_cachedCurrentPos = null;
		_cachedSelectedPos = null;
	}

	private void UpdateTunnelList(SelectDestinationTunnelInterfaceState newState)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		_currentTunnelNetEntityKey = null;
		string currentTunnelName = null;
		foreach (KeyValuePair<string, NetEntity> hiveTunnel in newState.HiveTunnels)
		{
			if (base.EntMan.GetEntity(hiveTunnel.Value) == ((BoundUserInterface)this).Owner)
			{
				currentTunnelName = hiveTunnel.Key;
				_currentTunnelNetEntityKey = (int)hiveTunnel.Value;
			}
			else
			{
				_window.SelectableTunnels.Add(new Item(_window.SelectableTunnels)
				{
					Text = hiveTunnel.Key,
					Metadata = hiveTunnel.Value
				});
			}
		}
		_window.UpdateCurrentTunnelDisplay(currentTunnelName);
	}

	private void UpdateSelectedTunnel(NetEntity? previouslySelectedTunnel)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (_window != null)
		{
			if (previouslySelectedTunnel.HasValue && _availableTunnels.ContainsValue(previouslySelectedTunnel.Value))
			{
				_selectedTunnel = previouslySelectedTunnel;
				((BaseButton)_window.SelectButton).Disabled = false;
				_window.UpdateSelectedTunnelDisplay(GetTunnelNameCached(_selectedTunnel.Value));
			}
			else
			{
				_selectedTunnel = null;
				((BaseButton)_window.SelectButton).Disabled = true;
				_window.UpdateSelectedTunnelDisplay(null);
			}
		}
	}

	private string? GetTunnelNameCached(NetEntity tunnel)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		int key = (int)tunnel;
		if (!_tunnelCache.TryGetValue(key, out var value))
		{
			return GetTunnelName(tunnel);
		}
		return value.Name;
	}

	private string? GetTunnelName(NetEntity tunnel)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<string, NetEntity> availableTunnel in _availableTunnels)
		{
			if (availableTunnel.Value == tunnel)
			{
				return availableTunnel.Key;
			}
		}
		return null;
	}

	private void UpdateTacticalMapDisplay()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			TacticalMapUserComponent tacticalMapUserComponent = default(TacticalMapUserComponent);
			AreaGridComponent item = default(AreaGridComponent);
			if (base.EntMan.TryGetComponent<TacticalMapUserComponent>(valueOrDefault, ref tacticalMapUserComponent) && base.EntMan.TryGetComponent<AreaGridComponent>(tacticalMapUserComponent.Map, ref item))
			{
				_window.TacticalMapWrapper.UpdateTexture(Entity<AreaGridComponent>.op_Implicit((tacticalMapUserComponent.Map.Value, item)));
			}
		}
	}

	private void BuildTunnelCache(TacticalMapUserComponent user)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if (_cacheValid)
		{
			return;
		}
		_tunnelCache.Clear();
		_positionToEntityCache.Clear();
		Dictionary<int, TacticalMapBlip>[] array = new Dictionary<int, TacticalMapBlip>[3] { user.XenoStructureBlips, user.XenoBlips, user.MarineBlips };
		for (int i = 0; i < array.Length; i++)
		{
			foreach (KeyValuePair<int, TacticalMapBlip> item in array[i])
			{
				int key = item.Key;
				TacticalMapBlip value = item.Value;
				string tunnelNameByEntityId = GetTunnelNameByEntityId(key);
				if (tunnelNameByEntityId != null && _availableTunnels.ContainsKey(tunnelNameByEntityId))
				{
					TunnelCacheEntry value2 = new TunnelCacheEntry
					{
						Position = value.Indices,
						Name = tunnelNameByEntityId,
						Entity = _availableTunnels[tunnelNameByEntityId],
						EntityId = key
					};
					_tunnelCache[key] = value2;
					_positionToEntityCache[value.Indices] = key;
				}
			}
		}
		_cacheValid = true;
	}

	private void UpdateBlips()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		if (_window != null)
		{
			EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
			if (localEntity.HasValue)
			{
				EntityUid valueOrDefault = localEntity.GetValueOrDefault();
				TacticalMapUserComponent user = default(TacticalMapUserComponent);
				if (base.EntMan.TryGetComponent<TacticalMapUserComponent>(valueOrDefault, ref user))
				{
					BuildTunnelCache(user);
					_reusableBlipsList.Clear();
					var (val, val2) = ProcessBlipCollections(user, _selectedTunnel.HasValue ? new int?((int)_selectedTunnel.Value) : ((int?)null), _reusableBlipsList);
					_window.TacticalMapWrapper.UpdateBlips(_reusableBlipsList.ToArray());
					Vector2i? val3 = val;
					Vector2i? cachedCurrentPos = _cachedCurrentPos;
					if (val3.HasValue == cachedCurrentPos.HasValue && (!val3.HasValue || !(val3.GetValueOrDefault() != cachedCurrentPos.GetValueOrDefault())))
					{
						cachedCurrentPos = val2;
						val3 = _cachedSelectedPos;
						if (cachedCurrentPos.HasValue == val3.HasValue && (!cachedCurrentPos.HasValue || !(cachedCurrentPos.GetValueOrDefault() != val3.GetValueOrDefault())))
						{
							return;
						}
					}
					UpdateDirectionalArrow(val, val2);
					_cachedCurrentPos = val;
					_cachedSelectedPos = val2;
					return;
				}
			}
		}
		_window?.TacticalMapWrapper.UpdateBlips(null);
	}

	private void GetTunnelEntityIds(int? selectedTunnelKey, HashSet<int> output)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (_currentTunnelNetEntityKey.HasValue)
		{
			output.Add(_currentTunnelNetEntityKey.Value);
		}
		foreach (NetEntity value in _availableTunnels.Values)
		{
			output.Add((int)value);
		}
		if (selectedTunnelKey.HasValue)
		{
			output.Add(selectedTunnelKey.Value);
		}
	}

	private (Vector2i? currentPos, Vector2i? selectedPos) ProcessBlipCollections(TacticalMapUserComponent user, int? selectedTunnelKey, List<TacticalMapBlip> blipsList)
	{
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		Vector2i? item = null;
		Vector2i? item2 = null;
		HashSet<int> hashSet = new HashSet<int>(_availableTunnels.Values.Select((NetEntity t) => (int)t));
		if (_currentTunnelNetEntityKey.HasValue)
		{
			hashSet.Add(_currentTunnelNetEntityKey.Value);
		}
		if (selectedTunnelKey.HasValue)
		{
			hashSet.Add(selectedTunnelKey.Value);
		}
		Dictionary<int, TacticalMapBlip>[] array = new Dictionary<int, TacticalMapBlip>[3] { user.XenoStructureBlips, user.XenoBlips, user.MarineBlips };
		for (int num = 0; num < array.Length; num++)
		{
			foreach (KeyValuePair<int, TacticalMapBlip> item3 in array[num])
			{
				item3.Deconstruct(out var key, out var value);
				int num2 = key;
				TacticalMapBlip blip = value;
				if (!_showOnlyTunnels || hashSet.Contains(num2))
				{
					blipsList.Add(HighlightBlip(blip, num2, selectedTunnelKey));
					int? currentTunnelNetEntityKey = _currentTunnelNetEntityKey;
					key = num2;
					if (currentTunnelNetEntityKey == key && !item.HasValue)
					{
						item = blip.Indices;
					}
					currentTunnelNetEntityKey = selectedTunnelKey;
					key = num2;
					if (currentTunnelNetEntityKey == key && !item2.HasValue)
					{
						item2 = blip.Indices;
					}
				}
			}
		}
		return (currentPos: item, selectedPos: item2);
	}

	private List<Vector2i> GetAllTunnelPositions()
	{
		return _tunnelCache.Values.Select((TunnelCacheEntry entry) => entry.Position).ToList();
	}

	private List<Vector2i> FindTunnelPath(Vector2i startPos, Vector2i endPos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		(Vector2i, Vector2i) key = (startPos, endPos);
		if (_pathCache.TryGetValue(key, out List<Vector2i> value))
		{
			return value;
		}
		List<Vector2i> allTunnelPositions = GetAllTunnelPositions();
		List<Vector2i> tunnelsOnDirectLine = GetTunnelsOnDirectLine(startPos, endPos, allTunnelPositions);
		List<Vector2i> list = BuildOptimalPath(startPos, endPos, tunnelsOnDirectLine);
		_pathCache[key] = list;
		return list;
	}

	private List<Vector2i> GetTunnelsOnDirectLine(Vector2i start, Vector2i end, List<Vector2i> allPositions)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return (from pos in allPositions
			where pos != start && pos != end
			where IsDirectlyOnPath(start, end, pos, 30.0)
			orderby CalculateProgressAlongPath(start, end, pos)
			select pos).ToList();
	}

	private bool IsDirectlyOnPath(Vector2i start, Vector2i end, Vector2i tunnel, double maxDistance)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (CalculateDistanceFromLine(start, end, tunnel) > maxDistance)
		{
			return false;
		}
		double num = 50.0;
		double num2 = CalculateDistance(start, tunnel);
		double num3 = CalculateDistance(end, tunnel);
		if (num2 < num || num3 < num)
		{
			return false;
		}
		double num4 = CalculateProgressAlongPath(start, end, tunnel);
		if (num4 >= 0.15)
		{
			return num4 <= 0.85;
		}
		return false;
	}

	private List<Vector2i> BuildOptimalPath(Vector2i start, Vector2i end, List<Vector2i> tunnelsOnLine)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		List<Vector2i> path = new List<Vector2i> { start };
		int num = 0;
		if (_pathfindingConfig.MaxIntermediateTunnels <= 0)
		{
			path.Add(end);
			return path;
		}
		foreach (var item in (from tunnel in tunnelsOnLine
			select new
			{
				Position = tunnel,
				Cost = CalculateTunnelCost(path.Last(), tunnel, end)
			} into t
			orderby t.Cost
			select t).ToList())
		{
			if (num >= _pathfindingConfig.MaxIntermediateTunnels)
			{
				break;
			}
			if (CalculateDistance(path.Last(), item.Position) <= _pathfindingConfig.MaxConnectionDistance)
			{
				path.Add(item.Position);
				num++;
			}
		}
		path.Add(end);
		return path;
	}

	private double CalculateTunnelCost(Vector2i current, Vector2i tunnel, Vector2i destination)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		double num = CalculateDistance(current, tunnel) * _pathfindingConfig.DirectDistanceWeight;
		double num2 = CalculateDistance(tunnel, destination) * _pathfindingConfig.DirectDistanceWeight;
		double tunnelHopPenalty = _pathfindingConfig.TunnelHopPenalty;
		double num3 = CalculateDistance(current, destination);
		double num4 = num + num2;
		double num5 = Math.Max(0.0, num3 - num4);
		return num4 + tunnelHopPenalty - num5;
	}

	private double CalculateDistanceFromLine(Vector2i start, Vector2i end, Vector2i point)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		Vector2 value = new Vector2(end.X - start.X, end.Y - start.Y);
		Vector2 value2 = new Vector2(point.X - start.X, point.Y - start.Y);
		if ((double)value.Length() < 0.001)
		{
			return Vector2.Distance(new Vector2(start.X, start.Y), new Vector2(point.X, point.Y));
		}
		Vector2 vector = Vector2.Normalize(value);
		float num = Vector2.Dot(value2, vector);
		Vector2 value3 = vector * num;
		return Vector2.Distance(value2, value3);
	}

	private double CalculateProgressAlongPath(Vector2i start, Vector2i end, Vector2i point)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Vector2 value = new Vector2(end.X - start.X, end.Y - start.Y);
		Vector2 value2 = new Vector2(point.X - start.X, point.Y - start.Y);
		float num = value.LengthSquared();
		if ((double)num < 0.001)
		{
			return 0.0;
		}
		return Vector2.Dot(value2, value) / num;
	}

	private double CalculateDistance(Vector2i pos1, Vector2i pos2)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		(Vector2i, Vector2i) key = (pos1, pos2);
		if (_distanceCache.TryGetValue(key, out var value))
		{
			return value;
		}
		double num = Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2.0) + Math.Pow(pos2.Y - pos1.Y, 2.0));
		_distanceCache[key] = num;
		return num;
	}

	private string? GetTunnelNameByEntityId(int entityId)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (_tunnelCache.TryGetValue(entityId, out var value))
		{
			return value.Name;
		}
		foreach (KeyValuePair<string, NetEntity> availableTunnel in _availableTunnels)
		{
			if ((int)availableTunnel.Value == entityId)
			{
				return availableTunnel.Key;
			}
		}
		return null;
	}

	private void UpdateDirectionalArrow(Vector2i? currentPosition, Vector2i? selectedPosition)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		_window.TacticalMapWrapper.Map.RemoveLinesByColor(Color.FromHex((ReadOnlySpan<char>)"#F535AA", (Color?)null));
		if (currentPosition.HasValue && selectedPosition.HasValue)
		{
			Vector2i? val = currentPosition;
			Vector2i? val2 = selectedPosition;
			if (val.HasValue != val2.HasValue || (val.HasValue && val.GetValueOrDefault() != val2.GetValueOrDefault()))
			{
				_window.TacticalMapWrapper.Map.AddDashedTunnelPathClosest(FindTunnelPath(currentPosition.Value, selectedPosition.Value), Color.FromHex((ReadOnlySpan<char>)"#F535AA", (Color?)null));
			}
		}
	}

	private TacticalMapBlip HighlightBlip(TacticalMapBlip blip, int entityId, int? selectedTunnelKey)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		if (_currentTunnelNetEntityKey.HasValue && entityId == _currentTunnelNetEntityKey.Value)
		{
			return blip with
			{
				Background = new Rsi(new ResPath("/Textures/_RMC14/Interface/map_blips.rsi"), "background"),
				HiveLeader = true
			};
		}
		if (selectedTunnelKey.HasValue && entityId == selectedTunnelKey.Value)
		{
			return blip with
			{
				Background = new Rsi(new ResPath("/Textures/_RMC14/Interface/map_blips.rsi"), "background"),
				HiveLeader = true
			};
		}
		return blip;
	}

	private void OnBlipClicked(Vector2i clickedIndices)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		TacticalMapUserComponent user = default(TacticalMapUserComponent);
		if (!base.EntMan.TryGetComponent<TacticalMapUserComponent>(valueOrDefault, ref user))
		{
			return;
		}
		int value;
		int? num = (_positionToEntityCache.TryGetValue(clickedIndices, out value) ? new int?(value) : FindEntityIdAtIndices(clickedIndices, user));
		if (num.HasValue)
		{
			string tunnelNameByEntityId = GetTunnelNameByEntityId(num.Value);
			if (tunnelNameByEntityId != null && _availableTunnels.TryGetValue(tunnelNameByEntityId, out var value2) && (!_currentTunnelNetEntityKey.HasValue || num.Value != _currentTunnelNetEntityKey.Value))
			{
				_selectedTunnel = value2;
				((BaseButton)_window.SelectButton).Disabled = false;
				_window.UpdateSelectedTunnelDisplay(tunnelNameByEntityId);
				UpdateBlips();
			}
		}
	}

	private void OnBlipRightClicked(Vector2i clickedIndices, string _)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		TacticalMapUserComponent user = default(TacticalMapUserComponent);
		if (!base.EntMan.TryGetComponent<TacticalMapUserComponent>(valueOrDefault, ref user))
		{
			return;
		}
		int value;
		int? num = (_positionToEntityCache.TryGetValue(clickedIndices, out value) ? new int?(value) : FindEntityIdAtIndices(clickedIndices, user));
		if (num.HasValue)
		{
			string tunnelNameByEntityId = GetTunnelNameByEntityId(num.Value);
			if (tunnelNameByEntityId != null && _availableTunnels.ContainsKey(tunnelNameByEntityId))
			{
				Vector2 screenPosition = _window.TacticalMapWrapper.Map.IndicesToPosition(clickedIndices);
				_window.TacticalMapWrapper.Map.ShowTunnelInfo(clickedIndices, tunnelNameByEntityId, screenPosition);
				_window.TacticalMapWrapper.Canvas.ShowTunnelInfo(clickedIndices, tunnelNameByEntityId, screenPosition);
			}
		}
	}

	private int? FindEntityIdAtIndices(Vector2i indices, TacticalMapUserComponent user)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<int, TacticalMapBlip> xenoStructureBlip in user.XenoStructureBlips)
		{
			if (xenoStructureBlip.Value.Indices == indices)
			{
				return xenoStructureBlip.Key;
			}
		}
		foreach (KeyValuePair<int, TacticalMapBlip> xenoBlip in user.XenoBlips)
		{
			if (xenoBlip.Value.Indices == indices)
			{
				return xenoBlip.Key;
			}
		}
		foreach (KeyValuePair<int, TacticalMapBlip> marineBlip in user.MarineBlips)
		{
			if (marineBlip.Value.Indices == indices)
			{
				return marineBlip.Key;
			}
		}
		return null;
	}

	public void ConfigurePathfinding(TunnelPathfindingConfig config)
	{
		_pathfindingConfig = config;
		_distanceCache.Clear();
		_pathCache.Clear();
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		SelectDestinationTunnelWindow window = _window;
		if (window != null && ((BaseWindow)window).IsOpen)
		{
			return;
		}
		_window = BoundUserInterfaceExt.CreateWindow<SelectDestinationTunnelWindow>((BoundUserInterface)(object)this);
		((BaseButton)_window.SelectButton).Disabled = true;
		_window.SetBlipUpdateCallback(delegate
		{
			UpdateBlips();
		});
		TacticalMapWrapper tacticalMapWrapper = _window.TacticalMapWrapper;
		TabContainer.SetTabVisible((Control)(object)tacticalMapWrapper.CanvasTab, false);
		tacticalMapWrapper.Tabs.CurrentTab = 0;
		((Control)tacticalMapWrapper.Map).MouseFilter = (MouseFilterMode)0;
		((Control)tacticalMapWrapper.Canvas).MouseFilter = (MouseFilterMode)0;
		((BaseButton)_window.ShowOnlyTunnelsCheckbox).OnPressed += delegate(ButtonEventArgs args)
		{
			_showOnlyTunnels = args.Button.Pressed;
			UpdateBlips();
		};
		_window.SelectableTunnels.OnItemSelected += delegate(ItemListSelectedEventArgs args)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			((BaseButton)_window.SelectButton).Disabled = false;
			_selectedTunnel = (NetEntity)((ItemListEventArgs)args).ItemList[args.ItemIndex].Metadata;
			_window.UpdateSelectedTunnelDisplay(GetTunnelNameCached(_selectedTunnel.Value));
			UpdateBlips();
		};
		((BaseButton)_window.SelectButton).OnButtonDown += delegate(ButtonEventArgs args)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			NetEntity? selectedTunnel = _selectedTunnel;
			if (!selectedTunnel.HasValue)
			{
				args.Button.Disabled = true;
			}
			else
			{
				GoToTunnel(_selectedTunnel.Value);
				((BoundUserInterface)this).Close();
			}
		};
		_window.TacticalMapWrapper.Map.OnBlipClicked = OnBlipClicked;
		_window.TacticalMapWrapper.Canvas.OnBlipClicked = OnBlipClicked;
		_window.TacticalMapWrapper.Map.OnBlipRightClicked = OnBlipRightClicked;
		_window.TacticalMapWrapper.Canvas.OnBlipRightClicked = OnBlipRightClicked;
	}

	private void GoToTunnel(NetEntity destinationTunnel)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new TraverseXenoTunnelMessage(destinationTunnel));
	}
}
