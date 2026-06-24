using System;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Inhands;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Xenonids.Inhands;

public sealed class XenoInhandsVisualsSystem : VisualizerSystem<XenoInhandsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, XenoInhandsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		string text = default(string);
		string text2 = default(string);
		if (sprite == null || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<string>(uid, (Enum)XenoInhandVisuals.RightHand, ref text, (AppearanceComponent)null) || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<string>(uid, (Enum)XenoInhandVisuals.LeftHand, ref text2, (AppearanceComponent)null))
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RMCXenoStateVisuals.Downed, ref flag, (AppearanceComponent)null);
		((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RMCXenoStateVisuals.Resting, ref flag2, (AppearanceComponent)null);
		((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RMCXenoStateVisuals.Ovipositor, ref flag3, (AppearanceComponent)null);
		string text3 = text2;
		XenoInhandVisualLayers xenoInhandVisualLayers = XenoInhandVisualLayers.Left;
		int num = default(int);
		State val2 = default(State);
		for (int i = 0; i < 2; i++)
		{
			if (i == 1)
			{
				text3 = text;
				xenoInhandVisualLayers = XenoInhandVisualLayers.Right;
			}
			if (!base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)xenoInhandVisualLayers, ref num, false))
			{
				continue;
			}
			if (text3 == string.Empty)
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, false);
				continue;
			}
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, true);
			string text4 = $"{component.Prefix}_{text3}_{xenoInhandVisualLayers.ToString().ToLower()}";
			if (flag3)
			{
				text4 = text4 + "_" + component.Ovi;
			}
			else if (flag)
			{
				text4 = text4 + "_" + component.Downed;
			}
			else if (flag2)
			{
				text4 = text4 + "_" + component.Resting;
			}
			RSI val = base.SpriteSystem.LayerGetEffectiveRsi(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num);
			if (val != null)
			{
				val.TryGetState(StateId.op_Implicit(text4), ref val2);
				if (val2 != null)
				{
					base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, StateId.op_Implicit(text4));
				}
				else
				{
					base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, false);
				}
			}
		}
	}
}
