using System;
using System.Numerics;
using Content.Client.UserInterface.Controls;
using Content.Shared._RMC14.Construction.Upgrades;
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

namespace Content.Client._RMC14.Construction.Upgrades;

public sealed class RMCConstructionUpgradeBui : BoundUserInterface
{
	[Dependency]
	private IClyde _displayManager;

	[Dependency]
	private IEyeManager _eye;

	[Dependency]
	private IPrototypeManager _prototypes;

	private readonly SpriteSystem _sprite;

	private readonly TransformSystem _transform;

	private RMCConstructionUpgradeMenu? _menu;

	public RMCConstructionUpgradeBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<RMCConstructionUpgradeBui>(this);
		_sprite = base.EntMan.System<SpriteSystem>();
		_transform = base.EntMan.System<TransformSystem>();
	}

	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
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
		_menu = BoundUserInterfaceExt.CreateWindow<RMCConstructionUpgradeMenu>((BoundUserInterface)(object)this);
		RMCConstructionUpgradeTargetComponent rMCConstructionUpgradeTargetComponent = default(RMCConstructionUpgradeTargetComponent);
		if (base.EntMan.TryGetComponent<RMCConstructionUpgradeTargetComponent>(((BoundUserInterface)this).Owner, ref rMCConstructionUpgradeTargetComponent))
		{
			EntProtoId[] upgrades = rMCConstructionUpgradeTargetComponent.Upgrades;
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
							((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCConstructionUpgradeBuiMsg(upgradeId));
						};
						((Control)radialMenuTextureButton2).AddChild((Control)(object)val2);
						((Control)_menu.Upgrades).AddChild((Control)(object)radialMenuTextureButton2);
					}
				}
			}
		}
		if (!base.EntMan.Deleted(((BoundUserInterface)this).Owner))
		{
			Vector2i screenSize = _displayManager.ScreenSize;
			Vector2 vector = _eye.WorldToScreen(((SharedTransformSystem)_transform).GetMapCoordinates(((BoundUserInterface)this).Owner, (TransformComponent)null).Position) / Vector2i.op_Implicit(screenSize);
			((BaseWindow)_menu).OpenCenteredAt(vector);
		}
	}
}
