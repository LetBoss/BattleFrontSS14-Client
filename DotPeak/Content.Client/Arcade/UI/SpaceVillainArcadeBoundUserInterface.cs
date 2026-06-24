// Decompiled with JetBrains decompiler
// Type: Content.Client.Arcade.UI.SpaceVillainArcadeBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Arcade;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Arcade.UI;

public sealed class SpaceVillainArcadeBoundUserInterface : BoundUserInterface
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private SpaceVillainArcadeMenu? _menu;

  public SpaceVillainArcadeBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this.SendAction(SharedSpaceVillainArcadeComponent.PlayerAction.RequestData);
  }

  public void SendAction(
    SharedSpaceVillainArcadeComponent.PlayerAction action)
  {
    this.SendMessage((BoundUserInterfaceMessage) new SharedSpaceVillainArcadeComponent.SpaceVillainArcadePlayerActionMessage(action));
  }

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<SpaceVillainArcadeMenu>((BoundUserInterface) this);
    this._menu.OnPlayerAction += new Action<SharedSpaceVillainArcadeComponent.PlayerAction>(this.SendAction);
  }

  protected virtual void ReceiveMessage(BoundUserInterfaceMessage message)
  {
    if (!(message is SharedSpaceVillainArcadeComponent.SpaceVillainArcadeDataUpdateMessage message1))
      return;
    this._menu?.UpdateInfo(message1);
  }
}
