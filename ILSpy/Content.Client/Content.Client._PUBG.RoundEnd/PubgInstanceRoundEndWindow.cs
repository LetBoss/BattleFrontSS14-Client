using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared._PUBG.RoundEnd;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client._PUBG.RoundEnd;

public sealed class PubgInstanceRoundEndWindow : DefaultWindow
{
	private readonly RichTextLabel _summaryText;

	private readonly BoxContainer _partyEntriesContainer;

	public PubgInstanceRoundEndWindow()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Expected O, but got Unknown
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Expected O, but got Unknown
		((DefaultWindow)this).Title = Loc.GetString("pubg-roundend-title");
		((BaseWindow)this).Resizable = false;
		((Control)this).MinSize = new Vector2(840f, 620f);
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 8,
			Margin = new Thickness(10f),
			VerticalExpand = true,
			HorizontalExpand = true
		};
		TabContainer val2 = new TabContainer
		{
			VerticalExpand = true,
			HorizontalExpand = true
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			Name = Loc.GetString("pubg-roundend-tab-summary")
		};
		ScrollContainer val4 = new ScrollContainer
		{
			VerticalExpand = true,
			HorizontalExpand = true
		};
		_summaryText = new RichTextLabel
		{
			VerticalExpand = true,
			HorizontalExpand = true
		};
		((Control)val4).AddChild((Control)(object)_summaryText);
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val2).AddChild((Control)(object)val3);
		BoxContainer val5 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			Name = Loc.GetString("pubg-roundend-tab-party"),
			SeparationOverride = 6
		};
		Label val6 = new Label
		{
			Text = Loc.GetString("pubg-roundend-party-subtitle"),
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#B7C2D8", (Color?)null)
		};
		((Control)val5).AddChild((Control)(object)val6);
		((Control)val5).AddChild(BuildPartyHeaderRow());
		ScrollContainer val7 = new ScrollContainer
		{
			VerticalExpand = true,
			HorizontalExpand = true
		};
		_partyEntriesContainer = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 8,
			VerticalExpand = true,
			HorizontalExpand = true
		};
		((Control)val7).AddChild((Control)(object)_partyEntriesContainer);
		((Control)val5).AddChild((Control)(object)val7);
		((Control)val2).AddChild((Control)(object)val5);
		((Control)val).AddChild((Control)(object)val2);
		Button val8 = new Button
		{
			Text = Loc.GetString("pubg-roundend-close"),
			HorizontalAlignment = (HAlignment)2,
			MinSize = new Vector2(160f, 34f)
		};
		((BaseButton)val8).OnPressed += delegate
		{
			((BaseWindow)this).Close();
		};
		((Control)val).AddChild((Control)(object)val8);
		((DefaultWindow)this).Contents.AddChild((Control)(object)val);
	}

	public void SetTitleText(string titleKey)
	{
		((DefaultWindow)this).Title = Loc.GetString(titleKey);
	}

	public void SetRoundEndText(string text)
	{
		FormattedMessage val = default(FormattedMessage);
		if (FormattedMessage.TryFromMarkup(text, ref val))
		{
			_summaryText.SetMessage(val, (Color?)null);
		}
		else
		{
			_summaryText.SetMessage(text, (Color?)null);
		}
	}

	public void SetPartyEntries(List<PubgRoundEndPartyEntry> entries)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Expected O, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Expected O, but got Unknown
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		((Control)_partyEntriesContainer).RemoveAllChildren();
		if (entries.Count == 0)
		{
			((Control)_partyEntriesContainer).AddChild((Control)new Label
			{
				Text = Loc.GetString("pubg-roundend-party-empty"),
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#A3AEBD", (Color?)null)
			});
			return;
		}
		foreach (IGrouping<int, PubgRoundEndPartyEntry> item in (from entry in entries
			group entry by entry.PartyId into @group
			orderby (@group.Key > 0) ? @group.Key : int.MaxValue
			select @group).ToList())
		{
			PanelContainer val = new PanelContainer();
			StyleBoxFlat val2 = new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#1B1F2CEE", (Color?)null),
				BorderColor = Color.FromHex((ReadOnlySpan<char>)"#4F5F82", (Color?)null),
				BorderThickness = new Thickness(1f)
			};
			((StyleBox)val2).SetContentMarginOverride((Margin)15, 8f);
			val.PanelOverride = (StyleBox)(object)val2;
			BoxContainer val3 = new BoxContainer
			{
				Orientation = (LayoutOrientation)1,
				SeparationOverride = 6,
				HorizontalExpand = true
			};
			Label val4 = new Label();
			val4.Text = ((item.Key > 0) ? Loc.GetString("pubg-roundend-party-group", new(string, object)[1] { ("party", item.Key) }) : Loc.GetString("pubg-roundend-party-group-solo"));
			val4.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFD05C", (Color?)null);
			((Control)val4).StyleClasses.Add("LabelHeading");
			((Control)val3).AddChild((Control)(object)val4);
			foreach (PubgRoundEndPartyEntry item2 in from e in item
				orderby e.Placement, e.Kills descending, e.Username
				select e)
			{
				((Control)val3).AddChild(BuildPartyEntryRow(item2));
			}
			((Control)val).AddChild((Control)(object)val3);
			((Control)_partyEntriesContainer).AddChild((Control)(object)val);
		}
	}

	private Control BuildPartyHeaderRow()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Expected O, but got Unknown
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Expected O, but got Unknown
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Expected O, but got Unknown
		PanelContainer val = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#101521EE", (Color?)null),
				BorderColor = Color.FromHex((ReadOnlySpan<char>)"#4F5F82", (Color?)null),
				BorderThickness = new Thickness(1f)
			}
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			Margin = new Thickness(8f, 6f),
			SeparationOverride = 10,
			HorizontalExpand = true
		};
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("pubg-roundend-party-col-placement"),
			MinWidth = 68f,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#A7B6CE", (Color?)null)
		});
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("pubg-roundend-party-col-player"),
			HorizontalExpand = true,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#A7B6CE", (Color?)null)
		});
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("pubg-roundend-party-col-kills"),
			MinWidth = 52f,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#A7B6CE", (Color?)null)
		});
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("pubg-roundend-party-col-damage"),
			MinWidth = 68f,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#A7B6CE", (Color?)null)
		});
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("pubg-roundend-party-col-taken"),
			MinWidth = 75f,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#A7B6CE", (Color?)null)
		});
		((Control)val).AddChild((Control)(object)val2);
		return (Control)val;
	}

	private Control BuildPartyEntryRow(PubgRoundEndPartyEntry entry)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Expected O, but got Unknown
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Expected O, but got Unknown
		//IL_01a5: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 10,
			HorizontalExpand = true
		};
		((Control)val).AddChild((Control)new Label
		{
			Text = $"#{entry.Placement}",
			MinWidth = 68f,
			FontColorOverride = GetPlacementColor(entry.Placement)
		});
		((Control)val).AddChild((Control)new Label
		{
			Text = entry.Username,
			HorizontalExpand = true,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#E2E8F4", (Color?)null)
		});
		((Control)val).AddChild((Control)new Label
		{
			Text = entry.Kills.ToString(),
			MinWidth = 52f,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FF9C97", (Color?)null)
		});
		((Control)val).AddChild((Control)new Label
		{
			Text = entry.DamageDealt.ToString(),
			MinWidth = 68f,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#8DC8FF", (Color?)null)
		});
		((Control)val).AddChild((Control)new Label
		{
			Text = entry.DamageTaken.ToString(),
			MinWidth = 75f,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#B8C0D0", (Color?)null)
		});
		return (Control)val;
	}

	private Color GetPlacementColor(int placement)
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (placement <= 10)
		{
			return (Color)(placement switch
			{
				1 => Color.FromHex((ReadOnlySpan<char>)"#FFD700", (Color?)null), 
				2 => Color.FromHex((ReadOnlySpan<char>)"#C0C0C0", (Color?)null), 
				3 => Color.FromHex((ReadOnlySpan<char>)"#CD7F32", (Color?)null), 
				_ => Color.FromHex((ReadOnlySpan<char>)"#73B6FF", (Color?)null), 
			});
		}
		return Color.FromHex((ReadOnlySpan<char>)"#A8B4C7", (Color?)null);
	}
}
