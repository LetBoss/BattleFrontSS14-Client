// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Hands.Controls.HandsContainer
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Systems.Inventory.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.UserInterface.Systems.Hands.Controls;

public sealed class HandsContainer : ItemSlotUIContainer<HandButton>
{
  private readonly GridContainer _grid;

  public int ColumnLimit
  {
    get => this._grid.Columns;
    set => this._grid.Columns = value;
  }

  public int MaxButtonCount { get; set; }

  public int MaxButtonsPerRow { get; set; } = 6;

  public string? Indexer { get; set; }

  public HandsContainer()
  {
    ((Control) this).AddChild((Control) (this._grid = new GridContainer()));
    this._grid.ExpandBackwards = true;
  }

  public override HandButton? AddButton(HandButton newButton)
  {
    if (this.MaxButtonCount > 0)
    {
      if (this.ButtonCount >= this.MaxButtonCount)
        return (HandButton) null;
      ((Control) this._grid).AddChild((Control) newButton);
    }
    else
      ((Control) this._grid).AddChild((Control) newButton);
    this._grid.Columns = Math.Min(((Control) this._grid).ChildCount, this.MaxButtonsPerRow);
    return base.AddButton(newButton);
  }

  public override void RemoveButton(string handName)
  {
    HandButton button = this.GetButton(handName);
    if (button == null)
      return;
    this.RemoveButton(button);
    ((Control) this._grid).RemoveChild((Control) button);
  }

  public bool TryGetLastButton(out HandButton? control)
  {
    if (this.Buttons.Count == 0)
    {
      control = (HandButton) null;
      return false;
    }
    control = this.Buttons.Values.Last<HandButton>();
    return true;
  }

  public bool TryRemoveLastHand(out HandButton? control)
  {
    int num = this.TryGetLastButton(out control) ? 1 : 0;
    if (control == null)
      return num != 0;
    this.RemoveButton(control);
    return num != 0;
  }

  public void Clear()
  {
    this.ClearButtons();
    ((Control) this._grid).DisposeAllChildren();
  }

  public IEnumerable<HandButton> GetButtons()
  {
    foreach (Control child in ((Control) this._grid).Children)
    {
      if (child is HandButton button)
        yield return button;
    }
  }

  public bool IsFull => this.MaxButtonCount != 0 && this.ButtonCount >= this.MaxButtonCount;

  public int ButtonCount => ((Control) this._grid).ChildCount;
}
