// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceNetwork.Systems.SharedNetworkConfiguratorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceNetwork.Components;
using Content.Shared.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared.DeviceNetwork.Systems;

public abstract class SharedNetworkConfiguratorSystem : EntitySystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<NetworkConfiguratorComponent, ActivatableUIOpenAttemptEvent>(new ComponentEventHandler<NetworkConfiguratorComponent, ActivatableUIOpenAttemptEvent>((object) this, __methodptr(OnUiOpenAttempt)), (Type[]) null, (Type[]) null);
  }

  private void OnUiOpenAttempt(
    EntityUid uid,
    NetworkConfiguratorComponent configurator,
    ActivatableUIOpenAttemptEvent args)
  {
    if (!configurator.LinkModeActive)
      return;
    args.Cancel();
  }
}
