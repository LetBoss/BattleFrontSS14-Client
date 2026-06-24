using Robust.Client.UserInterface;

namespace Content.Client.UserInterface.Controls;

public sealed class VSpacer : Control
{
	public float Spacing
	{
		get
		{
			return ((Control)this).MinWidth;
		}
		set
		{
			((Control)this).MinWidth = value;
		}
	}

	public VSpacer()
	{
		((Control)this).MinWidth = Spacing;
	}

	public VSpacer(float width = 5f)
	{
		Spacing = width;
		((Control)this).MinWidth = width;
	}
}
