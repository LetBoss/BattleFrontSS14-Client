using System;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Actions.UI;

public sealed class ActionAlertTooltip : PanelContainer
{
	private const float TooltipTextMaxWidth = 350f;

	private readonly RichTextLabel _cooldownLabel;

	private readonly RichTextLabel _dynamicMessageLabel;

	private readonly IGameTiming _gameTiming;

	public (TimeSpan Start, TimeSpan End)? Cooldown { get; set; }

	public string? DynamicMessage { get; set; }

	public ActionAlertTooltip(FormattedMessage name, FormattedMessage? desc, string? requires = null, FormattedMessage? charges = null)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0037: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		//IL_0136: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		//IL_016e: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		_gameTiming = IoCManager.Resolve<IGameTiming>();
		((Control)this).SetOnlyStyleClass("tooltipBox");
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			RectClipContent = true
		};
		BoxContainer val2 = val;
		((Control)this).AddChild((Control)val);
		RichTextLabel val3 = new RichTextLabel
		{
			MaxWidth = 350f,
			StyleClasses = { "tooltipActionTitle" }
		};
		val3.SetMessage(name, (Color?)null);
		((Control)val2).AddChild((Control)(object)val3);
		if (desc != null && !string.IsNullOrWhiteSpace(((object)desc).ToString()))
		{
			RichTextLabel val4 = new RichTextLabel
			{
				MaxWidth = 350f,
				StyleClasses = { "tooltipActionDesc" }
			};
			val4.SetMessage(desc, (Color?)null);
			((Control)val2).AddChild((Control)(object)val4);
		}
		if (charges != null && !string.IsNullOrWhiteSpace(((object)charges).ToString()))
		{
			RichTextLabel val5 = new RichTextLabel
			{
				MaxWidth = 350f,
				StyleClasses = { "tooltipActionCharges" }
			};
			val5.SetMessage(charges, (Color?)null);
			((Control)val2).AddChild((Control)(object)val5);
		}
		RichTextLabel val6 = new RichTextLabel
		{
			MaxWidth = 350f,
			StyleClasses = { "tooltipActionCooldown" },
			Visible = false
		};
		RichTextLabel val7 = val6;
		_cooldownLabel = val6;
		((Control)val2).AddChild((Control)(object)val7);
		RichTextLabel val8 = new RichTextLabel
		{
			MaxWidth = 350f,
			StyleClasses = { "tooltipActionDynamicMessage" },
			Visible = false
		};
		val7 = val8;
		_dynamicMessageLabel = val8;
		((Control)val2).AddChild((Control)(object)val7);
		if (!string.IsNullOrWhiteSpace(requires))
		{
			RichTextLabel val9 = new RichTextLabel
			{
				MaxWidth = 350f,
				StyleClasses = { "tooltipActionCooldown" }
			};
			FormattedMessage val10 = default(FormattedMessage);
			if (FormattedMessage.TryFromMarkup("[color=#635c5c]" + requires + "[/color]", ref val10))
			{
				val9.SetMessage(val10, (Color?)null);
				((Control)val2).AddChild((Control)(object)val9);
			}
		}
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		if (!Cooldown.HasValue)
		{
			((Control)_cooldownLabel).Visible = false;
		}
		else
		{
			TimeSpan timeSpan = Cooldown.Value.End - _gameTiming.CurTime;
			if (timeSpan > TimeSpan.Zero)
			{
				TimeSpan timeSpan2 = Cooldown.Value.End - Cooldown.Value.Start;
				FormattedMessage val = default(FormattedMessage);
				if (FormattedMessage.TryFromMarkup(Loc.GetString("ui-actionslot-duration", new(string, object)[2]
				{
					("duration", (int)timeSpan2.TotalSeconds),
					("timeLeft", (int)timeSpan.TotalSeconds + 1)
				}), ref val))
				{
					_cooldownLabel.SetMessage(val, (Color?)null);
					((Control)_cooldownLabel).Visible = true;
				}
			}
			else
			{
				((Control)_cooldownLabel).Visible = false;
			}
		}
		FormattedMessage val2 = default(FormattedMessage);
		if (string.IsNullOrWhiteSpace(DynamicMessage))
		{
			((Control)_dynamicMessageLabel).Visible = false;
		}
		else if (FormattedMessage.TryFromMarkup("[color=#ffffff]" + DynamicMessage + "[/color]", ref val2))
		{
			_dynamicMessageLabel.SetMessage(val2, (Color?)null);
			((Control)_dynamicMessageLabel).Visible = true;
		}
	}
}
