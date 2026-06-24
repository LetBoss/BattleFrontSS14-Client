// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceNetwork.DeviceNet
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceNetwork.Components;
using Robust.Shared.Localization;
using Robust.Shared.Random;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.DeviceNetwork;

public sealed class DeviceNet
{
  public readonly Dictionary<string, DeviceNetworkComponent> Devices = new Dictionary<string, DeviceNetworkComponent>();
  public readonly Dictionary<uint, HashSet<DeviceNetworkComponent>> ListeningDevices = new Dictionary<uint, HashSet<DeviceNetworkComponent>>();
  public readonly Dictionary<uint, HashSet<DeviceNetworkComponent>> ReceiveAllDevices = new Dictionary<uint, HashSet<DeviceNetworkComponent>>();
  private readonly IRobustRandom _random;
  public readonly int NetId;

  public DeviceNet(int netId, IRobustRandom random)
  {
    this._random = random;
    this.NetId = netId;
  }

  public bool Add(DeviceNetworkComponent device)
  {
    if (device.CustomAddress)
    {
      if (!this.Devices.TryAdd(device.Address, device))
        return false;
    }
    else
    {
      if (string.IsNullOrWhiteSpace(device.Address) || this.Devices.ContainsKey(device.Address))
        device.Address = this.GenerateValidAddress(device.Prefix);
      this.Devices[device.Address] = device;
    }
    uint? receiveFrequency = device.ReceiveFrequency;
    if (!receiveFrequency.HasValue)
      return true;
    uint valueOrDefault = receiveFrequency.GetValueOrDefault();
    HashSet<DeviceNetworkComponent> networkComponentSet1;
    if (!this.ListeningDevices.TryGetValue(valueOrDefault, out networkComponentSet1))
      this.ListeningDevices[valueOrDefault] = networkComponentSet1 = new HashSet<DeviceNetworkComponent>();
    networkComponentSet1.Add(device);
    if (!device.ReceiveAll)
      return true;
    HashSet<DeviceNetworkComponent> networkComponentSet2;
    if (!this.ReceiveAllDevices.TryGetValue(valueOrDefault, out networkComponentSet2))
      this.ReceiveAllDevices[valueOrDefault] = networkComponentSet2 = new HashSet<DeviceNetworkComponent>();
    networkComponentSet2.Add(device);
    return true;
  }

  public bool Remove(DeviceNetworkComponent device)
  {
    if (device.Address == null || !this.Devices.Remove(device.Address))
      return false;
    uint? receiveFrequency = device.ReceiveFrequency;
    if (!receiveFrequency.HasValue)
      return true;
    uint valueOrDefault = receiveFrequency.GetValueOrDefault();
    HashSet<DeviceNetworkComponent> networkComponentSet1;
    if (this.ListeningDevices.TryGetValue(valueOrDefault, out networkComponentSet1))
    {
      networkComponentSet1.Remove(device);
      if (networkComponentSet1.Count == 0)
        this.ListeningDevices.Remove(valueOrDefault);
    }
    HashSet<DeviceNetworkComponent> networkComponentSet2;
    if (device.ReceiveAll && this.ReceiveAllDevices.TryGetValue(valueOrDefault, out networkComponentSet2))
    {
      networkComponentSet2.Remove(device);
      if (networkComponentSet2.Count == 0)
        this.ListeningDevices.Remove(valueOrDefault);
    }
    return true;
  }

  public bool RandomizeAddress(string oldAddress, string? prefix = null)
  {
    DeviceNetworkComponent networkComponent;
    if (!this.Devices.Remove(oldAddress, out networkComponent))
      return false;
    networkComponent.Address = this.GenerateValidAddress(prefix ?? networkComponent.Prefix);
    networkComponent.CustomAddress = false;
    this.Devices[networkComponent.Address] = networkComponent;
    return true;
  }

  public bool UpdateAddress(string oldAddress, string newAddress)
  {
    DeviceNetworkComponent networkComponent;
    if (this.Devices.ContainsKey(newAddress) || !this.Devices.Remove(oldAddress, out networkComponent))
      return false;
    networkComponent.Address = newAddress;
    networkComponent.CustomAddress = true;
    this.Devices[newAddress] = networkComponent;
    return true;
  }

  public bool UpdateReceiveFrequency(string address, uint? newFrequency)
  {
    DeviceNetworkComponent networkComponent;
    if (!this.Devices.TryGetValue(address, out networkComponent))
      return false;
    uint? receiveFrequency = networkComponent.ReceiveFrequency;
    uint? nullable = newFrequency;
    if ((int) receiveFrequency.GetValueOrDefault() == (int) nullable.GetValueOrDefault() & receiveFrequency.HasValue == nullable.HasValue)
      return true;
    nullable = networkComponent.ReceiveFrequency;
    if (nullable.HasValue)
    {
      uint valueOrDefault = nullable.GetValueOrDefault();
      HashSet<DeviceNetworkComponent> networkComponentSet1;
      if (this.ListeningDevices.TryGetValue(valueOrDefault, out networkComponentSet1))
      {
        networkComponentSet1.Remove(networkComponent);
        if (networkComponentSet1.Count == 0)
          this.ListeningDevices.Remove(valueOrDefault);
      }
      HashSet<DeviceNetworkComponent> networkComponentSet2;
      if (networkComponent.ReceiveAll && this.ReceiveAllDevices.TryGetValue(valueOrDefault, out networkComponentSet2))
      {
        networkComponentSet2.Remove(networkComponent);
        if (networkComponentSet2.Count == 0)
          this.ListeningDevices.Remove(valueOrDefault);
      }
    }
    networkComponent.ReceiveFrequency = newFrequency;
    if (!newFrequency.HasValue)
      return true;
    HashSet<DeviceNetworkComponent> networkComponentSet3;
    if (!this.ListeningDevices.TryGetValue(newFrequency.Value, out networkComponentSet3))
      this.ListeningDevices[newFrequency.Value] = networkComponentSet3 = new HashSet<DeviceNetworkComponent>();
    networkComponentSet3.Add(networkComponent);
    if (!networkComponent.ReceiveAll)
      return true;
    HashSet<DeviceNetworkComponent> networkComponentSet4;
    if (!this.ReceiveAllDevices.TryGetValue(newFrequency.Value, out networkComponentSet4))
      this.ReceiveAllDevices[newFrequency.Value] = networkComponentSet4 = new HashSet<DeviceNetworkComponent>();
    networkComponentSet4.Add(networkComponent);
    return true;
  }

  public bool UpdateReceiveAll(string address, bool receiveAll)
  {
    DeviceNetworkComponent networkComponent;
    if (!this.Devices.TryGetValue(address, out networkComponent))
      return false;
    if (networkComponent.ReceiveAll == receiveAll)
      return true;
    networkComponent.ReceiveAll = receiveAll;
    uint? receiveFrequency = networkComponent.ReceiveFrequency;
    if (!receiveFrequency.HasValue)
      return true;
    uint valueOrDefault = receiveFrequency.GetValueOrDefault();
    if (receiveAll)
    {
      HashSet<DeviceNetworkComponent> networkComponentSet;
      if (!this.ReceiveAllDevices.TryGetValue(valueOrDefault, out networkComponentSet))
        this.ReceiveAllDevices[valueOrDefault] = networkComponentSet = new HashSet<DeviceNetworkComponent>();
      networkComponentSet.Add(networkComponent);
    }
    else
    {
      HashSet<DeviceNetworkComponent> networkComponentSet;
      if (this.ReceiveAllDevices.TryGetValue(valueOrDefault, out networkComponentSet))
      {
        networkComponentSet.Remove(networkComponent);
        if (networkComponentSet.Count == 0)
          this.ReceiveAllDevices.Remove(valueOrDefault);
      }
    }
    return true;
  }

  private string GenerateValidAddress(string? prefix)
  {
    prefix = string.IsNullOrWhiteSpace(prefix) ? (string) null : Loc.GetString(prefix);
    string key;
    do
    {
      int num = this._random.Next();
      key = $"{prefix}{num >> 16 /*0x10*/:X4}-{num & (int) ushort.MaxValue:X4}";
    }
    while (this.Devices.ContainsKey(key));
    return key;
  }
}
