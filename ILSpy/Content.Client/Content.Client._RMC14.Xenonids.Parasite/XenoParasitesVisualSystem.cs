using System;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Projectile.Parasite;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Xenonids.Parasite;

public sealed class XenoParasitesVisualSystem : VisualizerSystem<XenoParasiteThrowerComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, XenoParasiteThrowerComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		bool[] array = default(bool[]);
		if (sprite == null || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool[]>(uid, (Enum)ParasiteOverlayVisuals.States, ref array, (AppearanceComponent)null))
		{
			return;
		}
		string value = "para_";
		bool flag = default(bool);
		bool flag2 = default(bool);
		if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RMCXenoStateVisuals.Downed, ref flag, (AppearanceComponent)null) && flag)
		{
			value = "para_downed_";
		}
		else if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RMCXenoStateVisuals.Resting, ref flag2, (AppearanceComponent)null) && flag2)
		{
			value = "para_rest_";
		}
		ParasiteOverlayLayers[] values = Enum.GetValues<ParasiteOverlayLayers>();
		int num = default(int);
		foreach (ParasiteOverlayLayers parasiteOverlayLayers in values)
		{
			if (base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)parasiteOverlayLayers, ref num, false))
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)parasiteOverlayLayers, array[(int)parasiteOverlayLayers]);
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)parasiteOverlayLayers, StateId.op_Implicit($"{value}{(int)parasiteOverlayLayers}"));
			}
		}
	}
}
