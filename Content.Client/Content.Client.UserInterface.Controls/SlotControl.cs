using System;
using System.Numerics;
using Content.Client.Cooldown;
using Content.Client.UserInterface.Systems.Inventory.Controls;
using Content.Shared._PUBG.Armor;
using Content.Shared._RMC14.IconLabel;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Controls;

public abstract class SlotControl : Control, IEntityControl
{
	private static readonly ISawmill Sawmill = Logger.GetSawmill("ui.slot_control");

	[Dependency]
	private IEntityManager _entities;

	[Dependency]
	private ILocalizationManager _loc;

	public static int DefaultButtonSize = 64;

	private bool _slotNameSet;

	private string _slotName = "";

	private string? _blockedTexturePath;

	private string? _buttonTexturePath;

	private string? _fullButtonTexturePath;

	private string? _storageTexturePath;

	private string? _highlightTexturePath;

	public bool MouseIsHovering;

	private EntityUid? _cachedArmorTintEntity;

	private float _cachedArmorTintDurability;

	private float _cachedArmorTintMaxDurability;

	private bool _cachedArmorTintActive;

	public TextureRect ButtonRect { get; }

	public TextureRect BlockedRect { get; }

	public TextureRect HighlightRect { get; }

	public SpriteView HoverSpriteView { get; }

	public TextureButton StorageButton { get; }

	public CooldownGraphic CooldownDisplay { get; }

	public Label IconLabel { get; }

	private SpriteView SpriteView { get; }

	public EntityUid? Entity
	{
		get
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			Entity<SpriteComponent, TransformComponent>? entity = SpriteView.Entity;
			if (!entity.HasValue)
			{
				return null;
			}
			return Entity<SpriteComponent, TransformComponent>.op_Implicit(entity.GetValueOrDefault());
		}
	}

	public string SlotName
	{
		get
		{
			return _slotName;
		}
		set
		{
			if (_slotNameSet)
			{
				Sawmill.Warning("Tried to set slotName after init for:" + ((Control)this).Name);
				return;
			}
			_slotNameSet = true;
			if (((Control)this).Parent is IItemslotUIContainer itemslotUIContainer)
			{
				itemslotUIContainer.TryRegisterButton(this, value);
			}
			((Control)this).Name = "SlotButton_" + value;
			_slotName = value;
		}
	}

	public bool Highlight
	{
		get
		{
			return ((Control)HighlightRect).Visible;
		}
		set
		{
			((Control)HighlightRect).Visible = value;
		}
	}

	public bool Blocked
	{
		get
		{
			return ((Control)BlockedRect).Visible;
		}
		set
		{
			((Control)BlockedRect).Visible = value;
		}
	}

	public string? BlockedTexturePath
	{
		get
		{
			return _blockedTexturePath;
		}
		set
		{
			_blockedTexturePath = value;
			TextureRect blockedRect = BlockedRect;
			TextureResource obj = ((Control)this).Theme.ResolveTextureOrNull(_blockedTexturePath);
			blockedRect.Texture = ((obj != null) ? obj.Texture : null);
		}
	}

	public string? ButtonTexturePath
	{
		get
		{
			return _buttonTexturePath;
		}
		set
		{
			_buttonTexturePath = value;
			UpdateChildren();
		}
	}

	public string? FullButtonTexturePath
	{
		get
		{
			return _fullButtonTexturePath;
		}
		set
		{
			_fullButtonTexturePath = value;
			UpdateChildren();
		}
	}

	public string? StorageTexturePath
	{
		get
		{
			return _buttonTexturePath;
		}
		set
		{
			_storageTexturePath = value;
			TextureButton storageButton = StorageButton;
			TextureResource obj = ((Control)this).Theme.ResolveTextureOrNull(_storageTexturePath);
			storageButton.TextureNormal = ((obj != null) ? obj.Texture : null);
		}
	}

	public string? HighlightTexturePath
	{
		get
		{
			return _highlightTexturePath;
		}
		set
		{
			_highlightTexturePath = value;
			TextureRect highlightRect = HighlightRect;
			TextureResource obj = ((Control)this).Theme.ResolveTextureOrNull(_highlightTexturePath);
			highlightRect.Texture = ((obj != null) ? obj.Texture : null);
		}
	}

	public bool EntityHover => HoverSpriteView.Sprite != null;

	EntityUid? IEntityControl.UiEntity => Entity;

	public event Action<GUIBoundKeyEventArgs, SlotControl>? Pressed;

	public event Action<GUIBoundKeyEventArgs, SlotControl>? Unpressed;

	public event Action<GUIBoundKeyEventArgs, SlotControl>? StoragePressed;

	public event Action<GUIMouseHoverEventArgs, SlotControl>? Hover;

	public SlotControl()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0064: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_009b: Expected O, but got Unknown
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		//IL_0115: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Expected O, but got Unknown
		//IL_0161: Expected O, but got Unknown
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Expected O, but got Unknown
		//IL_01b4: Expected O, but got Unknown
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Expected O, but got Unknown
		//IL_01f2: Expected O, but got Unknown
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Expected O, but got Unknown
		//IL_02b9: Expected O, but got Unknown
		IoCManager.InjectDependencies<SlotControl>(this);
		((Control)this).Name = "SlotButton_null";
		((Control)this).MinSize = new Vector2(DefaultButtonSize, DefaultButtonSize);
		TextureRect val = new TextureRect
		{
			TextureScale = new Vector2(2f, 2f),
			MouseFilter = (MouseFilterMode)0
		};
		TextureRect val2 = val;
		ButtonRect = val;
		((Control)this).AddChild((Control)(object)val2);
		TextureRect val3 = new TextureRect
		{
			Visible = false,
			TextureScale = new Vector2(2f, 2f),
			MouseFilter = (MouseFilterMode)2
		};
		val2 = val3;
		HighlightRect = val3;
		((Control)this).AddChild((Control)(object)val2);
		((Control)ButtonRect).OnKeyBindDown += OnButtonPressed;
		((Control)ButtonRect).OnKeyBindUp += OnButtonUnpressed;
		SpriteView val4 = new SpriteView
		{
			Scale = new Vector2(2f, 2f),
			SetSize = new Vector2(DefaultButtonSize, DefaultButtonSize),
			OverrideDirection = (Direction)0
		};
		SpriteView val5 = val4;
		SpriteView = val4;
		((Control)this).AddChild((Control)(object)val5);
		SpriteView val6 = new SpriteView
		{
			Scale = new Vector2(2f, 2f),
			SetSize = new Vector2(DefaultButtonSize, DefaultButtonSize),
			OverrideDirection = (Direction)0
		};
		val5 = val6;
		HoverSpriteView = val6;
		((Control)this).AddChild((Control)(object)val5);
		Label val7 = new Label
		{
			Text = "",
			HorizontalAlignment = (HAlignment)1,
			VerticalAlignment = (VAlignment)2,
			Visible = true,
			Margin = new Thickness(10f, 0f, 0f, 0f)
		};
		Label val8 = val7;
		IconLabel = val7;
		((Control)this).AddChild((Control)(object)val8);
		TextureButton val9 = new TextureButton
		{
			Scale = new Vector2(0.75f, 0.75f),
			HorizontalAlignment = (HAlignment)3,
			VerticalAlignment = (VAlignment)3,
			Visible = false
		};
		TextureButton val10 = val9;
		StorageButton = val9;
		((Control)this).AddChild((Control)(object)val10);
		((Control)StorageButton).OnKeyBindDown += delegate(GUIBoundKeyEventArgs args)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			if (((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIClick)
			{
				OnButtonPressed(args);
			}
		};
		((BaseButton)StorageButton).OnPressed += OnStorageButtonPressed;
		((Control)ButtonRect).OnMouseEntered += delegate
		{
			MouseIsHovering = true;
		};
		((Control)ButtonRect).OnMouseEntered += OnButtonHover;
		((Control)ButtonRect).OnMouseExited += delegate
		{
			MouseIsHovering = false;
			ClearHover();
		};
		CooldownGraphic cooldownGraphic = new CooldownGraphic();
		((Control)cooldownGraphic).Visible = false;
		CooldownGraphic cooldownGraphic2 = cooldownGraphic;
		CooldownDisplay = cooldownGraphic;
		((Control)this).AddChild((Control)(object)cooldownGraphic2);
		TextureRect val11 = new TextureRect
		{
			TextureScale = new Vector2(2f, 2f),
			MouseFilter = (MouseFilterMode)0,
			Visible = false
		};
		val2 = val11;
		BlockedRect = val11;
		((Control)this).AddChild((Control)(object)val2);
		HighlightTexturePath = "slot_highlight";
		BlockedTexturePath = "blocked";
	}

	public void ClearHover()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (EntityHover)
		{
			Entity<SpriteComponent, TransformComponent>? entity = HoverSpriteView.Entity;
			if (entity.HasValue)
			{
				IEntityManager obj = IoCManager.Resolve<IEntityManager>();
				Entity<SpriteComponent, TransformComponent>? val = entity;
				obj.QueueDeleteEntity(val.HasValue ? new EntityUid?(Entity<SpriteComponent, TransformComponent>.op_Implicit(val.GetValueOrDefault())) : ((EntityUid?)null));
			}
			HoverSpriteView.SetEntity((EntityUid?)null);
		}
	}

	public void SetEntity(EntityUid? ent)
	{
		SpriteView.SetEntity(ent);
		UpdateChildren();
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		UpdateArmorTint();
	}

	private void UpdateChildren()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		TextureResource val = ((Control)this).Theme.ResolveTextureOrNull(_fullButtonTexturePath);
		object obj2;
		if (!Entity.HasValue || val == null)
		{
			TextureResource obj = ((Control)this).Theme.ResolveTextureOrNull(_buttonTexturePath);
			obj2 = ((obj != null) ? obj.Texture : null);
		}
		else
		{
			obj2 = val.Texture;
		}
		Texture texture = (Texture)obj2;
		ButtonRect.Texture = texture;
		IconLabel.Text = "";
		IconLabel.FontColorOverride = Color.White;
		IconLabelComponent iconLabelComponent = default(IconLabelComponent);
		if (_entities.TryGetComponent<IconLabelComponent>(Entity, ref iconLabelComponent))
		{
			LocId? labelTextLocId = iconLabelComponent.LabelTextLocId;
			if (labelTextLocId.HasValue)
			{
				ILocalizationManager loc = _loc;
				labelTextLocId = iconLabelComponent.LabelTextLocId;
				string text = default(string);
				if (loc.TryGetString(labelTextLocId.HasValue ? LocId.op_Implicit(labelTextLocId.GetValueOrDefault()) : null, ref text, iconLabelComponent.LabelTextParams.ToArray()))
				{
					if (text.Length > iconLabelComponent.LabelMaxSize)
					{
						text = text.Substring(0, iconLabelComponent.LabelMaxSize);
					}
					IconLabel.Text = text;
				}
			}
			Color value = default(Color);
			if (Color.TryFromName(iconLabelComponent.TextColor, ref value))
			{
				IconLabel.FontColorOverride = value;
			}
			((Control)IconLabel).SetSize = new Vector2(iconLabelComponent.TextSize);
		}
		UpdateArmorTint();
	}

	private void UpdateArmorTint()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? entity = Entity;
		if (entity.HasValue)
		{
			EntityUid valueOrDefault = entity.GetValueOrDefault();
			PubgArmorComponent pubgArmorComponent = default(PubgArmorComponent);
			if (_entities.TryGetComponent<PubgArmorComponent>(valueOrDefault, ref pubgArmorComponent))
			{
				if (_cachedArmorTintActive)
				{
					entity = _cachedArmorTintEntity;
					EntityUid val = valueOrDefault;
					if (entity.HasValue && entity.GetValueOrDefault() == val && _cachedArmorTintDurability == pubgArmorComponent.Durability && _cachedArmorTintMaxDurability == pubgArmorComponent.MaxDurability)
					{
						return;
					}
				}
				_cachedArmorTintEntity = valueOrDefault;
				_cachedArmorTintDurability = pubgArmorComponent.Durability;
				_cachedArmorTintMaxDurability = pubgArmorComponent.MaxDurability;
				_cachedArmorTintActive = true;
				Color durabilityColor = PubgArmorHelpers.GetDurabilityColor(PubgArmorHelpers.GetDurabilityRatio(pubgArmorComponent));
				((Control)SpriteView).Modulate = durabilityColor;
				((Control)HoverSpriteView).Modulate = durabilityColor;
				return;
			}
		}
		if (_cachedArmorTintActive)
		{
			_cachedArmorTintEntity = null;
			_cachedArmorTintDurability = 0f;
			_cachedArmorTintMaxDurability = 0f;
			_cachedArmorTintActive = false;
			((Control)SpriteView).Modulate = Color.White;
			((Control)HoverSpriteView).Modulate = Color.White;
		}
	}

	private void OnButtonPressed(GUIBoundKeyEventArgs args)
	{
		this.Pressed?.Invoke(args, this);
	}

	private void OnButtonUnpressed(GUIBoundKeyEventArgs args)
	{
		this.Unpressed?.Invoke(args, this);
	}

	private void OnStorageButtonPressed(ButtonEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (((BoundKeyEventArgs)args.Event).Function == EngineKeyFunctions.UIClick)
		{
			this.StoragePressed?.Invoke(args.Event, this);
		}
		else
		{
			this.Pressed?.Invoke(args.Event, this);
		}
	}

	private void OnButtonHover(GUIMouseHoverEventArgs args)
	{
		this.Hover?.Invoke(args, this);
	}

	protected override void OnThemeUpdated()
	{
		((Control)this).OnThemeUpdated();
		TextureButton storageButton = StorageButton;
		TextureResource obj = ((Control)this).Theme.ResolveTextureOrNull(_storageTexturePath);
		storageButton.TextureNormal = ((obj != null) ? obj.Texture : null);
		TextureRect highlightRect = HighlightRect;
		TextureResource obj2 = ((Control)this).Theme.ResolveTextureOrNull(_highlightTexturePath);
		highlightRect.Texture = ((obj2 != null) ? obj2.Texture : null);
		UpdateChildren();
	}
}
