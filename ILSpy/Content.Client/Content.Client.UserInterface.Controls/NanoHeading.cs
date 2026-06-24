using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Controls;

public sealed class NanoHeading : Container
{
	private readonly Label _label;

	private readonly PanelContainer _panel;

	public string? Text
	{
		get
		{
			return _label.Text;
		}
		set
		{
			_label.Text = value;
		}
	}

	public NanoHeading()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_002f: Expected O, but got Unknown
		//IL_003a: Expected O, but got Unknown
		PanelContainer val = new PanelContainer();
		OrderedChildCollection children = ((Control)val).Children;
		Label val2 = new Label
		{
			StyleClasses = { "LabelHeading" }
		};
		Label val3 = val2;
		_label = val2;
		children.Add((Control)(object)val3);
		_panel = val;
		((Control)this).AddChild((Control)(object)_panel);
		((Control)this).HorizontalAlignment = (HAlignment)1;
	}
}
