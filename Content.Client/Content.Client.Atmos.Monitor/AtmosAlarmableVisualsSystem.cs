using System.Collections.Generic;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Power;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.Atmos.Monitor;

public sealed class AtmosAlarmableVisualsSystem : VisualizerSystem<AtmosAlarmableVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, AtmosAlarmableVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if (args.Sprite == null || !base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.LayerMap, ref num, false) || !args.AppearanceData.TryGetValue(PowerDeviceVisuals.Powered, out var value) || !(value is bool flag))
		{
			return;
		}
		if (component.HideOnDepowered != null)
		{
			int num2 = default(int);
			foreach (string item in component.HideOnDepowered)
			{
				if (base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), item, ref num2, false))
				{
					base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, flag);
				}
			}
		}
		if (component.SetOnDepowered != null && !flag)
		{
			int num3 = default(int);
			foreach (var (text3, text4) in component.SetOnDepowered)
			{
				if (base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), text3, ref num3, false))
				{
					base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, new StateId(text4));
				}
			}
		}
		AtmosAlarmType key = default(AtmosAlarmType);
		int num4;
		if (args.AppearanceData.TryGetValue(AtmosMonitorVisuals.AlarmType, out var value2))
		{
			if (value2 is AtmosAlarmType)
			{
				key = (AtmosAlarmType)value2;
				num4 = 1;
			}
			else
			{
				num4 = 0;
			}
		}
		else
		{
			num4 = 0;
		}
		if (((uint)num4 & (flag ? 1u : 0u)) != 0 && component.AlarmStates.TryGetValue(key, out string value3))
		{
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, new StateId(value3));
		}
	}
}
