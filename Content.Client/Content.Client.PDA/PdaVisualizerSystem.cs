using System;
using Content.Shared.Light;
using Content.Shared.PDA;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.PDA;

public sealed class PdaVisualizerSystem : VisualizerSystem<PdaVisualsComponent>
{
	public enum PdaVisualLayers : byte
	{
		Base,
		Flashlight,
		IdLight
	}

	protected override void OnAppearanceChange(EntityUid uid, PdaVisualsComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			string text = default(string);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<string>(uid, (Enum)PdaVisuals.PdaType, ref text, args.Component))
			{
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PdaVisualLayers.Base, StateId.op_Implicit(text));
			}
			bool flag = default(bool);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)UnpoweredFlashlightVisuals.LightOn, ref flag, args.Component))
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PdaVisualLayers.Flashlight, flag);
			}
			bool flag2 = default(bool);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)PdaVisuals.IdCardInserted, ref flag2, args.Component))
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PdaVisualLayers.IdLight, flag2);
			}
		}
	}
}
