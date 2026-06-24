// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.EntitySystems.GasTankSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Atmos.EntitySystems;

public sealed class GasTankSystem : SharedGasTankSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasTankComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<GasTankComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnGasTankState)), (Type[]) null, (Type[]) null);
  }

  private void OnGasTankState(Entity<GasTankComponent> ent, ref AfterAutoHandleStateEvent args)
  {
    BoundUserInterface boundUserInterface;
    if (!this.UI.TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum) SharedGasTankUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.Update<GasTankBoundUserInterfaceState>();
  }

  public override void UpdateUserInterface(Entity<GasTankComponent> ent)
  {
    BoundUserInterface boundUserInterface;
    if (!this.UI.TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum) SharedGasTankUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.Update<GasTankBoundUserInterfaceState>();
  }
}
