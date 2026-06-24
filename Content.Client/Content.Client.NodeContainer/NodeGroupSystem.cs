using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.NodeContainer;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

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

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<NodeVis.MsgData>((EntityEventHandler<NodeVis.MsgData>)DataMsgHandler, (Type[])null, (Type[])null);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlayManager.RemoveOverlay<NodeVisualizationOverlay>();
	}

	private void DataMsgHandler(NodeVis.MsgData ev)
	{
		if (!VisEnabled)
		{
			return;
		}
		foreach (int groupDeletion in ev.GroupDeletions)
		{
			Groups.Remove(groupDeletion);
		}
		foreach (NodeVis.GroupData group in ev.Groups)
		{
			Groups.Add(group.NetId, group);
		}
		foreach (var (key, debugData) in ev.GroupDataUpdates)
		{
			if (Groups.TryGetValue(key, out NodeVis.GroupData value))
			{
				value.DebugData = debugData;
			}
		}
		Entities = (from n in Groups.Values.SelectMany((NodeVis.GroupData g) => g.Nodes, (NodeVis.GroupData data, NodeVis.NodeDatum nodeData) => (data: data, nodeData: nodeData))
			group n by ((EntitySystem)this).GetEntity(n.nodeData.Entity)).ToDictionary((IGrouping<EntityUid, (NodeVis.GroupData data, NodeVis.NodeDatum nodeData)> g) => g.Key, (IGrouping<EntityUid, (NodeVis.GroupData data, NodeVis.NodeDatum nodeData)> g) => g.ToArray());
		NodeLookup = Groups.Values.SelectMany((NodeVis.GroupData g) => g.Nodes, (NodeVis.GroupData data, NodeVis.NodeDatum nodeData) => (data: data, nodeData: nodeData)).ToDictionary(((NodeVis.GroupData data, NodeVis.NodeDatum nodeData) n) => (n.data.NetId, n.nodeData.NetId), ((NodeVis.GroupData data, NodeVis.NodeDatum nodeData) n) => n.nodeData);
	}

	public void SetVisEnabled(bool enabled)
	{
		VisEnabled = enabled;
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new NodeVis.MsgEnable(enabled));
		if (enabled)
		{
			NodeVisualizationOverlay nodeVisualizationOverlay = new NodeVisualizationOverlay(this, _entityLookup, _mapManager, _inputManager, _resourceCache, (IEntityManager)(object)base.EntityManager);
			_overlayManager.AddOverlay((Overlay)(object)nodeVisualizationOverlay);
		}
		else
		{
			Groups.Clear();
			Entities.Clear();
		}
	}
}
