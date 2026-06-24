// Decompiled with JetBrains decompiler
// Type: Content.Shared.Research.Components.ResearchClientBoundInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Research.Components;

[NetSerializable]
[Serializable]
public sealed class ResearchClientBoundInterfaceState : BoundUserInterfaceState
{
  public int ServerCount;
  public string[] ServerNames;
  public int[] ServerIds;
  public int SelectedServerId;

  public ResearchClientBoundInterfaceState(
    int serverCount,
    string[] serverNames,
    int[] serverIds,
    int selectedServerId = -1)
  {
    this.ServerCount = serverCount;
    this.ServerNames = serverNames;
    this.ServerIds = serverIds;
    this.SelectedServerId = selectedServerId;
  }
}
