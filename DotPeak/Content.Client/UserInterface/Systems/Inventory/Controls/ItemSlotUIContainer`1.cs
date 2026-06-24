// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Inventory.Controls.ItemSlotUIContainer`1
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Client.UserInterface.Systems.Inventory.Controls;

public abstract class ItemSlotUIContainer<T> : GridContainer, IItemslotUIContainer where T : SlotControl
{
  private static readonly ISawmill Sawmill = Logger.GetSawmill("ui.item_slot_container");
  protected readonly Dictionary<string, T> Buttons = new Dictionary<string, T>();
  private int? _maxColumns;

  public int? MaxColumns
  {
    get => this._maxColumns;
    set => this._maxColumns = value;
  }

  public virtual bool TryAddButton(T newButton, out T button)
  {
    if ((object) this.AddButton(newButton) == null)
    {
      button = newButton;
      return false;
    }
    button = newButton;
    return true;
  }

  public void ClearButtons()
  {
    foreach (T obj in this.Buttons.Values)
      obj.Orphan();
    this.Buttons.Clear();
  }

  public bool TryRegisterButton(SlotControl control, string newSlotName)
  {
    if (newSlotName == "" || !(control is T newButton))
      return false;
    T obj;
    if (this.Buttons.TryGetValue(newSlotName, out obj))
    {
      if (control == (object) obj)
        return true;
      throw new Exception($"Could not update button to slot:{newSlotName} slot already assigned!");
    }
    this.Buttons.Remove(newButton.SlotName);
    this.AddButton(newButton);
    return true;
  }

  public bool TryAddButton(SlotControl control)
  {
    return control is T newButton && (object) this.AddButton(newButton) != null;
  }

  public virtual T? AddButton(T newButton)
  {
    if (!((Control) this).Children.Contains((Control) newButton) && newButton.Parent == null && newButton.SlotName != "")
      ((Control) this).AddChild((Control) newButton);
    this.Columns = this._maxColumns ?? ((Control) this).ChildCount;
    return this.AddButtonToDict(newButton);
  }

  protected virtual T? AddButtonToDict(T newButton)
  {
    if (newButton.SlotName == "")
      ItemSlotUIContainer<T>.Sawmill.Warning($"Could not add button {newButton.Name}No slotname");
    return this.Buttons.TryAdd(newButton.SlotName, newButton) ? newButton : default (T);
  }

  public virtual void RemoveButton(string slotName)
  {
    T button;
    if (!this.Buttons.TryGetValue(slotName, out button))
      return;
    this.RemoveButton(button);
  }

  public virtual void RemoveButtons(params string[] slotNames)
  {
    foreach (string slotName in slotNames)
      this.RemoveButton(slotName);
  }

  public virtual void RemoveButtons(params T?[] buttons)
  {
    foreach (T button in buttons)
    {
      if ((object) button != null)
        this.RemoveButton(button);
    }
  }

  protected virtual void RemoveButtonFromDict(T button) => this.Buttons.Remove(button.SlotName);

  public virtual void RemoveButton(T button)
  {
    this.RemoveButtonFromDict(button);
    ((Control) this).Children.Remove((Control) button);
    button.Orphan();
  }

  public virtual T? GetButton(string slotName)
  {
    T obj;
    return this.Buttons.TryGetValue(slotName, out obj) ? obj : default (T);
  }

  public virtual bool TryGetButton(string slotName, [NotNullWhen(true)] out T? button)
  {
    return (object) (button = this.GetButton(slotName)) != null;
  }
}
