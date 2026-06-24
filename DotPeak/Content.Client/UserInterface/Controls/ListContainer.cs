// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.ListContainer
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Input;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Controls;

[Virtual]
public class ListContainer : Control
{
  public const string StylePropertySeparation = "separation";
  public const string StyleClassListContainerButton = "list-container-button";
  public Action<ListData, ListContainerButton>? GenerateItem;
  public Action<BaseButton.ButtonEventArgs, ListData>? ItemPressed;
  public Action<GUIBoundKeyEventArgs, ListData>? ItemKeyBindDown;
  public Action? NoItemSelected;
  private const int DefaultSeparation = 3;
  private readonly VScrollBar _vScrollBar;
  private readonly Dictionary<ListData, ListContainerButton> _buttons = new Dictionary<ListData, ListContainerButton>();
  private List<ListData> _data = new List<ListData>();
  private ListData? _selected;
  private float _itemHeight;
  private float _totalHeight;
  private int _topIndex;
  private int _bottomIndex;
  private bool _updateChildren;
  private bool _suppressScrollValueChanged;
  private ButtonGroup? _buttonGroup;

  public int? SeparationOverride { get; set; }

  public bool Group
  {
    get => this._buttonGroup != null;
    set => this._buttonGroup = value ? new ButtonGroup(true) : (ButtonGroup) null;
  }

  public bool Toggle { get; set; }

  public IReadOnlyList<ListData> Data => (IReadOnlyList<ListData>) this._data;

  public int ScrollSpeedY { get; set; } = 50;

  private int ActualSeparation
  {
    get
    {
      int num;
      return this.TryGetStyleProperty<int>("separation", ref num) ? num : this.SeparationOverride ?? 3;
    }
  }

  public ListContainer()
  {
    this.HorizontalExpand = true;
    this.VerticalExpand = true;
    this.RectClipContent = true;
    this.MouseFilter = (Control.MouseFilterMode) 1;
    VScrollBar vscrollBar = new VScrollBar();
    ((Control) vscrollBar).HorizontalExpand = false;
    ((Control) vscrollBar).HorizontalAlignment = (Control.HAlignment) 3;
    this._vScrollBar = vscrollBar;
    this.AddChild((Control) this._vScrollBar);
    ((Range) this._vScrollBar).OnValueChanged += new Action<Range>(this.ScrollValueChanged);
  }

  public virtual void PopulateList(IReadOnlyList<ListData> data)
  {
    if ((double) this._itemHeight != 0.0)
    {
      List<ListData> data1 = this._data;
      if (data1 == null || data1.Count != 0)
        goto label_6;
    }
    if (data.Count > 0)
    {
      ListContainerButton listContainerButton = new ListContainerButton(data[0], 0);
      Action<ListData, ListContainerButton> generateItem = this.GenerateItem;
      if (generateItem != null)
        generateItem(data[0], listContainerButton);
      this.AddChild((Control) listContainerButton);
      ((Control) listContainerButton).Measure(Vector2Helpers.Infinity);
      this._itemHeight = ((Control) listContainerButton).DesiredSize.Y;
      ((Control) listContainerButton).Orphan();
    }
label_6:
    foreach (Control control in this._buttons.Values)
      control.Orphan();
    this._buttons.Clear();
    this._data = data.ToList<ListData>();
    this._updateChildren = true;
    this.InvalidateArrange();
    if (!(this._selected != (ListData) null) || data.Contains<ListData>(this._selected))
      return;
    this._selected = (ListData) null;
    Action noItemSelected = this.NoItemSelected;
    if (noItemSelected == null)
      return;
    noItemSelected();
  }

  public void DirtyList()
  {
    this._updateChildren = true;
    this.InvalidateArrange();
  }

  public void Select(ListData data)
  {
    if (!this._data.Contains(data))
      return;
    ListContainerButton listContainerButton;
    if (this._buttons.TryGetValue(data, out listContainerButton) && this.Toggle)
      ((BaseButton) listContainerButton).Pressed = true;
    this._selected = data;
    if (listContainerButton == null)
      listContainerButton = new ListContainerButton(data, this._data.IndexOf(data));
    this.OnItemPressed(new BaseButton.ButtonEventArgs((BaseButton) listContainerButton, new GUIBoundKeyEventArgs(EngineKeyFunctions.UIClick, (BoundKeyState) 0, new ScreenCoordinates(0.0f, 0.0f, WindowId.Main), true, Vector2.Zero, Vector2.Zero)));
  }

  private void OnItemPressed(BaseButton.ButtonEventArgs args)
  {
    if (!(args.Button is ListContainerButton button))
      return;
    this._selected = button.Data;
    Action<BaseButton.ButtonEventArgs, ListData> itemPressed = this.ItemPressed;
    if (itemPressed == null)
      return;
    itemPressed(args, button.Data);
  }

  private void OnItemKeyBindDown(ListContainerButton button, GUIBoundKeyEventArgs args)
  {
    Action<GUIBoundKeyEventArgs, ListData> itemKeyBindDown = this.ItemKeyBindDown;
    if (itemKeyBindDown == null)
      return;
    itemKeyBindDown(args, button.Data);
  }

  private Vector2 GetScrollValue()
  {
    float y = ((Range) this._vScrollBar).Value;
    if (!((Control) this._vScrollBar).Visible)
      y = 0.0f;
    return new Vector2(0.0f, y);
  }

  protected virtual Vector2 ArrangeOverride(Vector2 finalSize)
  {
    float totalHeight = this._totalHeight;
    float x = ((Control) this._vScrollBar).DesiredSize.X;
    float num1;
    float num2;
    Vector2Helpers.Deconstruct(finalSize, ref num1, ref num2);
    float num3 = num1;
    float num4 = num2;
    try
    {
      this._suppressScrollValueChanged = true;
      if ((double) num4 < (double) totalHeight)
        num3 -= x;
      if ((double) num4 < (double) totalHeight)
      {
        ((Control) this._vScrollBar).Visible = true;
        ((Range) this._vScrollBar).Page = num4;
        ((Range) this._vScrollBar).MaxValue = totalHeight;
      }
      else
        ((Control) this._vScrollBar).Visible = false;
    }
    finally
    {
      this._suppressScrollValueChanged = false;
    }
    if (((Control) this._vScrollBar).Visible)
      ((Control) this._vScrollBar).Arrange(UIBox2.FromDimensions(Vector2.Zero, finalSize));
    Vector2 scrollValue = this.GetScrollValue();
    int topIndex1 = this._topIndex;
    this._topIndex = (int) (((double) scrollValue.Y + (double) this.ActualSeparation) / ((double) this._itemHeight + (double) this.ActualSeparation));
    if (this._topIndex != topIndex1)
      this._updateChildren = true;
    int bottomIndex = this._bottomIndex;
    this._bottomIndex = (int) Math.Ceiling(((double) scrollValue.Y + (double) num4) / ((double) this._itemHeight + (double) this.ActualSeparation));
    this._bottomIndex = Math.Min(this._bottomIndex, this._data.Count);
    if (this._bottomIndex != bottomIndex)
      this._updateChildren = true;
    if (this._updateChildren)
    {
      this._updateChildren = false;
      Dictionary<ListData, ListContainerButton> dictionary = new Dictionary<ListData, ListContainerButton>((IDictionary<ListData, ListContainerButton>) this._buttons);
      if (this._data.Count > 0)
      {
        for (int topIndex2 = this._topIndex; topIndex2 < this._bottomIndex; ++topIndex2)
        {
          ListData listData = this._data[topIndex2];
          ListContainerButton button;
          if (this._buttons.TryGetValue(listData, out button))
          {
            dictionary.Remove(listData);
          }
          else
          {
            button = new ListContainerButton(listData, topIndex2);
            ((BaseButton) button).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnItemPressed);
            ((Control) button).OnKeyBindDown += (Action<GUIBoundKeyEventArgs>) (args => this.OnItemKeyBindDown(button, args));
            ((BaseButton) button).ToggleMode = this.Toggle;
            ((BaseButton) button).Group = this._buttonGroup;
            Action<ListData, ListContainerButton> generateItem = this.GenerateItem;
            if (generateItem != null)
              generateItem(listData, button);
            this._buttons.Add(listData, button);
            if (this.Toggle && listData == this._selected)
              ((BaseButton) button).Pressed = true;
            this.AddChild((Control) button);
          }
          ((Control) button).SetPositionInParent(topIndex2 - this._topIndex);
          ((Control) button).Measure(finalSize);
        }
      }
      foreach ((ListData key, ListContainerButton listContainerButton) in dictionary)
      {
        this._buttons.Remove(key);
        ((Control) listContainerButton).Orphan();
      }
      ((Control) this._vScrollBar).SetPositionLast();
    }
    int num5 = (int) ((double) num3 * (double) this.UIScale);
    int num6 = (int) ((double) this.ActualSeparation * (double) this.UIScale);
    int num7 = (int) -(((double) scrollValue.Y - (double) this._topIndex * ((double) this._itemHeight + (double) this.ActualSeparation)) * (double) this.UIScale);
    bool flag = true;
    foreach (Control child in this.Children)
    {
      if (child != this._vScrollBar)
      {
        if (!flag)
          num7 += num6;
        flag = false;
        int y = child.DesiredPixelSize.Y;
        UIBox2i uiBox2i;
        // ISSUE: explicit constructor call
        ((UIBox2i) ref uiBox2i).\u002Ector(0, num7, num5, num7 + y);
        child.ArrangePixel(uiBox2i);
        num7 += y;
      }
    }
    return finalSize;
  }

  protected virtual Vector2 MeasureOverride(Vector2 availableSize)
  {
    ((Control) this._vScrollBar).Measure(availableSize);
    availableSize.X -= ((Control) this._vScrollBar).DesiredSize.X;
    Vector2 vector2_1 = new Vector2(availableSize.X, float.PositiveInfinity);
    Vector2 vector2_2 = Vector2.Zero;
    foreach (Control child in this.Children)
    {
      child.Measure(vector2_1);
      if (child != this._vScrollBar)
        vector2_2 = Vector2.Max(vector2_2, child.DesiredSize);
    }
    if ((double) this._itemHeight == 0.0 && (double) vector2_2.Y != 0.0)
      this._itemHeight = vector2_2.Y;
    this._totalHeight = this._itemHeight * (float) this._data.Count + (float) (this.ActualSeparation * (this._data.Count - 1));
    return new Vector2(vector2_2.X, 0.0f);
  }

  private void ScrollValueChanged(Range _)
  {
    if (this._suppressScrollValueChanged)
      return;
    this.InvalidateArrange();
  }

  protected virtual void MouseWheel(GUIMouseWheelEventArgs args)
  {
    base.MouseWheel(args);
    VScrollBar vScrollBar = this._vScrollBar;
    ((ScrollBar) vScrollBar).ValueTarget = ((ScrollBar) vScrollBar).ValueTarget - args.Delta.Y * (float) this.ScrollSpeedY;
    ((InputEventArgs) args).Handle();
  }
}
