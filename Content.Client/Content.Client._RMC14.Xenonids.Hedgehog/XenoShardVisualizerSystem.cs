using System;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Hedgehog;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Xenonids.Hedgehog;

public sealed class XenoShardVisualizerSystem : VisualizerSystem<XenoShardComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, XenoShardComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		XenoShardLevel value = default(XenoShardLevel);
		int num = default(int);
		if (sprite == null || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<XenoShardLevel>(uid, (Enum)XenoShardVisuals.Level, ref value, (AppearanceComponent)null) || !base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)XenoShardVisualLayers.Base, ref num, true))
		{
			return;
		}
		string text = $"hedgehog_{(int)value}";
		bool flag = default(bool);
		if (!(((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RMCXenoStateVisuals.Dead, ref flag, (AppearanceComponent)null) && flag))
		{
			bool flag2 = default(bool);
			bool flag3 = default(bool);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RMCXenoStateVisuals.Downed, ref flag2, (AppearanceComponent)null) && flag2)
			{
				text += "_crit";
			}
			else if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RMCXenoStateVisuals.Resting, ref flag3, (AppearanceComponent)null) && flag3)
			{
				text += "_resting";
			}
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, StateId.op_Implicit(text));
		}
	}
}
