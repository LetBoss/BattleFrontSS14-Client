// Decompiled with JetBrains decompiler
// Type: Content.Client.NodeContainer.NodeGroupSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.NodeContainer;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.NodeContainer;

public sealed class NodeGroupSystem : EntitySystem
{
  [Dependency]
  private IOverlayManager _overlayManager;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IResourceCache _resourceCache;

  public bool VisEnabled { get; private set; }

  public Dictionary<int, NodeVis.GroupData> Groups { get; } = new Dictionary<int, NodeVis.GroupData>();

  public HashSet<string> Filtered { get; } = new HashSet<string>();

  public Dictionary<EntityUid, (NodeVis.GroupData group, NodeVis.NodeDatum node)[]> Entities { get; private set; } = new Dictionary<EntityUid, (NodeVis.GroupData, NodeVis.NodeDatum)[]>();

  public Dictionary<(int group, int node), NodeVis.NodeDatum> NodeLookup { get; private set; } = new Dictionary<(int, int), NodeVis.NodeDatum>();

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<NodeVis.MsgData>(new EntityEventHandler<NodeVis.MsgData>(this.DataMsgHandler), (Type[]) null, (Type[]) null);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlayManager.RemoveOverlay<NodeVisualizationOverlay>();
  }

  private void DataMsgHandler(NodeVis.MsgData ev)
  {
    if (!this.VisEnabled)
      return;
    foreach (int groupDeletion in ev.GroupDeletions)
      this.Groups.Remove(groupDeletion);
    foreach (NodeVis.GroupData group in ev.Groups)
      this.Groups.Add(group.NetId, group);
    foreach ((int key, string str) in ev.GroupDataUpdates)
    {
      NodeVis.GroupData groupData;
      if (this.Groups.TryGetValue(key, out groupData))
        groupData.DebugData = str;
    }
    this.Entities = this.Groups.Values.SelectMany<NodeVis.GroupData, NodeVis.NodeDatum, (NodeVis.GroupData, NodeVis.NodeDatum)>((Func<NodeVis.GroupData, IEnumerable<NodeVis.NodeDatum>>) (g => (IEnumerable<NodeVis.NodeDatum>) g.Nodes), (Func<NodeVis.GroupData, NodeVis.NodeDatum, (NodeVis.GroupData, NodeVis.NodeDatum)>) ((data, nodeData) => (data, nodeData))).GroupBy<(NodeVis.GroupData, NodeVis.NodeDatum), EntityUid>((Func<(NodeVis.GroupData, NodeVis.NodeDatum), EntityUid>) (n => this.GetEntity(n.nodeData.Entity))).ToDictionary<IGrouping<EntityUid, (NodeVis.GroupData, NodeVis.NodeDatum)>, EntityUid, (NodeVis.GroupData, NodeVis.NodeDatum)[]>((Func<IGrouping<EntityUid, (NodeVis.GroupData, NodeVis.NodeDatum)>, EntityUid>) (g => g.Key), (Func<IGrouping<EntityUid, (NodeVis.GroupData, NodeVis.NodeDatum)>, (NodeVis.GroupData, NodeVis.NodeDatum)[]>) (g => g.ToArray<(NodeVis.GroupData, NodeVis.NodeDatum)>()));
    this.NodeLookup = this.Groups.Values.SelectMany<NodeVis.GroupData, NodeVis.NodeDatum, (NodeVis.GroupData, NodeVis.NodeDatum)>((Func<NodeVis.GroupData, IEnumerable<NodeVis.NodeDatum>>) (g => (IEnumerable<NodeVis.NodeDatum>) g.Nodes), (Func<NodeVis.GroupData, NodeVis.NodeDatum, (NodeVis.GroupData, NodeVis.NodeDatum)>) ((data, nodeData) => (data, nodeData))).ToDictionary<(NodeVis.GroupData, NodeVis.NodeDatum), (int, int), NodeVis.NodeDatum>((Func<(NodeVis.GroupData, NodeVis.NodeDatum), (int, int)>) (n => (n.data.NetId, n.nodeData.NetId)), (Func<(NodeVis.GroupData, NodeVis.NodeDatum), NodeVis.NodeDatum>) (n => n.nodeData));
  }

  public void SetVisEnabled(bool enabled)
  {
    this.VisEnabled = enabled;
    this.RaiseNetworkEvent((EntityEventArgs) new NodeVis.MsgEnable(enabled));
    if (enabled)
    {
      this._overlayManager.AddOverlay((Overlay) new NodeVisualizationOverlay(this, this._entityLookup, this._mapManager, this._inputManager, this._resourceCache, (IEntityManager) this.EntityManager));
    }
    else
    {
      this.Groups.Clear();
      this.Entities.Clear();
    }
  }
}
