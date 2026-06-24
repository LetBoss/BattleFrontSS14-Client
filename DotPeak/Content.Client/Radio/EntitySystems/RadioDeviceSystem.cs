// Decompiled with JetBrains decompiler
// Type: Content.Client.Radio.EntitySystems.RadioDeviceSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Radio.Ui;
using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Radio.EntitySystems;

public sealed class RadioDeviceSystem : EntitySystem
{
  [Dependency]
  private UserInterfaceSystem _ui;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<IntercomComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<IntercomComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnAfterHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnAfterHandleState(Entity<IntercomComponent> ent, ref AfterAutoHandleStateEvent args)
  {
    IntercomBoundUserInterface boundUserInterface;
    if (!((SharedUserInterfaceSystem) this._ui).TryGetOpenUi<IntercomBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum) IntercomUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.Update(ent);
  }
}
