using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared._CIV14merka.Supply;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client._CIV14merka.Supply;

public sealed class CivSupplyRefillWindow : DefaultWindow
{
	private sealed class RowRefs
	{
		public PanelContainer Panel;

		public Button Toggle;

		public LineEdit Input;

		public Label CountLabel;

		public bool Active;

		public int Stock;
	}

	private const int StockWidth = 56;

	private const int PriceWidth = 86;

	private const int InputWidth = 60;

	private const int ToggleWidth = 96;

	private const int NowWidth = 104;

	private const int IconSize = 28;

	private static readonly Color OnColor = Color.FromHex((ReadOnlySpan<char>)"#5FA85F", (Color?)null);

	private static readonly Color OffColor = Color.FromHex((ReadOnlySpan<char>)"#9A5C5C", (Color?)null);

	private static readonly Color MutedColor = Color.FromHex((ReadOnlySpan<char>)"#B7C2D8", (Color?)null);

	private static readonly Color StockColor = Color.FromHex((ReadOnlySpan<char>)"#FFD05C", (Color?)null);

	private static readonly Color EmptyStockColor = Color.FromHex((ReadOnlySpan<char>)"#E0544E", (Color?)null);

	private readonly BoxContainer _list;

	private readonly LineEdit _thresholdInput;

	private readonly SpriteSystem _sprite;

	private readonly Dictionary<string, RowRefs> _rows = new Dictionary<string, RowRefs>();

	private List<string> _lastProtoIds = new List<string>();

	public event Action<string, int>? OnSetPeriodic;

	public event Action<string, int>? OnRefillNow;

	public event Action<int>? OnSetThreshold;

	public CivSupplyRefillWindow()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Expected O, but got Unknown
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Expected O, but got Unknown
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Expected O, but got Unknown
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Expected O, but got Unknown
		_sprite = IoCManager.Resolve<IEntityManager>().System<SpriteSystem>();
		((DefaultWindow)this).Title = Loc.GetString("civ-supply-refill-title");
		((Control)this).MinSize = new Vector2(740f, 560f);
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 8,
			Margin = new Thickness(8f),
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((Control)val).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-supply-refill-hint"),
			FontColorOverride = MutedColor,
			Margin = new Thickness(2f, 0f, 2f, 2f)
		});
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 6,
			HorizontalExpand = true
		};
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-supply-refill-threshold-label"),
			FontColorOverride = MutedColor,
			VerticalAlignment = (VAlignment)2
		});
		_thresholdInput = new LineEdit
		{
			PlaceHolder = Loc.GetString("civ-supply-refill-threshold-placeholder"),
			MinWidth = 110f
		};
		_thresholdInput.OnTextEntered += delegate(LineEditEventArgs args)
		{
			int result;
			int obj = ((int.TryParse(args.Text, out result) && result > 0) ? result : 0);
			this.OnSetThreshold?.Invoke(obj);
		};
		((Control)val2).AddChild((Control)(object)_thresholdInput);
		((Control)val).AddChild((Control)(object)val2);
		PanelContainer val3 = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)BuildPanelStyle("#1B1F2CEE", "#445574")
		};
		BoxContainer val4 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 8,
			HorizontalExpand = true
		};
		((Control)val4).AddChild((Control)(object)ColLabel(Loc.GetString("civ-supply-refill-col-item"), expand: true));
		((Control)val4).AddChild((Control)(object)ColLabel(Loc.GetString("civ-supply-refill-col-stock"), expand: false, 56));
		((Control)val4).AddChild((Control)(object)ColLabel(Loc.GetString("civ-supply-refill-col-price"), expand: false, 86));
		((Control)val4).AddChild((Control)(object)ColLabel(Loc.GetString("civ-supply-refill-col-target"), expand: false, 60));
		((Control)val4).AddChild((Control)(object)ColLabel(Loc.GetString("civ-supply-refill-col-auto"), expand: false, 96));
		((Control)val4).AddChild((Control)(object)ColLabel(Loc.GetString("civ-supply-refill-col-now"), expand: false, 104));
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val).AddChild((Control)(object)val3);
		ScrollContainer val5 = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true
		};
		_list = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 4,
			HorizontalExpand = true
		};
		((Control)val5).AddChild((Control)(object)_list);
		((Control)val).AddChild((Control)(object)val5);
		((DefaultWindow)this).Contents.AddChild((Control)(object)val);
	}

	public void Populate(List<CivSupplyRefillEntry> entries)
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = entries.Select((CivSupplyRefillEntry e) => e.ProtoId).ToList();
		if (list.SequenceEqual(_lastProtoIds))
		{
			foreach (CivSupplyRefillEntry entry in entries)
			{
				if (_rows.TryGetValue(entry.ProtoId, out RowRefs value))
				{
					value.Stock = entry.Count;
					value.CountLabel.Text = $"x{entry.Count}";
					value.CountLabel.FontColorOverride = ((entry.Count <= 0) ? EmptyStockColor : StockColor);
					bool flag = entry.Periodic > 0;
					if (flag != value.Active)
					{
						SetRowActive(value, flag);
						if (!((Control)value.Input).HasKeyboardFocus())
						{
							value.Input.Text = (flag ? entry.Periodic.ToString() : string.Empty);
						}
					}
				}
			}
			return;
		}
		RebuildList(entries, list);
	}

	private void RebuildList(List<CivSupplyRefillEntry> entries, List<string> protoIds)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		((Control)_list).RemoveAllChildren();
		_rows.Clear();
		_lastProtoIds = protoIds;
		string text = null;
		foreach (CivSupplyRefillEntry entry in entries)
		{
			if (entry.Category != text)
			{
				text = entry.Category;
				((Control)_list).AddChild((Control)new Label
				{
					Text = entry.Category,
					FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFE39A", (Color?)null),
					StyleClasses = { "LabelHeading" },
					Margin = new Thickness(2f, 8f, 0f, 2f)
				});
			}
			((Control)_list).AddChild(BuildRow(entry));
		}
	}

	public void SetThreshold(int threshold)
	{
		if (!((Control)_thresholdInput).HasKeyboardFocus())
		{
			_thresholdInput.Text = ((threshold > 0) ? threshold.ToString() : string.Empty);
		}
	}

	private Control BuildRow(CivSupplyRefillEntry entry)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected O, but got Unknown
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Expected O, but got Unknown
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Expected O, but got Unknown
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Expected O, but got Unknown
		bool flag = entry.Periodic > 0;
		string proto = entry.ProtoId;
		RowRefs refs = new RowRefs
		{
			Active = flag,
			Stock = entry.Count
		};
		PanelContainer val = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)BuildPanelStyle(flag ? "#1E2A1BF4" : "#141B2BF4", flag ? "#5FA85F" : "#445574")
		};
		refs.Panel = val;
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 8,
			HorizontalExpand = true
		};
		((Control)val2).AddChild((Control)new TextureRect
		{
			Texture = ((IDirectionalTextureProvider)_sprite.GetPrototypeIcon(entry.ProtoId)).Default,
			Stretch = (StretchMode)7,
			MinSize = new Vector2(28f, 28f),
			VerticalAlignment = (VAlignment)2
		});
		((Control)val2).AddChild((Control)new Label
		{
			Text = entry.Name,
			HorizontalExpand = true,
			ClipText = true
		});
		Label val3 = new Label
		{
			Text = $"x{entry.Count}",
			MinWidth = 56f,
			FontColorOverride = ((entry.Count <= 0) ? EmptyStockColor : StockColor)
		};
		refs.CountLabel = val3;
		((Control)val2).AddChild((Control)(object)val3);
		Label val4 = new Label();
		val4.Text = Loc.GetString("civ-supply-refill-unit-price", new(string, object)[1] { ("price", entry.UnitPrice) });
		((Control)val4).MinWidth = 86f;
		val4.FontColorOverride = MutedColor;
		((Control)val2).AddChild((Control)(object)val4);
		LineEdit input = new LineEdit
		{
			PlaceHolder = Loc.GetString("civ-supply-refill-amount-placeholder"),
			Text = (flag ? entry.Periodic.ToString() : string.Empty),
			MinWidth = 60f
		};
		refs.Input = input;
		((Control)val2).AddChild((Control)(object)input);
		Button val5 = new Button
		{
			Text = Loc.GetString(flag ? "civ-supply-refill-toggle-on" : "civ-supply-refill-toggle-off"),
			MinWidth = 96f,
			ModulateSelfOverride = (flag ? OnColor : OffColor)
		};
		refs.Toggle = val5;
		((BaseButton)val5).OnPressed += delegate
		{
			if (refs.Active)
			{
				SetRowActive(refs, active: false);
				this.OnSetPeriodic?.Invoke(proto, 0);
			}
			else
			{
				int result;
				int arg = ((int.TryParse(input.Text, out result) && result > 0) ? result : ((refs.Stock <= 0) ? 1 : refs.Stock));
				if (string.IsNullOrWhiteSpace(input.Text))
				{
					input.Text = arg.ToString();
				}
				SetRowActive(refs, active: true);
				this.OnSetPeriodic?.Invoke(proto, arg);
			}
		};
		((Control)val2).AddChild((Control)(object)val5);
		Button val6 = new Button
		{
			Text = Loc.GetString("civ-supply-refill-now"),
			MinWidth = 104f
		};
		((BaseButton)val6).OnPressed += delegate
		{
			if (int.TryParse(input.Text, out var result) && result > 0)
			{
				this.OnRefillNow?.Invoke(proto, result);
			}
		};
		((Control)val2).AddChild((Control)(object)val6);
		input.OnTextEntered += delegate(LineEditEventArgs args)
		{
			int result;
			int num = ((int.TryParse(args.Text, out result) && result > 0) ? result : 0);
			SetRowActive(refs, num > 0);
			this.OnSetPeriodic?.Invoke(proto, num);
		};
		_rows[proto] = refs;
		((Control)val).AddChild((Control)(object)val2);
		return (Control)(object)val;
	}

	private void SetRowActive(RowRefs r, bool active)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		r.Active = active;
		r.Toggle.Text = Loc.GetString(active ? "civ-supply-refill-toggle-on" : "civ-supply-refill-toggle-off");
		((Control)r.Toggle).ModulateSelfOverride = (active ? OnColor : OffColor);
		r.Panel.PanelOverride = (StyleBox)(object)BuildPanelStyle(active ? "#1E2A1BF4" : "#141B2BF4", active ? "#5FA85F" : "#445574");
	}

	private static Label ColLabel(string text, bool expand = false, int width = 0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		Label val = new Label
		{
			Text = text,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#8FA0BE", (Color?)null)
		};
		if (expand)
		{
			((Control)val).HorizontalExpand = true;
		}
		if (width > 0)
		{
			((Control)val).MinWidth = width;
		}
		return val;
	}

	private static StyleBoxFlat BuildPanelStyle(string backgroundColor, string borderColor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		StyleBoxFlat val = new StyleBoxFlat
		{
			BackgroundColor = Color.FromHex((ReadOnlySpan<char>)backgroundColor, (Color?)null),
			BorderColor = Color.FromHex((ReadOnlySpan<char>)borderColor, (Color?)null),
			BorderThickness = new Thickness(1f)
		};
		((StyleBox)val).SetContentMarginOverride((Margin)15, 8f);
		return val;
	}
}
