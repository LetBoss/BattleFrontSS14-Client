// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Camera.RMCCameraSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Camera;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Camera;

public sealed class RMCCameraSystem : SharedRMCCameraSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RMCCameraComputerComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<RMCCameraComputerComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnComputerState)), (Type[]) null, (Type[]) null);
  }

  private void OnComputerState(
    Entity<RMCCameraComputerComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    try
    {
      UserInterfaceComponent interfaceComponent;
      if (!this.TryComp<UserInterfaceComponent>(Entity<RMCCameraComputerComponent>.op_Implicit(ent), ref interfaceComponent))
        return;
      foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
      {
        if (boundUserInterface is RMCCameraBui rmcCameraBui)
          rmcCameraBui.Refresh();
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error refreshing {"RMCCameraBui"}\n{ex}");
    }
  }
}
