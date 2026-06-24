// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceNetwork.Systems.SharedDeviceNetworkSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceNetwork.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.DeviceNetwork.Systems;

public abstract class SharedDeviceNetworkSystem : EntitySystem
{
  public virtual bool QueuePacket(
    EntityUid uid,
    string? address,
    NetworkPayload data,
    uint? frequency = null,
    int? network = null,
    DeviceNetworkComponent? device = null)
  {
    return false;
  }
}
