// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Fruit.XenoFruitChooseSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Fruit.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Fruit;

public sealed class XenoFruitChooseSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<XenoFruitPlanterComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<XenoFruitPlanterComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnXenoFruitAfterState)), (Type[]) null, (Type[]) null);
  }

  private void OnXenoFruitAfterState(
    Entity<XenoFruitPlanterComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    try
    {
      UserInterfaceComponent interfaceComponent;
      if (!this._timing.IsFirstTimePredicted || !this.TryComp<UserInterfaceComponent>(Entity<XenoFruitPlanterComponent>.op_Implicit(ent), ref interfaceComponent))
        return;
      foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
      {
        if (boundUserInterface is XenoFruitChooseBui xenoFruitChooseBui)
          xenoFruitChooseBui.Refresh();
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error refreshing {"XenoFruitChooseBui"}\n{ex}");
    }
  }
}
