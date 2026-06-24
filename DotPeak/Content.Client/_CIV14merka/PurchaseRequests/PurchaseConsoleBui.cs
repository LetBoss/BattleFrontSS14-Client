// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.PurchaseRequests.PurchaseConsoleBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.PurchaseRequest;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._CIV14merka.PurchaseRequests;

public sealed class PurchaseConsoleBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private PurchaseConsoleWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<PurchaseConsoleWindow>((BoundUserInterface) this);
    this._window.OnPurchaseSubmit += new Action<List<PurchaseItem>>(this.OnPurchaseSubmit);
    this._window.OnRequestApprove += new Action<Guid>(this.OnRequestApprove);
    this._window.OnRequestDeny += new Action<Guid>(this.OnRequestDeny);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is PurchaseConsoleBuiState state1))
      return;
    this._window?.UpdateState(state1);
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._window)?.Dispose();
  }

  private void OnPurchaseSubmit(List<PurchaseItem> items)
  {
    this.SendMessage((BoundUserInterfaceMessage) new PurchaseRequestSubmitMessage()
    {
      Items = items
    });
  }

  private void OnRequestApprove(Guid requestId)
  {
    this.SendMessage((BoundUserInterfaceMessage) new PurchaseRequestApproveMessage()
    {
      RequestId = requestId
    });
  }

  private void OnRequestDeny(Guid requestId)
  {
    this.SendMessage((BoundUserInterfaceMessage) new PurchaseRequestDenyMessage()
    {
      RequestId = requestId
    });
  }
}
