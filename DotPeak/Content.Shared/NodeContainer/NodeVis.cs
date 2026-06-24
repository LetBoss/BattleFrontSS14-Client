// Decompiled with JetBrains decompiler
// Type: Content.Shared.NodeContainer.NodeVis
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.NodeContainer;

public static class NodeVis
{
  [NetSerializable]
  [Serializable]
  public sealed class MsgEnable : EntityEventArgs
  {
    public MsgEnable(bool enabled) => this.Enabled = enabled;

    public bool Enabled { get; }
  }

  [NetSerializable]
  [Serializable]
  public sealed class MsgData : EntityEventArgs
  {
    public List<NodeVis.GroupData> Groups = new List<NodeVis.GroupData>();
    public List<int> GroupDeletions = new List<int>();
    public Dictionary<int, string?> GroupDataUpdates = new Dictionary<int, string>();
  }

  [NetSerializable]
  [Serializable]
  public sealed class GroupData
  {
    public int NetId;
    public string GroupId = "";
    public Color Color;
    public NodeVis.NodeDatum[] Nodes = Array.Empty<NodeVis.NodeDatum>();
    public string? DebugData;
  }

  [NetSerializable]
  [Serializable]
  public sealed class NodeDatum
  {
    public NetEntity Entity;
    public int NetId;
    public int[] Reachable = Array.Empty<int>();
    public string Name = "";
    public string Type = "";
  }
}
