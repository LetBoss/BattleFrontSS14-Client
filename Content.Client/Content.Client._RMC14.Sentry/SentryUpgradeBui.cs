using System;
using System.Numerics;
using Content.Client.UserInterface.Controls;
using Content.Shared._RMC14.Sentry;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.Sentry;

public sealed class SentryUpgradeBui : BoundUserInterface
{
	[Dependency]
	private IClyde _displayManager;

	[Dependency]
	private IEyeManager _eye;

	[Dependency]
	private IPrototypeManager _prototypes;

	private readonly SpriteSystem _sprite;

	private readonly TransformSystem _transform;

	private SentryUpgradeMenu? _menu;

	public SentryUpgradeBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<SentryUpgradeBui>(this);
		_sprite = base.EntMan.System<SpriteSystem>();
		_transform = base.EntMan.System<TransformSystem>();
	}

	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<SentryUpgradeMenu>((BoundUserInterface)(object)this);
		SentryComponent sentryComponent = default(SentryComponent);
		if (base.EntMan.TryGetComponent<SentryComponent>(((BoundUserInterface)this).Owner, ref sentryComponent))
		{
			EntProtoId[] upgrades = sentryComponent.Upgrades;
			if (upgrades != null)
			{
				EntProtoId[] array = upgrades;
				EntityPrototype val = default(EntityPrototype);
				foreach (EntProtoId upgradeId in array)
				{
					if (_prototypes.TryIndex(upgradeId, ref val))
					{
						RadialMenuTextureButton radialMenuTextureButton = new RadialMenuTextureButton();
						((Control)radialMenuTextureButton).StyleClasses.Add("RadialMenuButton");
						((Control)radialMenuTextureButton).SetSize = new Vector2(64f, 64f);
						((Control)radialMenuTextureButton).ToolTip = val.Name;
						RadialMenuTextureButton radialMenuTextureButton2 = radialMenuTextureButton;
						TextureRect val2 = new TextureRect
						{
							VerticalAlignment = (VAlignment)2,
							HorizontalAlignment = (HAlignment)2,
							Texture = _sprite.GetPrototypeIcon(val).GetFrame((RsiDirection)0, 0),
							TextureScale = new Vector2(2f, 2f)
						};
						((BaseButton)radialMenuTextureButton2).OnButtonDown += delegate
						{
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new SentryUpgradeBuiMsg(upgradeId));
						};
						((Control)radialMenuTextureButton2).AddChild((Control)(object)val2);
						((Control)_menu.Upgrades).AddChild((Control)(object)radialMenuTextureButton2);
					}
				}
			}
		}
		Vector2i screenSize = _displayManager.ScreenSize;
		Vector2 vector = _eye.WorldToScreen(((SharedTransformSystem)_transform).GetMapCoordinates(((BoundUserInterface)this).Owner, (TransformComponent)null).Position) / Vector2i.op_Implicit(screenSize);
		((BaseWindow)_menu).OpenCenteredAt(vector);
	}
}
