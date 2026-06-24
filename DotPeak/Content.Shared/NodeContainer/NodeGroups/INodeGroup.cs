// Decompiled with JetBrains decompiler
// Type: Content.Shared.NodeContainer.NodeGroups.INodeGroup
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.NodeContainer.NodeGroups;

public interface INodeGroup
{
  bool Remaking { get; }

  IReadOnlyList<Node> Nodes { get; }

  void Create(NodeGroupID groupId);

  void Initialize(Node sourceNode, IEntityManager entMan);

  void RemoveNode(Node node);

  void LoadNodes(List<Node> groupNodes);

  void AfterRemake(IEnumerable<IGrouping<INodeGroup?, Node>> newGroups);

  string? GetDebugData();
}
