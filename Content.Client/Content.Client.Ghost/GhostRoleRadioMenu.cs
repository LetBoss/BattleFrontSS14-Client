using System;
using System.Numerics;
using CompiledRobustXaml;
using Content.Client.UserInterface.Controls;
using Content.Shared.Ghost.Roles;
using Content.Shared.Ghost.Roles.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML.Proxy;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Client.Ghost;

[XamlMetadata("resm:Content.Client.Ghost.GhostRoleRadioMenu.xaml?assembly=Content.Client", "Content.Client.Ghost.GhostRoleRadioMenu.xaml", "<ui:RadialMenu\r\n    xmlns:ui=\"clr-namespace:Content.Client.UserInterface.Controls\"\r\n    CloseButtonStyleClass=\"RadialMenuCloseButton\"\r\n    VerticalExpand=\"True\"\r\n    HorizontalExpand=\"True\">\r\n    <ui:RadialContainer Name=\"Main\">\r\n    </ui:RadialContainer>\r\n</ui:RadialMenu>\r\n")]
public sealed class GhostRoleRadioMenu : RadialMenu
{
	[Dependency]
	private EntityManager _entityManager;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	public EntityUid Entity { get; set; }

	public event Action<ProtoId<GhostRolePrototype>>? SendGhostRoleRadioMessageAction;

	public GhostRoleRadioMenu()
	{
		IoCManager.InjectDependencies<GhostRoleRadioMenu>(this);
		_0021XamlIlPopulateTrampoline(this);
	}

	public void SetEntity(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Entity = uid;
		RefreshUI();
	}

	private void RefreshUI()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		RadialContainer radialContainer = ((Control)this).FindControl<RadialContainer>("Main");
		GhostRoleMobSpawnerComponent ghostRoleMobSpawnerComponent = default(GhostRoleMobSpawnerComponent);
		if (!_entityManager.TryGetComponent<GhostRoleMobSpawnerComponent>(Entity, ref ghostRoleMobSpawnerComponent))
		{
			return;
		}
		GhostRolePrototype ghostRolePrototype = default(GhostRolePrototype);
		EntityPrototype val2 = default(EntityPrototype);
		foreach (string selectablePrototype in ghostRoleMobSpawnerComponent.SelectablePrototypes)
		{
			if (_prototypeManager.TryIndex<GhostRolePrototype>(selectablePrototype, ref ghostRolePrototype))
			{
				GhostRoleRadioMenuButton ghostRoleRadioMenuButton = new GhostRoleRadioMenuButton();
				((Control)ghostRoleRadioMenuButton).SetSize = new Vector2(64f, 64f);
				((Control)ghostRoleRadioMenuButton).ToolTip = Loc.GetString(ghostRolePrototype.Name);
				ghostRoleRadioMenuButton.ProtoId = ProtoId<GhostRolePrototype>.op_Implicit(ghostRolePrototype.ID);
				GhostRoleRadioMenuButton ghostRoleRadioMenuButton2 = ghostRoleRadioMenuButton;
				EntityPrototypeView val = new EntityPrototypeView
				{
					SetSize = new Vector2(48f, 48f),
					VerticalAlignment = (VAlignment)2,
					HorizontalAlignment = (HAlignment)2,
					Stretch = (StretchMode)2
				};
				if (_prototypeManager.TryIndex(ghostRolePrototype.IconPrototype, ref val2))
				{
					val.SetPrototype((EntProtoId?)EntProtoId.op_Implicit(val2));
				}
				else
				{
					val.SetPrototype((EntProtoId?)ghostRolePrototype.EntityPrototype);
				}
				((Control)ghostRoleRadioMenuButton2).AddChild((Control)(object)val);
				((Control)radialContainer).AddChild((Control)(object)ghostRoleRadioMenuButton2);
				AddGhostRoleRadioMenuButtonOnClickActions((Control)(object)radialContainer);
			}
		}
	}

	private unsafe void AddGhostRoleRadioMenuButtonOnClickActions(Control control)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!(control is RadialContainer radialContainer))
		{
			return;
		}
		Enumerator enumerator = ((Control)radialContainer).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator)).Current;
				GhostRoleRadioMenuButton castChild = current as GhostRoleRadioMenuButton;
				if (castChild != null)
				{
					((BaseButton)castChild).OnButtonUp += delegate
					{
						//IL_0017: Unknown result type (might be due to invalid IL or missing references)
						this.SendGhostRoleRadioMessageAction?.Invoke(castChild.ProtoId);
						((BaseWindow)this).Close();
					};
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public static void Populate_003AContent_002EClient_002EGhost_002EGhostRoleRadioMenu_002Examl(IServiceProvider P_0, RadialMenu P_1)
	{
		XamlIlContext.Context<RadialMenu> context = new XamlIlContext.Context<RadialMenu>(P_0, null, "resm:Content.Client.Ghost.GhostRoleRadioMenu.xaml?assembly=Content.Client");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		P_1.CloseButtonStyleClass = "RadialMenuCloseButton";
		((Control)P_1).VerticalExpand = true;
		((Control)P_1).HorizontalExpand = true;
		RadialContainer radialContainer = new RadialContainer();
		((Control)radialContainer).Name = "Main";
		Control val = (Control)(object)radialContainer;
		context.RobustNameScope.Register("Main", val);
		val = (Control)(object)radialContainer;
		((Control)P_1).XamlChildren.Add(val);
		if ((val = (Control)(object)((P_1 is Control) ? P_1 : null)) != null)
		{
			context.RobustNameScope.Absorb(val.NameScope);
			val.NameScope = context.RobustNameScope;
		}
		context.RobustNameScope.Complete();
	}

	private static void _0021XamlIlPopulateTrampoline(GhostRoleRadioMenu P_0)
	{
		if (!IoCManager.Resolve<IXamlProxyHelper>().Populate(typeof(GhostRoleRadioMenu), (object)P_0))
		{
			Populate_003AContent_002EClient_002EGhost_002EGhostRoleRadioMenu_002Examl(null, P_0);
		}
	}
}
