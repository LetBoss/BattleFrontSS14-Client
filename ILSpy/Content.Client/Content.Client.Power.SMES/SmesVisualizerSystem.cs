using System;
using Content.Shared.Power;
using Content.Shared.SMES;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.Power.SMES;

public sealed class SmesVisualizerSystem : VisualizerSystem<SmesComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, SmesComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			int num = default(int);
			if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<int>(uid, (Enum)SmesVisuals.LastChargeLevel, ref num, args.Component) || num == 0)
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)SmesVisualLayers.Charge, false);
			}
			else
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)SmesVisualLayers.Charge, true);
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)SmesVisualLayers.Charge, StateId.op_Implicit($"{comp.ChargeOverlayPrefix}{num}"));
			}
			ChargeState chargeState = default(ChargeState);
			if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<ChargeState>(uid, (Enum)SmesVisuals.LastChargeState, ref chargeState, args.Component))
			{
				chargeState = ChargeState.Still;
			}
			switch (chargeState)
			{
			case ChargeState.Still:
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)SmesVisualLayers.Input, StateId.op_Implicit(comp.InputOverlayPrefix + "0"));
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)SmesVisualLayers.Output, StateId.op_Implicit(comp.OutputOverlayPrefix + "1"));
				break;
			case ChargeState.Charging:
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)SmesVisualLayers.Input, StateId.op_Implicit(comp.InputOverlayPrefix + "1"));
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)SmesVisualLayers.Output, StateId.op_Implicit(comp.OutputOverlayPrefix + "1"));
				break;
			case ChargeState.Discharging:
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)SmesVisualLayers.Input, StateId.op_Implicit(comp.InputOverlayPrefix + "0"));
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)SmesVisualLayers.Output, StateId.op_Implicit(comp.OutputOverlayPrefix + "2"));
				break;
			}
		}
	}
}
