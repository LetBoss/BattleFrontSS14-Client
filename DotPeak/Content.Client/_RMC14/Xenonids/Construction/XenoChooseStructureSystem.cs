// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Construction.XenoChooseStructureSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Construction;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Construction;

public sealed class XenoChooseStructureSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<XenoConstructionComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<XenoConstructionComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnXenoConstructionAfterState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<QueenBuildingBoostComponent, ComponentStartup>(new EntityEventRefHandler<QueenBuildingBoostComponent, ComponentStartup>((object) this, __methodptr(OnBoostAdded)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<QueenBuildingBoostComponent, ComponentRemove>(new EntityEventRefHandler<QueenBuildingBoostComponent, ComponentRemove>((object) this, __methodptr(OnBoostRemoved)), (Type[]) null, (Type[]) null);
  }

  private void OnBoostAdded(Entity<QueenBuildingBoostComponent> ent, ref ComponentStartup args)
  {
    this.RefreshUI(ent.Owner);
  }

  private void OnBoostRemoved(Entity<QueenBuildingBoostComponent> ent, ref ComponentRemove args)
  {
    this.RefreshUI(ent.Owner);
  }

  private void RefreshUI(EntityUid entity)
  {
    UserInterfaceComponent interfaceComponent;
    if (!this._timing.IsFirstTimePredicted || !this.TryComp<UserInterfaceComponent>(entity, ref interfaceComponent))
      return;
    foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
    {
      if (boundUserInterface is XenoChooseStructureBui chooseStructureBui)
        chooseStructureBui.Close();
    }
  }

  private void OnXenoConstructionAfterState(
    Entity<XenoConstructionComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    UserInterfaceComponent interfaceComponent;
    if (!this._timing.IsFirstTimePredicted || !this.TryComp<UserInterfaceComponent>(Entity<XenoConstructionComponent>.op_Implicit(ent), ref interfaceComponent))
      return;
    foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
    {
      if (boundUserInterface is XenoChooseStructureBui chooseStructureBui)
        chooseStructureBui.Refresh();
    }
  }
}
