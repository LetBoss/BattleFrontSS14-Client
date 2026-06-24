using System;
using System.Collections.Generic;
using System.Numerics;
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

namespace Content.Client._RMC14.TacticalMap;

public sealed class TacticalMapComputerBui : RMCPopOutBui<TacticalMapWindow>
{
	[Dependency]
	private IPlayerManager _player;

	private bool _refreshed;

	private string? _currentMapName;

	protected override TacticalMapWindow? Window { get; set; }

	public TacticalMapComputerBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		TacticalMapComputerComponent componentOrNull = EntityManagerExt.GetComponentOrNull<TacticalMapComputerComponent>(((BoundUserInterface)this).EntMan, ((BoundUserInterface)this).Owner);
		if (componentOrNull != null && componentOrNull.Map.HasValue)
		{
			_ = componentOrNull.Map.Value;
		}
		Window = ((BoundUserInterface)(object)this).CreatePopOutableWindow<TacticalMapWindow>();
		if (_currentMapName != null)
		{
			Window.SetMapEntity(_currentMapName);
		}
		TabContainer.SetTabTitle((Control)(object)Window.Wrapper.MapTab, Loc.GetString("ui-tactical-map-tab-map"));
		TabContainer.SetTabVisible((Control)(object)Window.Wrapper.MapTab, true);
		if (componentOrNull != null)
		{
			EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
			if (localEntity.HasValue)
			{
				EntityUid valueOrDefault = localEntity.GetValueOrDefault();
				if (((BoundUserInterface)this).EntMan.System<SkillsSystem>().HasSkill(Entity<SkillsComponent>.op_Implicit(valueOrDefault), componentOrNull.Skill, componentOrNull.SkillLevel))
				{
					TabContainer.SetTabTitle((Control)(object)Window.Wrapper.CanvasTab, Loc.GetString("ui-tactical-map-tab-canvas"));
					TabContainer.SetTabVisible((Control)(object)Window.Wrapper.CanvasTab, true);
					goto IL_011f;
				}
			}
		}
		TabContainer.SetTabVisible((Control)(object)Window.Wrapper.CanvasTab, false);
		goto IL_011f;
		IL_011f:
		AreaGridComponent item = default(AreaGridComponent);
		if (componentOrNull != null && ((BoundUserInterface)this).EntMan.TryGetComponent<AreaGridComponent>(componentOrNull.Map, ref item))
		{
			Window.Wrapper.UpdateTexture(Entity<AreaGridComponent>.op_Implicit((componentOrNull.Map.Value, item)));
		}
		if (_currentMapName != null)
		{
			Window.Wrapper.SetMapEntity(_currentMapName);
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
			Logger.GetSawmill("tactical_map_settings").Error($"Failed to load tactical map settings for map '{_currentMapName}': {value}");
		}
		Refresh();
		((BaseButton)Window.Wrapper.UpdateCanvasButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new TacticalMapUpdateCanvasMsg(Window.Wrapper.Canvas.Lines, Window.Wrapper.Canvas.TacticalLabels));
		};
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
				Logger.GetSawmill("tactical_map_settings").Error($"Failed to save tactical map settings during disposal for map '{_currentMapName}': {value}");
			}
		}
		base.Dispose(disposing);
	}

	public void Refresh()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (Window == null)
		{
			return;
		}
		int lineLimit = ((BoundUserInterface)this).EntMan.System<TacticalMapSystem>().LineLimit;
		Window.Wrapper.SetLineLimit(lineLimit);
		UpdateBlips();
		UpdateLabels();
		TacticalMapComputerComponent tacticalMapComputerComponent = default(TacticalMapComputerComponent);
		if (((BoundUserInterface)this).EntMan.TryGetComponent<TacticalMapComputerComponent>(((BoundUserInterface)this).Owner, ref tacticalMapComputerComponent))
		{
			Window.Wrapper.LastUpdateAt = tacticalMapComputerComponent.LastAnnounceAt;
			Window.Wrapper.NextUpdateAt = tacticalMapComputerComponent.NextAnnounceAt;
		}
		Window.Wrapper.Map.Lines.Clear();
		TacticalMapLinesComponent componentOrNull = EntityManagerExt.GetComponentOrNull<TacticalMapLinesComponent>(((BoundUserInterface)this).EntMan, ((BoundUserInterface)this).Owner);
		if (componentOrNull != null)
		{
			Window.Wrapper.Map.Lines.AddRange(componentOrNull.MarineLines);
		}
		if (!_refreshed)
		{
			if (componentOrNull != null)
			{
				Window.Wrapper.Canvas.Lines.AddRange(componentOrNull.MarineLines);
			}
			_refreshed = true;
		}
	}

	private void UpdateBlips()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		if (Window == null)
		{
			return;
		}
		TacticalMapComputerComponent tacticalMapComputerComponent = default(TacticalMapComputerComponent);
		if (!((BoundUserInterface)this).EntMan.TryGetComponent<TacticalMapComputerComponent>(((BoundUserInterface)this).Owner, ref tacticalMapComputerComponent))
		{
			Window.Wrapper.UpdateBlips(null);
			return;
		}
		TacticalMapBlip[] array = new TacticalMapBlip[tacticalMapComputerComponent.Blips.Count];
		int[] array2 = new int[tacticalMapComputerComponent.Blips.Count];
		int num = 0;
		foreach (var (num3, tacticalMapBlip2) in tacticalMapComputerComponent.Blips)
		{
			array[num] = tacticalMapBlip2;
			array2[num] = num3;
			num++;
		}
		Window.Wrapper.UpdateBlips(array, array2);
		int? localPlayerEntityId = (((ISharedPlayerManager)_player).LocalEntity.HasValue ? new int?((int)((BoundUserInterface)this).EntMan.GetNetEntity(((ISharedPlayerManager)_player).LocalEntity.Value, (MetaDataComponent)null)) : ((int?)null));
		Window.Wrapper.Map.SetLocalPlayerEntityId(localPlayerEntityId);
		Window.Wrapper.Canvas.SetLocalPlayerEntityId(localPlayerEntityId);
	}

	private void UpdateLabels()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (Window == null)
		{
			return;
		}
		TacticalMapLabelsComponent componentOrNull = EntityManagerExt.GetComponentOrNull<TacticalMapLabelsComponent>(((BoundUserInterface)this).EntMan, ((BoundUserInterface)this).Owner);
		if (componentOrNull != null)
		{
			Window.Wrapper.Map.UpdateTacticalLabels(componentOrNull.MarineLabels);
			if (!_refreshed)
			{
				Window.Wrapper.Canvas.UpdateTacticalLabels(componentOrNull.MarineLabels);
			}
		}
		else
		{
			Window.Wrapper.Map.UpdateTacticalLabels(new Dictionary<Vector2i, string>());
			if (!_refreshed)
			{
				Window.Wrapper.Canvas.UpdateTacticalLabels(new Dictionary<Vector2i, string>());
			}
		}
	}
}
