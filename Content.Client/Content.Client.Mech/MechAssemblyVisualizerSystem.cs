using System;
using Content.Shared.Mech;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.Mech;

public sealed class MechAssemblyVisualizerSystem : VisualizerSystem<MechAssemblyVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, MechAssemblyVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if (args.Sprite != null && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<int>(uid, (Enum)MechAssemblyVisuals.State, ref num, args.Component))
		{
			string text = component.StatePrefix + num;
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, StateId.op_Implicit(text));
		}
	}
}
