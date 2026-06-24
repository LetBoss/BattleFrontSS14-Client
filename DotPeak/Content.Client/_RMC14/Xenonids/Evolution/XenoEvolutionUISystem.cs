// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Evolution.XenoEvolutionUISystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Evolution;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Evolution;

public sealed class XenoEvolutionUISystem : EntitySystem
{
  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<XenoEvolutionComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<XenoEvolutionComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnXenoEvolutionAfterState)), (Type[]) null, (Type[]) null);
  }

  private void OnXenoEvolutionAfterState(
    Entity<XenoEvolutionComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    UserInterfaceComponent interfaceComponent;
    if (!this.TryComp<UserInterfaceComponent>(Entity<XenoEvolutionComponent>.op_Implicit(ent), ref interfaceComponent))
      return;
    foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
    {
      if (boundUserInterface is XenoEvolutionBui xenoEvolutionBui)
        xenoEvolutionBui.Refresh();
    }
  }
}
