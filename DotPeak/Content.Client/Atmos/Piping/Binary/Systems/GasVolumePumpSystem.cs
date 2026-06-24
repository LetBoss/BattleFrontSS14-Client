// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.Piping.Binary.Systems.GasVolumePumpSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Atmos.Piping.Binary.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Atmos.Piping.Binary.Systems;

public sealed class GasVolumePumpSystem : SharedGasVolumePumpSystem
{
  [Dependency]
  private UserInterfaceSystem _ui;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasVolumePumpComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<GasVolumePumpComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnPumpState)), (Type[]) null, (Type[]) null);
  }

  protected override void UpdateUi(Entity<GasVolumePumpComponent> entity)
  {
    BoundUserInterface boundUserInterface;
    if (!((SharedUserInterfaceSystem) this._ui).TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(entity.Owner), (Enum) GasVolumePumpUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.Update();
  }

  private void OnPumpState(Entity<GasVolumePumpComponent> ent, ref AfterAutoHandleStateEvent args)
  {
    this.UpdateUi(ent);
  }
}
