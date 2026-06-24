// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.QuickDialogSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Content.Shared.Administration;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Administration;

public sealed class QuickDialogSystem : EntitySystem
{
  public virtual void Initialize()
  {
    this.SubscribeNetworkEvent<QuickDialogOpenEvent>(new EntityEventHandler<QuickDialogOpenEvent>(this.OpenDialog), (Type[]) null, (Type[]) null);
  }

  private void OpenDialog(QuickDialogOpenEvent ev)
  {
    bool ok = (ev.Buttons & QuickDialogButtonFlag.OkButton) != 0;
    bool cancel = (ev.Buttons & QuickDialogButtonFlag.CancelButton) != 0;
    DialogWindow dialogWindow = new DialogWindow(ev.Title, ev.Prompts, ok, cancel);
    dialogWindow.OnConfirmed += (Action<Dictionary<string, string>>) (responses => this.RaiseNetworkEvent((EntityEventArgs) new QuickDialogResponseEvent(ev.DialogId, responses, QuickDialogButtonFlag.OkButton)));
    dialogWindow.OnCancelled += (Action) (() => this.RaiseNetworkEvent((EntityEventArgs) new QuickDialogResponseEvent(ev.DialogId, new Dictionary<string, string>(), QuickDialogButtonFlag.CancelButton)));
  }
}
