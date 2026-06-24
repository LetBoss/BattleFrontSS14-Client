using System;
using System.Collections.Generic;
using Content.Shared.Mobs;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.DamageState;

public sealed class DamageStateVisualizerSystem : VisualizerSystem<DamageStateVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, DamageStateVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		MobState mobState = default(MobState);
		if (sprite == null || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<MobState>(uid, (Enum)MobStateVisuals.State, ref mobState, args.Component) || !component.States.TryGetValue(mobState, out Dictionary<DamageStateVisualLayers, string> value))
		{
			return;
		}
		DamageStateVisualLayers[] array = new DamageStateVisualLayers[2]
		{
			DamageStateVisualLayers.Base,
			DamageStateVisualLayers.BaseUnshaded
		};
		int i;
		int num = default(int);
		for (i = 0; i < array.Length; i++)
		{
			DamageStateVisualLayers damageStateVisualLayers = array[i];
			if (base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)damageStateVisualLayers, ref num, false))
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)damageStateVisualLayers, false);
			}
		}
		foreach (var (damageStateVisualLayers3, text2) in value)
		{
			if (base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)damageStateVisualLayers3, ref i, false))
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)damageStateVisualLayers3, true);
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)damageStateVisualLayers3, StateId.op_Implicit(text2));
			}
		}
		if (mobState == MobState.Dead)
		{
			if (sprite.DrawDepth > -4)
			{
				component.OriginalDrawDepth = sprite.DrawDepth;
				base.SpriteSystem.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, sprite)), -4);
			}
		}
		else if (component.OriginalDrawDepth.HasValue)
		{
			base.SpriteSystem.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, sprite)), component.OriginalDrawDepth.Value);
			component.OriginalDrawDepth = null;
		}
	}
}
