using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs;

public sealed class PubgSkinItemCard : Control
{
	[Dependency]
	private IEntityManager _entMan;

	[Dependency]
	private IPrototypeManager _protoManager;

	[Dependency]
	private IGameTiming _timing;

	private static readonly Color GoldAccent = Color.FromHex((ReadOnlySpan<char>)"#FFB800", (Color?)null);

	private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>)"#00FF88", (Color?)null);

	private static readonly Color DarkPanel = Color.FromHex((ReadOnlySpan<char>)"#0a0a0f", (Color?)null);

	private static readonly Color CardBorderColor = Color.FromHex((ReadOnlySpan<char>)"#2a2a3a", (Color?)null);

	private static readonly Color CardHoverColor = Color.FromHex((ReadOnlySpan<char>)"#252530", (Color?)null);

	private static readonly Color AccentGlow = Color.FromHex((ReadOnlySpan<char>)"#FFD700", (Color?)null);

	private static readonly Color SpriteBgColor = Color.FromHex((ReadOnlySpan<char>)"#0f0a1e", (Color?)null);

	private static readonly Color OwnedColor = Color.FromHex((ReadOnlySpan<char>)"#4CAF50", (Color?)null);

	private static readonly Color RareColor = Color.FromHex((ReadOnlySpan<char>)"#9C27B0", (Color?)null);

	private static readonly Color EpicColor = Color.FromHex((ReadOnlySpan<char>)"#FF9800", (Color?)null);

	private static readonly Color LegendaryColor = Color.FromHex((ReadOnlySpan<char>)"#FFD700", (Color?)null);

	private static readonly Color UniqueColor = Color.FromHex((ReadOnlySpan<char>)"#00E5FF", (Color?)null);

	private string _itemName = "";

	private string _protoId = "";

	private bool _isOwned;

	private bool _isEquipped;

	private bool _isNew;

	private string _rarity = "common";

	private bool _hovered;

	private SpriteView? _spriteView;

	private PanelContainer? _spriteContainer;

	private Label? _nameLabel;

	private Label? _statusLabel;

	private PanelContainer? _rarityBadge;

	private Label? _rarityLabel;

	private StyleBoxFlat? _rarityBadgeStyle;

	public string ItemName
	{
		get
		{
			return _itemName;
		}
		set
		{
			_itemName = value;
			if (_nameLabel != null)
			{
				_nameLabel.Text = value;
			}
		}
	}

	public string ProtoId
	{
		get
		{
			return _protoId;
		}
		set
		{
			_protoId = value;
			UpdateSprite();
		}
	}

	public bool IsOwned
	{
		get
		{
			return _isOwned;
		}
		set
		{
			_isOwned = value;
			UpdateStatus();
		}
	}

	public bool IsEquipped
	{
		get
		{
			return _isEquipped;
		}
		set
		{
			_isEquipped = value;
			UpdateStatus();
			UpdateRarityBadge();
		}
	}

	public bool IsNew
	{
		get
		{
			return _isNew;
		}
		set
		{
			_isNew = value;
		}
	}

	public string Rarity
	{
		get
		{
			return _rarity;
		}
		set
		{
			_rarity = value;
			UpdateRarityBadge();
		}
	}

	public event Action<string>? OnCardClicked;

	public event Action<string>? OnCardRightClicked;

	public PubgSkinItemCard()
	{
		IoCManager.InjectDependencies<PubgSkinItemCard>(this);
		((Control)this).MouseFilter = (MouseFilterMode)0;
		((Control)this).MinSize = new Vector2(95f, 102f);
		BuildUI();
	}

	private void BuildUI()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Expected O, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Expected O, but got Unknown
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Expected O, but got Unknown
		_spriteContainer = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = ((Color)(ref SpriteBgColor)).WithAlpha(0.6f)
			},
			Margin = new Thickness(4f, 4f, 4f, 0f)
		};
		_spriteView = new SpriteView(_entMan)
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			Stretch = (StretchMode)1
		};
		((Control)_spriteContainer).AddChild((Control)(object)_spriteView);
		((Control)this).AddChild((Control)(object)_spriteContainer);
		_nameLabel = new Label
		{
			Text = _itemName,
			FontColorOverride = Color.White,
			HorizontalAlignment = (HAlignment)2,
			MaxWidth = 90f,
			ClipText = true
		};
		((Control)this).AddChild((Control)(object)_nameLabel);
		_statusLabel = new Label
		{
			Text = "",
			FontColorOverride = Color.Gray,
			HorizontalAlignment = (HAlignment)2
		};
		((Control)this).AddChild((Control)(object)_statusLabel);
		_rarityBadgeStyle = new StyleBoxFlat
		{
			BackgroundColor = Color.Transparent
		};
		_rarityLabel = new Label
		{
			Text = "",
			FontColorOverride = Color.White,
			HorizontalAlignment = (HAlignment)2
		};
		_rarityBadge = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)_rarityBadgeStyle,
			MinSize = new Vector2(48f, 16f),
			MouseFilter = (MouseFilterMode)2
		};
		((Control)_rarityLabel).MouseFilter = (MouseFilterMode)2;
		((Control)_rarityBadge).AddChild((Control)(object)_rarityLabel);
		((Control)this).AddChild((Control)(object)_rarityBadge);
		UpdateRarityBadge();
	}

	private void UpdateSprite()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (_spriteView == null || string.IsNullOrEmpty(_protoId) || !_protoManager.HasIndex<EntityPrototype>(_protoId))
		{
			return;
		}
		try
		{
			EntityUid value = _entMan.SpawnEntity(_protoId, MapCoordinates.Nullspace, (ComponentRegistry)null);
			_spriteView.SetEntity((EntityUid?)value);
			_spriteView.Scale = new Vector2(1.8f, 1.8f);
		}
		catch
		{
		}
	}

	private void UpdateStatus()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (_statusLabel != null)
		{
			if (_isEquipped)
			{
				_statusLabel.Text = "ЭКИПИРОВАН";
				_statusLabel.FontColorOverride = GreenSuccess;
			}
			else if (_isOwned)
			{
				_statusLabel.Text = "В НАЛИЧИИ";
				_statusLabel.FontColorOverride = OwnedColor;
			}
			else
			{
				_statusLabel.Text = "НЕ КУПЛЕН";
				_statusLabel.FontColorOverride = Color.Gray;
			}
		}
	}

	protected override Vector2 ArrangeOverride(Vector2 finalSize)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (_spriteContainer != null)
		{
			((Control)_spriteContainer).Arrange(new UIBox2(4f, 4f, finalSize.X - 4f, 75f));
		}
		if (_nameLabel != null)
		{
			((Control)_nameLabel).Arrange(new UIBox2(0f, 67f, finalSize.X, 79f));
		}
		if (_statusLabel != null)
		{
			((Control)_statusLabel).Arrange(new UIBox2(0f, 79f, finalSize.X, 91f));
		}
		if (_rarityBadge != null)
		{
			((Control)_rarityBadge).Arrange(new UIBox2(6f, 6f, 60f, 22f));
		}
		return finalSize;
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		UIBox2 val = default(UIBox2);
		((UIBox2)(ref val))._002Ector(0f, 0f, (float)((Control)this).PixelSize.X, (float)((Control)this).PixelSize.Y);
		float num = (_isOwned ? 0.8f : 0.4f);
		handle.DrawRect(val, ((Color)(ref DarkPanel)).WithAlpha(num), true);
		if (_hovered)
		{
			float num2 = 2f;
			UIBox2 val2 = default(UIBox2);
			((UIBox2)(ref val2))._002Ector(val.Left - num2, val.Top - num2, val.Right + num2, val.Bottom + num2);
			handle.DrawRect(val2, ((Color)(ref CardHoverColor)).WithAlpha(0.5f), true);
		}
		Color rarityColor = GetRarityColor();
		float num3 = (_isEquipped ? 3f : 2f);
		UIBox2 val3 = default(UIBox2);
		for (float num4 = 0f; num4 < num3; num4 += 1f)
		{
			((UIBox2)(ref val3))._002Ector(val.Left + num4, val.Top + num4, val.Right - num4, val.Bottom - num4);
			handle.DrawRect(val3, Color.Transparent, true);
			((DrawingHandleBase)handle).DrawLine(val3.TopLeft, ((UIBox2)(ref val3)).TopRight, rarityColor);
			((DrawingHandleBase)handle).DrawLine(((UIBox2)(ref val3)).TopRight, val3.BottomRight, rarityColor);
			((DrawingHandleBase)handle).DrawLine(val3.BottomRight, ((UIBox2)(ref val3)).BottomLeft, rarityColor);
			((DrawingHandleBase)handle).DrawLine(((UIBox2)(ref val3)).BottomLeft, val3.TopLeft, rarityColor);
		}
		if (_isNew && _isOwned)
		{
			double totalSeconds = _timing.RealTime.TotalSeconds;
			float num5 = 0.3f + MathF.Sin((float)totalSeconds * 3f) * 0.2f;
			UIBox2 val4 = default(UIBox2);
			((UIBox2)(ref val4))._002Ector((float)(((Control)this).PixelSize.X - 50), 6f, (float)(((Control)this).PixelSize.X - 6), 20f);
			handle.DrawRect(val4, ((Color)(ref GoldAccent)).WithAlpha(num5), true);
		}
		if (_isEquipped)
		{
			double totalSeconds2 = _timing.RealTime.TotalSeconds;
			float num6 = 0.2f + MathF.Sin((float)totalSeconds2 * 2f) * 0.1f;
			UIBox2 val5 = default(UIBox2);
			((UIBox2)(ref val5))._002Ector(val.Left - 3f, val.Top - 3f, val.Right + 3f, val.Bottom + 3f);
			handle.DrawRect(val5, ((Color)(ref GreenSuccess)).WithAlpha(num6), true);
		}
	}

	private Color GetRarityColor()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(_rarity.ToLowerInvariant() switch
		{
			"unique" => UniqueColor, 
			"legendary" => LegendaryColor, 
			"epic" => EpicColor, 
			"rare" => RareColor, 
			_ => _isEquipped ? GreenSuccess : CardBorderColor, 
		});
	}

	private void UpdateRarityBadge()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (_rarityBadge != null && _rarityLabel != null && _rarityBadgeStyle != null)
		{
			string rarityLocKey = GetRarityLocKey(_rarity);
			if (rarityLocKey == null)
			{
				((Control)_rarityBadge).Visible = false;
				return;
			}
			_rarityLabel.Text = Loc.GetString(rarityLocKey);
			StyleBoxFlat? rarityBadgeStyle = _rarityBadgeStyle;
			Color rarityColor = GetRarityColor();
			rarityBadgeStyle.BackgroundColor = ((Color)(ref rarityColor)).WithAlpha(0.65f);
			((Control)_rarityBadge).Visible = true;
		}
	}

	private static string? GetRarityLocKey(string rarity)
	{
		return rarity.ToLowerInvariant() switch
		{
			"common" => "mainmenu-skin-rarity-common", 
			"rare" => "mainmenu-skin-rarity-rare", 
			"epic" => "mainmenu-skin-rarity-epic", 
			"legendary" => "mainmenu-skin-rarity-legendary", 
			"unique" => "mainmenu-skin-rarity-unique", 
			_ => null, 
		};
	}

	protected override void MouseEntered()
	{
		((Control)this).MouseEntered();
		_hovered = true;
		((Control)this).UserInterfaceManager.HoverSound();
		((Control)this).InvalidateMeasure();
	}

	protected override void MouseExited()
	{
		((Control)this).MouseExited();
		_hovered = false;
		((Control)this).InvalidateMeasure();
	}

	protected override void KeyBindDown(GUIBoundKeyEventArgs args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).KeyBindDown(args);
		if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UIClick)
		{
			((Control)this).UserInterfaceManager.ClickSound();
			this.OnCardClicked?.Invoke(_protoId);
			((BoundKeyEventArgs)args).Handle();
		}
		else if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UIRightClick)
		{
			this.OnCardRightClicked?.Invoke(_protoId);
			((BoundKeyEventArgs)args).Handle();
		}
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		if (_isNew || _isEquipped)
		{
			((Control)this).InvalidateMeasure();
		}
	}
}
