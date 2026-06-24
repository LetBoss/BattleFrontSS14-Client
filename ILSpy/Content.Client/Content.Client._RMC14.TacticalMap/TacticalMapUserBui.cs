using System;
using System.Collections.Generic;
using System.Numerics;
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

namespace Content.Client._RMC14.TacticalMap;

public sealed class TacticalMapUserBui : RMCPopOutBui<TacticalMapWindow>
{
	[Dependency]
	private IPlayerManager _player;

	private static readonly ISawmill _logger = Logger.GetSawmill("tactical_map_settings");

	private bool _refreshed;

	private string? _currentMapName;

	protected override TacticalMapWindow? Window { get; set; }

	public TacticalMapUserBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		EntityUid? val = null;
		TacticalMapUserComponent tacticalMapUserComponent = default(TacticalMapUserComponent);
		if (((BoundUserInterface)this).EntMan.TryGetComponent<TacticalMapUserComponent>(((BoundUserInterface)this).Owner, ref tacticalMapUserComponent) && tacticalMapUserComponent.Map.HasValue)
		{
			val = tacticalMapUserComponent.Map.Value;
		}
		Window = ((BoundUserInterface)(object)this).CreatePopOutableWindow<TacticalMapWindow>();
		if (val.HasValue)
		{
			Window.SetMapEntity(_currentMapName);
		}
		TabContainer.SetTabTitle((Control)(object)Window.Wrapper.MapTab, Loc.GetString("ui-tactical-map-tab-map"));
		TabContainer.SetTabVisible((Control)(object)Window.Wrapper.MapTab, true);
		if (val.HasValue)
		{
			Window.Wrapper.SetMapEntity(_currentMapName);
		}
		AreaGridComponent item = default(AreaGridComponent);
		if (tacticalMapUserComponent != null && tacticalMapUserComponent.Map.HasValue && ((BoundUserInterface)this).EntMan.TryGetComponent<AreaGridComponent>(tacticalMapUserComponent.Map.Value, ref item))
		{
			Window.Wrapper.UpdateTexture(Entity<AreaGridComponent>.op_Implicit((tacticalMapUserComponent.Map.Value, item)));
		}
		try
		{
			TacticalMapSettings settings = IoCManager.Resolve<TacticalMapSettingsManager>().LoadSettings(_currentMapName);
			if (_currentMapName != null)
			{
				Window.Wrapper.LoadMapSpecificSettings(settings, _currentMapName);
			}
		}
		catch (Exception value)
		{
			_logger.Error($"Failed to load tactical map user settings for map '{_currentMapName}': {value}");
		}
		Refresh();
		Window.Wrapper.SetupUpdateButton(delegate(TacticalMapUpdateCanvasMsg msg)
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)msg);
		});
		TacticalMapControl map = Window.Wrapper.Map;
		map.OnQueenEyeMove = (Action<Vector2i>)Delegate.Combine(map.OnQueenEyeMove, (Action<Vector2i>)delegate(Vector2i position)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new TacticalMapQueenEyeMoveMsg(position));
		});
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		if (state is TacticalMapBuiState tacticalMapBuiState)
		{
			_currentMapName = tacticalMapBuiState.MapName;
			Window?.SetMapEntity(_currentMapName);
			Window?.Wrapper.SetMapEntity(_currentMapName);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && Window?.Wrapper != null)
		{
			try
			{
				TacticalMapSettingsManager tacticalMapSettingsManager = IoCManager.Resolve<TacticalMapSettingsManager>();
				TacticalMapSettings currentSettings = Window.Wrapper.GetCurrentSettings();
				currentSettings.WindowSize = new Vector2(((Control)Window).SetSize.X, ((Control)Window).SetSize.Y);
				currentSettings.WindowPosition = new Vector2(((Control)Window).Position.X, ((Control)Window).Position.Y);
				tacticalMapSettingsManager.SaveSettings(currentSettings, _currentMapName);
			}
			catch (Exception value)
			{
				_logger.Error($"Failed to save tactical map user settings during disposal for map '{_currentMapName}': {value}");
			}
		}
		base.Dispose(disposing);
	}

	public void Refresh()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		if (Window == null)
		{
			return;
		}
		int lineLimit = ((BoundUserInterface)this).EntMan.System<TacticalMapSystem>().LineLimit;
		Window.Wrapper.SetLineLimit(lineLimit);
		UpdateBlips();
		UpdateLabels();
		UpdateTimestamps();
		Window.Wrapper.Map.Lines.Clear();
		TacticalMapLinesComponent componentOrNull = EntityManagerExt.GetComponentOrNull<TacticalMapLinesComponent>(((BoundUserInterface)this).EntMan, ((BoundUserInterface)this).Owner);
		if (componentOrNull != null)
		{
			Window.Wrapper.Map.Lines.AddRange(componentOrNull.MarineLines);
			Window.Wrapper.Map.Lines.AddRange(componentOrNull.XenoLines);
		}
		if (!_refreshed)
		{
			Window.Wrapper.Canvas.Lines.Clear();
			if (componentOrNull != null)
			{
				Window.Wrapper.Canvas.Lines.AddRange(componentOrNull.MarineLines);
				Window.Wrapper.Canvas.Lines.AddRange(componentOrNull.XenoLines);
			}
			TacticalMapUserComponent componentOrNull2 = EntityManagerExt.GetComponentOrNull<TacticalMapUserComponent>(((BoundUserInterface)this).EntMan, ((BoundUserInterface)this).Owner);
			if (componentOrNull2 != null && componentOrNull2.CanDraw)
			{
				TabContainer.SetTabTitle((Control)(object)Window.Wrapper.CanvasTab, Loc.GetString("ui-tactical-map-tab-canvas"));
				TabContainer.SetTabVisible((Control)(object)Window.Wrapper.CanvasTab, true);
			}
			else
			{
				TabContainer.SetTabVisible((Control)(object)Window.Wrapper.CanvasTab, false);
			}
			_refreshed = true;
		}
	}

	private void UpdateBlips()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		if (Window == null)
		{
			return;
		}
		TacticalMapUserComponent tacticalMapUserComponent = default(TacticalMapUserComponent);
		if (!((BoundUserInterface)this).EntMan.TryGetComponent<TacticalMapUserComponent>(((BoundUserInterface)this).Owner, ref tacticalMapUserComponent))
		{
			Window.Wrapper.UpdateBlips(null);
			return;
		}
		int num = tacticalMapUserComponent.MarineBlips.Count + tacticalMapUserComponent.XenoBlips.Count + tacticalMapUserComponent.XenoStructureBlips.Count;
		TacticalMapBlip[] array = new TacticalMapBlip[num];
		int[] array2 = new int[num];
		int num2 = 0;
		int key;
		TacticalMapBlip value;
		foreach (KeyValuePair<int, TacticalMapBlip> marineBlip in tacticalMapUserComponent.MarineBlips)
		{
			marineBlip.Deconstruct(out key, out value);
			int num3 = key;
			TacticalMapBlip tacticalMapBlip = value;
			array[num2] = tacticalMapBlip;
			array2[num2] = num3;
			num2++;
		}
		foreach (KeyValuePair<int, TacticalMapBlip> xenoBlip in tacticalMapUserComponent.XenoBlips)
		{
			xenoBlip.Deconstruct(out key, out value);
			int num4 = key;
			TacticalMapBlip tacticalMapBlip2 = value;
			array[num2] = tacticalMapBlip2;
			array2[num2] = num4;
			num2++;
		}
		foreach (KeyValuePair<int, TacticalMapBlip> xenoStructureBlip in tacticalMapUserComponent.XenoStructureBlips)
		{
			xenoStructureBlip.Deconstruct(out key, out value);
			int num5 = key;
			TacticalMapBlip tacticalMapBlip3 = value;
			array[num2] = tacticalMapBlip3;
			array2[num2] = num5;
			num2++;
		}
		Window.Wrapper.UpdateBlips(array, array2);
		int? localPlayerEntityId = (((ISharedPlayerManager)_player).LocalEntity.HasValue ? new int?((int)((BoundUserInterface)this).EntMan.GetNetEntity(((ISharedPlayerManager)_player).LocalEntity.Value, (MetaDataComponent)null)) : ((int?)null));
		Window.Wrapper.Map.SetLocalPlayerEntityId(localPlayerEntityId);
		Window.Wrapper.Canvas.SetLocalPlayerEntityId(localPlayerEntityId);
	}

	private void UpdateLabels()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (Window == null)
		{
			return;
		}
		TacticalMapLabelsComponent componentOrNull = EntityManagerExt.GetComponentOrNull<TacticalMapLabelsComponent>(((BoundUserInterface)this).EntMan, ((BoundUserInterface)this).Owner);
		if (componentOrNull != null)
		{
			Dictionary<Vector2i, string> dictionary = new Dictionary<Vector2i, string>();
			foreach (KeyValuePair<Vector2i, string> marineLabel in componentOrNull.MarineLabels)
			{
				dictionary[marineLabel.Key] = marineLabel.Value;
			}
			foreach (KeyValuePair<Vector2i, string> xenoLabel in componentOrNull.XenoLabels)
			{
				dictionary[xenoLabel.Key] = xenoLabel.Value;
			}
			Window.Wrapper.Map.UpdateTacticalLabels(dictionary);
		}
		else
		{
			Window.Wrapper.Map.UpdateTacticalLabels(new Dictionary<Vector2i, string>());
		}
	}

	private void UpdateTimestamps()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		TacticalMapUserComponent tacticalMapUserComponent = default(TacticalMapUserComponent);
		if (Window != null && ((BoundUserInterface)this).EntMan.TryGetComponent<TacticalMapUserComponent>(((BoundUserInterface)this).Owner, ref tacticalMapUserComponent))
		{
			Window.Wrapper.LastUpdateAt = tacticalMapUserComponent.LastAnnounceAt;
			Window.Wrapper.NextUpdateAt = tacticalMapUserComponent.NextAnnounceAt;
		}
	}
}
