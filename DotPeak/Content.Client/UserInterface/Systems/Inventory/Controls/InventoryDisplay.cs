// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Inventory.Controls.InventoryDisplay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Systems.Inventory.Controls;

public sealed class InventoryDisplay : LayoutContainer
{
  private static readonly ISawmill Sawmill = Logger.GetSawmill("ui.inventory_display");
  private int Columns;
  private int Rows;
  private const int MarginThickness = 10;
  private const int ButtonSpacing = 5;
  private const int ButtonSize = 75;
  private readonly Control resizer;
  private readonly Dictionary<string, (SlotControl, Vector2i)> _buttons = new Dictionary<string, (SlotControl, Vector2i)>();

  public InventoryDisplay()
  {
    this.resizer = new Control();
    ((Control) this).AddChild(this.resizer);
  }

  public SlotControl AddButton(SlotControl newButton, Vector2i buttonOffset)
  {
    ((Control) this).AddChild((Control) newButton);
    ((Control) this).HorizontalExpand = true;
    ((Control) this).VerticalExpand = true;
    this.InheritChildMeasure = true;
    if (!this._buttons.TryAdd(newButton.SlotName, (newButton, buttonOffset)))
      InventoryDisplay.Sawmill.Warning("Tried to add button without a slot!");
    LayoutContainer.SetPosition((Control) newButton, Vector2i.op_Implicit(Vector2i.op_Multiply(buttonOffset, 75)) + new Vector2(5f, 5f));
    this.UpdateSizeData(buttonOffset);
    return newButton;
  }

  public SlotControl? GetButton(string slotName)
  {
    (SlotControl, Vector2i) tuple;
    return this._buttons.TryGetValue(slotName, out tuple) ? tuple.Item1 : (SlotControl) null;
  }

  private void UpdateSizeData(Vector2i buttonOffset)
  {
    Vector2i vector2i1 = buttonOffset;
    int num1;
    int num2;
    ((Vector2i) ref vector2i1).Deconstruct(ref num1, ref num2);
    int num3 = num1;
    if (num3 > this.Columns)
      this.Columns = num3;
    Vector2i vector2i2 = buttonOffset;
    ((Vector2i) ref vector2i2).Deconstruct(ref num2, ref num1);
    int num4 = num1;
    if (num4 > this.Rows)
      this.Rows = num4;
    this.resizer.SetHeight = (float) ((this.Rows + 1) * 80 /*0x50*/);
    this.resizer.SetWidth = (float) ((this.Columns + 1) * 80 /*0x50*/);
  }

  public bool TryGetButton(string slotName, out SlotControl? button)
  {
    (SlotControl, Vector2i) tuple;
    int num = this._buttons.TryGetValue(slotName, out tuple) ? 1 : 0;
    button = tuple.Item1;
    return num != 0;
  }

  public void RemoveButton(string slotName)
  {
    if (!this._buttons.Remove(slotName))
      return;
    this.Columns = 0;
    this.Rows = 0;
    foreach ((string _, (SlotControl, Vector2i) tuple) in this._buttons)
      this.UpdateSizeData(tuple.Item2);
  }

  public void ClearButtons() => ((Control) this).Children.Clear();
}
