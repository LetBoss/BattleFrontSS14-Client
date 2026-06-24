// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nuke.NukeUiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Nuke;

[NetSerializable]
[Serializable]
public sealed class NukeUiState : BoundUserInterfaceState
{
  public bool DiskInserted;
  public NukeStatus Status;
  public int RemainingTime;
  public int CooldownTime;
  public bool IsAnchored;
  public int EnteredCodeLength;
  public int MaxCodeLength;
  public bool AllowArm;
}
