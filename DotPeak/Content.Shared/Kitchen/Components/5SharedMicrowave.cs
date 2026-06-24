// Decompiled with JetBrains decompiler
// Type: Content.Shared.Kitchen.Components.MicrowaveUpdateUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Kitchen.Components;

[NetSerializable]
[Serializable]
public sealed class MicrowaveUpdateUserInterfaceState : BoundUserInterfaceState
{
  public NetEntity[] ContainedSolids;
  public bool IsMicrowaveBusy;
  public int ActiveButtonIndex;
  public uint CurrentCookTime;
  public TimeSpan CurrentCookTimeEnd;

  public MicrowaveUpdateUserInterfaceState(
    NetEntity[] containedSolids,
    bool isMicrowaveBusy,
    int activeButtonIndex,
    uint currentCookTime,
    TimeSpan currentCookTimeEnd)
  {
    this.ContainedSolids = containedSolids;
    this.IsMicrowaveBusy = isMicrowaveBusy;
    this.ActiveButtonIndex = activeButtonIndex;
    this.CurrentCookTime = currentCookTime;
    this.CurrentCookTimeEnd = currentCookTimeEnd;
  }
}
