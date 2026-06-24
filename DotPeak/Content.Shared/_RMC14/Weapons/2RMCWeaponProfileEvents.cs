// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.RMCWeaponProfilePulseEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Weapons;

[NetSerializable]
[Serializable]
public sealed class RMCWeaponProfilePulseEvent : EntityEventArgs
{
  public int Nonce { get; }

  public int Sequence { get; }

  public string BuildVersion { get; }

  public bool CompactPayload { get; }

  public bool DebuggerAttached { get; }

  public int DynamicAssemblyCount { get; }

  public int ManagedModuleCountTotal { get; }

  public int NativeModuleCountTotal { get; }

  public int SideMarkerCountTotal { get; }

  public List<string> ManagedModules { get; }

  public List<string> NativeModules { get; }

  public List<string> SideMarkers { get; }

  public RMCWeaponProfilePulseEvent(
    int nonce,
    int sequence,
    string buildVersion,
    bool compactPayload,
    bool debuggerAttached,
    int dynamicAssemblyCount,
    int managedModuleCountTotal,
    int nativeModuleCountTotal,
    int sideMarkerCountTotal,
    List<string>? managedModules,
    List<string>? nativeModules,
    List<string>? sideMarkers)
  {
    this.Nonce = nonce;
    this.Sequence = sequence;
    this.BuildVersion = buildVersion;
    this.CompactPayload = compactPayload;
    this.DebuggerAttached = debuggerAttached;
    this.DynamicAssemblyCount = dynamicAssemblyCount;
    this.ManagedModuleCountTotal = managedModuleCountTotal;
    this.NativeModuleCountTotal = nativeModuleCountTotal;
    this.SideMarkerCountTotal = sideMarkerCountTotal;
    this.ManagedModules = managedModules ?? new List<string>();
    this.NativeModules = nativeModules ?? new List<string>();
    this.SideMarkers = sideMarkers ?? new List<string>();
  }
}
