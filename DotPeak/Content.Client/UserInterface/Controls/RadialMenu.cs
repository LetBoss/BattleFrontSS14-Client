// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.RadialMenu
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Analyzers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Controls;

[Virtual]
public class RadialMenu : BaseWindow
{
  private readonly List<Control> _path = new List<Control>();
  private string? _backButtonStyleClass;
  private string? _closeButtonStyleClass;

  public RadialMenuContextualCentralTextureButton ContextualButton { get; }

  public RadialMenuOuterAreaButton MenuOuterAreaButton { get; }

  public string? BackButtonStyleClass
  {
    get => this._backButtonStyleClass;
    set
    {
      this._backButtonStyleClass = value;
      if (this._path.Count <= 0 || this.ContextualButton == null || this._backButtonStyleClass == null)
        return;
      ((Control) this.ContextualButton).SetOnlyStyleClass(this._backButtonStyleClass);
    }
  }

  public string? CloseButtonStyleClass
  {
    get => this._closeButtonStyleClass;
    set
    {
      this._closeButtonStyleClass = value;
      if (this._path.Count != 0 || this.ContextualButton == null || this._closeButtonStyleClass == null)
        return;
      ((Control) this.ContextualButton).SetOnlyStyleClass(this._closeButtonStyleClass);
    }
  }

  public RadialMenu()
  {
    if (((Control) this).ChildCount > 1)
    {
      for (int index = 1; index < ((Control) this).ChildCount; ++index)
        ((Control) this).GetChild(index).Visible = false;
    }
    RadialMenuContextualCentralTextureButton centralTextureButton = new RadialMenuContextualCentralTextureButton();
    ((Control) centralTextureButton).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) centralTextureButton).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) centralTextureButton).SetSize = new Vector2(64f, 64f);
    this.ContextualButton = centralTextureButton;
    this.MenuOuterAreaButton = new RadialMenuOuterAreaButton();
    ((BaseButton) this.ContextualButton).OnButtonUp += (Action<BaseButton.ButtonEventArgs>) (_ => this.ReturnToPreviousLayer());
    ((BaseButton) this.MenuOuterAreaButton).OnButtonUp += (Action<BaseButton.ButtonEventArgs>) (_ => this.Close());
    ((Control) this).AddChild((Control) this.ContextualButton);
    ((Control) this).AddChild((Control) this.MenuOuterAreaButton);
    ((Control) this).OnChildAdded += (Action<Control>) (child =>
    {
      child.Visible = this.GetCurrentActiveLayer() == child;
      this.SetupContextualButtonData(child);
    });
  }

  private void SetupContextualButtonData(Control child)
  {
    if (!(child is RadialContainer radialContainer) || !child.Visible)
      return;
    Vector2 vector2 = ((Control) this).MinSize * 0.5f;
    this.ContextualButton.ParentCenter = new Vector2?(vector2);
    this.MenuOuterAreaButton.ParentCenter = new Vector2?(vector2);
    this.ContextualButton.InnerRadius = radialContainer.CalculatedRadius * radialContainer.InnerRadiusMultiplier;
    this.MenuOuterAreaButton.OuterRadius = radialContainer.CalculatedRadius * radialContainer.OuterRadiusMultiplier;
  }

  protected virtual Vector2 ArrangeOverride(Vector2 finalSize)
  {
    Vector2 vector2 = ((Control) this).ArrangeOverride(finalSize);
    Control currentActiveLayer = this.GetCurrentActiveLayer();
    if (currentActiveLayer == null)
      return vector2;
    this.SetupContextualButtonData(currentActiveLayer);
    return vector2;
  }

  private Control? GetCurrentActiveLayer()
  {
    IEnumerable<Control> source = ((IEnumerable<Control>) ((Control) this).Children).Where<Control>((Func<Control, bool>) (x => x != this.ContextualButton && x != this.MenuOuterAreaButton));
    return !source.Any<Control>() ? (Control) null : source.First<Control>((Func<Control, bool>) (x => x.Visible));
  }

  public bool TryToMoveToNewLayer(Control newLayer)
  {
    Control currentActiveLayer = this.GetCurrentActiveLayer();
    if (currentActiveLayer == null)
      return false;
    bool moveToNewLayer = false;
    foreach (Control child in ((Control) this).Children)
    {
      if (child != this.ContextualButton && child != this.MenuOuterAreaButton)
      {
        if (moveToNewLayer || child != newLayer)
        {
          child.Visible = false;
        }
        else
        {
          child.Visible = true;
          this.SetupContextualButtonData(child);
          moveToNewLayer = true;
        }
      }
    }
    if (moveToNewLayer)
      this._path.Add(currentActiveLayer);
    if (this._path.Count > 0 && this.ContextualButton != null && this.BackButtonStyleClass != null)
      ((Control) this.ContextualButton).SetOnlyStyleClass(this.BackButtonStyleClass);
    return moveToNewLayer;
  }

  public bool TryToMoveToNewLayer(string targetLayerControlName)
  {
    foreach (Control child in ((Control) this).Children)
    {
      if (child.Name == targetLayerControlName && child is RadialContainer)
        return this.TryToMoveToNewLayer(child);
    }
    return false;
  }

  public void ReturnToPreviousLayer()
  {
    if (this._path.Count == 0)
    {
      this.Close();
    }
    else
    {
      List<Control> path = this._path;
      Control control = path[path.Count - 1];
      foreach (Control child in ((Control) this).Children)
      {
        if (child != this.ContextualButton && child != this.MenuOuterAreaButton)
          child.Visible = false;
      }
      control.Visible = true;
      this._path.RemoveAt(this._path.Count - 1);
      if (this._path.Count != 0 || this.ContextualButton == null || this.CloseButtonStyleClass == null)
        return;
      ((Control) this.ContextualButton).SetOnlyStyleClass(this.CloseButtonStyleClass);
    }
  }
}
