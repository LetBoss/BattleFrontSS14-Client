using Content.Client.Message;
using Content.Shared.Radiation.Components;
using Content.Shared.Radiation.Systems;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Radiation.UI;

public sealed class GeigerItemControl : Control
{
	private readonly GeigerComponent _component;

	private readonly RichTextLabel _label;

	public GeigerItemControl(GeigerComponent component)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		_component = component;
		_label = new RichTextLabel
		{
			StyleClasses = { "ItemStatus" }
		};
		((Control)this).AddChild((Control)(object)_label);
		Update();
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		if (_component.UiUpdateNeeded)
		{
			Update();
		}
	}

	private void Update()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		string markup;
		if (_component.IsEnabled)
		{
			Color val = SharedGeigerSystem.LevelToColor(_component.DangerLevel);
			float currentRadiation = _component.CurrentRadiation;
			string item = currentRadiation.ToString("N1");
			markup = Loc.GetString("geiger-item-control-status", new(string, object)[2]
			{
				("rads", item),
				("color", val)
			});
		}
		else
		{
			markup = Loc.GetString("geiger-item-control-disabled");
		}
		_label.SetMarkup(markup);
		_component.UiUpdateNeeded = false;
	}
}
