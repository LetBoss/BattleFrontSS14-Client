// Decompiled with JetBrains decompiler
// Type: Content.Client.MachineLinking.UI.SignalTimerBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.MachineLinking;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.MachineLinking.UI;

public sealed class SignalTimerBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private SignalTimerWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<SignalTimerWindow>((BoundUserInterface) this);
    this._window.OnStartTimer += new Action(this.StartTimer);
    this._window.OnCurrentTextChanged += new Action<string>(this.OnTextChanged);
    this._window.OnCurrentDelayMinutesChanged += new Action<string>(this.OnDelayChanged);
    this._window.OnCurrentDelaySecondsChanged += new Action<string>(this.OnDelayChanged);
  }

  public void StartTimer()
  {
    this.SendMessage((BoundUserInterfaceMessage) new SignalTimerStartMessage());
  }

  private void OnTextChanged(string newText)
  {
    this.SendMessage((BoundUserInterfaceMessage) new SignalTimerTextChangedMessage(newText));
  }

  private void OnDelayChanged(string newDelay)
  {
    if (this._window == null)
      return;
    this.SendMessage((BoundUserInterfaceMessage) new SignalTimerDelayChangedMessage(this._window.GetDelay()));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (this._window == null || !(state is SignalTimerBoundUserInterfaceState userInterfaceState))
      return;
    this._window.SetCurrentText(userInterfaceState.CurrentText);
    this._window.SetCurrentDelayMinutes(userInterfaceState.CurrentDelayMinutes);
    this._window.SetCurrentDelaySeconds(userInterfaceState.CurrentDelaySeconds);
    this._window.SetShowText(userInterfaceState.ShowText);
    this._window.SetTriggerTime(userInterfaceState.TriggerTime);
    this._window.SetTimerStarted(userInterfaceState.TimerStarted);
    this._window.SetHasAccess(userInterfaceState.HasAccess);
  }
}
