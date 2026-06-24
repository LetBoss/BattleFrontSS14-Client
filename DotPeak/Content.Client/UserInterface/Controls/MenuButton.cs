// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.MenuButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public sealed class MenuButton : ContainerButton
{
  [Dependency]
  private IInputManager _inputManager;
  public const string StyleClassLabelTopButton = "topButtonLabel";
  public const string StyleClassRedTopButton = "topButtonLabel";
  private static readonly Color ColorNormal = Color.FromHex((ReadOnlySpan<char>) "#7b7e9e", new Color?());
  private static readonly Color ColorRedNormal = Color.FromHex((ReadOnlySpan<char>) "#FEFEFE", new Color?());
  private static readonly Color ColorHovered = Color.FromHex((ReadOnlySpan<char>) "#9699bb", new Color?());
  private static readonly Color ColorRedHovered = Color.FromHex((ReadOnlySpan<char>) "#FFFFFF", new Color?());
  private static readonly Color ColorPressed = Color.FromHex((ReadOnlySpan<char>) "#789B8C", new Color?());
  private const float VertPad = 8f;
  private BoundKeyFunction _function;
  private readonly BoxContainer _root;
  private readonly TextureRect? _buttonIcon;
  private readonly Label? _buttonLabel;

  private Color NormalColor
  {
    get
    {
      return !((Control) this).HasStyleClass("topButtonLabel") ? MenuButton.ColorNormal : MenuButton.ColorRedNormal;
    }
  }

  private Color HoveredColor
  {
    get
    {
      return !((Control) this).HasStyleClass("topButtonLabel") ? MenuButton.ColorHovered : MenuButton.ColorRedHovered;
    }
  }

  public string AppendStyleClass
  {
    set => ((Control) this).AddStyleClass(value);
  }

  public Texture? Icon
  {
    get => this._buttonIcon.Texture;
    set => this._buttonIcon.Texture = value;
  }

  public BoundKeyFunction BoundKey
  {
    get => this._function;
    set
    {
      this._function = value;
      this._buttonLabel.Text = BoundKeyHelper.ShortKeyName(value);
    }
  }

  public BoxContainer ButtonRoot => this._root;

  public MenuButton()
  {
    IoCManager.InjectDependencies<MenuButton>(this);
    TextureRect textureRect = new TextureRect();
    textureRect.TextureScale = new Vector2(0.5f, 0.5f);
    ((Control) textureRect).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) textureRect).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) textureRect).VerticalExpand = true;
    ((Control) textureRect).Margin = new Thickness(0.0f, 8f);
    ((Control) textureRect).ModulateSelfOverride = new Color?(this.NormalColor);
    textureRect.Stretch = (TextureRect.StretchMode) 4;
    this._buttonIcon = textureRect;
    Label label = new Label();
    label.Text = "";
    ((Control) label).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label).ModulateSelfOverride = new Color?(this.NormalColor);
    ((Control) label).StyleClasses.Add("topButtonLabel");
    this._buttonLabel = label;
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer).Children.Add((Control) this._buttonIcon);
    ((Control) boxContainer).Children.Add((Control) this._buttonLabel);
    this._root = boxContainer;
    ((Control) this).AddChild((Control) this._root);
    ((BaseButton) this).ToggleMode = true;
  }

  protected virtual void EnteredTree()
  {
    this._inputManager.OnKeyBindingAdded += new Action<IKeyBinding>(this.OnKeyBindingChanged);
    this._inputManager.OnKeyBindingRemoved += new Action<IKeyBinding>(this.OnKeyBindingChanged);
    this._inputManager.OnInputModeChanged += new Action(this.OnKeyBindingChanged);
  }

  protected virtual void ExitedTree()
  {
    this._inputManager.OnKeyBindingAdded -= new Action<IKeyBinding>(this.OnKeyBindingChanged);
    this._inputManager.OnKeyBindingRemoved -= new Action<IKeyBinding>(this.OnKeyBindingChanged);
    this._inputManager.OnInputModeChanged -= new Action(this.OnKeyBindingChanged);
  }

  private void OnKeyBindingChanged(IKeyBinding obj)
  {
    this._buttonLabel.Text = BoundKeyHelper.ShortKeyName(this._function);
  }

  private void OnKeyBindingChanged()
  {
    this._buttonLabel.Text = BoundKeyHelper.ShortKeyName(this._function);
  }

  protected virtual void StylePropertiesChanged()
  {
    ((Control) this).StylePropertiesChanged();
    this.UpdateChildColors();
  }

  private void UpdateChildColors()
  {
    if (this._buttonIcon == null || this._buttonLabel == null)
      return;
    switch ((int) ((BaseButton) this).DrawMode)
    {
      case 0:
        ((Control) this._buttonIcon).ModulateSelfOverride = new Color?(this.NormalColor);
        ((Control) this._buttonLabel).ModulateSelfOverride = new Color?(this.NormalColor);
        break;
      case 1:
        ((Control) this._buttonIcon).ModulateSelfOverride = new Color?(MenuButton.ColorPressed);
        ((Control) this._buttonLabel).ModulateSelfOverride = new Color?(MenuButton.ColorPressed);
        break;
      case 2:
        ((Control) this._buttonIcon).ModulateSelfOverride = new Color?(this.HoveredColor);
        ((Control) this._buttonLabel).ModulateSelfOverride = new Color?(this.HoveredColor);
        break;
    }
  }

  protected virtual void DrawModeChanged()
  {
    base.DrawModeChanged();
    this.UpdateChildColors();
  }
}
