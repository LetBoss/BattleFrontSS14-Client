// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Actions.Controls.ActionButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Numerics;

#nullable enable
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
      this._keybind = value;
      if (!this._keybind.HasValue)
        return;
      this.Label.Text = BoundKeyHelper.ShortKeyName(this._keybind.Value);
    }
  }

  public Entity<ActionComponent>? Action { get; private set; }

  public bool Locked { get; set; }

  public event System.Action<GUIBoundKeyEventArgs, ActionButton>? ActionPressed;

  public event System.Action<GUIBoundKeyEventArgs, ActionButton>? ActionUnpressed;

  public event System.Action<ActionButton>? ActionFocusExited;

  public ActionButton(
    IEntityManager entities,
    SpriteSystem? spriteSys = null,
    ActionUIController? controller = null)
  {
    this._entities = entities;
    this._spriteSys = spriteSys;
    this._sharedChargesSys = this._entities.System<SharedChargesSystem>();
    this._controller = controller;
    this.MouseFilter = (Control.MouseFilterMode) 1;
    TextureRect textureRect1 = new TextureRect();
    ((Control) textureRect1).Name = nameof (Button);
    textureRect1.TextureScale = new Vector2(2f, 2f);
    this.Button = textureRect1;
    PanelContainer panelContainer = new PanelContainer();
    ((Control) panelContainer).StyleClasses.Add("HandSlotHighlight");
    ((Control) panelContainer).MinSize = new Vector2(32f, 32f);
    ((Control) panelContainer).Visible = false;
    this.HighlightRect = panelContainer;
    TextureRect textureRect2 = new TextureRect();
    ((Control) textureRect2).HorizontalExpand = true;
    ((Control) textureRect2).VerticalExpand = true;
    textureRect2.Stretch = (TextureRect.StretchMode) 1;
    ((Control) textureRect2).Visible = false;
    this._bigActionIcon = textureRect2;
    TextureRect textureRect3 = new TextureRect();
    ((Control) textureRect3).HorizontalAlignment = (Control.HAlignment) 3;
    ((Control) textureRect3).VerticalAlignment = (Control.VAlignment) 3;
    textureRect3.Stretch = (TextureRect.StretchMode) 1;
    ((Control) textureRect3).Visible = false;
    this._smallActionIcon = textureRect3;
    Label label = new Label();
    ((Control) label).Name = nameof (Label);
    ((Control) label).HorizontalAlignment = (Control.HAlignment) 1;
    ((Control) label).VerticalAlignment = (Control.VAlignment) 1;
    ((Control) label).Margin = new Thickness(5f, 0.0f, 0.0f, 0.0f);
    this.Label = label;
    SpriteView spriteView1 = new SpriteView();
    ((Control) spriteView1).Name = "Big Sprite";
    ((Control) spriteView1).HorizontalExpand = true;
    ((Control) spriteView1).VerticalExpand = true;
    spriteView1.Scale = new Vector2(2f, 2f);
    ((Control) spriteView1).SetSize = new Vector2(64f, 64f);
    ((Control) spriteView1).Visible = false;
    spriteView1.OverrideDirection = new Direction?((Direction) 0);
    this._bigItemSpriteView = spriteView1;
    SpriteView spriteView2 = new SpriteView();
    ((Control) spriteView2).Name = "Small Sprite";
    ((Control) spriteView2).HorizontalAlignment = (Control.HAlignment) 3;
    ((Control) spriteView2).VerticalAlignment = (Control.VAlignment) 3;
    ((Control) spriteView2).Visible = false;
    spriteView2.OverrideDirection = new Direction?((Direction) 0);
    this._smallItemSpriteView = spriteView2;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    ((Control) boxContainer1).MinSize = new Vector2(64f, 64f);
    BoxContainer boxContainer2 = boxContainer1;
    ((Control) boxContainer2).AddChild(new Control()
    {
      MinSize = new Vector2(32f, 32f)
    });
    BoxContainer boxContainer3 = boxContainer2;
    Control control = new Control();
    control.Children.Add((Control) this._smallActionIcon);
    control.Children.Add((Control) this._smallItemSpriteView);
    ((Control) boxContainer3).AddChild(control);
    CooldownGraphic cooldownGraphic = new CooldownGraphic();
    cooldownGraphic.Visible = false;
    this.Cooldown = cooldownGraphic;
    this.AddChild((Control) this.Button);
    this.AddChild((Control) this._bigActionIcon);
    this.AddChild((Control) this._bigItemSpriteView);
    this.AddChild((Control) this.HighlightRect);
    this.AddChild((Control) this.Label);
    this.AddChild((Control) this.Cooldown);
    this.AddChild((Control) boxContainer2);
    ((Control) this.Button).Modulate = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte) 150);
    base.OnThemeUpdated();
    this.OnKeyBindDown += new System.Action<GUIBoundKeyEventArgs>(this.OnPressed);
    this.OnKeyBindUp += new System.Action<GUIBoundKeyEventArgs>(this.OnUnpressed);
    // ISSUE: method pointer
    this.TooltipSupplier = new TooltipSupplier((object) this, __methodptr(SupplyTooltip));
  }

  protected virtual void OnThemeUpdated()
  {
    base.OnThemeUpdated();
    this._buttonBackgroundTexture = this.Theme.ResolveTexture("SlotBackground");
    this.Label.FontColorOverride = new Color?(this.Theme.ResolveColorOrSpecified("whiteText", new Color()));
  }

  private void OnPressed(GUIBoundKeyEventArgs args)
  {
    if (BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick) && BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIRightClick))
      return;
    if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIRightClick))
      this.Depress(args, true);
    System.Action<GUIBoundKeyEventArgs, ActionButton> actionPressed = this.ActionPressed;
    if (actionPressed == null)
      return;
    actionPressed(args, this);
  }

  private void OnUnpressed(GUIBoundKeyEventArgs args)
  {
    if (BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick) && BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIRightClick))
      return;
    if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIRightClick))
      this.Depress(args, false);
    System.Action<GUIBoundKeyEventArgs, ActionButton> actionUnpressed = this.ActionUnpressed;
    if (actionUnpressed == null)
      return;
    actionUnpressed(args, this);
  }

  private Control? SupplyTooltip(Control sender)
  {
    IEntityManager entities1 = this._entities;
    Entity<ActionComponent>? action = this.Action;
    EntityUid? nullable1 = action.HasValue ? new EntityUid?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : new EntityUid?();
    MetaDataComponent metaDataComponent;
    ref MetaDataComponent local1 = ref metaDataComponent;
    if (!entities1.TryGetComponent<MetaDataComponent>(nullable1, ref local1))
      return (Control) null;
    FormattedMessage name = FormattedMessage.FromMarkupPermissive(Loc.GetString(metaDataComponent.EntityName));
    FormattedMessage formattedMessage1 = FormattedMessage.FromMarkupPermissive(Loc.GetString(metaDataComponent.EntityDescription));
    FormattedMessage formattedMessage2 = (FormattedMessage) null;
    IEntityManager entities2 = this._entities;
    action = this.Action;
    EntityUid? nullable2 = action.HasValue ? new EntityUid?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : new EntityUid?();
    LimitedChargesComponent chargesComponent;
    ref LimitedChargesComponent local2 = ref chargesComponent;
    if (entities2.TryGetComponent<LimitedChargesComponent>(nullable2, ref local2))
    {
      SharedChargesSystem sharedChargesSys1 = this._sharedChargesSys;
      action = this.Action;
      Entity<LimitedChargesComponent, AutoRechargeComponent> entity1 = Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action.Value), chargesComponent, (AutoRechargeComponent) null));
      formattedMessage2 = FormattedMessage.FromMarkupPermissive(Loc.GetString($"Charges: {sharedChargesSys1.GetCurrentCharges(entity1).ToString()}/{chargesComponent.MaxCharges}"));
      IEntityManager entities3 = this._entities;
      action = this.Action;
      EntityUid? nullable3 = action.HasValue ? new EntityUid?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : new EntityUid?();
      AutoRechargeComponent rechargeComponent;
      ref AutoRechargeComponent local3 = ref rechargeComponent;
      if (entities3.TryGetComponent<AutoRechargeComponent>(nullable3, ref local3))
      {
        SharedChargesSystem sharedChargesSys2 = this._sharedChargesSys;
        action = this.Action;
        Entity<LimitedChargesComponent, AutoRechargeComponent> entity2 = Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action.Value), chargesComponent, rechargeComponent));
        TimeSpan nextRechargeTime = sharedChargesSys2.GetNextRechargeTime(entity2);
        formattedMessage2.AddText(Loc.GetString($"{Environment.NewLine}Time Til Recharge: {nextRechargeTime}"));
      }
    }
    FormattedMessage desc = formattedMessage1;
    FormattedMessage charges = formattedMessage2;
    return (Control) new ActionAlertTooltip(name, desc, charges: charges);
  }

  protected virtual void ControlFocusExited()
  {
    System.Action<ActionButton> actionFocusExited = this.ActionFocusExited;
    if (actionFocusExited == null)
      return;
    actionFocusExited(this);
  }

  private void UpdateItemIcon()
  {
    Entity<ActionComponent>? action = this.Action;
    ref Entity<ActionComponent>? local1 = ref action;
    ActionComponent comp = local1.HasValue ? local1.GetValueOrDefault().Comp : (ActionComponent) null;
    EntityUid? nullable1;
    if (comp != null)
    {
      nullable1 = comp.EntityIcon;
      if (nullable1.HasValue)
      {
        EntityUid valueOrDefault = nullable1.GetValueOrDefault();
        if (this._entities.HasComponent<SpriteComponent>(valueOrDefault))
        {
          action = this.Action;
          ref Entity<ActionComponent>? local2 = ref action;
          ItemActionIconStyle? nullable2 = local2.HasValue ? new ItemActionIconStyle?(local2.GetValueOrDefault().Comp.ItemIconStyle) : new ItemActionIconStyle?();
          if (!nullable2.HasValue)
            return;
          switch (nullable2.GetValueOrDefault())
          {
            case ItemActionIconStyle.BigItem:
              ((Control) this._bigItemSpriteView).Visible = true;
              this._bigItemSpriteView.SetEntity(new EntityUid?(valueOrDefault));
              ((Control) this._smallItemSpriteView).Visible = false;
              SpriteView smallItemSpriteView1 = this._smallItemSpriteView;
              nullable1 = new EntityUid?();
              EntityUid? nullable3 = nullable1;
              smallItemSpriteView1.SetEntity(nullable3);
              return;
            case ItemActionIconStyle.BigAction:
              ((Control) this._bigItemSpriteView).Visible = false;
              SpriteView bigItemSpriteView1 = this._bigItemSpriteView;
              nullable1 = new EntityUid?();
              EntityUid? nullable4 = nullable1;
              bigItemSpriteView1.SetEntity(nullable4);
              ((Control) this._smallItemSpriteView).Visible = true;
              this._smallItemSpriteView.SetEntity(new EntityUid?(valueOrDefault));
              return;
            case ItemActionIconStyle.NoItem:
              ((Control) this._bigItemSpriteView).Visible = false;
              SpriteView bigItemSpriteView2 = this._bigItemSpriteView;
              nullable1 = new EntityUid?();
              EntityUid? nullable5 = nullable1;
              bigItemSpriteView2.SetEntity(nullable5);
              ((Control) this._smallItemSpriteView).Visible = false;
              SpriteView smallItemSpriteView2 = this._smallItemSpriteView;
              nullable1 = new EntityUid?();
              EntityUid? nullable6 = nullable1;
              smallItemSpriteView2.SetEntity(nullable6);
              return;
            default:
              return;
          }
        }
      }
    }
    ((Control) this._bigItemSpriteView).Visible = false;
    SpriteView bigItemSpriteView = this._bigItemSpriteView;
    nullable1 = new EntityUid?();
    EntityUid? nullable7 = nullable1;
    bigItemSpriteView.SetEntity(nullable7);
    ((Control) this._smallItemSpriteView).Visible = false;
    SpriteView smallItemSpriteView = this._smallItemSpriteView;
    nullable1 = new EntityUid?();
    EntityUid? nullable8 = nullable1;
    smallItemSpriteView.SetEntity(nullable8);
  }

  private void SetActionIcon(Texture? texture)
  {
    Entity<ActionComponent>? action = this.Action;
    ref Entity<ActionComponent>? local = ref action;
    ActionComponent comp = local.HasValue ? local.GetValueOrDefault().Comp : (ActionComponent) null;
    if (comp == null || texture == null)
    {
      this._bigActionIcon.Texture = (Texture) null;
      ((Control) this._bigActionIcon).Visible = false;
      this._smallActionIcon.Texture = (Texture) null;
      ((Control) this._smallActionIcon).Visible = false;
    }
    else if (comp.EntityIcon.HasValue && comp.ItemIconStyle == ItemActionIconStyle.BigItem)
    {
      this._smallActionIcon.Texture = texture;
      ((Control) this._smallActionIcon).Modulate = comp.IconColor;
      ((Control) this._smallActionIcon).Visible = true;
      this._bigActionIcon.Texture = (Texture) null;
      ((Control) this._bigActionIcon).Visible = false;
    }
    else
    {
      this._bigActionIcon.Texture = texture;
      ((Control) this._bigActionIcon).Modulate = comp.IconColor;
      ((Control) this._bigActionIcon).Visible = true;
      this._smallActionIcon.Texture = (Texture) null;
      ((Control) this._smallActionIcon).Visible = false;
    }
  }

  public void UpdateIcons()
  {
    this.UpdateItemIcon();
    this.UpdateBackground();
    Entity<ActionComponent>? action = this.Action;
    if (action.HasValue)
    {
      Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
      if (this._controller == null)
        this._controller = this.UserInterfaceManager.GetUIController<ActionUIController>();
      if (this._spriteSys == null)
        this._spriteSys = this._entities.System<SpriteSystem>();
      SpriteSpecifier spriteSpecifier = valueOrDefault.Comp.Icon;
      EntityUid? selectingTargetFor = this._controller.SelectingTargetFor;
      EntityUid entityUid = Entity<ActionComponent>.op_Implicit(valueOrDefault);
      if ((selectingTargetFor.HasValue ? (EntityUid.op_Equality(selectingTargetFor.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) != 0 || valueOrDefault.Comp.Toggled)
      {
        SpriteSpecifier iconOn = valueOrDefault.Comp.IconOn;
        if (iconOn != null)
          spriteSpecifier = iconOn;
        SpriteSpecifier backgroundOn = valueOrDefault.Comp.BackgroundOn;
        if (backgroundOn != null)
          this._buttonBackgroundTexture = this._spriteSys.Frame0(backgroundOn);
      }
      else
        this._buttonBackgroundTexture = this.Theme.ResolveTexture("SlotBackground");
      this.SetActionIcon(spriteSpecifier != null ? this._spriteSys.Frame0(spriteSpecifier) : (Texture) null);
    }
    else
      this.SetActionIcon((Texture) null);
  }

  public void UpdateBackground()
  {
    if (this._controller == null)
      this._controller = this.UserInterfaceManager.GetUIController<ActionUIController>();
    if (!this.Action.HasValue)
    {
      if (this._controller.IsDragging)
      {
        int positionInParent = this.GetPositionInParent();
        Control parent = this.Parent;
        int? nullable = parent != null ? new int?(parent.ChildCount - 1) : new int?();
        int valueOrDefault = nullable.GetValueOrDefault();
        if (positionInParent == valueOrDefault & nullable.HasValue)
          goto label_5;
      }
      this.Button.Texture = (Texture) null;
      return;
    }
label_5:
    this.Button.Texture = this._buttonBackgroundTexture;
  }

  public bool TryReplaceWith(EntityUid actionId, ActionsSystem system)
  {
    if (this.Locked)
      return false;
    this.UpdateData(new EntityUid?(actionId), system);
    return true;
  }

  public void UpdateData(EntityUid? actionId, ActionsSystem system)
  {
    ActionsSystem actionsSystem = system;
    EntityUid? nullable = actionId;
    Entity<ActionComponent>? action = nullable.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(nullable.GetValueOrDefault())) : new Entity<ActionComponent>?();
    this.Action = actionsSystem.GetAction(action);
    ((Control) this.Label).Visible = this.Action.HasValue;
    this.UpdateIcons();
  }

  public void ClearData()
  {
    this.Action = new Entity<ActionComponent>?();
    this.Cooldown.Visible = false;
    this.Cooldown.Progress = 1f;
    ((Control) this.Label).Visible = false;
    this.UpdateIcons();
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    this.UpdateBackground();
    CooldownGraphic cooldown1 = this.Cooldown;
    Entity<ActionComponent>? action = this.Action;
    ref Entity<ActionComponent>? local1 = ref action;
    int num = local1.HasValue ? (local1.GetValueOrDefault().Comp.Cooldown.HasValue ? 1 : 0) : 0;
    cooldown1.Visible = num != 0;
    action = this.Action;
    ref Entity<ActionComponent>? local2 = ref action;
    ActionComponent comp = local2.HasValue ? local2.GetValueOrDefault().Comp : (ActionComponent) null;
    if (comp == null)
      return;
    ActionCooldown? cooldown2 = comp.Cooldown;
    if (cooldown2.HasValue)
    {
      ActionCooldown valueOrDefault = cooldown2.GetValueOrDefault();
      this.Cooldown.FromTime(valueOrDefault.Start, valueOrDefault.End);
    }
    if (this._toggled == comp.Toggled)
      return;
    this._toggled = comp.Toggled;
  }

  protected virtual void MouseEntered()
  {
    base.MouseEntered();
    this.UserInterfaceManager.HoverSound();
    this._beingHovered = true;
    this.DrawModeChanged();
  }

  protected virtual void MouseExited()
  {
    base.MouseExited();
    this._beingHovered = false;
    this.DrawModeChanged();
  }

  public void Depress(GUIBoundKeyEventArgs args, bool depress)
  {
    Entity<ActionComponent>? action = this.Action;
    ref Entity<ActionComponent>? local = ref action;
    ActionComponent comp = local.HasValue ? local.GetValueOrDefault().Comp : (ActionComponent) null;
    if (comp == null || !comp.Enabled)
      return;
    this._depressed = depress;
    this.DrawModeChanged();
  }

  public void DrawModeChanged()
  {
    if (this._controller == null)
      this._controller = this.UserInterfaceManager.GetUIController<ActionUIController>();
    PanelContainer highlightRect = this.HighlightRect;
    Entity<ActionComponent>? action;
    int num;
    if (this._beingHovered)
    {
      action = this.Action;
      num = action.HasValue ? 1 : (this._controller.IsDragging ? 1 : 0);
    }
    else
      num = 0;
    ((Control) highlightRect).Visible = num != 0;
    action = this.Action;
    ref Entity<ActionComponent>? local1 = ref action;
    ActionComponent comp = local1.HasValue ? local1.GetValueOrDefault().Comp : (ActionComponent) null;
    if (comp == null)
    {
      this.SetOnlyStylePseudoClass("normal");
    }
    else
    {
      if (this._beingHovered && (this._controller.IsDragging || comp.Enabled))
        this.SetOnlyStylePseudoClass("hover");
      if (this._depressed && !this._beingHovered)
      {
        ((Control) this.HighlightRect).Visible = false;
        this.SetOnlyStylePseudoClass("pressed");
      }
      else
      {
        if (!comp.Toggled)
        {
          EntityUid? selectingTargetFor = this._controller.SelectingTargetFor;
          action = this.Action;
          ref Entity<ActionComponent>? local2 = ref action;
          EntityUid? nullable = local2.HasValue ? new EntityUid?(local2.GetValueOrDefault().Owner) : new EntityUid?();
          if ((selectingTargetFor.HasValue == nullable.HasValue ? (selectingTargetFor.HasValue ? (EntityUid.op_Equality(selectingTargetFor.GetValueOrDefault(), nullable.GetValueOrDefault()) ? 1 : 0) : 1) : 0) == 0)
          {
            if (!comp.Enabled)
            {
              this.SetOnlyStylePseudoClass("disabled");
              return;
            }
            this.SetOnlyStylePseudoClass("normal");
            return;
          }
        }
        this.SetOnlyStylePseudoClass(comp.IconOn != null ? "normal" : "pressed");
      }
    }
  }

  EntityUid? IEntityControl.UiEntity
  {
    get
    {
      Entity<ActionComponent>? action = this.Action;
      return !action.HasValue ? new EntityUid?() : new EntityUid?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault()));
    }
  }
}
