// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Mortar.CivMortarBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Mortar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._CIV14merka.Mortar;

public sealed class CivMortarBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private CivMortarWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<CivMortarWindow>((BoundUserInterface) this);
    ((BaseButton) this._window.SetTargetButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new CivMortarTargetBuiMsg(Vector2i.op_Implicit((Parse(this._window.TargetX), Parse(this._window.TargetY))))));
    this._window.DialControl.OnDialChanged += (Action<Vector2i>) (dial => this.SendPredictedMessage((BoundUserInterfaceMessage) new CivMortarDialBuiMsg(dial)));
    this.Refresh();

    static int Parse(FloatSpinBox spinBox) => (int) spinBox.Value;
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is CivMortarBuiState state1))
      return;
    CivMortarWindow window = this._window;
    if (window == null || !((BaseWindow) window).IsOpen)
      return;
    window.UpdateState(state1, new Action<int>(this.AcceptRequest), new Action<int>(this.RejectRequest));
  }

  public void Refresh()
  {
    CivMortarWindow window = this._window;
    if (window == null || !((BaseWindow) window).IsOpen)
      return;
    CivMortarComponent civMortarComponent;
    if (!this.EntMan.TryGetComponent<CivMortarComponent>(this.Owner, ref civMortarComponent))
      window.SetEmptyRequests();
    else
      window.SetTargetDial(civMortarComponent.Target, civMortarComponent.Dial);
  }

  private void AcceptRequest(int requestId)
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new CivMortarAcceptRequestBuiMsg(requestId));
  }

  private void RejectRequest(int requestId)
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new CivMortarRejectRequestBuiMsg(requestId));
  }
}
