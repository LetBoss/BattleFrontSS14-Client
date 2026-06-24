using Content.Shared.Power;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Power;

public sealed class PowerMonitoringWindowSubEntry : PowerMonitoringWindowBaseEntry
{
	public TextureRect? Icon;

	public PowerMonitoringWindowSubEntry(PowerMonitoringConsoleEntry entry)
		: base(entry)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		((BoxContainer)this).Orientation = (LayoutOrientation)0;
		((Control)this).HorizontalExpand = true;
		Icon = new TextureRect
		{
			VerticalAlignment = (VAlignment)2,
			Margin = new Thickness(0f, 0f, 2f, 0f)
		};
		((Control)this).AddChild((Control)(object)Icon);
		((Control)Button).StyleClasses.Add("OpenBoth");
		((Control)this).AddChild((Control)(object)Button);
	}
}
