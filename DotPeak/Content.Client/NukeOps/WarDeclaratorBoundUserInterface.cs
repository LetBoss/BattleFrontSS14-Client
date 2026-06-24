// Decompiled with JetBrains decompiler
// Type: Content.Client.NukeOps.WarDeclaratorBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.NukeOps;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.NukeOps;

public sealed class WarDeclaratorBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Dependency]
  private IConfigurationManager _cfg;
  [Robust.Shared.ViewVariables.ViewVariables]
  private WarDeclaratorWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<WarDeclaratorWindow>((BoundUserInterface) this);
    this._window.OnActivated += new Action<string>(this.OnWarDeclaratorActivated);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (this._window == null || !(state is WarDeclaratorBoundUserInterfaceState state1))
      return;
    this._window?.UpdateState(state1);
  }

  private void OnWarDeclaratorActivated(string message)
  {
    int cvar = this._cfg.GetCVar<int>(CCVars.ChatMaxAnnouncementLength);
    this.SendMessage((BoundUserInterfaceMessage) new WarDeclaratorActivateMessage(SharedChatSystem.SanitizeAnnouncement(message, cvar)));
  }
}
