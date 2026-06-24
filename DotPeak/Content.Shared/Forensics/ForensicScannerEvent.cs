// Decompiled with JetBrains decompiler
// Type: Content.Shared.Forensics.ForensicScannerBoundUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Forensics;

[NetSerializable]
[Serializable]
public sealed class ForensicScannerBoundUserInterfaceState : BoundUserInterfaceState
{
  public readonly List<string> Fingerprints = new List<string>();
  public readonly List<string> Fibers = new List<string>();
  public readonly List<string> TouchDNAs = new List<string>();
  public readonly List<string> SolutionDNAs = new List<string>();
  public readonly List<string> Residues = new List<string>();
  public readonly string LastScannedName = string.Empty;
  public readonly TimeSpan PrintCooldown = TimeSpan.Zero;
  public readonly TimeSpan PrintReadyAt = TimeSpan.Zero;

  public ForensicScannerBoundUserInterfaceState(
    List<string> fingerprints,
    List<string> fibers,
    List<string> touchDnas,
    List<string> solutionDnas,
    List<string> residues,
    string lastScannedName,
    TimeSpan printCooldown,
    TimeSpan printReadyAt)
  {
    this.Fingerprints = fingerprints;
    this.Fibers = fibers;
    this.TouchDNAs = touchDnas;
    this.SolutionDNAs = solutionDnas;
    this.Residues = residues;
    this.LastScannedName = lastScannedName;
    this.PrintCooldown = printCooldown;
    this.PrintReadyAt = printReadyAt;
  }
}
