// Decompiled with JetBrains decompiler
// Type: Content.Client.Body.Systems.InternalsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.Components;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Body.Systems;

public sealed class InternalsSystem : SharedInternalsSystem
{
  [Dependency]
  private SharedUserInterfaceSystem _ui;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InternalsComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<InternalsComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnInternalsAfterState)), (Type[]) null, (Type[]) null);
  }

  private void OnInternalsAfterState(
    Entity<InternalsComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    BoundUserInterface boundUserInterface;
    if (!ent.Comp.GasTankEntity.HasValue || !this._ui.TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Comp.GasTankEntity.Value), (Enum) SharedGasTankUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.Update();
  }
}
