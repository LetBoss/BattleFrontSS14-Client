using Content.Shared.Smoking;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.Smoking;

public sealed class BurnStateVisualizerSystem : VisualizerSystem<BurnStateVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, BurnStateVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite == null || !args.AppearanceData.TryGetValue(SmokingVisuals.Smoking, out var value))
		{
			return;
		}
		if (!(value is SmokableState smokableState))
		{
			goto IL_004a;
		}
		string text;
		if (smokableState != SmokableState.Lit)
		{
			if (smokableState != SmokableState.Burnt)
			{
				goto IL_004a;
			}
			text = component.BurntIcon;
		}
		else
		{
			text = component.LitIcon;
		}
		goto IL_0051;
		IL_0051:
		string text2 = text;
		base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, StateId.op_Implicit(text2));
		return;
		IL_004a:
		text = component.UnlitIcon;
		goto IL_0051;
	}
}
