using System;
using System.Numerics;
using Content.Client.UserInterface.Controls;
using Content.Shared._RMC14.UniformAccessories;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.UniformAccessories;

public sealed class UniformAccessoryBui : BoundUserInterface
{
	[Dependency]
	private IClyde _displayManager;

	[Dependency]
	private IEyeManager _eye;

	private readonly TransformSystem _transform;

	private readonly SharedContainerSystem _container;

	private UniformAccessoryMenu? _menu;

	public UniformAccessoryBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<UniformAccessoryBui>(this);
		_transform = base.EntMan.System<TransformSystem>();
		_container = base.EntMan.System<SharedContainerSystem>();
	}

	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<UniformAccessoryMenu>((BoundUserInterface)(object)this);
		if (!base.EntMan.Deleted(((BoundUserInterface)this).Owner))
		{
			Refresh();
		}
	}

	public void Refresh()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		UniformAccessoryHolderComponent uniformAccessoryHolderComponent = default(UniformAccessoryHolderComponent);
		BaseContainer val = default(BaseContainer);
		if (_menu == null || !base.EntMan.TryGetComponent<UniformAccessoryHolderComponent>(((BoundUserInterface)this).Owner, ref uniformAccessoryHolderComponent) || !_container.TryGetContainer(((BoundUserInterface)this).Owner, uniformAccessoryHolderComponent.ContainerId, ref val, (ContainerManagerComponent)null))
		{
			return;
		}
		UniformAccessoryMenu? menu = _menu;
		if (menu != null)
		{
			((Control)menu.Accessories).Children.Clear();
		}
		MetaDataComponent val2 = default(MetaDataComponent);
		foreach (EntityUid containedEntity in val.ContainedEntities)
		{
			if (base.EntMan.TryGetComponent<MetaDataComponent>(containedEntity, ref val2))
			{
				RadialMenuTextureButton radialMenuTextureButton = new RadialMenuTextureButton();
				((Control)radialMenuTextureButton).StyleClasses.Add("RadialMenuButton");
				((Control)radialMenuTextureButton).SetSize = new Vector2(64f, 64f);
				((Control)radialMenuTextureButton).ToolTip = val2.EntityName;
				RadialMenuTextureButton radialMenuTextureButton2 = radialMenuTextureButton;
				SpriteView val3 = new SpriteView
				{
					OverrideDirection = (Direction)0,
					Scale = new Vector2(2f, 2f),
					MaxSize = new Vector2(112f, 112f),
					Stretch = (StretchMode)2
				};
				val3.SetEntity((EntityUid?)containedEntity);
				NetEntity netEnt = base.EntMan.GetNetEntity(containedEntity, (MetaDataComponent)null);
				((BaseButton)radialMenuTextureButton2).OnButtonDown += delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new UniformAccessoriesBuiMsg(netEnt));
				};
				((Control)radialMenuTextureButton2).AddChild((Control)(object)val3);
				UniformAccessoryMenu? menu2 = _menu;
				if (menu2 != null)
				{
					((Control)menu2.Accessories).AddChild((Control)(object)radialMenuTextureButton2);
				}
			}
		}
		Vector2i screenSize = _displayManager.ScreenSize;
		Vector2 vector = _eye.WorldToScreen(((SharedTransformSystem)_transform).GetMapCoordinates(((BoundUserInterface)this).Owner, (TransformComponent)null).Position) / Vector2i.op_Implicit(screenSize);
		UniformAccessoryMenu? menu3 = _menu;
		if (menu3 != null)
		{
			((BaseWindow)menu3).OpenCenteredAt(vector);
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (((BoundUserInterface)this).State is UniformAccessoriesBuiState)
		{
			Refresh();
		}
	}
}
