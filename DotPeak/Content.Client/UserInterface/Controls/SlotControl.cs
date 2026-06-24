// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.SlotControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Cooldown;
using Content.Client.UserInterface.Systems.Inventory.Controls;
using Content.Shared._PUBG.Armor;
using Content.Shared._RMC14.IconLabel;
using Robust.Client.GameObjects;
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
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public abstract class SlotControl : Control, IEntityControl
{
  private static readonly ISawmill Sawmill = Logger.GetSawmill("ui.slot_control");
  [Dependency]
  private IEntityManager _entities;
  [Dependency]
  private ILocalizationManager _loc;
  public static int DefaultButtonSize = 64 /*0x40*/;
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
      Robust.Shared.GameObjects.Entity<SpriteComponent, TransformComponent>? entity = this.SpriteView.Entity;
      return !entity.HasValue ? new EntityUid?() : new EntityUid?(Robust.Shared.GameObjects.Entity<SpriteComponent, TransformComponent>.op_Implicit(entity.GetValueOrDefault()));
    }
  }

  public string SlotName
  {
    get => this._slotName;
    set
    {
      if (this._slotNameSet)
      {
        SlotControl.Sawmill.Warning("Tried to set slotName after init for:" + this.Name);
      }
      else
      {
        this._slotNameSet = true;
        if (this.Parent is IItemslotUIContainer parent)
          parent.TryRegisterButton(this, value);
        this.Name = "SlotButton_" + value;
        this._slotName = value;
      }
    }
  }

  public bool Highlight
  {
    get => ((Control) this.HighlightRect).Visible;
    set => ((Control) this.HighlightRect).Visible = value;
  }

  public bool Blocked
  {
    get => ((Control) this.BlockedRect).Visible;
    set => ((Control) this.BlockedRect).Visible = value;
  }

  public string? BlockedTexturePath
  {
    get => this._blockedTexturePath;
    set
    {
      this._blockedTexturePath = value;
      this.BlockedRect.Texture = this.Theme.ResolveTextureOrNull(this._blockedTexturePath)?.Texture;
    }
  }

  public string? ButtonTexturePath
  {
    get => this._buttonTexturePath;
    set
    {
      this._buttonTexturePath = value;
      this.UpdateChildren();
    }
  }

  public string? FullButtonTexturePath
  {
    get => this._fullButtonTexturePath;
    set
    {
      this._fullButtonTexturePath = value;
      this.UpdateChildren();
    }
  }

  public string? StorageTexturePath
  {
    get => this._buttonTexturePath;
    set
    {
      this._storageTexturePath = value;
      this.StorageButton.TextureNormal = this.Theme.ResolveTextureOrNull(this._storageTexturePath)?.Texture;
    }
  }

  public string? HighlightTexturePath
  {
    get => this._highlightTexturePath;
    set
    {
      this._highlightTexturePath = value;
      this.HighlightRect.Texture = this.Theme.ResolveTextureOrNull(this._highlightTexturePath)?.Texture;
    }
  }

  public event Action<GUIBoundKeyEventArgs, SlotControl>? Pressed;

  public event Action<GUIBoundKeyEventArgs, SlotControl>? Unpressed;

  public event Action<GUIBoundKeyEventArgs, SlotControl>? StoragePressed;

  public event Action<GUIMouseHoverEventArgs, SlotControl>? Hover;

  public bool EntityHover => this.HoverSpriteView.Sprite != null;

  public SlotControl()
  {
    IoCManager.InjectDependencies<SlotControl>(this);
    this.Name = "SlotButton_null";
    this.MinSize = new Vector2((float) SlotControl.DefaultButtonSize, (float) SlotControl.DefaultButtonSize);
    TextureRect textureRect1 = new TextureRect();
    textureRect1.TextureScale = new Vector2(2f, 2f);
    ((Control) textureRect1).MouseFilter = (Control.MouseFilterMode) 0;
    TextureRect textureRect2 = textureRect1;
    this.ButtonRect = textureRect1;
    this.AddChild((Control) textureRect2);
    TextureRect textureRect3 = new TextureRect();
    ((Control) textureRect3).Visible = false;
    textureRect3.TextureScale = new Vector2(2f, 2f);
    ((Control) textureRect3).MouseFilter = (Control.MouseFilterMode) 2;
    TextureRect textureRect4 = textureRect3;
    this.HighlightRect = textureRect3;
    this.AddChild((Control) textureRect4);
    ((Control) this.ButtonRect).OnKeyBindDown += new Action<GUIBoundKeyEventArgs>(this.OnButtonPressed);
    ((Control) this.ButtonRect).OnKeyBindUp += new Action<GUIBoundKeyEventArgs>(this.OnButtonUnpressed);
    SpriteView spriteView1 = new SpriteView();
    spriteView1.Scale = new Vector2(2f, 2f);
    ((Control) spriteView1).SetSize = new Vector2((float) SlotControl.DefaultButtonSize, (float) SlotControl.DefaultButtonSize);
    spriteView1.OverrideDirection = new Direction?((Direction) 0);
    SpriteView spriteView2 = spriteView1;
    this.SpriteView = spriteView1;
    this.AddChild((Control) spriteView2);
    SpriteView spriteView3 = new SpriteView();
    spriteView3.Scale = new Vector2(2f, 2f);
    ((Control) spriteView3).SetSize = new Vector2((float) SlotControl.DefaultButtonSize, (float) SlotControl.DefaultButtonSize);
    spriteView3.OverrideDirection = new Direction?((Direction) 0);
    SpriteView spriteView4 = spriteView3;
    this.HoverSpriteView = spriteView3;
    this.AddChild((Control) spriteView4);
    Label label1 = new Label();
    label1.Text = "";
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 1;
    ((Control) label1).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) label1).Visible = true;
    ((Control) label1).Margin = new Thickness(10f, 0.0f, 0.0f, 0.0f);
    Label label2 = label1;
    this.IconLabel = label1;
    this.AddChild((Control) label2);
    TextureButton textureButton1 = new TextureButton();
    textureButton1.Scale = new Vector2(0.75f, 0.75f);
    ((Control) textureButton1).HorizontalAlignment = (Control.HAlignment) 3;
    ((Control) textureButton1).VerticalAlignment = (Control.VAlignment) 3;
    ((Control) textureButton1).Visible = false;
    TextureButton textureButton2 = textureButton1;
    this.StorageButton = textureButton1;
    this.AddChild((Control) textureButton2);
    ((Control) this.StorageButton).OnKeyBindDown += (Action<GUIBoundKeyEventArgs>) (args =>
    {
      if (!BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick))
        return;
      this.OnButtonPressed(args);
    });
    ((BaseButton) this.StorageButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnStorageButtonPressed);
    ((Control) this.ButtonRect).OnMouseEntered += (Action<GUIMouseHoverEventArgs>) (_ => this.MouseIsHovering = true);
    ((Control) this.ButtonRect).OnMouseEntered += new Action<GUIMouseHoverEventArgs>(this.OnButtonHover);
    ((Control) this.ButtonRect).OnMouseExited += (Action<GUIMouseHoverEventArgs>) (_ =>
    {
      this.MouseIsHovering = false;
      this.ClearHover();
    });
    CooldownGraphic cooldownGraphic1 = new CooldownGraphic();
    cooldownGraphic1.Visible = false;
    CooldownGraphic cooldownGraphic2 = cooldownGraphic1;
    this.CooldownDisplay = cooldownGraphic1;
    this.AddChild((Control) cooldownGraphic2);
    TextureRect textureRect5 = new TextureRect();
    textureRect5.TextureScale = new Vector2(2f, 2f);
    ((Control) textureRect5).MouseFilter = (Control.MouseFilterMode) 0;
    ((Control) textureRect5).Visible = false;
    TextureRect textureRect6 = textureRect5;
    this.BlockedRect = textureRect5;
    this.AddChild((Control) textureRect6);
    this.HighlightTexturePath = "slot_highlight";
    this.BlockedTexturePath = "blocked";
  }

  public void ClearHover()
  {
    if (!this.EntityHover)
      return;
    Robust.Shared.GameObjects.Entity<SpriteComponent, TransformComponent>? entity = this.HoverSpriteView.Entity;
    if (entity.HasValue)
    {
      IEntityManager ientityManager = IoCManager.Resolve<IEntityManager>();
      Robust.Shared.GameObjects.Entity<SpriteComponent, TransformComponent>? nullable1 = entity;
      EntityUid? nullable2 = nullable1.HasValue ? new EntityUid?(Robust.Shared.GameObjects.Entity<SpriteComponent, TransformComponent>.op_Implicit(nullable1.GetValueOrDefault())) : new EntityUid?();
      ientityManager.QueueDeleteEntity(nullable2);
    }
    this.HoverSpriteView.SetEntity(new EntityUid?());
  }

  public void SetEntity(EntityUid? ent)
  {
    this.SpriteView.SetEntity(ent);
    this.UpdateChildren();
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    this.UpdateArmorTint();
  }

  private void UpdateChildren()
  {
    TextureResource textureResource = this.Theme.ResolveTextureOrNull(this._fullButtonTexturePath);
    this.ButtonRect.Texture = !this.Entity.HasValue || textureResource == null ? this.Theme.ResolveTextureOrNull(this._buttonTexturePath)?.Texture : textureResource.Texture;
    this.IconLabel.Text = "";
    this.IconLabel.FontColorOverride = new Color?(Color.White);
    IconLabelComponent iconLabelComponent;
    if (this._entities.TryGetComponent<IconLabelComponent>(this.Entity, ref iconLabelComponent))
    {
      if (iconLabelComponent.LabelTextLocId.HasValue)
      {
        ILocalizationManager loc = this._loc;
        LocId? labelTextLocId = iconLabelComponent.LabelTextLocId;
        string str1 = labelTextLocId.HasValue ? LocId.op_Implicit(labelTextLocId.GetValueOrDefault()) : (string) null;
        string str2;
        ref string local = ref str2;
        (string, object)[] array = iconLabelComponent.LabelTextParams.ToArray();
        if (loc.TryGetString(str1, ref local, array))
        {
          if (str2.Length > iconLabelComponent.LabelMaxSize)
            str2 = str2.Substring(0, iconLabelComponent.LabelMaxSize);
          this.IconLabel.Text = str2;
        }
      }
      Color color;
      if (Color.TryFromName(iconLabelComponent.TextColor, ref color))
        this.IconLabel.FontColorOverride = new Color?(color);
      ((Control) this.IconLabel).SetSize = new Vector2((float) iconLabelComponent.TextSize);
    }
    this.UpdateArmorTint();
  }

  private void UpdateArmorTint()
  {
    EntityUid? entity = this.Entity;
    if (entity.HasValue)
    {
      EntityUid valueOrDefault = entity.GetValueOrDefault();
      PubgArmorComponent component;
      if (this._entities.TryGetComponent<PubgArmorComponent>(valueOrDefault, ref component))
      {
        if (this._cachedArmorTintActive)
        {
          EntityUid? cachedArmorTintEntity = this._cachedArmorTintEntity;
          EntityUid entityUid = valueOrDefault;
          if ((cachedArmorTintEntity.HasValue ? (EntityUid.op_Equality(cachedArmorTintEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) != 0 && (double) this._cachedArmorTintDurability == (double) component.Durability && (double) this._cachedArmorTintMaxDurability == (double) component.MaxDurability)
            return;
        }
        this._cachedArmorTintEntity = new EntityUid?(valueOrDefault);
        this._cachedArmorTintDurability = component.Durability;
        this._cachedArmorTintMaxDurability = component.MaxDurability;
        this._cachedArmorTintActive = true;
        Color durabilityColor = PubgArmorHelpers.GetDurabilityColor(PubgArmorHelpers.GetDurabilityRatio(component));
        ((Control) this.SpriteView).Modulate = durabilityColor;
        ((Control) this.HoverSpriteView).Modulate = durabilityColor;
        return;
      }
    }
    if (!this._cachedArmorTintActive)
      return;
    this._cachedArmorTintEntity = new EntityUid?();
    this._cachedArmorTintDurability = 0.0f;
    this._cachedArmorTintMaxDurability = 0.0f;
    this._cachedArmorTintActive = false;
    ((Control) this.SpriteView).Modulate = Color.White;
    ((Control) this.HoverSpriteView).Modulate = Color.White;
  }

  private void OnButtonPressed(GUIBoundKeyEventArgs args)
  {
    Action<GUIBoundKeyEventArgs, SlotControl> pressed = this.Pressed;
    if (pressed == null)
      return;
    pressed(args, this);
  }

  private void OnButtonUnpressed(GUIBoundKeyEventArgs args)
  {
    Action<GUIBoundKeyEventArgs, SlotControl> unpressed = this.Unpressed;
    if (unpressed == null)
      return;
    unpressed(args, this);
  }

  private void OnStorageButtonPressed(BaseButton.ButtonEventArgs args)
  {
    if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args.Event).Function, EngineKeyFunctions.UIClick))
    {
      Action<GUIBoundKeyEventArgs, SlotControl> storagePressed = this.StoragePressed;
      if (storagePressed == null)
        return;
      storagePressed(args.Event, this);
    }
    else
    {
      Action<GUIBoundKeyEventArgs, SlotControl> pressed = this.Pressed;
      if (pressed == null)
        return;
      pressed(args.Event, this);
    }
  }

  private void OnButtonHover(GUIMouseHoverEventArgs args)
  {
    Action<GUIMouseHoverEventArgs, SlotControl> hover = this.Hover;
    if (hover == null)
      return;
    hover(args, this);
  }

  protected virtual void OnThemeUpdated()
  {
    base.OnThemeUpdated();
    this.StorageButton.TextureNormal = this.Theme.ResolveTextureOrNull(this._storageTexturePath)?.Texture;
    this.HighlightRect.Texture = this.Theme.ResolveTextureOrNull(this._highlightTexturePath)?.Texture;
    this.UpdateChildren();
  }

  EntityUid? IEntityControl.UiEntity => this.Entity;
}
