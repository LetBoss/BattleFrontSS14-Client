using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Client.UserInterface.Controls;

public sealed class MonotoneButton : Button
{
	[ViewVariables]
	public Color AltTextColor { get; set; } = new Color(0.2f, 0.2f, 0.2f, 1f);

	public MonotoneButton()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).RemoveStyleClass("button");
		UpdateAppearance();
	}

	private void UpdateAppearance()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (((Button)this).Label != null)
		{
			((Control)((Button)this).Label).ModulateSelfOverride = (((int)((BaseButton)this).DrawMode == 1) ? new Color?(AltTextColor) : ((Color?)null));
		}
		((Control)this).Modulate = (((BaseButton)this).Disabled ? Color.Gray : Color.White);
	}

	protected override void StylePropertiesChanged()
	{
		((Button)this).StylePropertiesChanged();
		UpdateAppearance();
	}

	protected override void DrawModeChanged()
	{
		((ContainerButton)this).DrawModeChanged();
		UpdateAppearance();
	}
}
