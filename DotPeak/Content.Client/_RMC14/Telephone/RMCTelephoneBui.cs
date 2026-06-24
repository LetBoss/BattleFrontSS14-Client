// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Telephone.RMCTelephoneBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.ControlExtensions;
using Content.Shared._RMC14.Telephone;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Telephone;

public sealed class RMCTelephoneBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private static readonly List<string> TabOrder = new List<string>()
  {
    "MP Dept.",
    "Almayer",
    "Command",
    "Offices",
    "ARES",
    "Dropship",
    "Marine"
  };
  private TelephoneWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<TelephoneWindow>((BoundUserInterface) this);
    MetaDataComponent metaDataComponent;
    if (this.EntMan.TryGetComponent<MetaDataComponent>(this.Owner, ref metaDataComponent))
      this._window.Title = metaDataComponent.EntityName;
    this.Refresh();
  }

  protected virtual void UpdateState(BoundUserInterfaceState state) => this.Refresh();

  private void Refresh()
  {
    TelephoneWindow window = this._window;
    if (window == null || !((BaseWindow) window).IsOpen || !(this.State is RMCTelephoneBuiState state))
      return;
    ((Control) this._window.Tabs).DisposeAllChildren();
    Dictionary<string, BoxContainer> dictionary = new Dictionary<string, BoxContainer>();
    foreach (RMCPhone phone1 in state.Phones)
    {
      RMCPhone phone = phone1;
      BoxContainer boxContainer1;
      if (!dictionary.TryGetValue(phone.Category, out boxContainer1))
      {
        boxContainer1 = new BoxContainer()
        {
          Orientation = (BoxContainer.LayoutOrientation) 1
        };
        dictionary[phone.Category] = boxContainer1;
        ScrollContainer scrollContainer1 = new ScrollContainer();
        scrollContainer1.HScrollEnabled = false;
        scrollContainer1.VScrollEnabled = true;
        ((Control) scrollContainer1).VerticalExpand = true;
        ScrollContainer scrollContainer2 = scrollContainer1;
        BoxContainer boxContainer2 = new BoxContainer()
        {
          Orientation = (BoxContainer.LayoutOrientation) 1
        };
        ((Control) scrollContainer2).AddChild((Control) boxContainer2);
        LineEdit lineEdit1 = new LineEdit();
        ((Control) boxContainer1).AddChild((Control) lineEdit1);
        ((Control) boxContainer1).AddChild((Control) scrollContainer2);
        lineEdit1.OnTextChanged += (Action<LineEdit.LineEditEventArgs>) (args =>
        {
          foreach (Control parent in ((Control) this._window.Tabs).GetControlOfType<ScrollContainer>())
          {
            foreach (Control control in parent.GetControlOfType<BoxContainer>())
            {
              foreach (Control child in control.Children)
              {
                if (child is LineEdit lineEdit3)
                  lineEdit3.SetText(args.Text, false);
                else if (child is Button button3)
                {
                  Button button2 = button3;
                  string text = button3.Text;
                  int num = text != null ? (text.Contains(args.Text, StringComparison.OrdinalIgnoreCase) ? 1 : 0) : 0;
                  ((Control) button2).Visible = num != 0;
                }
              }
            }
          }
        });
      }
      foreach (Control child1 in ((Control) boxContainer1).Children)
      {
        if (child1 is ScrollContainer scrollContainer)
        {
          foreach (Control child2 in ((Control) scrollContainer).Children)
          {
            if (child2 is BoxContainer boxContainer3)
            {
              Button button4 = new Button();
              button4.Text = phone.Name;
              ((Control) button4).StyleClasses.Add("OpenBoth");
              Button button5 = button4;
              ((BaseButton) button5).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCTelephoneCallBuiMsg(phone.Id)));
              ((Control) boxContainer3).AddChild((Control) button5);
              break;
            }
          }
        }
      }
    }
    foreach (string key in RMCTelephoneBui.TabOrder)
    {
      BoxContainer boxContainer;
      if (dictionary.Remove(key, out boxContainer))
      {
        ((Control) this._window.Tabs).AddChild((Control) boxContainer);
        TabContainer.SetTabTitle((Control) boxContainer, key);
      }
    }
    foreach ((string key, BoxContainer boxContainer) in dictionary)
    {
      ((Control) this._window.Tabs).AddChild((Control) boxContainer);
      TabContainer.SetTabTitle((Control) boxContainer, key);
    }
    ((Control) this._window.Buttons).DisposeAllChildren();
    if (state.Dnd)
    {
      Button button6 = new Button();
      button6.Text = Loc.GetString("phone-dnd-button");
      ((Control) button6).StyleClasses.Add("OpenBoth");
      ((Control) button6).StyleClasses.Add("Caution");
      ((Control) button6).ToolTip = Loc.GetString("phone-dnd-tooltip-enabled");
      Button button7 = button6;
      ((BaseButton) button7).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCTelephoneDndBuiMsg(false)));
      ((Control) this._window.Buttons).AddChild((Control) button7);
    }
    else if (state.CanDnd)
    {
      Button button8 = new Button();
      button8.Text = Loc.GetString("phone-dnd-button");
      ((Control) button8).StyleClasses.Add("OpenBoth");
      ((Control) button8).ToolTip = Loc.GetString("phone-dnd-tooltip-disabled");
      Button button9 = button8;
      ((BaseButton) button9).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCTelephoneDndBuiMsg(true)));
      ((Control) this._window.Buttons).AddChild((Control) button9);
    }
    else
    {
      Button button = new Button();
      button.Text = Loc.GetString("phone-dnd-button");
      ((Control) button).StyleClasses.Add("OpenBoth");
      ((Control) button).ToolTip = Loc.GetString("phone-dnd-tooltip-locked");
      ((BaseButton) button).Disabled = true;
      ((Control) this._window.Buttons).AddChild((Control) button);
    }
  }
}
