using System;
using Content.Shared.Paper;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.Paper.UI;

public sealed class PaperVisualizerSystem : VisualizerSystem<PaperVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, PaperVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite == null)
		{
			return;
		}
		PaperComponent.PaperStatus paperStatus = default(PaperComponent.PaperStatus);
		if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<PaperComponent.PaperStatus>(uid, (Enum)PaperComponent.PaperVisuals.Status, ref paperStatus, args.Component))
		{
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PaperVisualLayers.Writing, paperStatus == PaperComponent.PaperStatus.Written);
		}
		string text = default(string);
		if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<string>(uid, (Enum)PaperComponent.PaperVisuals.Stamp, ref text, args.Component))
		{
			if (text != string.Empty)
			{
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PaperVisualLayers.Stamp, StateId.op_Implicit(text));
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PaperVisualLayers.Stamp, true);
			}
			else
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PaperVisualLayers.Stamp, false);
			}
		}
	}
}
