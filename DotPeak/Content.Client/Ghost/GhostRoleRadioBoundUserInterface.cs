// Decompiled with JetBrains decompiler
// Type: Content.Client.Ghost.GhostRoleRadioBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Ghost.Roles;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Ghost;

public sealed class GhostRoleRadioBoundUserInterface : BoundUserInterface
{
  private GhostRoleRadioMenu? _ghostRoleRadioMenu;

  public GhostRoleRadioBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    IoCManager.InjectDependencies<GhostRoleRadioBoundUserInterface>(this);
  }

  protected virtual void Open()
  {
    base.Open();
    this._ghostRoleRadioMenu = BoundUserInterfaceExt.CreateWindow<GhostRoleRadioMenu>((BoundUserInterface) this);
    this._ghostRoleRadioMenu.SetEntity(this.Owner);
    this._ghostRoleRadioMenu.SendGhostRoleRadioMessageAction += new Action<ProtoId<GhostRolePrototype>>(this.SendGhostRoleRadioMessage);
  }

  private void SendGhostRoleRadioMessage(ProtoId<GhostRolePrototype> protoId)
  {
    this.SendMessage((BoundUserInterfaceMessage) new GhostRoleRadioMessage(protoId));
  }
}
