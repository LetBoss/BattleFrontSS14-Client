// Decompiled with JetBrains decompiler
// Type: Content.Shared.Gateway.GatewayBoundUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Gateway;

[NetSerializable]
[Serializable]
public sealed class GatewayBoundUserInterfaceState : BoundUserInterfaceState
{
  public readonly List<GatewayDestinationData> Destinations;
  public readonly NetEntity? Current;
  public readonly TimeSpan NextReady;
  public readonly TimeSpan Cooldown;
  public readonly TimeSpan NextUnlock;
  public readonly TimeSpan UnlockTime;

  public GatewayBoundUserInterfaceState(
    List<GatewayDestinationData> destinations,
    NetEntity? current,
    TimeSpan nextReady,
    TimeSpan cooldown,
    TimeSpan nextUnlock,
    TimeSpan unlockTime)
  {
    this.Destinations = destinations;
    this.Current = current;
    this.NextReady = nextReady;
    this.Cooldown = cooldown;
    this.NextUnlock = nextUnlock;
    this.UnlockTime = unlockTime;
  }
}
