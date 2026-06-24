using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls;

public sealed class HLine : Container
{
	private readonly PanelContainer _line;

	public Color? Color
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			StyleBox panelOverride = _line.PanelOverride;
			StyleBoxFlat val = (StyleBoxFlat)(object)((panelOverride is StyleBoxFlat) ? panelOverride : null);
			if (val != null)
			{
				return val.BackgroundColor;
			}
			return null;
		}
		set
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			StyleBox panelOverride = _line.PanelOverride;
			StyleBoxFlat val = (StyleBoxFlat)(object)((panelOverride is StyleBoxFlat) ? panelOverride : null);
			if (val != null)
			{
				val.BackgroundColor = value.Value;
			}
		}
	}

	public float? Thickness
	{
		get
		{
			StyleBox panelOverride = _line.PanelOverride;
			StyleBoxFlat val = (StyleBoxFlat)(object)((panelOverride is StyleBoxFlat) ? panelOverride : null);
			if (val != null)
			{
				return ((StyleBox)val).ContentMarginTopOverride;
			}
			return null;
		}
		set
		{
			StyleBox panelOverride = _line.PanelOverride;
			StyleBoxFlat val = (StyleBoxFlat)(object)((panelOverride is StyleBoxFlat) ? panelOverride : null);
			if (val != null)
			{
				((StyleBox)val).ContentMarginTopOverride = value.Value;
			}
		}
	}

	public HLine()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		_line = new PanelContainer();
		_line.PanelOverride = (StyleBox)new StyleBoxFlat();
		_line.PanelOverride.ContentMarginTopOverride = Thickness;
		((Control)this).AddChild((Control)(object)_line);
	}
}
