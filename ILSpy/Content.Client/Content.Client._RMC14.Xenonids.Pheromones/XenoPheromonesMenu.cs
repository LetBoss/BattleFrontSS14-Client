using System;
using CompiledRobustXaml;
using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.XAML.Proxy;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Xenonids.Pheromones;

[XamlMetadata("resm:Content.Client._RMC14.Xenonids.Pheromones.XenoPheromonesMenu.xaml?assembly=Content.Client", "Content.Client._RMC14.Xenonids.Pheromones.XenoPheromonesMenu.xaml", "<ui:RadialMenu\r\n    xmlns:ui=\"clr-namespace:Content.Client.UserInterface.Controls\"\r\n    CloseButtonStyleClass=\"RadialMenuCloseButton\"\r\n    VerticalExpand=\"True\"\r\n    HorizontalExpand=\"True\">\r\n    <ui:RadialContainer Name=\"Main\">\r\n    </ui:RadialContainer>\r\n</ui:RadialMenu>\r\n")]
public sealed class XenoPheromonesMenu : RadialMenu
{
	public XenoPheromonesMenu()
	{
		_0021XamlIlPopulateTrampoline(this);
	}

	public static void Populate_003AContent_002EClient_002E_RMC14_002EXenonids_002EPheromones_002EXenoPheromonesMenu_002Examl(IServiceProvider P_0, RadialMenu P_1)
	{
		XamlIlContext.Context<RadialMenu> context = new XamlIlContext.Context<RadialMenu>(P_0, null, "resm:Content.Client._RMC14.Xenonids.Pheromones.XenoPheromonesMenu.xaml?assembly=Content.Client");
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

	private static void _0021XamlIlPopulateTrampoline(XenoPheromonesMenu P_0)
	{
		if (!IoCManager.Resolve<IXamlProxyHelper>().Populate(typeof(XenoPheromonesMenu), (object)P_0))
		{
			Populate_003AContent_002EClient_002E_RMC14_002EXenonids_002EPheromones_002EXenoPheromonesMenu_002Examl(null, P_0);
		}
	}
}
