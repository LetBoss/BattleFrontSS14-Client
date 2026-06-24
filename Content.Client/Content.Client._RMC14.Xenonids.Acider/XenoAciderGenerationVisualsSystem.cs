using System;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.AciderGeneration;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Xenonids.Acider;

public sealed class XenoAciderGenerationVisualsSystem : VisualizerSystem<XenoAciderGenerationComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, XenoAciderGenerationComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		bool flag = default(bool);
		int num = default(int);
		if (sprite == null || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)XenoAcidGeneratingVisuals.Generating, ref flag, (AppearanceComponent)null) || !base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)XenoAcidGeneratingVisualLayers.Base, ref num, false))
		{
			return;
		}
		if (!flag)
		{
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, false);
			return;
		}
		base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, true);
		string text = "acid";
		bool flag2 = default(bool);
		bool flag3 = default(bool);
		if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RMCXenoStateVisuals.Downed, ref flag2, (AppearanceComponent)null) && flag2)
		{
			text += "_downed";
		}
		else if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RMCXenoStateVisuals.Resting, ref flag3, (AppearanceComponent)null) && flag3)
		{
			text += "_rest";
		}
		base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, StateId.op_Implicit(text));
	}
}
