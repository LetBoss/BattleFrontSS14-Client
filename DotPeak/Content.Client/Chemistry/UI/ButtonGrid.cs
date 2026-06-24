// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.UI.ButtonGrid
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using System;

#nullable enable
namespace Content.Client.Chemistry.UI;

public sealed class ButtonGrid : GridContainer
{
  private string _buttonList = "";
  private string? _selected;
  public Action<string>? OnButtonPressed;

  public string ButtonList
  {
    get => this._buttonList;
    set
    {
      this._buttonList = value;
      this.Update();
    }
  }

  public bool RadioGroup { get; set; }

  public string? Selected
  {
    get => this._selected;
    set
    {
      this._selected = value;
      this.Update();
    }
  }

  public int Columns
  {
    get => base.Columns;
    set
    {
      base.Columns = value;
      this.Update();
    }
  }

  public int Rows
  {
    get => base.Rows;
    set
    {
      base.Rows = value;
      this.Update();
    }
  }

  private void Update()
  {
    if (this.ButtonList == "")
      return;
    ((Control) this).Children.Clear();
    int num1 = 0;
    string[] strArray = this.ButtonList.Split(",");
    ButtonGroup buttonGroup = new ButtonGroup(true);
    foreach (string str in strArray)
    {
      string button = str;
      Button btn = new Button();
      btn.Text = button;
      ((BaseButton) btn).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        if (this.RadioGroup)
          ((BaseButton) btn).Pressed = true;
        this.Selected = button;
        Action<string> onButtonPressed = this.OnButtonPressed;
        if (onButtonPressed == null)
          return;
        onButtonPressed(button);
      });
      if (button == this.Selected)
        ((BaseButton) btn).Pressed = true;
      this.HSeparationOverride.GetValueOrDefault();
      ((BaseButton) btn).Group = buttonGroup;
      int num2 = num1 / this.Columns;
      int num3 = num1 % this.Columns;
      bool flag1 = num1 == strArray.Length - 1;
      bool flag2 = num1 == this.Columns - 1;
      bool flag3 = num2 == strArray.Length / this.Columns - 1;
      if (num2 == 0 && flag2 | flag1)
        ((Control) btn).AddStyleClass("OpenLeft");
      else if (num3 == 0 & flag3)
        ((Control) btn).AddStyleClass("OpenRight");
      else
        ((Control) btn).AddStyleClass("OpenBoth");
      ((Control) this).Children.Add((Control) btn);
      ++num1;
    }
  }
}
