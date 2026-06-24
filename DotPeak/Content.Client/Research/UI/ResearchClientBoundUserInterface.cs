// Decompiled with JetBrains decompiler
// Type: Content.Client.Research.UI.ResearchClientBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Research.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Research.UI;

public sealed class ResearchClientBoundUserInterface : BoundUserInterface
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private ResearchClientServerSelectionMenu? _menu;

  public ResearchClientBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this.SendMessage((BoundUserInterfaceMessage) new ResearchClientSyncMessage());
  }

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<ResearchClientServerSelectionMenu>((BoundUserInterface) this);
    this._menu.OnServerSelected += new Action<int>(this.SelectServer);
    this._menu.OnServerDeselected += new Action(this.DeselectServer);
  }

  public void SelectServer(int serverId)
  {
    this.SendMessage((BoundUserInterfaceMessage) new ResearchClientServerSelectedMessage(serverId));
  }

  public void DeselectServer()
  {
    this.SendMessage((BoundUserInterfaceMessage) new ResearchClientServerDeselectedMessage());
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is ResearchClientBoundInterfaceState boundInterfaceState))
      return;
    this._menu?.Populate(boundInterfaceState.ServerCount, boundInterfaceState.ServerNames, boundInterfaceState.ServerIds, boundInterfaceState.SelectedServerId);
  }
}
