// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.EntitySystems.GasPressureRegulatorSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Atmos.Piping.Binary.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Atmos.EntitySystems;

public sealed class GasPressureRegulatorSystem : SharedGasPressureRegulatorSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasPressureRegulatorComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<GasPressureRegulatorComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnValveUpdate)), (Type[]) null, (Type[]) null);
  }

  private void OnValveUpdate(
    Entity<GasPressureRegulatorComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.UpdateUi(ent);
  }

  protected override void UpdateUi(Entity<GasPressureRegulatorComponent> ent)
  {
    BoundUserInterface boundUserInterface;
    if (!this.UserInterfaceSystem.TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum) GasPressureRegulatorUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.Update();
  }
}
