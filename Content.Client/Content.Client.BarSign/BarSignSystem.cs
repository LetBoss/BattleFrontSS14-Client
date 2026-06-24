using System;
using Content.Client.BarSign.Ui;
using Content.Shared.BarSign;
using Content.Shared.Power;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.BarSign;

public sealed class BarSignSystem : VisualizerSystem<BarSignComponent>
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private UserInterfaceSystem _ui;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<BarSignComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<BarSignComponent, AfterAutoHandleStateEvent>)OnAfterAutoHandleState, (Type[])null, (Type[])null);
	}

	private void OnAfterAutoHandleState(EntityUid uid, BarSignComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		BarSignBoundUserInterface barSignBoundUserInterface = default(BarSignBoundUserInterface);
		if (((SharedUserInterfaceSystem)_ui).TryGetOpenUi<BarSignBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)BarSignUiKey.Key, ref barSignBoundUserInterface))
		{
			barSignBoundUserInterface.Update(component.Current);
		}
		UpdateAppearance(uid, component);
	}

	protected override void OnAppearanceChange(EntityUid uid, BarSignComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(uid, component, args.Component, args.Sprite);
	}

	private void UpdateAppearance(EntityUid id, BarSignComponent sign, AppearanceComponent? appearance = null, SpriteComponent? sprite = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<AppearanceComponent, SpriteComponent>(id, ref appearance, ref sprite, true))
		{
			bool flag = default(bool);
			((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(id, (Enum)PowerDeviceVisuals.Powered, ref flag, appearance);
			BarSignPrototype barSignPrototype = default(BarSignPrototype);
			if (flag && sign.Current.HasValue && _prototypeManager.TryIndex<BarSignPrototype>(sign.Current, ref barSignPrototype))
			{
				base.SpriteSystem.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((id, sprite)), 0, barSignPrototype.Icon);
				sprite.LayerSetShader(0, "unshaded");
			}
			else
			{
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((id, sprite)), 0, StateId.op_Implicit("empty"));
				sprite.LayerSetShader(0, (ShaderInstance)null, (string)null);
			}
		}
	}
}
