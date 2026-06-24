using System;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;

namespace Content.Client.Administration.UI.CustomControls;

public sealed class UICommandButton : CommandButton
{
	private DefaultWindow? _window;

	public Type? WindowType { get; set; }

	protected override void Execute(ButtonEventArgs obj)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		if (!(WindowType == null))
		{
			_window = (DefaultWindow)IoCManager.Resolve<IDynamicTypeFactory>().CreateInstance(WindowType, false, true);
			DefaultWindow? window = _window;
			if (window != null)
			{
				((BaseWindow)window).OpenCentered();
			}
		}
	}
}
