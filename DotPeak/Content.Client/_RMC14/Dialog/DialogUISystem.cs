// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Dialog.DialogUISystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.UserInterface;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Dialog;

public sealed class DialogUISystem : EntitySystem
{
  [Dependency]
  private RMCUserInterfaceSystem _rmcUI;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DialogComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<DialogComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnDialogState)), (Type[]) null, (Type[]) null);
  }

  private void OnDialogState(Entity<DialogComponent> ent, ref AfterAutoHandleStateEvent args)
  {
    this._rmcUI.TryBui<DialogBui>(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Action<DialogBui>) (bui => bui.Refresh()));
  }
}
