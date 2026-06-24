using System;
using Content.Shared._RMC14.Medical.HUD;
using Content.Shared._RMC14.Medical.HUD.Components;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Medical.HUD.Holocard;

public sealed class HolocardContainerVisualizerSystem : VisualizerSystem<HolocardContainerComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, HolocardContainerComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		HolocardStatus holocardStatus = default(HolocardStatus);
		bool flag = default(bool);
		if (args.Sprite == null || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<HolocardStatus>(uid, (Enum)HolocardContainerVisuals.State, ref holocardStatus, args.Component) || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)StorageVisuals.Open, ref flag, args.Component))
		{
			return;
		}
		(EntityUid, SpriteComponent) tuple = (uid, args.Sprite);
		int num = default(int);
		if (!base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit(tuple), (Enum)HolocardContainerVisualLayers.Base, ref num, false))
		{
			return;
		}
		if (flag && component.HideOnOpen)
		{
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit(tuple), num, false);
			return;
		}
		string prefix = component.Prefix;
		switch (holocardStatus)
		{
		case HolocardStatus.Urgent:
			prefix += "_holoorange";
			break;
		case HolocardStatus.Emergency:
			prefix += "_holored";
			break;
		case HolocardStatus.Xeno:
			prefix += "_holopurple";
			break;
		case HolocardStatus.Permadead:
			prefix += "_holoblack";
			break;
		default:
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit(tuple), num, false);
			return;
		}
		base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit(tuple), num, StateId.op_Implicit(prefix));
		base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit(tuple), num, true);
	}
}
