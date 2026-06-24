using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Controls;

public sealed class HighDivider : Control
{
	public HighDivider()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		((Control)this).Children.Add((Control)new PanelContainer
		{
			StyleClasses = { "HighDivider" }
		});
	}
}
