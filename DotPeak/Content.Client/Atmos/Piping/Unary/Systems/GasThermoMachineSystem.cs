// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.Piping.Unary.Systems.GasThermoMachineSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Atmos.UI;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Atmos.Piping.Unary.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Atmos.Piping.Unary.Systems;

public sealed class GasThermoMachineSystem : SharedGasThermoMachineSystem
{
  [Dependency]
  private SharedUserInterfaceSystem _ui;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasThermoMachineComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<GasThermoMachineComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnGasAfterState)), (Type[]) null, (Type[]) null);
  }

  private void OnGasAfterState(
    Entity<GasThermoMachineComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.DirtyUI(ent.Owner, ent.Comp, (UserInterfaceComponent) null);
  }

  protected override void DirtyUI(
    EntityUid uid,
    GasThermoMachineComponent? thermoMachine,
    UserInterfaceComponent? ui = null)
  {
    GasThermomachineBoundUserInterface boundUserInterface;
    if (!this._ui.TryGetOpenUi<GasThermomachineBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum) ThermomachineUiKey.Key, ref boundUserInterface))
      return;
    ((BoundUserInterface) boundUserInterface).Update();
  }
}
