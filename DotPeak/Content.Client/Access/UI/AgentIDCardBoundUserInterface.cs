// Decompiled with JetBrains decompiler
// Type: Content.Client.Access.UI.AgentIDCardBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Access.Systems;
using Content.Shared.StatusIcon;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Access.UI;

public sealed class AgentIDCardBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private AgentIDCardWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<AgentIDCardWindow>((BoundUserInterface) this);
    this._window.OnNameChanged += new Action<string>(this.OnNameChanged);
    this._window.OnJobChanged += new Action<string>(this.OnJobChanged);
    this._window.OnJobIconChanged += new Action<ProtoId<JobIconPrototype>>(this.OnJobIconChanged);
  }

  private void OnNameChanged(string newName)
  {
    this.SendMessage((BoundUserInterfaceMessage) new AgentIDCardNameChangedMessage(newName));
  }

  private void OnJobChanged(string newJob)
  {
    this.SendMessage((BoundUserInterfaceMessage) new AgentIDCardJobChangedMessage(newJob));
  }

  public void OnJobIconChanged(ProtoId<JobIconPrototype> newJobIconId)
  {
    this.SendMessage((BoundUserInterfaceMessage) new AgentIDCardJobIconChangedMessage(newJobIconId));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (this._window == null || !(state is AgentIDCardBoundUserInterfaceState userInterfaceState))
      return;
    this._window.SetCurrentName(userInterfaceState.CurrentName);
    this._window.SetCurrentJob(userInterfaceState.CurrentJob);
    this._window.SetAllowedIcons(userInterfaceState.CurrentJobIconId);
  }
}
