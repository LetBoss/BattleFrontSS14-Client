// Decompiled with JetBrains decompiler
// Type: Content.Shared.Salvage.Expeditions.SalvageExpeditionConsoleState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Salvage.Expeditions;

[NetSerializable]
[Serializable]
public sealed class SalvageExpeditionConsoleState : BoundUserInterfaceState
{
  public TimeSpan NextOffer;
  public bool Claimed;
  public bool Cooldown;
  public ushort ActiveMission;
  public List<SalvageMissionParams> Missions;

  public SalvageExpeditionConsoleState(
    TimeSpan nextOffer,
    bool claimed,
    bool cooldown,
    ushort activeMission,
    List<SalvageMissionParams> missions)
  {
    this.NextOffer = nextOffer;
    this.Claimed = claimed;
    this.Cooldown = cooldown;
    this.ActiveMission = activeMission;
    this.Missions = missions;
  }
}
