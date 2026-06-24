// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.Piping.Unary.Systems.GasCanisterSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Atmos.UI;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Atmos.Piping.Unary.Systems;
using Content.Shared.NodeContainer;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Atmos.Piping.Unary.Systems;

public sealed class GasCanisterSystem : SharedGasCanisterSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasCanisterComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<GasCanisterComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnGasState)), (Type[]) null, (Type[]) null);
  }

  private void OnGasState(Entity<GasCanisterComponent> ent, ref AfterAutoHandleStateEvent args)
  {
    GasCanisterBoundUserInterface boundUserInterface;
    if (!this.UI.TryGetOpenUi<GasCanisterBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum) GasCanisterUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.Update<GasCanisterBoundUserInterfaceState>();
  }

  protected override void DirtyUI(
    EntityUid uid,
    GasCanisterComponent? component = null,
    NodeContainerComponent? nodes = null)
  {
    GasCanisterBoundUserInterface boundUserInterface;
    if (!this.UI.TryGetOpenUi<GasCanisterBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum) GasCanisterUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.Update<GasCanisterBoundUserInterfaceState>();
  }
}
