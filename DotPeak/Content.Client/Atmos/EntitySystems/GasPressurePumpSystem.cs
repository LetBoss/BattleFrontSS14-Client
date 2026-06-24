// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.EntitySystems.GasPressurePumpSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Atmos.Piping.Binary.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Atmos.EntitySystems;

public sealed class GasPressurePumpSystem : SharedGasPressurePumpSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasPressurePumpComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<GasPressurePumpComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnPumpUpdate)), (Type[]) null, (Type[]) null);
  }

  private void OnPumpUpdate(
    Entity<GasPressurePumpComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.UpdateUi(ent);
  }

  protected override void UpdateUi(Entity<GasPressurePumpComponent> ent)
  {
    BoundUserInterface boundUserInterface;
    if (!this.UserInterfaceSystem.TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum) GasPressurePumpUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.Update();
  }
}
