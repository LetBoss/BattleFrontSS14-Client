// Decompiled with JetBrains decompiler
// Type: Content.Client.Weapons.Melee.UI.MeleeSpeechBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Speech.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Weapons.Melee.UI;

public sealed class MeleeSpeechBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private MeleeSpeechWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<MeleeSpeechWindow>((BoundUserInterface) this);
    this._window.OnBattlecryEntered += new Action<string>(this.OnBattlecryChanged);
  }

  private void OnBattlecryChanged(string newBattlecry)
  {
    this.SendMessage((BoundUserInterfaceMessage) new MeleeSpeechBattlecryChangedMessage(newBattlecry));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (this._window == null || !(state is MeleeSpeechBoundUserInterfaceState userInterfaceState))
      return;
    this._window.SetCurrentBattlecry(userInterfaceState.CurrentBattlecry);
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._window)?.Orphan();
  }
}
