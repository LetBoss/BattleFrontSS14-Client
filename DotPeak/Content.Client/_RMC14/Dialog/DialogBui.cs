// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Dialog.DialogBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Dialog;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client._RMC14.Dialog;

public sealed class DialogBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private RMCDialogWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<RMCDialogWindow>((BoundUserInterface) this);
    this.Refresh();
  }

  private void UpdateOptions(DialogComponent s)
  {
    RMCDialogWindow window = this._window;
    if (window == null || !((BaseWindow) window).IsOpen)
      return;
    RMCDialogOptionsContainer container = this._window.Container as RMCDialogOptionsContainer;
    if (container == null)
    {
      ((Control) this._window.Container)?.Orphan();
      this._window.Container = (BoxContainer) null;
      container = new RMCDialogOptionsContainer();
      container.Search.OnTextChanged += (Action<LineEdit.LineEditEventArgs>) (args =>
      {
        foreach (Control child in ((Control) container.Options).Children)
        {
          if (child is Button button2 && button2.Text != null)
            ((Control) button2).Visible = button2.Text.Contains(args.Text, StringComparison.OrdinalIgnoreCase);
        }
      });
      this._window.Container = (BoxContainer) container;
      ((Control) this._window).AddChild((Control) this._window.Container);
    }
    this._window.Title = s.Title;
    container.Message.Text = s.Message.Text;
    RichTextLabel message = container.Message;
    string text = container.Message.Text;
    int num = text != null ? (text.Length > 0 ? 1 : 0) : 0;
    ((Control) message).Visible = num != 0;
    ((Control) container.Options).DisposeAllChildren();
    for (int index1 = 0; index1 < s.Options.Count; ++index1)
    {
      DialogOption option = s.Options[index1];
      Button button3 = new Button();
      button3.Text = option.Text;
      ((Control) button3).StyleClasses.Add("OpenBoth");
      Button button4 = button3;
      ((Control) button4.Label).AddStyleClass("CMAlignLeft");
      int index = index1;
      ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DialogOptionBuiMsg(index)));
      ((Control) container.Options).AddChild((Control) button4);
    }
  }

  private void UpdateInput(DialogComponent s)
  {
    RMCDialogWindow window = this._window;
    if (window == null || !((BaseWindow) window).IsOpen)
      return;
    RMCDialogInputContainer container = this._window.Container as RMCDialogInputContainer;
    if (container == null)
    {
      ((Control) this._window.Container)?.Orphan();
      this._window.Container = (BoxContainer) null;
      container = new RMCDialogInputContainer();
      container.MessageLineEdit.OnTextEntered += (Action<LineEdit.LineEditEventArgs>) (args => this.SendPredictedMessage((BoundUserInterfaceMessage) new DialogInputBuiMsg(args.Text)));
      container.MessageLineEdit.OnTextChanged += (Action<LineEdit.LineEditEventArgs>) (args => this.OnInputTextChanged(container, args.Text.Length, s.CharacterLimit));
      container.MessageTextEdit.OnTextChanged += (Action<TextEdit.TextEditEventArgs>) (args => this.OnInputTextChanged(container, (int) Rope.CalcTotalLength(args.TextRope), s.CharacterLimit));
      ((BaseButton) container.CancelButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.Close());
      ((BaseButton) container.OkButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        DialogComponent componentOrNull = EntityManagerExt.GetComponentOrNull<DialogComponent>(this.EntMan, this.Owner);
        this.SendPredictedMessage((BoundUserInterfaceMessage) new DialogInputBuiMsg((componentOrNull != null ? (componentOrNull.LargeInput ? 1 : 0) : 0) != 0 ? Rope.Collapse(container.MessageTextEdit.TextRope) : container.MessageLineEdit.Text));
      });
      this._window.Container = (BoxContainer) container;
      ((Control) this._window).AddChild((Control) this._window.Container);
      this.OnInputTextChanged(container, 0, s.CharacterLimit);
    }
    this._window.Title = string.Empty;
    container.MessageLabel.Text = s.Message.Text;
    ((Control) container.MessageLineEdit).Visible = !s.LargeInput;
    ((Control) container.MessageTextEdit).Visible = s.LargeInput;
    if (!s.AutoFocus)
      return;
    if (!s.LargeInput)
      ((Control) container.MessageLineEdit).GrabKeyboardFocus();
    else
      ((Control) container.MessageTextEdit).GrabKeyboardFocus();
  }

  private void UpdateConfirm(DialogComponent s)
  {
    RMCDialogWindow window = this._window;
    if (window == null || !((BaseWindow) window).IsOpen)
      return;
    if (!(this._window.Container is RMCDialogConfirmContainer confirmContainer))
    {
      ((Control) this._window.Container)?.Orphan();
      this._window.Container = (BoxContainer) null;
      confirmContainer = new RMCDialogConfirmContainer();
      ((BaseButton) confirmContainer.CancelButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.Close());
      ((BaseButton) confirmContainer.OkButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DialogConfirmBuiMsg()));
      this._window.Container = (BoxContainer) confirmContainer;
      ((Control) this._window).AddChild((Control) this._window.Container);
    }
    this._window.Title = s.Title;
    confirmContainer.MessageLabel.Text = s.Message.Text;
  }

  public void Refresh()
  {
    DialogComponent s;
    if (!this.EntMan.TryGetComponent<DialogComponent>(this.Owner, ref s))
      return;
    switch (s.DialogType)
    {
      case DialogType.Options:
        this.UpdateOptions(s);
        break;
      case DialogType.Input:
        this.UpdateInput(s);
        break;
      case DialogType.Confirm:
        this.UpdateConfirm(s);
        break;
    }
    ((BaseWindow) this._window)?.OpenCentered();
  }

  private void OnInputTextChanged(RMCDialogInputContainer container, int textLength, int max)
  {
    container.CharacterCount.Text = $"{textLength} / {max}";
    ((BaseButton) container.OkButton).Disabled = textLength > max;
  }
}
