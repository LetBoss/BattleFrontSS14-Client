using System;
using Content.Client.Power;
using Content.Shared.Lathe;
using Content.Shared.Power;
using Content.Shared.Research.Prototypes;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Lathe;

public sealed class LatheSystem : SharedLatheSystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<LatheComponent, AppearanceChangeEvent>((ComponentEventRefHandler<LatheComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnAppearanceChange(EntityUid uid, LatheComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite == null)
		{
			return;
		}
		bool flag = default(bool);
		int num = default(int);
		if (_appearance.TryGetData<bool>(uid, (Enum)LatheVisuals.IsRunning, ref flag, args.Component) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)LatheVisualLayers.IsRunning, ref num, false) && component.RunningState != null && component.IdleState != null)
		{
			string text = (flag ? component.RunningState : component.IdleState);
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, StateId.op_Implicit(text));
		}
		bool flag2 = default(bool);
		int num2 = default(int);
		if (_appearance.TryGetData<bool>(uid, (Enum)PowerDeviceVisuals.Powered, ref flag2, args.Component) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PowerDeviceVisualLayers.Powered, ref num2, false))
		{
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, flag2);
			if (component.UnlitIdleState != null && component.UnlitRunningState != null)
			{
				string text2 = (flag ? component.UnlitRunningState : component.UnlitIdleState);
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, StateId.op_Implicit(text2));
			}
		}
	}

	protected override bool HasRecipe(EntityUid uid, LatheRecipePrototype recipe, LatheComponent component)
	{
		return true;
	}
}
