// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.Components.CargoBountyConsoleState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Cargo.Components;

[NetSerializable]
[Serializable]
public sealed class CargoBountyConsoleState : BoundUserInterfaceState
{
  public List<CargoBountyData> Bounties;
  public List<CargoBountyHistoryData> History;
  public TimeSpan UntilNextSkip;

  public CargoBountyConsoleState(
    List<CargoBountyData> bounties,
    List<CargoBountyHistoryData> history,
    TimeSpan untilNextSkip)
  {
    this.Bounties = bounties;
    this.History = history;
    this.UntilNextSkip = untilNextSkip;
  }
}
