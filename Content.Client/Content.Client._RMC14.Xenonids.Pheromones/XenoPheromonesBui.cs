using System;
using System.Numerics;
using Content.Client.UserInterface.Controls;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Pheromones;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Xenonids.Pheromones;

public sealed class XenoPheromonesBui : BoundUserInterface
{
	[Dependency]
	private IClyde _displayManager;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IEyeManager _eye;

	private readonly SpriteSystem _sprite;

	private readonly TransformSystem _transform;

	private readonly SharedXenoPheromonesSystem _pheros;

	[ViewVariables]
	private XenoPheromonesMenu? _xenoPheromonesMenu;

	private const string HelpButtonTexture = "radial_help";

	public XenoPheromonesBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<XenoPheromonesBui>(this);
		_sprite = base.EntMan.System<SpriteSystem>();
		_transform = base.EntMan.System<TransformSystem>();
		_pheros = base.EntMan.System<SharedXenoPheromonesSystem>();
	}

	protected override void Open()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_xenoPheromonesMenu = BoundUserInterfaceExt.CreateWindow<XenoPheromonesMenu>((BoundUserInterface)(object)this);
		RadialContainer radialContainer = ((Control)_xenoPheromonesMenu).FindControl<RadialContainer>("Main");
		if (base.EntMan.HasComponent<XenoComponent>(((BoundUserInterface)this).Owner))
		{
			TextureRect val = new TextureRect
			{
				VerticalAlignment = (VAlignment)2,
				HorizontalAlignment = (HAlignment)2,
				Texture = _sprite.Frame0((SpriteSpecifier)new Rsi(new ResPath("/Textures/_RMC14/Interface/radial.rsi"), "radial_help")),
				TextureScale = new Vector2(2f, 2f)
			};
			RadialMenuTextureButton radialMenuTextureButton = new RadialMenuTextureButton();
			((Control)radialMenuTextureButton).StyleClasses.Add("RadialMenuButton");
			((Control)radialMenuTextureButton).SetSize = new Vector2(64f, 64f);
			RadialMenuTextureButton radialMenuTextureButton2 = radialMenuTextureButton;
			((BaseButton)radialMenuTextureButton2).OnButtonDown += delegate
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new XenoPheromonesHelpButtonBuiMsg());
			};
			((Control)radialMenuTextureButton2).AddChild((Control)(object)val);
			((Control)radialContainer).AddChild((Control)(object)radialMenuTextureButton2);
			AddPheromonesButton(XenoPheromones.Frenzy, radialContainer, ((BoundUserInterface)this).Owner);
			AddPheromonesButton(XenoPheromones.Warding, radialContainer, ((BoundUserInterface)this).Owner);
			AddPheromonesButton(XenoPheromones.Recovery, radialContainer, ((BoundUserInterface)this).Owner);
		}
		Vector2i screenSize = _displayManager.ScreenSize;
		Vector2 vector = _inputManager.MouseScreenPosition.Position / Vector2i.op_Implicit(screenSize);
		EyeComponent val2 = default(EyeComponent);
		if (base.EntMan.TryGetComponent<EyeComponent>(((BoundUserInterface)this).Owner, ref val2) && val2.Target.HasValue)
		{
			vector = _eye.WorldToScreen(((SharedTransformSystem)_transform).GetMapCoordinates(val2.Target.Value, (TransformComponent)null).Position) / Vector2i.op_Implicit(screenSize);
		}
		else
		{
			EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
			if (localEntity.HasValue)
			{
				EntityUid valueOrDefault = localEntity.GetValueOrDefault();
				vector = _eye.WorldToScreen(((SharedTransformSystem)_transform).GetMapCoordinates(valueOrDefault, (TransformComponent)null).Position) / Vector2i.op_Implicit(screenSize);
			}
		}
		((BaseWindow)_xenoPheromonesMenu).OpenCenteredAt(vector);
	}

	private void AddPheromonesButton(XenoPheromones pheromone, RadialContainer parent, EntityUid owner)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		string text = pheromone.ToString().ToLowerInvariant();
		string text2 = _pheros.GetPheroSuffix(Entity<XenoPheromonesComponent>.op_Implicit((ValueTuple<EntityUid, XenoPheromonesComponent>)(owner, null)));
		if (text2 != null)
		{
			text2 = "_" + text2;
		}
		TextureRect val = new TextureRect
		{
			VerticalAlignment = (VAlignment)2,
			HorizontalAlignment = (HAlignment)2,
			Texture = _sprite.Frame0((SpriteSpecifier)new Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_pheromones.rsi"), text + text2)),
			TextureScale = new Vector2(2f, 2f)
		};
		RadialMenuTextureButton radialMenuTextureButton = new RadialMenuTextureButton();
		((Control)radialMenuTextureButton).StyleClasses.Add("RadialMenuButton");
		((Control)radialMenuTextureButton).SetSize = new Vector2(64f, 64f);
		((Control)radialMenuTextureButton).ToolTip = text;
		RadialMenuTextureButton radialMenuTextureButton2 = radialMenuTextureButton;
		((BaseButton)radialMenuTextureButton2).OnButtonDown += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new XenoPheromonesChosenBuiMsg(pheromone));
		};
		((Control)radialMenuTextureButton2).AddChild((Control)(object)val);
		((Control)parent).AddChild((Control)(object)radialMenuTextureButton2);
	}
}
