// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Pheromones.XenoPheromonesMenu
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using CompiledRobustXaml;
using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.XAML.Proxy;
using Robust.Shared.IoC;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Content.Client._RMC14.Xenonids.Pheromones;

[XamlMetadata("resm:Content.Client._RMC14.Xenonids.Pheromones.XenoPheromonesMenu.xaml?assembly=Content.Client", "Content.Client._RMC14.Xenonids.Pheromones.XenoPheromonesMenu.xaml", "<ui:RadialMenu\r\n    xmlns:ui=\"clr-namespace:Content.Client.UserInterface.Controls\"\r\n    CloseButtonStyleClass=\"RadialMenuCloseButton\"\r\n    VerticalExpand=\"True\"\r\n    HorizontalExpand=\"True\">\r\n    <ui:RadialContainer Name=\"Main\">\r\n    </ui:RadialContainer>\r\n</ui:RadialMenu>\r\n")]
public sealed class XenoPheromonesMenu : RadialMenu
{
  public XenoPheromonesMenu() => XenoPheromonesMenu.\u0021XamlIlPopulateTrampoline(this);

  public static void Populate\u003AContent\u002EClient\u002E_RMC14\u002EXenonids\u002EPheromones\u002EXenoPheromonesMenu\u002Examl(
    [In] IServiceProvider obj0,
    [In] RadialMenu obj1)
  {
    XamlIlContext.Context<RadialMenu> context = new XamlIlContext.Context<RadialMenu>(obj0, (object[]) null, "resm:Content.Client._RMC14.Xenonids.Pheromones.XenoPheromonesMenu.xaml?assembly=Content.Client");
    context.RootObject = obj1;
    context.IntermediateRoot = (object) obj1;
    RadialMenu radialMenu = obj1;
    radialMenu.CloseButtonStyleClass = "RadialMenuCloseButton";
    ((Control) radialMenu).VerticalExpand = true;
    ((Control) radialMenu).HorizontalExpand = true;
    RadialContainer radialContainer = new RadialContainer();
    ((Control) radialContainer).Name = "Main";
    Control control1 = (Control) radialContainer;
    context.RobustNameScope.Register("Main", control1);
    ((Control) radialMenu).XamlChildren.Add((Control) radialContainer);
    if (radialMenu is Control control2)
    {
      context.RobustNameScope.Absorb(control2.NameScope);
      control2.NameScope = context.RobustNameScope;
    }
    context.RobustNameScope.Complete();
  }

  private static void \u0021XamlIlPopulateTrampoline([In] XenoPheromonesMenu obj0)
  {
    if (IoCManager.Resolve<IXamlProxyHelper>().Populate(typeof (XenoPheromonesMenu), (object) obj0))
      return;
    XenoPheromonesMenu.Populate\u003AContent\u002EClient\u002E_RMC14\u002EXenonids\u002EPheromones\u002EXenoPheromonesMenu\u002Examl((IServiceProvider) null, (RadialMenu) obj0);
  }
}
