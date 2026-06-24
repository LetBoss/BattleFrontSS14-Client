// Decompiled with JetBrains decompiler
// Type: Content.Client.DeadSpace.UserInterface.Controls.HeadedOptionButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.DeadSpace.UserInterface.Controls;

[Virtual]
public class HeadedOptionButton : ContainerButton
{
  public const string StyleClassOptionButton = "optionButton";
  public const string StyleClassPopup = "optionButtonPopup";
  public const string StyleClassOptionTriangle = "optionTriangle";
  public const string StyleClassOptionsBackground = "optionButtonBackground";
  public readonly ScrollContainer OptionsScroll;
  public readonly List<HeadedOptionButton.ButtonData> _buttonData = new List<HeadedOptionButton.ButtonData>();
  private readonly Dictionary<int, int> _idMap = new Dictionary<int, int>();
  private readonly Popup _popup;
  private readonly BoxContainer _popupVBox;
  private readonly Label _label;
  private readonly TextureRect _triangle;
  public BoxContainer ScrollHeading;
  private bool _hideTriangle;

  public int ItemCount => this._buttonData.Count;

  public bool HideTriangle
  {
    get => this._hideTriangle;
    set
    {
      this._hideTriangle = value;
      ((Control) this._triangle).Visible = !this._hideTriangle;
    }
  }

  public ICollection<string> OptionStyleClasses { get; }

  public event Action<HeadedOptionButton.ItemSelectedEventArgs>? OnItemSelected;

  public string Prefix { get; set; } = string.Empty;

  public bool PrefixMargin { get; set; } = true;

  public HeadedOptionButton()
  {
    this.OptionStyleClasses = (ICollection<string>) new List<string>();
    ((Control) this).AddStyleClass("button");
    ((BaseButton) this).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnPressedInternal);
    BoxContainer boxContainer1 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0
    };
    ((Control) this).AddChild((Control) boxContainer1);
    this._popupVBox = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1
    };
    ScrollContainer scrollContainer = new ScrollContainer();
    ((Control) scrollContainer).Children.Add((Control) this._popupVBox);
    scrollContainer.ReturnMeasure = true;
    ((Control) scrollContainer).MaxHeight = 300f;
    this.OptionsScroll = scrollContainer;
    this.ScrollHeading = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1
    };
    BoxContainer boxContainer2 = new BoxContainer();
    boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer2).Children.Add((Control) this.ScrollHeading);
    ((Control) boxContainer2).Children.Add((Control) this.OptionsScroll);
    BoxContainer boxContainer3 = boxContainer2;
    Popup popup = new Popup();
    Control.OrderedChildCollection children = ((Control) popup).Children;
    PanelContainer panelContainer = new PanelContainer();
    ((Control) panelContainer).StyleClasses.Add("optionButtonBackground");
    children.Add((Control) panelContainer);
    ((Control) popup).Children.Add((Control) boxContainer3);
    ((Control) popup).StyleClasses.Add("optionButtonPopup");
    this._popup = popup;
    this._popup.OnPopupHide += new Action(this.OnPopupHide);
    Label label = new Label();
    ((Control) label).StyleClasses.Add("optionButton");
    ((Control) label).HorizontalExpand = true;
    this._label = label;
    ((Control) boxContainer1).AddChild((Control) this._label);
    TextureRect textureRect = new TextureRect();
    ((Control) textureRect).StyleClasses.Add("optionTriangle");
    ((Control) textureRect).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) textureRect).Visible = !this.HideTriangle;
    this._triangle = textureRect;
    ((Control) boxContainer1).AddChild((Control) this._triangle);
  }

  public void AddItem(Texture icon, string label, int? id = null) => this.AddItem(label, id);

  public virtual void ButtonOverride(Button button)
  {
  }

  public void AddItem(string label, int? id = null)
  {
    if (!id.HasValue)
      id = new int?(this._buttonData.Count);
    if (this._idMap.ContainsKey(id.Value))
      throw new ArgumentException("An item with the same ID already exists.");
    Button button1 = new Button();
    button1.Text = label;
    ((BaseButton) button1).ToggleMode = true;
    Button button2 = button1;
    foreach (string optionStyleClass in (IEnumerable<string>) this.OptionStyleClasses)
      ((Control) button2).AddStyleClass(optionStyleClass);
    ((BaseButton) button2).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.ButtonOnPressed);
    HeadedOptionButton.ButtonData buttonData = new HeadedOptionButton.ButtonData(label, button2)
    {
      Id = id.Value
    };
    this._idMap.Add(id.Value, this._buttonData.Count);
    this._buttonData.Add(buttonData);
    ((Control) this._popupVBox).AddChild((Control) button2);
    if (this._buttonData.Count == 1)
      this.Select(0);
    this.ButtonOverride(button2);
  }

  private void TogglePopup(bool show)
  {
    if (show)
    {
      Vector2 globalPosition = ((Control) this).GlobalPosition;
      globalPosition.Y += ((Control) this).Size.Y + 1f;
      ref float local = ref globalPosition.Y;
      double num1 = (double) local;
      Thickness margin = ((Control) this).Margin;
      double sumVertical = (double) ((Thickness) ref margin).SumVertical;
      local = (float) (num1 - sumVertical);
      ScrollContainer optionsScroll = this.OptionsScroll;
      Vector2i? size = ((Control) this).Window?.Size;
      Vector2 vector2 = size.HasValue ? Vector2i.op_Implicit(size.GetValueOrDefault()) : Vector2Helpers.Infinity;
      ((Control) optionsScroll).Measure(vector2);
      float num2;
      float num3;
      Vector2Helpers.Deconstruct(((Control) this.OptionsScroll).DesiredSize, ref num2, ref num3);
      float val1 = num2;
      float y = num3;
      UIBox2 uiBox2 = UIBox2.FromDimensions(globalPosition, new Vector2(Math.Max(val1, ((Control) this).Width), y));
      ((Control) ((Control) this).UserInterfaceManager.ModalRoot).AddChild((Control) this._popup);
      this._popup.Open(new UIBox2?(uiBox2), new Vector2?(), new Vector2?());
    }
    else
      this._popup.Close();
  }

  private void OnPopupHide()
  {
    ((Control) ((Control) this).UserInterfaceManager.ModalRoot).RemoveChild((Control) this._popup);
  }

  private void ButtonOnPressed(BaseButton.ButtonEventArgs obj)
  {
    obj.Button.Pressed = false;
    this.TogglePopup(false);
    foreach (HeadedOptionButton.ButtonData buttonData in this._buttonData)
    {
      if (buttonData.Button == obj.Button)
      {
        Action<HeadedOptionButton.ItemSelectedEventArgs> onItemSelected = this.OnItemSelected;
        if (onItemSelected == null)
          return;
        onItemSelected(new HeadedOptionButton.ItemSelectedEventArgs(buttonData.Id, this));
        return;
      }
    }
    throw new InvalidOperationException();
  }

  public void Clear()
  {
    this._idMap.Clear();
    foreach (HeadedOptionButton.ButtonData buttonData in this._buttonData)
      ((BaseButton) buttonData.Button).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.ButtonOnPressed);
    this._buttonData.Clear();
    ((Control) this._popupVBox).DisposeAllChildren();
    this.SelectedId = 0;
  }

  public int GetItemId(int idx) => this._buttonData[idx].Id;

  public object? GetItemMetadata(int idx) => this._buttonData[idx].Metadata;

  public int SelectedId { get; private set; }

  public object? SelectedMetadata => this._buttonData[this._idMap[this.SelectedId]].Metadata;

  public bool IsItemDisabled(int idx) => this._buttonData[idx].Disabled;

  public void RemoveItem(int idx)
  {
    HeadedOptionButton.ButtonData buttonData1 = this._buttonData[idx];
    ((BaseButton) buttonData1.Button).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.ButtonOnPressed);
    this._idMap.Remove(buttonData1.Id);
    ((Control) this._popupVBox).RemoveChild((Control) buttonData1.Button);
    this._buttonData.RemoveAt(idx);
    int num = 0;
    foreach (HeadedOptionButton.ButtonData buttonData2 in this._buttonData)
      this._idMap[buttonData2.Id] = num++;
  }

  public void Select(int idx)
  {
    int index;
    if (this._idMap.TryGetValue(this.SelectedId, out index))
      ((BaseButton) this._buttonData[index].Button).Pressed = false;
    HeadedOptionButton.ButtonData buttonData = this._buttonData[idx];
    this.SelectedId = buttonData.Id;
    this._label.Text = this.PrefixMargin ? $"{this.Prefix} {buttonData.Text}" : this.Prefix + buttonData.Text;
    ((BaseButton) buttonData.Button).Pressed = true;
  }

  public bool TrySelect(int idx)
  {
    if (idx < 0 || idx >= this._buttonData.Count)
      return false;
    this.Select(idx);
    return true;
  }

  public void SelectId(int id) => this.Select(this.GetIdx(id));

  public bool TrySelectId(int id)
  {
    int idx;
    return this._idMap.TryGetValue(id, out idx) && this.TrySelect(idx);
  }

  public int GetIdx(int id) => this._idMap[id];

  public void SetItemDisabled(int idx, bool disabled)
  {
    HeadedOptionButton.ButtonData buttonData = this._buttonData[idx];
    buttonData.Disabled = disabled;
    ((BaseButton) buttonData.Button).Disabled = disabled;
  }

  public void SetItemId(int idx, int id)
  {
    int num;
    if (this._idMap.TryGetValue(id, out num) && num != idx)
      throw new InvalidOperationException("An item with said ID already exists.");
    HeadedOptionButton.ButtonData buttonData = this._buttonData[idx];
    this._idMap.Remove(buttonData.Id);
    this._idMap.Add(id, idx);
    buttonData.Id = id;
  }

  public void SetItemMetadata(int idx, object metadata)
  {
    this._buttonData[idx].Metadata = metadata;
  }

  public void SetItemText(int idx, string text)
  {
    HeadedOptionButton.ButtonData buttonData = this._buttonData[idx];
    buttonData.Text = text;
    if (this.SelectedId == buttonData.Id)
      this._label.Text = text;
    buttonData.Button.Text = text;
  }

  private void OnPressedInternal(BaseButton.ButtonEventArgs args) => this.TogglePopup(true);

  protected virtual void ExitedTree()
  {
    ((Control) this).ExitedTree();
    this.TogglePopup(false);
  }

  public sealed class ItemSelectedEventArgs : EventArgs
  {
    public HeadedOptionButton Button { get; }

    public int Id { get; }

    public ItemSelectedEventArgs(int id, HeadedOptionButton button)
    {
      this.Id = id;
      this.Button = button;
    }
  }

  public sealed class ButtonData
  {
    public string Text;
    public bool Disabled;
    public object? Metadata;
    public int Id;
    public Button Button;

    public ButtonData(string text, Button button)
    {
      this.Text = text;
      this.Button = button;
    }
  }
}
