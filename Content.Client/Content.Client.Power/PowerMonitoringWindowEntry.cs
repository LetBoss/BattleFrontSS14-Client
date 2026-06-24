using Content.Shared.Power;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Power;

public sealed class PowerMonitoringWindowEntry : PowerMonitoringWindowBaseEntry
{
	public BoxContainer MainContainer;

	public BoxContainer SourcesContainer;

	public BoxContainer LoadsContainer;

	public PowerMonitoringWindowEntry(PowerMonitoringConsoleEntry entry)
		: base(entry)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		Entry = entry;
		((BoxContainer)this).Orientation = (LayoutOrientation)1;
		((Control)this).HorizontalExpand = true;
		((Control)Button).StyleClasses.Add("OpenLeft");
		((Control)this).AddChild((Control)(object)Button);
		MainContainer = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			Margin = new Thickness(8f, 0f, 0f, 0f),
			Visible = false
		};
		((Control)this).AddChild((Control)(object)MainContainer);
		SourcesContainer = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true
		};
		((Control)MainContainer).AddChild((Control)(object)SourcesContainer);
		LoadsContainer = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true
		};
		((Control)MainContainer).AddChild((Control)(object)LoadsContainer);
	}
}
