using System;
using Content.Shared.Conveyor;
using Content.Shared.Materials;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.Materials;

public sealed class RecyclerVisualizerSystem : VisualizerSystem<RecyclerVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, RecyclerVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if (args.Sprite != null && base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)RecyclerVisualLayers.Main, ref num, false))
		{
			ConveyorState conveyorState = default(ConveyorState);
			((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<ConveyorState>(uid, (Enum)ConveyorVisuals.State, ref conveyorState, (AppearanceComponent)null);
			bool flag = default(bool);
			((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RecyclerVisuals.Bloody, ref flag, (AppearanceComponent)null);
			bool flag2 = default(bool);
			((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RecyclerVisuals.Broken, ref flag2, (AppearanceComponent)null);
			int value = ((conveyorState != ConveyorState.Off) ? 1 : 0);
			if (flag2)
			{
				value = 2;
			}
			string value2 = (flag ? component.BloodyKey : string.Empty);
			string text = $"{component.BaseKey}{value}{value2}";
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, StateId.op_Implicit(text));
		}
	}
}
