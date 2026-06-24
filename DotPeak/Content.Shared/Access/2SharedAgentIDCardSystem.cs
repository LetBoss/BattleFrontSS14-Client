// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Systems.AgentIDCardBoundUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Access.Systems;

[NetSerializable]
[Serializable]
public sealed class AgentIDCardBoundUserInterfaceState : BoundUserInterfaceState
{
  public string CurrentName { get; }

  public string CurrentJob { get; }

  public string CurrentJobIconId { get; }

  public AgentIDCardBoundUserInterfaceState(
    string currentName,
    string currentJob,
    string currentJobIconId)
  {
    this.CurrentName = currentName;
    this.CurrentJob = currentJob;
    this.CurrentJobIconId = currentJobIconId;
  }
}
