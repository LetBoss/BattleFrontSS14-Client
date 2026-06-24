using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Controls;

public sealed class Placeholder : PanelContainer
{
	public const string StyleClassPlaceholderText = "PlaceholderText";

	private readonly Label _label;

	public string? PlaceholderText
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

	public Placeholder()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		_label = new Label
		{
			VerticalAlignment = (VAlignment)0,
			Align = (AlignMode)1,
			VAlign = (VAlignMode)1
		};
		((Control)_label).AddStyleClass("PlaceholderText");
		((Control)this).AddChild((Control)(object)_label);
	}
}
