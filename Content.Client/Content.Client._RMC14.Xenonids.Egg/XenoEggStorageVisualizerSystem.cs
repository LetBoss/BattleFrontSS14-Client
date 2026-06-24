using System;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Egg.EggRetriever;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Xenonids.Egg;

public sealed class XenoEggStorageVisualizerSystem : VisualizerSystem<XenoEggStorageVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, XenoEggStorageVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		int num = default(int);
		int num2 = default(int);
		if (sprite != null && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<int>(uid, (Enum)XenoEggStorageVisuals.Number, ref num, (AppearanceComponent)null) && base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)XenoEggStorageVisualLayers.Base, ref num2, false))
		{
			string text = "eggsac_";
			text += Math.Clamp((int)Math.Ceiling((double)num / (double)component.MaxEggs * (double)component.FullStates), 0, component.FullStates);
			bool flag = default(bool);
			bool flag2 = default(bool);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RMCXenoStateVisuals.Downed, ref flag, (AppearanceComponent)null) && flag)
			{
				text += "_downed";
			}
			else if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RMCXenoStateVisuals.Resting, ref flag2, (AppearanceComponent)null) && flag2)
			{
				text += "_rest";
			}
			bool flag3 = default(bool);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)XenoEggStorageVisuals.Active, ref flag3, (AppearanceComponent)null) && flag3)
			{
				text += "_active";
			}
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, StateId.op_Implicit(text));
			bool flag4 = default(bool);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RMCXenoStateVisuals.Dead, ref flag4, (AppearanceComponent)null) && flag4)
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, false);
			}
			else
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, true);
			}
		}
	}
}
