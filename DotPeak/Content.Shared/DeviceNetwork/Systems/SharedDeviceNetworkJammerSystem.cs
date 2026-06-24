// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceNetwork.Systems.SharedDeviceNetworkJammerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceNetwork.Components;
using Robust.Shared.GameObjects;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.DeviceNetwork.Systems;

public abstract class SharedDeviceNetworkJammerSystem : EntitySystem
{
  public void SetRange(Entity<DeviceNetworkJammerComponent> ent, float value)
  {
    ent.Comp.Range = value;
    this.Dirty<DeviceNetworkJammerComponent>(ent, (MetaDataComponent) null);
  }

  public bool TrySetRange(Entity<DeviceNetworkJammerComponent?> ent, float value)
  {
    if (!this.Resolve<DeviceNetworkJammerComponent>(Entity<DeviceNetworkJammerComponent>.op_Implicit(ent), ref ent.Comp, false))
      return false;
    this.SetRange(Entity<DeviceNetworkJammerComponent>.op_Implicit((Entity<DeviceNetworkJammerComponent>.op_Implicit(ent), ent.Comp)), value);
    return true;
  }

  public IReadOnlySet<string> GetJammableNetworks(Entity<DeviceNetworkJammerComponent> ent)
  {
    return (IReadOnlySet<string>) ent.Comp.JammableNetworks;
  }

  public void AddJammableNetwork(Entity<DeviceNetworkJammerComponent> ent, string networkId)
  {
    if (!ent.Comp.JammableNetworks.Add(networkId))
      return;
    this.Dirty<DeviceNetworkJammerComponent>(ent, (MetaDataComponent) null);
  }

  public void RemoveJammableNetwork(Entity<DeviceNetworkJammerComponent> ent, string networkId)
  {
    if (!ent.Comp.JammableNetworks.Remove(networkId))
      return;
    this.Dirty<DeviceNetworkJammerComponent>(ent, (MetaDataComponent) null);
  }

  public void ClearJammableNetworks(Entity<DeviceNetworkJammerComponent> ent)
  {
    if (ent.Comp.JammableNetworks.Count == 0)
      return;
    ent.Comp.JammableNetworks.Clear();
    this.Dirty<DeviceNetworkJammerComponent>(ent, (MetaDataComponent) null);
  }
}
