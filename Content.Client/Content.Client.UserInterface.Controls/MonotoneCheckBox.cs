using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls;

public sealed class MonotoneCheckBox : CheckBox
{
	public const string StyleClassMonotoneCheckBox = "monotoneCheckBox";

	public MonotoneCheckBox()
	{
		((Control)((CheckBox)this).TextureRect).AddStyleClass("monotoneCheckBox");
	}

	protected override void DrawModeChanged()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		((CheckBox)this).DrawModeChanged();
		((Control)this).Modulate = (((BaseButton)this).Disabled ? Color.Gray : Color.White);
	}
}
