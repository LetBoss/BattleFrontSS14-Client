using System;
using System.Collections.Generic;
using Content.Client.Pinpointer.UI;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Medical.CrewMonitoring;

public sealed class CrewMonitoringNavMapControl : NavMapControl
{
	public NetEntity? Focus;

	public Dictionary<NetEntity, string> LocalizedNames = new Dictionary<NetEntity, string>();

	private Label _trackedEntityLabel;

	private PanelContainer _trackedEntityPanel;

	public CrewMonitoringNavMapControl()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		WallColor = new Color((byte)192, (byte)122, (byte)196, byte.MaxValue);
		TileColor = new Color((byte)71, (byte)42, (byte)72, byte.MaxValue);
		BackgroundColor = Color.FromSrgb(((Color)(ref TileColor)).WithAlpha(BackgroundOpacity));
		_trackedEntityLabel = new Label
		{
			Margin = new Thickness(10f, 8f),
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2,
			Modulate = Color.White
		};
		_trackedEntityPanel = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = BackgroundColor
			},
			Margin = new Thickness(5f, 10f),
			HorizontalAlignment = (HAlignment)1,
			VerticalAlignment = (VAlignment)3,
			Visible = false
		};
		((Control)_trackedEntityPanel).AddChild((Control)(object)_trackedEntityLabel);
		((Control)this).AddChild((Control)(object)_trackedEntityPanel);
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		base.FrameUpdate(args);
		if (!Focus.HasValue)
		{
			_trackedEntityLabel.Text = string.Empty;
			((Control)_trackedEntityPanel).Visible = false;
			return;
		}
		foreach (KeyValuePair<NetEntity, NavMapBlip> trackedEntity in TrackedEntities)
		{
			trackedEntity.Deconstruct(out var key, out var value);
			NetEntity val = key;
			NavMapBlip navMapBlip = value;
			key = val;
			NetEntity? focus = Focus;
			if (focus.HasValue && !(key != focus.GetValueOrDefault()))
			{
				if (!LocalizedNames.TryGetValue(val, out string value2))
				{
					value2 = "Unknown";
				}
				string text = value2 + "\nLocation: [x = " + MathF.Round(((EntityCoordinates)(ref navMapBlip.Coordinates)).X) + ", y = " + MathF.Round(((EntityCoordinates)(ref navMapBlip.Coordinates)).Y) + "]";
				_trackedEntityLabel.Text = text;
				((Control)_trackedEntityPanel).Visible = true;
				return;
			}
		}
		_trackedEntityLabel.Text = string.Empty;
		((Control)_trackedEntityPanel).Visible = false;
	}
}
