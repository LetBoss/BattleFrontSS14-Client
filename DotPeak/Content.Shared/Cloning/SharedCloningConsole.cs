// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cloning.CloningConsole.CloningConsoleBoundUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Cloning.CloningConsole;

[NetSerializable]
[Serializable]
public sealed class CloningConsoleBoundUserInterfaceState : BoundUserInterfaceState
{
  public readonly string? ScannerBodyInfo;
  public readonly string? ClonerBodyInfo;
  public readonly bool MindPresent;
  public readonly ClonerStatus CloningStatus;
  public readonly bool ScannerConnected;
  public readonly bool ScannerInRange;
  public readonly bool ClonerConnected;
  public readonly bool ClonerInRange;

  public CloningConsoleBoundUserInterfaceState(
    string? scannerBodyInfo,
    string? cloningBodyInfo,
    bool mindPresent,
    ClonerStatus cloningStatus,
    bool scannerConnected,
    bool scannerInRange,
    bool clonerConnected,
    bool clonerInRange)
  {
    this.ScannerBodyInfo = scannerBodyInfo;
    this.ClonerBodyInfo = cloningBodyInfo;
    this.MindPresent = mindPresent;
    this.CloningStatus = cloningStatus;
    this.ScannerConnected = scannerConnected;
    this.ScannerInRange = scannerInRange;
    this.ClonerConnected = clonerConnected;
    this.ClonerInRange = clonerInRange;
  }
}
