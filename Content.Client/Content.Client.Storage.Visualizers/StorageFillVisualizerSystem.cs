using System;
using Content.Shared.Storage.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.Storage.Visualizers;

public sealed class StorageFillVisualizerSystem : VisualizerSystem<StorageFillVisualizerComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, StorageFillVisualizerComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		int value = default(int);
		if (args.Sprite != null && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<int>(uid, (Enum)StorageFillVisuals.FillLevel, ref value, args.Component))
		{
			string text = $"{component.FillBaseName}-{value}";
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)StorageFillLayers.Fill, StateId.op_Implicit(text));
		}
	}
}
