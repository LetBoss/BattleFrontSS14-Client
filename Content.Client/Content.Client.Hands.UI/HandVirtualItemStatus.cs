using System;
using CompiledRobustXaml;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML.Proxy;
using Robust.Shared.IoC;

namespace Content.Client.Hands.UI;

[XamlMetadata("resm:Content.Client.Hands.UI.HandVirtualItemStatus.xaml?assembly=Content.Client", "Content.Client.Hands.UI.HandVirtualItemStatus.xaml", "<Control xmlns=\"https://spacestation14.io\">\r\n    <Label StyleClasses=\"ItemStatus\" Text=\"{Loc 'hands-system-blocked-by'}\" />\r\n</Control>\r\n")]
public sealed class HandVirtualItemStatus : Control
{
	public HandVirtualItemStatus()
	{
		_0021XamlIlPopulateTrampoline(this);
	}

	public static void Populate_003AContent_002EClient_002EHands_002EUI_002EHandVirtualItemStatus_002Examl(IServiceProvider P_0, Control P_1)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		XamlIlContext.Context<Control> context = new XamlIlContext.Context<Control>(P_0, null, "resm:Content.Client.Hands.UI.HandVirtualItemStatus.xaml?assembly=Content.Client");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		Label val = new Label();
		string item = "ItemStatus";
		((Control)val).StyleClasses.Add(item);
		val.Text = (string)new LocExtension("hands-system-blocked-by").ProvideValue();
		Control item2 = (Control)val;
		P_1.XamlChildren.Add(item2);
		if ((item2 = ((P_1 is Control) ? P_1 : null)) != null)
		{
			context.RobustNameScope.Absorb(item2.NameScope);
			item2.NameScope = context.RobustNameScope;
		}
		context.RobustNameScope.Complete();
	}

	private static void _0021XamlIlPopulateTrampoline(HandVirtualItemStatus P_0)
	{
		if (!IoCManager.Resolve<IXamlProxyHelper>().Populate(typeof(HandVirtualItemStatus), (object)P_0))
		{
			Populate_003AContent_002EClient_002EHands_002EUI_002EHandVirtualItemStatus_002Examl(null, (Control)(object)P_0);
		}
	}
}
