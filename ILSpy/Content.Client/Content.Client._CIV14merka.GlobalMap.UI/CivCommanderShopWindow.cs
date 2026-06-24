using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client._CIV14merka.Teams;
using Content.Client.UserInterface.Controls;
using Content.Shared._CIV14merka.Commander;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._CIV14merka.GlobalMap.UI;

public sealed class CivCommanderShopWindow : FancyWindow
{
	private sealed class ShopEntryCard
	{
		public PanelContainer Root { get; }

		public Label TitleLabel { get; }

		public RichTextLabel ComparisonPriceLabel { get; }

		public Label PriceLabel { get; }

		public Label UnitLabel { get; }

		public Control CooldownSpacer { get; }

		public Label CooldownLabel { get; }

		public Button ActionButton { get; }

		public ShopEntryCard(PanelContainer root, Label titleLabel, RichTextLabel comparisonPriceLabel, Label priceLabel, Label unitLabel, Control cooldownSpacer, Label cooldownLabel, Button actionButton)
		{
			Root = root;
			TitleLabel = titleLabel;
			ComparisonPriceLabel = comparisonPriceLabel;
			PriceLabel = priceLabel;
			UnitLabel = unitLabel;
			CooldownSpacer = cooldownSpacer;
			CooldownLabel = cooldownLabel;
			ActionButton = actionButton;
		}
	}

	private sealed class RequestCard
	{
		public PanelContainer Root { get; }

		public Label RequesterLabel { get; }

		public Label TotalLabel { get; }

		public Label MetaLabel { get; }

		public BoxContainer Items { get; }

		public List<Label> ItemLabels { get; } = new List<Label>();

		public Button ApproveButton { get; }

		public RequestCard(PanelContainer root, Label requesterLabel, Label totalLabel, Label metaLabel, BoxContainer items, Button approveButton)
		{
			Root = root;
			RequesterLabel = requesterLabel;
			TotalLabel = totalLabel;
			MetaLabel = metaLabel;
			Items = items;
			ApproveButton = approveButton;
		}
	}

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	private readonly SpriteSystem _sprite;

	private readonly CivGlobalMapSystem _system;

	private readonly TextureRect _teamIcon;

	private readonly Label _titleLabel;

	private readonly Label _currencyLabel;

	private readonly Control _shopTab;

	private readonly Control _requestsTab;

	private readonly BoxContainer _placementEntries;

	private readonly BoxContainer _serviceEntries;

	private readonly Label _requestsHintLabel;

	private readonly BoxContainer _requestList;

	private readonly Label _emptyPlacementLabel;

	private readonly Label _emptyServiceLabel;

	private readonly Label _emptyRequestLabel;

	private readonly Dictionary<string, ShopEntryCard> _shopCards = new Dictionary<string, ShopEntryCard>();

	private readonly Dictionary<string, RequestCard> _requestCards = new Dictionary<string, RequestCard>();

	private CivCommanderState? _state;

	public CivCommanderShopWindow(CivGlobalMapSystem system)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Expected O, but got Unknown
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Expected O, but got Unknown
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Expected O, but got Unknown
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Expected O, but got Unknown
		//IL_02c3: Expected O, but got Unknown
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Expected O, but got Unknown
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Expected O, but got Unknown
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Expected O, but got Unknown
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Expected O, but got Unknown
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Expected O, but got Unknown
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Expected O, but got Unknown
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Expected O, but got Unknown
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Expected O, but got Unknown
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Expected O, but got Unknown
		IoCManager.InjectDependencies<CivCommanderShopWindow>(this);
		_sprite = _entityManager.System<SpriteSystem>();
		_system = system;
		base.Title = Loc.GetString("civ-commander-shop-title");
		((Control)this).MinSize = new Vector2(1020f, 620f);
		((Control)this).SetSize = new Vector2(1120f, 720f);
		((BaseWindow)this).Resizable = true;
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 12,
			HorizontalExpand = true,
			VerticalExpand = true,
			Margin = new Thickness(8f)
		};
		PanelContainer val2 = MakePanel(Color.FromHex((ReadOnlySpan<char>)"#1A2129", (Color?)null), Color.FromHex((ReadOnlySpan<char>)"#4A5C72", (Color?)null));
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 12,
			HorizontalExpand = true
		};
		_teamIcon = new TextureRect
		{
			MinSize = new Vector2(56f, 56f),
			MaxSize = new Vector2(56f, 56f),
			Stretch = (StretchMode)7,
			Visible = false
		};
		((Control)val3).AddChild(MakeIconPanel((Control)(object)_teamIcon, Color.FromHex((ReadOnlySpan<char>)"#4D87D9", (Color?)null)));
		_titleLabel = new Label
		{
			Text = Loc.GetString("civ-commander-shop-title-side"),
			StyleClasses = { "FancyWindowTitle" },
			VerticalAlignment = (VAlignment)2
		};
		((Control)val3).AddChild((Control)(object)_titleLabel);
		((Control)val3).AddChild(new Control
		{
			HorizontalExpand = true
		});
		Label val4 = new Label();
		val4.Text = Loc.GetString("civ-commander-shop-currency", new(string, object)[1] { ("amount", 0) });
		val4.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#8CF09B", (Color?)null);
		_currencyLabel = val4;
		PanelContainer val5 = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#172129", (Color?)null),
				BorderColor = Color.FromHex((ReadOnlySpan<char>)"#6CC07A", (Color?)null),
				BorderThickness = new Thickness(1f),
				ContentMarginLeftOverride = 14f,
				ContentMarginTopOverride = 10f,
				ContentMarginRightOverride = 14f,
				ContentMarginBottomOverride = 10f
			}
		};
		((Control)val5).AddChild((Control)(object)_currencyLabel);
		((Control)val3).AddChild((Control)(object)val5);
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		_placementEntries = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 10,
			HorizontalExpand = true
		};
		_serviceEntries = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 10,
			HorizontalExpand = true
		};
		_requestList = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 10,
			HorizontalExpand = true
		};
		_emptyPlacementLabel = MakeEmptyLabel(Loc.GetString("civ-commander-shop-empty-placement"));
		_emptyServiceLabel = MakeEmptyLabel(Loc.GetString("civ-commander-shop-empty-service"));
		_emptyRequestLabel = MakeEmptyLabel(Loc.GetString("civ-commander-shop-empty-request"));
		TabContainer val6 = new TabContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true
		};
		BoxContainer val7 = (BoxContainer)(object)(_shopTab = (Control)new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 12,
			HorizontalExpand = true,
			VerticalExpand = true
		});
		TabContainer.SetTabTitle(_shopTab, Loc.GetString("civ-commander-shop-tab-shop"));
		BoxContainer val8 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 12,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		PanelContainer val9 = MakeSectionPanel(Loc.GetString("civ-commander-shop-section-placement"), Color.FromHex((ReadOnlySpan<char>)"#6CC07A", (Color?)null), (Control)(object)_placementEntries);
		((Control)val9).SizeFlagsStretchRatio = 1f;
		((Control)val8).AddChild((Control)(object)val9);
		PanelContainer val10 = MakeSectionPanel(Loc.GetString("civ-commander-shop-section-service"), Color.FromHex((ReadOnlySpan<char>)"#4D87D9", (Color?)null), (Control)(object)_serviceEntries);
		((Control)val10).SizeFlagsStretchRatio = 1f;
		((Control)val8).AddChild((Control)(object)val10);
		((Control)val7).AddChild((Control)(object)val8);
		((Control)val6).AddChild((Control)(object)val7);
		BoxContainer val11 = (BoxContainer)(object)(_requestsTab = (Control)new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 8,
			HorizontalExpand = true,
			VerticalExpand = true
		});
		TabContainer.SetTabTitle(_requestsTab, Loc.GetString("civ-commander-shop-tab-requests"));
		_requestsHintLabel = new Label
		{
			Text = Loc.GetString("civ-commander-shop-requests-hint-none"),
			FontColorOverride = Color.LightGray
		};
		((Control)val11).AddChild((Control)(object)_requestsHintLabel);
		ScrollContainer val12 = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			VScrollEnabled = true,
			HScrollEnabled = false
		};
		((Control)val12).AddChild((Control)(object)_requestList);
		((Control)val11).AddChild((Control)(object)val12);
		((Control)val6).AddChild((Control)(object)val11);
		((Control)val).AddChild((Control)(object)val6);
		base.ContentsContainer.AddChild((Control)(object)val);
	}

	public void UpdateState(CivCommanderState? state)
	{
		_state = state;
		UpdateView();
	}

	private void UpdateView()
	{
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		TabContainer.SetTabTitle(_shopTab, Loc.GetString("civ-commander-shop-tab-shop"));
		if (_state == null)
		{
			((Control)_teamIcon).Visible = false;
			_titleLabel.Text = Loc.GetString("civ-commander-shop-title-side");
			_currencyLabel.Text = Loc.GetString("civ-commander-shop-currency", new(string, object)[1] { ("amount", 0) });
			_currencyLabel.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#8CF09B", (Color?)null);
			ClearShopCards();
			ClearRequestCards();
			SetEmptyState((Control)(object)_placementEntries, (Control)(object)_emptyPlacementLabel, visible: true);
			SetEmptyState((Control)(object)_serviceEntries, (Control)(object)_emptyServiceLabel, visible: true);
			SetEmptyState((Control)(object)_requestList, (Control)(object)_emptyRequestLabel, visible: true);
			_requestsHintLabel.Text = Loc.GetString("civ-commander-shop-requests-hint-none");
			TabContainer.SetTabTitle(_requestsTab, Loc.GetString("civ-commander-shop-tab-requests"));
		}
		else
		{
			Color teamAccent = GetTeamAccent(_state.TeamId);
			string item = Loc.GetString((_state.TeamId == 2) ? "civ-team-short-rf" : "civ-team-short-usa");
			_teamIcon.Texture = _sprite.Frame0((SpriteSpecifier)(object)CivTeamIconResolver.GetTeamBadge(_state.TeamId));
			((Control)_teamIcon).Visible = true;
			_titleLabel.Text = Loc.GetString("civ-commander-shop-title-team", new(string, object)[1] { ("team", item) });
			_currencyLabel.Text = Loc.GetString("civ-commander-shop-currency", new(string, object)[1] { ("amount", _state.Currency) });
			_currencyLabel.FontColorOverride = teamAccent;
			UpdateShopEntries(_state);
			UpdateRequestEntries(_state);
		}
	}

	private void UpdateShopEntries(CivCommanderState state)
	{
		HashSet<string> hashSet = new HashSet<string>();
		int num = 0;
		int num2 = 0;
		CivCommanderShopEntryPrototype civCommanderShopEntryPrototype = default(CivCommanderShopEntryPrototype);
		foreach (CivCommanderShopEntryState item in from entry in state.ShopEntries
			orderby ResolveOrder(entry.EntryId), entry.EntryId
			select entry)
		{
			if (_prototypeManager.TryIndex<CivCommanderShopEntryPrototype>(item.EntryId, ref civCommanderShopEntryPrototype) && civCommanderShopEntryPrototype.Enabled)
			{
				hashSet.Add(item.EntryId);
				if (!_shopCards.TryGetValue(item.EntryId, out ShopEntryCard value))
				{
					value = CreateShopEntryCard(civCommanderShopEntryPrototype);
					_shopCards[item.EntryId] = value;
				}
				UpdateShopEntryCard(value, civCommanderShopEntryPrototype, item, state.Currency);
				if (civCommanderShopEntryPrototype.Kind == CivCommanderShopEntryKind.EntityPlacement)
				{
					PlaceChild((Control)(object)_placementEntries, (Control)(object)value.Root, num);
					num++;
				}
				else
				{
					PlaceChild((Control)(object)_serviceEntries, (Control)(object)value.Root, num2);
					num2++;
				}
			}
		}
		foreach (var (text2, shopEntryCard2) in _shopCards.ToList())
		{
			if (!hashSet.Contains(text2))
			{
				Control parent = ((Control)shopEntryCard2.Root).Parent;
				if (parent != null)
				{
					parent.RemoveChild((Control)(object)shopEntryCard2.Root);
				}
				_shopCards.Remove(text2);
			}
		}
		SetEmptyState((Control)(object)_placementEntries, (Control)(object)_emptyPlacementLabel, num == 0);
		SetEmptyState((Control)(object)_serviceEntries, (Control)(object)_emptyServiceLabel, num2 == 0);
	}

	private ShopEntryCard CreateShopEntryCard(CivCommanderShopEntryPrototype entry)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Expected O, but got Unknown
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Expected O, but got Unknown
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Expected O, but got Unknown
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Expected O, but got Unknown
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Expected O, but got Unknown
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Expected O, but got Unknown
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Expected O, but got Unknown
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		Color entryAccent = GetEntryAccent(entry);
		PanelContainer obj = MakePanel(Color.FromHex((ReadOnlySpan<char>)"#202732", (Color?)null), entryAccent);
		((Control)obj).HorizontalExpand = true;
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 12,
			HorizontalExpand = true
		};
		if (TryGetEntryTexture(entry, out Texture texture))
		{
			TextureRect icon = new TextureRect
			{
				Texture = texture,
				MinSize = new Vector2(52f, 52f),
				MaxSize = new Vector2(52f, 52f),
				Stretch = (StretchMode)7
			};
			((Control)val).AddChild(MakeIconPanel((Control)(object)icon, entryAccent));
		}
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 8,
			HorizontalExpand = true
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 8,
			HorizontalExpand = true
		};
		Label val4 = new Label
		{
			StyleClasses = { "FancyWindowTitle" },
			HorizontalExpand = true,
			ClipText = true
		};
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val2).AddChild((Control)(object)val3);
		BoxContainer val5 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 10,
			HorizontalExpand = true
		};
		RichTextLabel val6 = new RichTextLabel
		{
			VerticalAlignment = (VAlignment)2,
			Visible = false
		};
		((Control)val5).AddChild((Control)(object)val6);
		Label val7 = new Label
		{
			StyleClasses = { "FancyWindowTitle" },
			FontColorOverride = entryAccent,
			VerticalAlignment = (VAlignment)2
		};
		((Control)val5).AddChild((Control)(object)val7);
		Label val8 = new Label
		{
			Text = Loc.GetString("civ-commander-shop-unit"),
			FontColorOverride = ((Color)(ref entryAccent)).WithAlpha(0.9f),
			VerticalAlignment = (VAlignment)2
		};
		((Control)val5).AddChild((Control)(object)val8);
		Control val9 = new Control
		{
			HorizontalExpand = true,
			Visible = false
		};
		((Control)val5).AddChild(val9);
		Label val10 = new Label
		{
			VerticalAlignment = (VAlignment)2,
			Visible = false
		};
		((Control)val5).AddChild((Control)(object)val10);
		((Control)val2).AddChild((Control)(object)val5);
		BoxContainer val11 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 8,
			HorizontalExpand = true
		};
		((Control)val11).AddChild(new Control
		{
			HorizontalExpand = true
		});
		Button val12 = new Button
		{
			MinSize = new Vector2(156f, 38f)
		};
		((BaseButton)val12).OnPressed += delegate
		{
			if (entry.Kind == CivCommanderShopEntryKind.EntityPlacement)
			{
				_system.TryStartCommanderShopPlacement(entry.ID);
			}
			else
			{
				_system.RequestCommanderShopPurchase(entry.ID);
			}
		};
		((Control)val11).AddChild((Control)(object)val12);
		((Control)val2).AddChild((Control)(object)val11);
		((Control)val).AddChild((Control)(object)val2);
		((Control)obj).AddChild((Control)(object)val);
		return new ShopEntryCard(obj, val4, val6, val7, val8, val9, val10, val12);
	}

	private void UpdateShopEntryCard(ShopEntryCard card, CivCommanderShopEntryPrototype entry, CivCommanderShopEntryState entryState, int currency)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		Color entryAccent = GetEntryAccent(entry);
		card.TitleLabel.Text = entry.Name;
		card.PriceLabel.Text = $"{entryState.Price}";
		card.PriceLabel.FontColorOverride = entryAccent;
		card.UnitLabel.FontColorOverride = ((Color)(ref entryAccent)).WithAlpha(0.9f);
		card.ActionButton.Text = BuildActionLabel(entry, entryState.Price);
		((BaseButton)card.ActionButton).Disabled = currency < entryState.Price;
		if (!HasPriceCooldown(entry))
		{
			((Control)card.ComparisonPriceLabel).Visible = false;
			card.CooldownSpacer.Visible = false;
			((Control)card.CooldownLabel).Visible = false;
			return;
		}
		int num = ((entryState.PriceCooldownRemainingSeconds > 0f) ? entryState.BasePrice : GetFollowUpPrice(entry, entryState));
		bool flag = num > 0 && num != entryState.Price;
		((Control)card.ComparisonPriceLabel).Visible = flag;
		if (flag)
		{
			card.ComparisonPriceLabel.Text = $"[color=#C9D2DC][s]{num}[/s][/color]";
		}
		card.CooldownSpacer.Visible = true;
		((Control)card.CooldownLabel).Visible = true;
		card.CooldownLabel.Text = ((entryState.PriceCooldownRemainingSeconds > 0f) ? Loc.GetString("civ-commander-shop-discount-returns", new(string, object)[1] { ("time", FormatCooldown(entryState.PriceCooldownRemainingSeconds)) }) : Loc.GetString("civ-commander-shop-discount-active"));
		card.CooldownLabel.FontColorOverride = ((entryState.PriceCooldownRemainingSeconds > 0f) ? entryAccent : Color.FromHex((ReadOnlySpan<char>)"#C9D2DC", (Color?)null));
	}

	private void UpdateRequestEntries(CivCommanderState state)
	{
		List<PurchaseRequestEntryState> list = state.PurchaseRequests.OrderByDescending((PurchaseRequestEntryState entry) => entry.RequestTime).ToList();
		if (list.Count == 0)
		{
			ClearRequestCards();
			_requestsHintLabel.Text = Loc.GetString("civ-commander-shop-requests-hint-empty");
			TabContainer.SetTabTitle(_requestsTab, Loc.GetString("civ-commander-shop-tab-requests"));
			SetEmptyState((Control)(object)_requestList, (Control)(object)_emptyRequestLabel, visible: true);
			return;
		}
		HashSet<string> hashSet = new HashSet<string>();
		for (int num = 0; num < list.Count; num++)
		{
			PurchaseRequestEntryState purchaseRequestEntryState = list[num];
			hashSet.Add(purchaseRequestEntryState.RequestId);
			if (!_requestCards.TryGetValue(purchaseRequestEntryState.RequestId, out RequestCard value))
			{
				value = CreateRequestCard(purchaseRequestEntryState.RequestId);
				_requestCards[purchaseRequestEntryState.RequestId] = value;
			}
			UpdateRequestCard(value, purchaseRequestEntryState, state.Currency);
			PlaceChild((Control)(object)_requestList, (Control)(object)value.Root, num);
		}
		foreach (var (text2, requestCard2) in _requestCards.ToList())
		{
			if (!hashSet.Contains(text2))
			{
				Control parent = ((Control)requestCard2.Root).Parent;
				if (parent != null)
				{
					parent.RemoveChild((Control)(object)requestCard2.Root);
				}
				_requestCards.Remove(text2);
			}
		}
		_requestsHintLabel.Text = Loc.GetString("civ-commander-shop-requests-hint-active");
		TabContainer.SetTabTitle(_requestsTab, Loc.GetString("civ-commander-shop-tab-requests-count", new(string, object)[1] { ("count", list.Count) }));
		SetEmptyState((Control)(object)_requestList, (Control)(object)_emptyRequestLabel, visible: false);
	}

	private RequestCard CreateRequestCard(string requestId)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected O, but got Unknown
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Expected O, but got Unknown
		Color val = Color.FromHex((ReadOnlySpan<char>)"#C28D36", (Color?)null);
		PanelContainer obj = MakePanel(Color.FromHex((ReadOnlySpan<char>)"#202732", (Color?)null), val);
		((Control)obj).HorizontalExpand = true;
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 8,
			HorizontalExpand = true,
			VerticalExpand = false
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 8,
			HorizontalExpand = true
		};
		Label val4 = new Label
		{
			StyleClasses = { "FancyWindowTitle" },
			HorizontalExpand = true,
			ClipText = true
		};
		((Control)val3).AddChild((Control)(object)val4);
		Label val5 = new Label
		{
			FontColorOverride = val
		};
		((Control)val3).AddChild((Control)(object)val5);
		((Control)val2).AddChild((Control)(object)val3);
		Label val6 = new Label
		{
			FontColorOverride = Color.LightGray
		};
		((Control)val2).AddChild((Control)(object)val6);
		BoxContainer val7 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 4,
			HorizontalExpand = true
		};
		((Control)val2).AddChild((Control)(object)val7);
		BoxContainer val8 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 8,
			HorizontalExpand = true
		};
		((Control)val8).AddChild(new Control
		{
			HorizontalExpand = true
		});
		Button val9 = new Button
		{
			Text = Loc.GetString("civ-commander-shop-approve"),
			MinSize = new Vector2(120f, 34f)
		};
		((BaseButton)val9).OnPressed += delegate
		{
			_system.ApprovePurchaseRequest(requestId);
		};
		((Control)val8).AddChild((Control)(object)val9);
		Button val10 = new Button
		{
			Text = Loc.GetString("civ-commander-shop-deny"),
			MinSize = new Vector2(120f, 34f)
		};
		((BaseButton)val10).OnPressed += delegate
		{
			_system.DenyPurchaseRequest(requestId);
		};
		((Control)val8).AddChild((Control)(object)val10);
		((Control)val2).AddChild((Control)(object)val8);
		((Control)obj).AddChild((Control)(object)val2);
		return new RequestCard(obj, val4, val5, val6, val7, val9);
	}

	private void UpdateRequestCard(RequestCard card, PurchaseRequestEntryState request, int currency)
	{
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		card.RequesterLabel.Text = request.RequesterName;
		card.TotalLabel.Text = Loc.GetString("civ-commander-shop-request-total", new(string, object)[1] { ("price", request.TotalPrice) });
		card.MetaLabel.Text = Loc.GetString("civ-commander-shop-request-meta", new(string, object)[2]
		{
			("faction", request.Faction),
			("time", FormatRequestTime(request.RequestTime))
		});
		((BaseButton)card.ApproveButton).Disabled = currency < request.TotalPrice;
		while (card.ItemLabels.Count > request.Items.Count)
		{
			int index = card.ItemLabels.Count - 1;
			Label val = card.ItemLabels[index];
			((Control)card.Items).RemoveChild((Control)(object)val);
			card.ItemLabels.RemoveAt(index);
		}
		for (int i = 0; i < request.Items.Count; i++)
		{
			if (i >= card.ItemLabels.Count)
			{
				Label val2 = new Label
				{
					FontColorOverride = Color.White,
					HorizontalExpand = true,
					ClipText = true
				};
				card.ItemLabels.Add(val2);
				((Control)card.Items).AddChild((Control)(object)val2);
			}
			PurchaseRequestItemState purchaseRequestItemState = request.Items[i];
			card.ItemLabels[i].Text = Loc.GetString("civ-commander-shop-request-item", new(string, object)[2]
			{
				("name", purchaseRequestItemState.ItemName),
				("count", purchaseRequestItemState.Quantity)
			});
			((Control)card.ItemLabels[i]).SetPositionInParent(i);
		}
	}

	private void ClearShopCards()
	{
		foreach (ShopEntryCard value in _shopCards.Values)
		{
			Control parent = ((Control)value.Root).Parent;
			if (parent != null)
			{
				parent.RemoveChild((Control)(object)value.Root);
			}
		}
		_shopCards.Clear();
		SetEmptyState((Control)(object)_placementEntries, (Control)(object)_emptyPlacementLabel, visible: false);
		SetEmptyState((Control)(object)_serviceEntries, (Control)(object)_emptyServiceLabel, visible: false);
	}

	private void ClearRequestCards()
	{
		foreach (RequestCard value in _requestCards.Values)
		{
			Control parent = ((Control)value.Root).Parent;
			if (parent != null)
			{
				parent.RemoveChild((Control)(object)value.Root);
			}
		}
		_requestCards.Clear();
		SetEmptyState((Control)(object)_requestList, (Control)(object)_emptyRequestLabel, visible: false);
	}

	private static void SetEmptyState(Control parent, Control emptyControl, bool visible)
	{
		if (visible)
		{
			if (!parent.Children.Contains(emptyControl))
			{
				parent.AddChild(emptyControl);
			}
			emptyControl.SetPositionInParent(0);
		}
		else if (parent.Children.Contains(emptyControl))
		{
			parent.RemoveChild(emptyControl);
		}
	}

	private static void PlaceChild(Control parent, Control child, int index)
	{
		if (child.Parent != parent)
		{
			Control parent2 = child.Parent;
			if (parent2 != null)
			{
				parent2.RemoveChild(child);
			}
			parent.AddChild(child);
		}
		child.SetPositionInParent(index);
	}

	private int ResolveOrder(string entryId)
	{
		CivCommanderShopEntryPrototype civCommanderShopEntryPrototype = default(CivCommanderShopEntryPrototype);
		if (!_prototypeManager.TryIndex<CivCommanderShopEntryPrototype>(entryId, ref civCommanderShopEntryPrototype))
		{
			return int.MaxValue;
		}
		return civCommanderShopEntryPrototype.Order;
	}

	private bool TryGetEntryTexture(CivCommanderShopEntryPrototype entry, out Texture texture)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		texture = _sprite.GetFallbackTexture();
		if (entry.IconEntity.HasValue)
		{
			IPrototypeManager prototypeManager = _prototypeManager;
			EntProtoId? iconEntity = entry.IconEntity;
			EntityPrototype val = default(EntityPrototype);
			if (prototypeManager.TryIndex<EntityPrototype>(iconEntity.HasValue ? EntProtoId.op_Implicit(iconEntity.GetValueOrDefault()) : null, ref val))
			{
				IconComponent val2 = default(IconComponent);
				if (val.TryGetComponent<IconComponent>(ref val2, _entityManager.ComponentFactory))
				{
					texture = _sprite.GetIcon(val2);
					return true;
				}
				SpriteComponent val3 = default(SpriteComponent);
				if (!val.TryGetComponent<SpriteComponent>(ref val3, _entityManager.ComponentFactory))
				{
					return false;
				}
				State val5 = default(State);
				foreach (ISpriteLayer allLayer in val3.AllLayers)
				{
					if (!allLayer.Visible)
					{
						continue;
					}
					if (allLayer.Texture != null)
					{
						texture = allLayer.Texture;
						return true;
					}
					RSI val4 = allLayer.Rsi ?? allLayer.ActualRsi ?? val3.BaseRSI;
					if (val4 != null)
					{
						StateId rsiState = allLayer.RsiState;
						if (((StateId)(ref rsiState)).IsValid && val4.TryGetState(allLayer.RsiState, ref val5))
						{
							texture = val5.Frame0;
							return true;
						}
					}
				}
				return false;
			}
		}
		return false;
	}

	private static string BuildActionLabel(CivCommanderShopEntryPrototype entry, int price)
	{
		bool flag = entry.Kind == CivCommanderShopEntryKind.EntityPlacement;
		if (price > 0)
		{
			return Loc.GetString(flag ? "civ-commander-shop-action-place-price" : "civ-commander-shop-action-buy-price", new(string, object)[1] { ("price", price) });
		}
		return Loc.GetString(flag ? "civ-commander-shop-action-place" : "civ-commander-shop-action-buy");
	}

	private static bool HasPriceCooldown(CivCommanderShopEntryPrototype entry)
	{
		if (entry.PriceAfterPurchaseCooldownSeconds > 0)
		{
			return entry.PriceAfterPurchaseMultiplier > 1f;
		}
		return false;
	}

	private static int GetFollowUpPrice(CivCommanderShopEntryPrototype entry, CivCommanderShopEntryState entryState)
	{
		if (!HasPriceCooldown(entry) || entryState.BasePrice <= 0)
		{
			return entryState.Price;
		}
		return Math.Max(entryState.Price, (int)MathF.Ceiling((float)entryState.BasePrice * entry.PriceAfterPurchaseMultiplier));
	}

	private static string FormatRequestTime(double timeSeconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(Math.Max(0, (int)Math.Round(timeSeconds)));
		if (!(timeSpan.TotalHours >= 1.0))
		{
			return timeSpan.ToString("mm\\:ss");
		}
		return timeSpan.ToString("hh\\:mm\\:ss");
	}

	private static string FormatCooldown(float remainingSeconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(Math.Max(0, (int)MathF.Ceiling(remainingSeconds)));
		if (!(timeSpan.TotalHours >= 1.0))
		{
			return timeSpan.ToString("mm\\:ss");
		}
		return timeSpan.ToString("hh\\:mm\\:ss");
	}

	private static PanelContainer MakeSectionPanel(string title, Color accent, Control content)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		PanelContainer val = new PanelContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#151A22", (Color?)null),
				BorderColor = accent,
				BorderThickness = new Thickness(1f),
				ContentMarginLeftOverride = 12f,
				ContentMarginTopOverride = 12f,
				ContentMarginRightOverride = 12f,
				ContentMarginBottomOverride = 12f
			}
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 10,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((Control)val2).AddChild((Control)new Label
		{
			Text = title,
			StyleClasses = { "FancyWindowTitle" },
			FontColorOverride = accent
		});
		ScrollContainer val3 = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			VScrollEnabled = true,
			HScrollEnabled = false
		};
		((Control)val3).AddChild(content);
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		return val;
	}

	private static Label MakeEmptyLabel(string text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		return new Label
		{
			Text = text,
			FontColorOverride = Color.LightGray
		};
	}

	private static PanelContainer MakePanel(Color background, Color border)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0067: Expected O, but got Unknown
		return new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = background,
				BorderColor = border,
				BorderThickness = new Thickness(1f),
				ContentMarginLeftOverride = 12f,
				ContentMarginTopOverride = 12f,
				ContentMarginRightOverride = 12f,
				ContentMarginBottomOverride = 12f
			}
		};
	}

	private static Control MakeIconPanel(Control icon, Color accent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		PanelContainer val = new PanelContainer
		{
			MinSize = new Vector2(72f, 72f),
			MaxSize = new Vector2(72f, 72f),
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = ((Color)(ref accent)).WithAlpha(0.14f),
				BorderColor = accent,
				BorderThickness = new Thickness(1f),
				ContentMarginLeftOverride = 10f,
				ContentMarginTopOverride = 10f,
				ContentMarginRightOverride = 10f,
				ContentMarginBottomOverride = 10f
			}
		};
		((Control)val).AddChild(icon);
		return (Control)val;
	}

	private static Color GetEntryAccent(CivCommanderShopEntryPrototype entry)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (entry.Kind != CivCommanderShopEntryKind.EntityPlacement)
		{
			if (entry.ServiceType != CivCommanderShopPurchaseType.RefillFactionSupply)
			{
				return Color.FromHex((ReadOnlySpan<char>)"#4D87D9", (Color?)null);
			}
			return Color.FromHex((ReadOnlySpan<char>)"#C28D36", (Color?)null);
		}
		return Color.FromHex((ReadOnlySpan<char>)"#6CC07A", (Color?)null);
	}

	private static Color GetTeamAccent(int teamId)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (teamId != 2)
		{
			return Color.FromHex((ReadOnlySpan<char>)"#4D87D9", (Color?)null);
		}
		return Color.FromHex((ReadOnlySpan<char>)"#C24E4E", (Color?)null);
	}
}
