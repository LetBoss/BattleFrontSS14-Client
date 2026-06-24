using System;
using System.Numerics;
using Content.Client.Actions;
using Content.Client.Actions.UI;
using Content.Client.Cooldown;
using Content.Shared.Actions.Components;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Systems.Actions.Controls;

public sealed class ActionButton : Control, IEntityControl
{
	private IEntityManager _entities;

	private SpriteSystem? _spriteSys;

	private ActionUIController? _controller;

	private SharedChargesSystem _sharedChargesSys;

	private bool _beingHovered;

	private bool _depressed;

	private bool _toggled;

	private BoundKeyFunction? _keybind;

	public readonly TextureRect Button;

	public readonly PanelContainer HighlightRect;

	private readonly TextureRect _bigActionIcon;

	private readonly TextureRect _smallActionIcon;

	public readonly Label Label;

	public readonly CooldownGraphic Cooldown;

	private readonly SpriteView _smallItemSpriteView;

	private readonly SpriteView _bigItemSpriteView;

	private Texture? _buttonBackgroundTexture;

	public BoundKeyFunction? KeyBind
	{
		set
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			_keybind = value;
			if (_keybind.HasValue)
			{
				Label.Text = BoundKeyHelper.ShortKeyName(_keybind.Value);
			}
		}
	}

	public Entity<ActionComponent>? Action { get; private set; }

	public bool Locked { get; set; }

	EntityUid? IEntityControl.UiEntity
	{
		get
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			Entity<ActionComponent>? action = Action;
			if (!action.HasValue)
			{
				return null;
			}
			return Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault());
		}
	}

	public event Action<GUIBoundKeyEventArgs, ActionButton>? ActionPressed;

	public event Action<GUIBoundKeyEventArgs, ActionButton>? ActionUnpressed;

	public event Action<ActionButton>? ActionFocusExited;

	public ActionButton(IEntityManager entities, SpriteSystem? spriteSys = null, ActionUIController? controller = null)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Expected O, but got Unknown
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Expected O, but got Unknown
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Expected O, but got Unknown
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Expected O, but got Unknown
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Expected O, but got Unknown
		_entities = entities;
		_spriteSys = spriteSys;
		_sharedChargesSys = _entities.System<SharedChargesSystem>();
		_controller = controller;
		((Control)this).MouseFilter = (MouseFilterMode)1;
		Button = new TextureRect
		{
			Name = "Button",
			TextureScale = new Vector2(2f, 2f)
		};
		HighlightRect = new PanelContainer
		{
			StyleClasses = { "HandSlotHighlight" },
			MinSize = new Vector2(32f, 32f),
			Visible = false
		};
		_bigActionIcon = new TextureRect
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			Stretch = (StretchMode)1,
			Visible = false
		};
		_smallActionIcon = new TextureRect
		{
			HorizontalAlignment = (HAlignment)3,
			VerticalAlignment = (VAlignment)3,
			Stretch = (StretchMode)1,
			Visible = false
		};
		Label = new Label
		{
			Name = "Label",
			HorizontalAlignment = (HAlignment)1,
			VerticalAlignment = (VAlignment)1,
			Margin = new Thickness(5f, 0f, 0f, 0f)
		};
		_bigItemSpriteView = new SpriteView
		{
			Name = "Big Sprite",
			HorizontalExpand = true,
			VerticalExpand = true,
			Scale = new Vector2(2f, 2f),
			SetSize = new Vector2(64f, 64f),
			Visible = false,
			OverrideDirection = (Direction)0
		};
		_smallItemSpriteView = new SpriteView
		{
			Name = "Small Sprite",
			HorizontalAlignment = (HAlignment)3,
			VerticalAlignment = (VAlignment)3,
			Visible = false,
			OverrideDirection = (Direction)0
		};
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true,
			VerticalExpand = true,
			MinSize = new Vector2(64f, 64f)
		};
		((Control)val).AddChild(new Control
		{
			MinSize = new Vector2(32f, 32f)
		});
		Control val2 = new Control();
		val2.Children.Add((Control)(object)_smallActionIcon);
		val2.Children.Add((Control)(object)_smallItemSpriteView);
		((Control)val).AddChild(val2);
		CooldownGraphic cooldownGraphic = new CooldownGraphic();
		((Control)cooldownGraphic).Visible = false;
		Cooldown = cooldownGraphic;
		((Control)this).AddChild((Control)(object)Button);
		((Control)this).AddChild((Control)(object)_bigActionIcon);
		((Control)this).AddChild((Control)(object)_bigItemSpriteView);
		((Control)this).AddChild((Control)(object)HighlightRect);
		((Control)this).AddChild((Control)(object)Label);
		((Control)this).AddChild((Control)(object)Cooldown);
		((Control)this).AddChild((Control)(object)val);
		((Control)Button).Modulate = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)150);
		((Control)this).OnThemeUpdated();
		((Control)this).OnKeyBindDown += OnPressed;
		((Control)this).OnKeyBindUp += OnUnpressed;
		((Control)this).TooltipSupplier = new TooltipSupplier(SupplyTooltip);
	}

	protected override void OnThemeUpdated()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).OnThemeUpdated();
		_buttonBackgroundTexture = ((Control)this).Theme.ResolveTexture("SlotBackground");
		Label.FontColorOverride = ((Control)this).Theme.ResolveColorOrSpecified("whiteText", default(Color));
	}

	private void OnPressed(GUIBoundKeyEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIClick) || !(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIRightClick))
		{
			if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UIRightClick)
			{
				Depress(args, depress: true);
			}
			this.ActionPressed?.Invoke(args, this);
		}
	}

	private void OnUnpressed(GUIBoundKeyEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIClick) || !(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIRightClick))
		{
			if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UIRightClick)
			{
				Depress(args, depress: false);
			}
			this.ActionUnpressed?.Invoke(args, this);
		}
	}

	private Control? SupplyTooltip(Control sender)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		IEntityManager entities = _entities;
		Entity<ActionComponent>? action = Action;
		MetaDataComponent val = default(MetaDataComponent);
		if (!entities.TryGetComponent<MetaDataComponent>(action.HasValue ? new EntityUid?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((EntityUid?)null), ref val))
		{
			return null;
		}
		FormattedMessage name = FormattedMessage.FromMarkupPermissive(Loc.GetString(val.EntityName));
		FormattedMessage desc = FormattedMessage.FromMarkupPermissive(Loc.GetString(val.EntityDescription));
		FormattedMessage val2 = null;
		IEntityManager entities2 = _entities;
		action = Action;
		LimitedChargesComponent limitedChargesComponent = default(LimitedChargesComponent);
		if (entities2.TryGetComponent<LimitedChargesComponent>(action.HasValue ? new EntityUid?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((EntityUid?)null), ref limitedChargesComponent))
		{
			int currentCharges = _sharedChargesSys.GetCurrentCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit((ValueTuple<EntityUid, LimitedChargesComponent, AutoRechargeComponent>)(Entity<ActionComponent>.op_Implicit(Action.Value), limitedChargesComponent, null)));
			val2 = FormattedMessage.FromMarkupPermissive(Loc.GetString($"Charges: {currentCharges.ToString()}/{limitedChargesComponent.MaxCharges}"));
			IEntityManager entities3 = _entities;
			action = Action;
			AutoRechargeComponent item = default(AutoRechargeComponent);
			if (entities3.TryGetComponent<AutoRechargeComponent>(action.HasValue ? new EntityUid?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((EntityUid?)null), ref item))
			{
				TimeSpan nextRechargeTime = _sharedChargesSys.GetNextRechargeTime(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(Action.Value), limitedChargesComponent, item)));
				val2.AddText(Loc.GetString($"{Environment.NewLine}Time Til Recharge: {nextRechargeTime}"));
			}
		}
		return (Control?)(object)new ActionAlertTooltip(name, desc, null, val2);
	}

	protected override void ControlFocusExited()
	{
		this.ActionFocusExited?.Invoke(this);
	}

	private void UpdateItemIcon()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		ActionComponent actionComponent = Action?.Comp;
		if (actionComponent != null)
		{
			EntityUid? entityIcon = actionComponent.EntityIcon;
			if (entityIcon.HasValue)
			{
				EntityUid valueOrDefault = entityIcon.GetValueOrDefault();
				if (_entities.HasComponent<SpriteComponent>(valueOrDefault))
				{
					ItemActionIconStyle? itemActionIconStyle = Action?.Comp.ItemIconStyle;
					if (itemActionIconStyle.HasValue)
					{
						switch (itemActionIconStyle.GetValueOrDefault())
						{
						case ItemActionIconStyle.BigItem:
							((Control)_bigItemSpriteView).Visible = true;
							_bigItemSpriteView.SetEntity((EntityUid?)valueOrDefault);
							((Control)_smallItemSpriteView).Visible = false;
							_smallItemSpriteView.SetEntity((EntityUid?)null);
							break;
						case ItemActionIconStyle.BigAction:
							((Control)_bigItemSpriteView).Visible = false;
							_bigItemSpriteView.SetEntity((EntityUid?)null);
							((Control)_smallItemSpriteView).Visible = true;
							_smallItemSpriteView.SetEntity((EntityUid?)valueOrDefault);
							break;
						case ItemActionIconStyle.NoItem:
							((Control)_bigItemSpriteView).Visible = false;
							_bigItemSpriteView.SetEntity((EntityUid?)null);
							((Control)_smallItemSpriteView).Visible = false;
							_smallItemSpriteView.SetEntity((EntityUid?)null);
							break;
						}
					}
					return;
				}
			}
		}
		((Control)_bigItemSpriteView).Visible = false;
		_bigItemSpriteView.SetEntity((EntityUid?)null);
		((Control)_smallItemSpriteView).Visible = false;
		_smallItemSpriteView.SetEntity((EntityUid?)null);
	}

	private void SetActionIcon(Texture? texture)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		ActionComponent actionComponent = Action?.Comp;
		if (actionComponent == null || texture == null)
		{
			_bigActionIcon.Texture = null;
			((Control)_bigActionIcon).Visible = false;
			_smallActionIcon.Texture = null;
			((Control)_smallActionIcon).Visible = false;
		}
		else if (actionComponent.EntityIcon.HasValue && actionComponent.ItemIconStyle == ItemActionIconStyle.BigItem)
		{
			_smallActionIcon.Texture = texture;
			((Control)_smallActionIcon).Modulate = actionComponent.IconColor;
			((Control)_smallActionIcon).Visible = true;
			_bigActionIcon.Texture = null;
			((Control)_bigActionIcon).Visible = false;
		}
		else
		{
			_bigActionIcon.Texture = texture;
			((Control)_bigActionIcon).Modulate = actionComponent.IconColor;
			((Control)_bigActionIcon).Visible = true;
			_smallActionIcon.Texture = null;
			((Control)_smallActionIcon).Visible = false;
		}
	}

	public void UpdateIcons()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		UpdateItemIcon();
		UpdateBackground();
		Entity<ActionComponent>? action = Action;
		if (action.HasValue)
		{
			Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
			if (_controller == null)
			{
				_controller = ((Control)this).UserInterfaceManager.GetUIController<ActionUIController>();
			}
			if (_spriteSys == null)
			{
				_spriteSys = _entities.System<SpriteSystem>();
			}
			SpriteSpecifier val = valueOrDefault.Comp.Icon;
			EntityUid? selectingTargetFor = _controller.SelectingTargetFor;
			EntityUid val2 = Entity<ActionComponent>.op_Implicit(valueOrDefault);
			if ((selectingTargetFor.HasValue && selectingTargetFor.GetValueOrDefault() == val2) || valueOrDefault.Comp.Toggled)
			{
				SpriteSpecifier iconOn = valueOrDefault.Comp.IconOn;
				if (iconOn != null)
				{
					val = iconOn;
				}
				SpriteSpecifier backgroundOn = valueOrDefault.Comp.BackgroundOn;
				if (backgroundOn != null)
				{
					_buttonBackgroundTexture = _spriteSys.Frame0(backgroundOn);
				}
			}
			else
			{
				_buttonBackgroundTexture = ((Control)this).Theme.ResolveTexture("SlotBackground");
			}
			SetActionIcon((val != null) ? _spriteSys.Frame0(val) : null);
		}
		else
		{
			SetActionIcon(null);
		}
	}

	public void UpdateBackground()
	{
		if (_controller == null)
		{
			_controller = ((Control)this).UserInterfaceManager.GetUIController<ActionUIController>();
		}
		if (!Action.HasValue)
		{
			if (_controller.IsDragging)
			{
				int positionInParent = ((Control)this).GetPositionInParent();
				Control parent = ((Control)this).Parent;
				if (positionInParent == ((parent != null) ? new int?(parent.ChildCount - 1) : ((int?)null)))
				{
					goto IL_0071;
				}
			}
			Button.Texture = null;
			return;
		}
		goto IL_0071;
		IL_0071:
		Button.Texture = _buttonBackgroundTexture;
	}

	public bool TryReplaceWith(EntityUid actionId, ActionsSystem system)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (Locked)
		{
			return false;
		}
		UpdateData(actionId, system);
		return true;
	}

	public void UpdateData(EntityUid? actionId, ActionsSystem system)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = actionId;
		Action = system.GetAction(val.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(val.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		((Control)Label).Visible = Action.HasValue;
		UpdateIcons();
	}

	public void ClearData()
	{
		Action = null;
		((Control)Cooldown).Visible = false;
		Cooldown.Progress = 1f;
		((Control)Label).Visible = false;
		UpdateIcons();
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		UpdateBackground();
		((Control)Cooldown).Visible = Action?.Comp.Cooldown.HasValue ?? false;
		ActionComponent actionComponent = Action?.Comp;
		if (actionComponent != null)
		{
			ActionCooldown? cooldown = actionComponent.Cooldown;
			if (cooldown.HasValue)
			{
				ActionCooldown valueOrDefault = cooldown.GetValueOrDefault();
				Cooldown.FromTime(valueOrDefault.Start, valueOrDefault.End);
			}
			if (_toggled != actionComponent.Toggled)
			{
				_toggled = actionComponent.Toggled;
			}
		}
	}

	protected override void MouseEntered()
	{
		((Control)this).MouseEntered();
		((Control)this).UserInterfaceManager.HoverSound();
		_beingHovered = true;
		DrawModeChanged();
	}

	protected override void MouseExited()
	{
		((Control)this).MouseExited();
		_beingHovered = false;
		DrawModeChanged();
	}

	public void Depress(GUIBoundKeyEventArgs args, bool depress)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		ActionComponent actionComponent = Action?.Comp;
		if (actionComponent != null && actionComponent.Enabled)
		{
			_depressed = depress;
			DrawModeChanged();
		}
	}

	public void DrawModeChanged()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		if (_controller == null)
		{
			_controller = ((Control)this).UserInterfaceManager.GetUIController<ActionUIController>();
		}
		((Control)HighlightRect).Visible = _beingHovered && (Action.HasValue || _controller.IsDragging);
		ActionComponent actionComponent = Action?.Comp;
		if (actionComponent == null)
		{
			((Control)this).SetOnlyStylePseudoClass("normal");
			return;
		}
		if (_beingHovered && (_controller.IsDragging || actionComponent.Enabled))
		{
			((Control)this).SetOnlyStylePseudoClass("hover");
		}
		if (_depressed && !_beingHovered)
		{
			((Control)HighlightRect).Visible = false;
			((Control)this).SetOnlyStylePseudoClass("pressed");
			return;
		}
		if (!actionComponent.Toggled)
		{
			EntityUid? selectingTargetFor = _controller.SelectingTargetFor;
			EntityUid? val = Action?.Owner;
			if (selectingTargetFor.HasValue != val.HasValue || (selectingTargetFor.HasValue && !(selectingTargetFor.GetValueOrDefault() == val.GetValueOrDefault())))
			{
				if (!actionComponent.Enabled)
				{
					((Control)this).SetOnlyStylePseudoClass("disabled");
				}
				else
				{
					((Control)this).SetOnlyStylePseudoClass("normal");
				}
				return;
			}
		}
		((Control)this).SetOnlyStylePseudoClass((actionComponent.IconOn != null) ? "normal" : "pressed");
	}
}
