using System;
using Content.Shared.Singularity.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.ParticleAccelerator;

public sealed class ParticleAcceleratorPartVisualizerSystem : VisualizerSystem<ParticleAcceleratorPartVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, ParticleAcceleratorPartVisualsComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if (args.Sprite != null && base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ParticleAcceleratorVisualLayers.Unlit, ref num, false))
		{
			ParticleAcceleratorVisualState particleAcceleratorVisualState = default(ParticleAcceleratorVisualState);
			if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<ParticleAcceleratorVisualState>(uid, (Enum)ParticleAcceleratorVisuals.VisualState, ref particleAcceleratorVisualState, args.Component))
			{
				particleAcceleratorVisualState = ParticleAcceleratorVisualState.Unpowered;
			}
			if (particleAcceleratorVisualState != ParticleAcceleratorVisualState.Unpowered)
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, true);
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, StateId.op_Implicit(comp.StateBase + comp.StatesSuffixes[particleAcceleratorVisualState]));
			}
			else
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, false);
			}
		}
	}
}
