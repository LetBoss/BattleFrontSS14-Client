// Decompiled with JetBrains decompiler
// Type: Content.Client.Disposal.Mailing.MailingUnitSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Disposal;
using Content.Shared.Disposal.Components;
using Content.Shared.Disposal.Mailing;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Disposal.Mailing;

public sealed class MailingUnitSystem : SharedMailingUnitSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MailingUnitComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<MailingUnitComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnMailingState)), (Type[]) null, (Type[]) null);
  }

  private void OnMailingState(Entity<MailingUnitComponent> ent, ref AfterAutoHandleStateEvent args)
  {
    MailingUnitBoundUserInterface boundUserInterface;
    if (!this.UserInterfaceSystem.TryGetOpenUi<MailingUnitBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum) MailingUnitUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.Refresh(ent);
  }
}
