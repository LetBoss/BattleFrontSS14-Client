// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Power.RMCPowerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Power.Components;
using Content.Shared._RMC14.Power;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Power;

public sealed class RMCPowerSystem : SharedRMCPowerSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RMCApcComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<RMCApcComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnApcState)), (Type[]) null, (Type[]) null);
  }

  public override bool IsPowered(EntityUid ent)
  {
    ApcPowerReceiverComponent receiverComponent;
    return this.TryComp<ApcPowerReceiverComponent>(ent, ref receiverComponent) && receiverComponent.Powered;
  }

  private void OnApcState(Entity<RMCApcComponent> ent, ref AfterAutoHandleStateEvent args)
  {
    try
    {
      UserInterfaceComponent interfaceComponent;
      if (!this.TryComp<UserInterfaceComponent>(Entity<RMCApcComponent>.op_Implicit(ent), ref interfaceComponent))
        return;
      foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
      {
        if (boundUserInterface is RMCApcBui rmcApcBui)
          rmcApcBui.Refresh();
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error refreshing {"RMCApcBui"}\n{ex}");
    }
  }
}
