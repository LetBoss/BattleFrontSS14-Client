// Decompiled with JetBrains decompiler
// Type: Content.Client.Silicons.Borgs.BorgSelectTypeUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Silicons.Borgs;
using Content.Shared.Silicons.Borgs.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Silicons.Borgs;

public sealed class BorgSelectTypeUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private BorgSelectTypeMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<BorgSelectTypeMenu>((BoundUserInterface) this);
    this._menu.ConfirmedBorgType += (Action<ProtoId<BorgTypePrototype>>) (prototype => this.SendPredictedMessage((BoundUserInterfaceMessage) new BorgSelectTypeMessage(prototype)));
  }
}
