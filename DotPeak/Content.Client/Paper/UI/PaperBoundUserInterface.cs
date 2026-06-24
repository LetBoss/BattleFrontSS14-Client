// Decompiled with JetBrains decompiler
// Type: Content.Client.Paper.UI.PaperBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Paper;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client.Paper.UI;

public sealed class PaperBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private PaperWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<PaperWindow>((BoundUserInterface) this);
    this._window.OnSaved += new Action<string>(this.InputOnTextEntered);
    this._window.OnSignatureRequested += new Action<int>(this.OnSignatureRequested);
    PaperComponent paperComponent;
    if (this.EntMan.TryGetComponent<PaperComponent>(this.Owner, ref paperComponent))
      this._window.MaxInputLength = paperComponent.ContentSize;
    PaperVisualsComponent visuals;
    if (!this.EntMan.TryGetComponent<PaperVisualsComponent>(this.Owner, ref visuals))
      return;
    this._window.InitVisuals(this.Owner, visuals);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    this._window?.Populate((PaperComponent.PaperBoundUserInterfaceState) state);
  }

  private void InputOnTextEntered(string text)
  {
    this.SendMessage((BoundUserInterfaceMessage) new PaperComponent.PaperInputTextMessage(text));
    if (this._window == null)
      return;
    this._window.Input.TextRope = (Rope.Node) Rope.Leaf.Empty;
    this._window.Input.CursorPosition = new TextEdit.CursorPos(0, (TextEdit.LineBreakBias) 0);
  }

  private void OnSignatureRequested(int signatureIndex)
  {
    this.SendMessage((BoundUserInterfaceMessage) new PaperComponent.PaperSignatureRequestMessage(signatureIndex));
  }
}
