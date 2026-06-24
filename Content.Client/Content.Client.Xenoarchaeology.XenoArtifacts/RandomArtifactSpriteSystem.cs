using System;
using Content.Shared.Xenoarchaeology.XenoArtifacts;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.Xenoarchaeology.XenoArtifacts;

public sealed class RandomArtifactSpriteSystem : VisualizerSystem<RandomArtifactSpriteComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, RandomArtifactSpriteComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if (args.Sprite == null || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<int>(uid, (Enum)SharedArtifactsVisuals.SpriteIndex, ref num, args.Component))
		{
			return;
		}
		bool flag = default(bool);
		if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)SharedArtifactsVisuals.IsUnlocking, ref flag, args.Component))
		{
			flag = false;
		}
		bool flag2 = default(bool);
		if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)SharedArtifactsVisuals.IsActivated, ref flag2, args.Component))
		{
			flag2 = false;
		}
		string text = num.ToString("D2");
		string text2 = (flag ? "_on" : "");
		int num2 = default(int);
		if (base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ArtifactsVisualLayers.UnlockingEffect, ref num2, false))
		{
			string text3 = "ano" + text;
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ArtifactsVisualLayers.Base, StateId.op_Implicit(text3));
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, StateId.op_Implicit(text3 + "_on"));
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, flag);
			int num3 = default(int);
			if (base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ArtifactsVisualLayers.ActivationEffect, ref num3, false))
			{
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, StateId.op_Implicit("artifact-activation"));
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, flag2);
			}
		}
		else
		{
			string text4 = "ano" + text + text2;
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ArtifactsVisualLayers.Base, StateId.op_Implicit(text4));
		}
	}
}
