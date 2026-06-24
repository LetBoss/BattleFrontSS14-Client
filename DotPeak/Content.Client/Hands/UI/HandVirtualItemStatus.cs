// Decompiled with JetBrains decompiler
// Type: Content.Client.Hands.UI.HandVirtualItemStatus
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using CompiledRobustXaml;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML.Proxy;
using Robust.Shared.IoC;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Content.Client.Hands.UI;

[XamlMetadata("resm:Content.Client.Hands.UI.HandVirtualItemStatus.xaml?assembly=Content.Client", "Content.Client.Hands.UI.HandVirtualItemStatus.xaml", "<Control xmlns=\"https://spacestation14.io\">\r\n    <Label StyleClasses=\"ItemStatus\" Text=\"{Loc 'hands-system-blocked-by'}\" />\r\n</Control>\r\n")]
public sealed class HandVirtualItemStatus : Control
{
  public HandVirtualItemStatus() => HandVirtualItemStatus.\u0021XamlIlPopulateTrampoline(this);

  public static void Populate\u003AContent\u002EClient\u002EHands\u002EUI\u002EHandVirtualItemStatus\u002Examl(
    [In] IServiceProvider obj0,
    [In] Control obj1)
  {
    XamlIlContext.Context<Control> context = new XamlIlContext.Context<Control>(obj0, (object[]) null, "resm:Content.Client.Hands.UI.HandVirtualItemStatus.xaml?assembly=Content.Client");
    context.RootObject = obj1;
    context.IntermediateRoot = (object) obj1;
    Control control1 = obj1;
    Label label = new Label();
    ((Control) label).StyleClasses.Add("ItemStatus");
    label.Text = (string) new LocExtension("hands-system-blocked-by").ProvideValue();
    control1.XamlChildren.Add((Control) label);
    if (control1 is Control control2)
    {
      context.RobustNameScope.Absorb(control2.NameScope);
      control2.NameScope = context.RobustNameScope;
    }
    context.RobustNameScope.Complete();
  }

  private static void \u0021XamlIlPopulateTrampoline([In] HandVirtualItemStatus obj0)
  {
    if (IoCManager.Resolve<IXamlProxyHelper>().Populate(typeof (HandVirtualItemStatus), (object) obj0))
      return;
    HandVirtualItemStatus.Populate\u003AContent\u002EClient\u002EHands\u002EUI\u002EHandVirtualItemStatus\u002Examl((IServiceProvider) null, (Control) obj0);
  }
}
