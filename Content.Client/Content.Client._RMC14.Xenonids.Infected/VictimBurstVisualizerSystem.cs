using System;
using Content.Shared._RMC14.Xenonids.Parasite;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Xenonids.Infected;

public sealed class VictimBurstVisualizerSystem : VisualizerSystem<VictimBurstComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, VictimBurstComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		base.OnAppearanceChange(uid, component, ref args);
		VictimBurstState victimBurstState = default(VictimBurstState);
		if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<VictimBurstState>(uid, (Enum)BurstVisuals.Visuals, ref victimBurstState, args.Component) || args.Sprite == null)
		{
			return;
		}
		ResPath rsiPath = component.RsiPath;
		string text = victimBurstState switch
		{
			VictimBurstState.Bursting => component.BurstingState, 
			VictimBurstState.Burst => component.BurstState, 
			_ => null, 
		};
		if (!string.IsNullOrWhiteSpace(text))
		{
			int num = default(int);
			if (!base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)BurstLayer.Base, ref num, false))
			{
				num = base.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)BurstLayer.Base);
				base.SpriteSystem.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, rsiPath, (StateId?)null);
			}
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, StateId.op_Implicit(text));
		}
	}
}
