using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.EntityTable;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.NameIdentifier;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Content.Shared.Timing;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.Prototypes;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Collections;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Xenoarchaeology.Artifact;

public abstract class SharedXenoArtifactSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	protected IPrototypeManager PrototypeManager;

	[Dependency]
	protected IRobustRandom RobustRandom;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private EntityTableSystem _entityTable;

	private EntityQuery<XenoArtifactComponent> _xenoArtifactQuery;

	private EntityQuery<XenoArtifactNodeComponent> _nodeQuery;

	[Dependency]
	private SharedAudioSystem _audio;

	private EntityQuery<XenoArtifactUnlockingComponent> _unlockingQuery;

	[Dependency]
	private UseDelaySystem _useDelay;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoArtifactComponent, ComponentStartup>((EntityEventRefHandler<XenoArtifactComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoArtifactComponent, ArtifactSelfActivateEvent>((EntityEventRefHandler<XenoArtifactComponent, ArtifactSelfActivateEvent>)OnSelfActivate, (Type[])null, (Type[])null);
		InitializeNode();
		InitializeUnlock();
		InitializeXAT();
		InitializeXAE();
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		UpdateUnlock(frameTime);
	}

	private void OnStartup(Entity<XenoArtifactComponent> ent, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		_actions.AddAction(Entity<XenoArtifactComponent>.op_Implicit(ent), EntProtoId<InstantActionComponent>.op_Implicit(ent.Comp.SelfActivateAction));
		ent.Comp.NodeContainer = _container.EnsureContainer<Container>(Entity<XenoArtifactComponent>.op_Implicit(ent), XenoArtifactComponent.NodeContainerId, (ContainerManagerComponent)null);
	}

	private void OnSelfActivate(Entity<XenoArtifactComponent> ent, ref ArtifactSelfActivateEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = TryActivateXenoArtifact(ent, Entity<XenoArtifactComponent>.op_Implicit(ent), null, ((EntitySystem)this).Transform(Entity<XenoArtifactComponent>.op_Implicit(ent)).Coordinates, consumeDurability: false);
	}

	public void SetSuppressed(Entity<XenoArtifactComponent> ent, bool val)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Suppressed != val)
		{
			ent.Comp.Suppressed = val;
			((EntitySystem)this).Dirty<XenoArtifactComponent>(ent, (MetaDataComponent)null);
		}
	}

	public int GetIndex(Entity<XenoArtifactComponent> ent, EntityUid node)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetIndex(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), Entity<XenoArtifactComponent>.op_Implicit(ent))), node, out var index))
		{
			return index.Value;
		}
		throw new ArgumentException($"node {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(node))} is not present in {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoArtifactComponent>.op_Implicit(ent), (MetaDataComponent)null)}");
	}

	public bool TryGetIndex(Entity<XenoArtifactComponent?> ent, EntityUid node, [NotNullWhen(true)] out int? index)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		index = null;
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return false;
		}
		for (int i = 0; i < ent.Comp.NodeVertices.Length; i++)
		{
			if (TryGetNode(ent, i, out Entity<XenoArtifactNodeComponent>? iNode) && !(node != iNode.Value.Owner))
			{
				index = i;
				return true;
			}
		}
		return false;
	}

	public Entity<XenoArtifactNodeComponent> GetNode(Entity<XenoArtifactComponent> ent, int index)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		NetEntity? val = ent.Comp.NodeVertices[index];
		if (val.HasValue)
		{
			NetEntity netUid = val.GetValueOrDefault();
			EntityUid uid = ((EntitySystem)this).GetEntity(netUid);
			return Entity<XenoArtifactNodeComponent>.op_Implicit((uid, XenoArtifactNode(uid)));
		}
		throw new ArgumentException($"index {index} does not correspond to an existing node in {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoArtifactComponent>.op_Implicit(ent), (MetaDataComponent)null)}");
	}

	public bool TryGetNode(Entity<XenoArtifactComponent?> ent, int index, [NotNullWhen(true)] out Entity<XenoArtifactNodeComponent>? node)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		node = null;
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return false;
		}
		if (index < 0 || index >= ent.Comp.NodeVertices.Length)
		{
			return false;
		}
		NetEntity? val = ent.Comp.NodeVertices[index];
		if (val.HasValue)
		{
			NetEntity netUid = val.GetValueOrDefault();
			EntityUid uid = ((EntitySystem)this).GetEntity(netUid);
			node = Entity<XenoArtifactNodeComponent>.op_Implicit((uid, XenoArtifactNode(uid)));
		}
		return node.HasValue;
	}

	public int GetFreeNodeIndex(Entity<XenoArtifactComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		int length = ent.Comp.NodeVertices.Length;
		for (int i = 0; i < length; i++)
		{
			if (!ent.Comp.NodeVertices[i].HasValue)
			{
				return i;
			}
		}
		ResizeNodeGraph(ent, length + 1);
		return length;
	}

	public IEnumerable<Entity<XenoArtifactNodeComponent>> GetAllNodes(Entity<XenoArtifactComponent> ent)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		NetEntity?[] nodeVertices = ent.Comp.NodeVertices;
		EntityUid? node = default(EntityUid?);
		foreach (NetEntity? netNode in nodeVertices)
		{
			if (((EntitySystem)this).TryGetEntity(netNode, ref node))
			{
				yield return Entity<XenoArtifactNodeComponent>.op_Implicit((node.Value, XenoArtifactNode(node.Value)));
			}
		}
	}

	public IEnumerable<int> GetAllNodeIndices(Entity<XenoArtifactComponent> ent)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < ent.Comp.NodeVertices.Length; i++)
		{
			NetEntity? val = ent.Comp.NodeVertices[i];
			if (val.HasValue)
			{
				yield return i;
			}
		}
	}

	public bool AddEdge(Entity<XenoArtifactComponent?> ent, EntityUid from, EntityUid to, bool dirty = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return false;
		}
		if (!TryGetIndex(ent, from, out var fromIdx) || !TryGetIndex(ent, to, out var toIdx))
		{
			return false;
		}
		return AddEdge(ent, fromIdx.Value, toIdx.Value, dirty);
	}

	public bool AddEdge(Entity<XenoArtifactComponent?> ent, int fromIdx, int toIdx, bool dirty = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return false;
		}
		if (ent.Comp.NodeAdjacencyMatrix[fromIdx][toIdx])
		{
			return false;
		}
		ent.Comp.NodeAdjacencyMatrix[fromIdx][toIdx] = true;
		if (dirty)
		{
			RebuildXenoArtifactMetaData(ent);
		}
		return true;
	}

	public bool RemoveEdge(Entity<XenoArtifactComponent?> ent, EntityUid from, EntityUid to, bool dirty = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return false;
		}
		if (!TryGetIndex(ent, from, out var fromIdx) || !TryGetIndex(ent, to, out var toIdx))
		{
			return false;
		}
		return RemoveEdge(ent, fromIdx.Value, toIdx.Value, dirty);
	}

	public bool RemoveEdge(Entity<XenoArtifactComponent?> ent, int fromIdx, int toIdx, bool dirty = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return false;
		}
		if (!ent.Comp.NodeAdjacencyMatrix[fromIdx][toIdx])
		{
			return false;
		}
		ent.Comp.NodeAdjacencyMatrix[fromIdx][toIdx] = false;
		if (dirty)
		{
			RebuildXenoArtifactMetaData(ent);
		}
		return true;
	}

	public bool AddNode(Entity<XenoArtifactComponent?> ent, EntProtoId entProtoId, [NotNullWhen(true)] out Entity<XenoArtifactNodeComponent>? node, bool dirty = true)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		node = null;
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return false;
		}
		EntityUid uid = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(entProtoId), (ComponentRegistry)null, true);
		node = Entity<XenoArtifactNodeComponent>.op_Implicit((uid, XenoArtifactNode(uid)));
		return AddNode(ent, Entity<XenoArtifactNodeComponent>.op_Implicit((Entity<XenoArtifactNodeComponent>.op_Implicit(node.Value), node.Value.Comp)), dirty);
	}

	public bool AddNode(Entity<XenoArtifactComponent?> ent, Entity<XenoArtifactNodeComponent?> node, bool dirty = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return false;
		}
		ref XenoArtifactNodeComponent comp = ref node.Comp;
		if (comp == null)
		{
			comp = XenoArtifactNode(Entity<XenoArtifactNodeComponent>.op_Implicit(node));
		}
		node.Comp.Attached = ((EntitySystem)this).GetNetEntity(Entity<XenoArtifactComponent>.op_Implicit(ent), (MetaDataComponent)null);
		int nodeIdx = GetFreeNodeIndex(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), ent.Comp)));
		_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(node.Owner), (BaseContainer)(object)ent.Comp.NodeContainer, (TransformComponent)null, false);
		ent.Comp.NodeVertices[nodeIdx] = ((EntitySystem)this).GetNetEntity(Entity<XenoArtifactNodeComponent>.op_Implicit(node), (MetaDataComponent)null);
		((EntitySystem)this).Dirty<XenoArtifactNodeComponent>(node, (MetaDataComponent)null);
		if (dirty)
		{
			RebuildXenoArtifactMetaData(ent);
		}
		return true;
	}

	public bool RemoveNode(Entity<XenoArtifactComponent?> ent, Entity<XenoArtifactNodeComponent?> node, bool dirty = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return false;
		}
		ref XenoArtifactNodeComponent comp = ref node.Comp;
		if (comp == null)
		{
			comp = XenoArtifactNode(Entity<XenoArtifactNodeComponent>.op_Implicit(node));
		}
		if (!TryGetIndex(ent, Entity<XenoArtifactNodeComponent>.op_Implicit(node), out var idx))
		{
			return false;
		}
		RemoveAllNodeEdges(ent, idx.Value, dirty: false);
		_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(node.Owner), (BaseContainer)(object)ent.Comp.NodeContainer, true, false, (EntityCoordinates?)null, (Angle?)null);
		node.Comp.Attached = null;
		ent.Comp.NodeVertices[idx.Value] = null;
		if (dirty)
		{
			RebuildXenoArtifactMetaData(ent);
		}
		((EntitySystem)this).Dirty<XenoArtifactNodeComponent>(node, (MetaDataComponent)null);
		return true;
	}

	public void RemoveAllNodeEdges(Entity<XenoArtifactComponent?> ent, int nodeIdx, bool dirty = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return;
		}
		foreach (int p in GetDirectPredecessorNodes(ent, nodeIdx))
		{
			RemoveEdge(ent, p, nodeIdx, dirty: false);
		}
		foreach (int s in GetDirectSuccessorNodes(ent, nodeIdx))
		{
			RemoveEdge(ent, nodeIdx, s, dirty: false);
		}
		if (dirty)
		{
			RebuildXenoArtifactMetaData(ent);
		}
	}

	public HashSet<Entity<XenoArtifactNodeComponent>> GetDirectPredecessorNodes(Entity<XenoArtifactComponent?> ent, EntityUid node)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return new HashSet<Entity<XenoArtifactNodeComponent>>();
		}
		if (!TryGetIndex(ent, node, out var index))
		{
			return new HashSet<Entity<XenoArtifactNodeComponent>>();
		}
		HashSet<int> directPredecessorNodes = GetDirectPredecessorNodes(ent, index.Value);
		HashSet<Entity<XenoArtifactNodeComponent>> output = new HashSet<Entity<XenoArtifactNodeComponent>>();
		foreach (int i in directPredecessorNodes)
		{
			if (TryGetNode(ent, i, out Entity<XenoArtifactNodeComponent>? predecessor))
			{
				output.Add(predecessor.Value);
			}
		}
		return output;
	}

	public HashSet<int> GetDirectPredecessorNodes(Entity<XenoArtifactComponent?> ent, int nodeIdx)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return new HashSet<int>();
		}
		HashSet<int> indices = new HashSet<int>();
		for (int i = 0; i < ent.Comp.NodeAdjacencyMatrixRows; i++)
		{
			if (ent.Comp.NodeAdjacencyMatrix[i][nodeIdx])
			{
				indices.Add(i);
			}
		}
		return indices;
	}

	public HashSet<Entity<XenoArtifactNodeComponent>> GetDirectSuccessorNodes(Entity<XenoArtifactComponent?> ent, EntityUid node)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return new HashSet<Entity<XenoArtifactNodeComponent>>();
		}
		if (!TryGetIndex(ent, node, out var index))
		{
			return new HashSet<Entity<XenoArtifactNodeComponent>>();
		}
		HashSet<int> directSuccessorNodes = GetDirectSuccessorNodes(ent, index.Value);
		HashSet<Entity<XenoArtifactNodeComponent>> output = new HashSet<Entity<XenoArtifactNodeComponent>>();
		foreach (int i in directSuccessorNodes)
		{
			if (TryGetNode(ent, i, out Entity<XenoArtifactNodeComponent>? successor))
			{
				output.Add(successor.Value);
			}
		}
		return output;
	}

	public HashSet<int> GetDirectSuccessorNodes(Entity<XenoArtifactComponent?> ent, int nodeIdx)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return new HashSet<int>();
		}
		HashSet<int> indices = new HashSet<int>();
		for (int i = 0; i < ent.Comp.NodeAdjacencyMatrixColumns; i++)
		{
			if (ent.Comp.NodeAdjacencyMatrix[nodeIdx][i])
			{
				indices.Add(i);
			}
		}
		return indices;
	}

	public HashSet<Entity<XenoArtifactNodeComponent>> GetPredecessorNodes(Entity<XenoArtifactComponent?> ent, Entity<XenoArtifactNodeComponent> node)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return new HashSet<Entity<XenoArtifactNodeComponent>>();
		}
		HashSet<int> predecessorNodes = GetPredecessorNodes(ent, GetIndex(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), ent.Comp)), Entity<XenoArtifactNodeComponent>.op_Implicit(node)));
		HashSet<Entity<XenoArtifactNodeComponent>> output = new HashSet<Entity<XenoArtifactNodeComponent>>();
		foreach (int p in predecessorNodes)
		{
			output.Add(GetNode(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), ent.Comp)), p));
		}
		return output;
	}

	public HashSet<int> GetPredecessorNodes(Entity<XenoArtifactComponent?> ent, int nodeIdx)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return new HashSet<int>();
		}
		HashSet<int> predecessors = GetDirectPredecessorNodes(ent, nodeIdx);
		if (predecessors.Count == 0)
		{
			return new HashSet<int>();
		}
		HashSet<int> output = new HashSet<int>();
		foreach (int p in predecessors)
		{
			output.Add(p);
			foreach (int rp in GetPredecessorNodes(ent, p))
			{
				output.Add(rp);
			}
		}
		return output;
	}

	public HashSet<Entity<XenoArtifactNodeComponent>> GetSuccessorNodes(Entity<XenoArtifactComponent?> ent, Entity<XenoArtifactNodeComponent> node)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return new HashSet<Entity<XenoArtifactNodeComponent>>();
		}
		HashSet<int> successorNodes = GetSuccessorNodes(ent, GetIndex(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), ent.Comp)), Entity<XenoArtifactNodeComponent>.op_Implicit(node)));
		HashSet<Entity<XenoArtifactNodeComponent>> output = new HashSet<Entity<XenoArtifactNodeComponent>>();
		foreach (int s in successorNodes)
		{
			output.Add(GetNode(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), ent.Comp)), s));
		}
		return output;
	}

	public HashSet<int> GetSuccessorNodes(Entity<XenoArtifactComponent?> ent, int nodeIdx)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return new HashSet<int>();
		}
		HashSet<int> successors = GetDirectSuccessorNodes(ent, nodeIdx);
		if (successors.Count == 0)
		{
			return new HashSet<int>();
		}
		HashSet<int> output = new HashSet<int>();
		foreach (int s in successors)
		{
			output.Add(s);
			foreach (int rs in GetSuccessorNodes(ent, s))
			{
				output.Add(rs);
			}
		}
		return output;
	}

	public bool NodeHasEdge(Entity<XenoArtifactComponent?> ent, Entity<XenoArtifactNodeComponent?> from, Entity<XenoArtifactNodeComponent?> to)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return false;
		}
		int fromIdx = GetIndex(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), ent.Comp)), Entity<XenoArtifactNodeComponent>.op_Implicit(from));
		int toIdx = GetIndex(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), ent.Comp)), Entity<XenoArtifactNodeComponent>.op_Implicit(to));
		return ent.Comp.NodeAdjacencyMatrix[fromIdx][toIdx];
	}

	protected void ResizeNodeGraph(Entity<XenoArtifactComponent> ent, int newSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		Array.Resize(ref ent.Comp.NodeVertices, newSize);
		while (ent.Comp.NodeAdjacencyMatrix.Count < newSize)
		{
			ent.Comp.NodeAdjacencyMatrix.Add(new List<bool>());
		}
		foreach (List<bool> row in ent.Comp.NodeAdjacencyMatrix)
		{
			while (row.Count < newSize)
			{
				row.Add(item: false);
			}
		}
		((EntitySystem)this).Dirty<XenoArtifactComponent>(ent, (MetaDataComponent)null);
	}

	private void CancelUnlockingOnGraphStructureChange(Entity<XenoArtifactComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		XenoArtifactUnlockingComponent unlockingComponent = default(XenoArtifactUnlockingComponent);
		if (((EntitySystem)this).TryComp<XenoArtifactUnlockingComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref unlockingComponent))
		{
			Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent> artifactEnt = Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), unlockingComponent, ent.Comp));
			CancelUnlockingState(artifactEnt);
		}
	}

	private void InitializeNode()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).SubscribeLocalEvent<XenoArtifactNodeComponent, MapInitEvent>((EntityEventRefHandler<XenoArtifactNodeComponent, MapInitEvent>)OnNodeMapInit, (Type[])null, (Type[])null);
		_xenoArtifactQuery = ((EntitySystem)this).GetEntityQuery<XenoArtifactComponent>();
		_nodeQuery = ((EntitySystem)this).GetEntityQuery<XenoArtifactNodeComponent>();
	}

	private void OnNodeMapInit(Entity<XenoArtifactNodeComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		XenoArtifactNodeComponent nodeComponent = Entity<XenoArtifactNodeComponent>.op_Implicit(ent);
		nodeComponent.MaxDurability -= nodeComponent.MaxDurabilityCanDecreaseBy.Next(RobustRandom);
		SetNodeDurability(Entity<XenoArtifactNodeComponent>.op_Implicit((Entity<XenoArtifactNodeComponent>.op_Implicit(ent), Entity<XenoArtifactNodeComponent>.op_Implicit(ent))), nodeComponent.MaxDurability);
	}

	public XenoArtifactNodeComponent XenoArtifactNode(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Entity<XenoArtifactNodeComponent>.op_Implicit(_nodeQuery.Get(uid));
	}

	public void SetNodeUnlocked(Entity<XenoArtifactNodeComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactNodeComponent>(Entity<XenoArtifactNodeComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return;
		}
		NetEntity? attached = ent.Comp.Attached;
		if (attached.HasValue)
		{
			NetEntity netArtifact = attached.GetValueOrDefault();
			EntityUid artifact = ((EntitySystem)this).GetEntity(netArtifact);
			XenoArtifactComponent artifactComponent = default(XenoArtifactComponent);
			if (((EntitySystem)this).TryComp<XenoArtifactComponent>(artifact, ref artifactComponent))
			{
				SetNodeUnlocked(Entity<XenoArtifactComponent>.op_Implicit((artifact, artifactComponent)), Entity<XenoArtifactNodeComponent>.op_Implicit((Entity<XenoArtifactNodeComponent>.op_Implicit(ent), ent.Comp)));
			}
		}
	}

	public void SetNodeUnlocked(Entity<XenoArtifactComponent> artifact, Entity<XenoArtifactNodeComponent> node)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (node.Comp.Locked)
		{
			node.Comp.Locked = false;
			RebuildCachedActiveNodes(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(artifact), Entity<XenoArtifactComponent>.op_Implicit(artifact))));
			((EntitySystem)this).Dirty<XenoArtifactNodeComponent>(node, (MetaDataComponent)null);
		}
	}

	public void AdjustNodeDurability(Entity<XenoArtifactNodeComponent?> ent, int durabilityDelta)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<XenoArtifactNodeComponent>(Entity<XenoArtifactNodeComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			SetNodeDurability(ent, ent.Comp.Durability + durabilityDelta);
		}
	}

	public void SetNodeDurability(Entity<XenoArtifactNodeComponent?> ent, int durability)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<XenoArtifactNodeComponent>(Entity<XenoArtifactNodeComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			ent.Comp.Durability = Math.Clamp(durability, 0, ent.Comp.MaxDurability);
			UpdateNodeResearchValue(Entity<XenoArtifactNodeComponent>.op_Implicit((Entity<XenoArtifactNodeComponent>.op_Implicit(ent), ent.Comp)));
			((EntitySystem)this).Dirty<XenoArtifactNodeComponent>(ent, (MetaDataComponent)null);
		}
	}

	public Entity<XenoArtifactNodeComponent> CreateNode(Entity<XenoArtifactComponent> ent, ProtoId<XenoArchTriggerPrototype> trigger, int depth = 0)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		XenoArchTriggerPrototype triggerProto = PrototypeManager.Index<XenoArchTriggerPrototype>(trigger);
		return CreateNode(ent, triggerProto, depth);
	}

	public Entity<XenoArtifactNodeComponent> CreateNode(Entity<XenoArtifactComponent> ent, XenoArchTriggerPrototype trigger, int depth = 0)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		EntProtoId entProtoId = _entityTable.GetSpawns(ent.Comp.EffectsTable).First();
		AddNode(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), Entity<XenoArtifactComponent>.op_Implicit(ent))), entProtoId, out Entity<XenoArtifactNodeComponent>? nodeEnt, dirty: false);
		XenoArtifactNodeComponent comp = nodeEnt.Value.Comp;
		comp.Depth = depth;
		comp.TriggerTip = trigger.Tip;
		base.EntityManager.AddComponents(Entity<XenoArtifactNodeComponent>.op_Implicit(nodeEnt.Value), trigger.Components, true);
		((EntitySystem)this).Dirty<XenoArtifactNodeComponent>(nodeEnt.Value, (MetaDataComponent)null);
		return nodeEnt.Value;
	}

	public bool HasUnlockedPredecessor(Entity<XenoArtifactComponent> ent, EntityUid node)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		HashSet<Entity<XenoArtifactNodeComponent>> predecessors = GetDirectPredecessorNodes(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), Entity<XenoArtifactComponent>.op_Implicit(ent))), node);
		if (predecessors.Count == 0)
		{
			return true;
		}
		foreach (Entity<XenoArtifactNodeComponent> item in predecessors)
		{
			if (item.Comp.Locked)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsNodeActive(Entity<XenoArtifactComponent> ent, EntityUid node)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return ent.Comp.CachedActiveNodes.Contains(((EntitySystem)this).GetNetEntity(node, (MetaDataComponent)null));
	}

	public List<Entity<XenoArtifactNodeComponent>> GetActiveNodes(Entity<XenoArtifactComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return ent.Comp.CachedActiveNodes.Select((NetEntity activeNode) => _nodeQuery.Get(((EntitySystem)this).GetEntity(activeNode))).ToList();
	}

	public int GetResearchValue(Entity<XenoArtifactNodeComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Locked)
		{
			return 0;
		}
		return Math.Max(0, ent.Comp.ResearchValue - ent.Comp.ConsumedResearchValue);
	}

	public void SetConsumedResearchValue(Entity<XenoArtifactNodeComponent> ent, int value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.ConsumedResearchValue = value;
		((EntitySystem)this).Dirty<XenoArtifactNodeComponent>(ent, (MetaDataComponent)null);
	}

	public string GetNodeId(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return (((EntitySystem)this).CompOrNull<NameIdentifierComponent>(uid)?.Identifier ?? 0).ToString("D3");
	}

	public List<List<Entity<XenoArtifactNodeComponent>>> GetSegments(Entity<XenoArtifactComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		List<List<Entity<XenoArtifactNodeComponent>>> output = new List<List<Entity<XenoArtifactNodeComponent>>>();
		foreach (List<NetEntity> cachedSegment in ent.Comp.CachedSegments)
		{
			List<Entity<XenoArtifactNodeComponent>> outSegment = new List<Entity<XenoArtifactNodeComponent>>();
			foreach (NetEntity netNode in cachedSegment)
			{
				EntityUid node = ((EntitySystem)this).GetEntity(netNode);
				outSegment.Add(Entity<XenoArtifactNodeComponent>.op_Implicit((node, XenoArtifactNode(node))));
			}
			output.Add(outSegment);
		}
		return output;
	}

	public Dictionary<int, List<Entity<XenoArtifactNodeComponent>>> GetDepthOrderedNodes(IEnumerable<Entity<XenoArtifactNodeComponent>> nodes)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<int, List<Entity<XenoArtifactNodeComponent>>> nodesByDepth = new Dictionary<int, List<Entity<XenoArtifactNodeComponent>>>();
		foreach (Entity<XenoArtifactNodeComponent> node in nodes)
		{
			if (!nodesByDepth.TryGetValue(node.Comp.Depth, out var depthList))
			{
				depthList = new List<Entity<XenoArtifactNodeComponent>>();
				nodesByDepth.Add(node.Comp.Depth, depthList);
			}
			depthList.Add(node);
		}
		return nodesByDepth;
	}

	public void RebuildXenoArtifactMetaData(Entity<XenoArtifactComponent?> artifact)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(artifact), ref artifact.Comp, true))
		{
			return;
		}
		RebuildCachedActiveNodes(artifact);
		RebuildCachedSegments(artifact);
		foreach (Entity<XenoArtifactNodeComponent> node in GetAllNodes(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(artifact), artifact.Comp))))
		{
			RebuildNodeMetaData(node);
		}
		CancelUnlockingOnGraphStructureChange(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(artifact), artifact.Comp)));
	}

	public void RebuildNodeMetaData(Entity<XenoArtifactNodeComponent> node)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateNodeResearchValue(node);
	}

	public void RebuildCachedActiveNodes(Entity<XenoArtifactComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return;
		}
		ent.Comp.CachedActiveNodes.Clear();
		foreach (Entity<XenoArtifactNodeComponent> node in GetAllNodes(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), ent.Comp))))
		{
			if (node.Comp.Locked)
			{
				continue;
			}
			HashSet<Entity<XenoArtifactNodeComponent>> successors = GetDirectSuccessorNodes(ent, Entity<XenoArtifactNodeComponent>.op_Implicit(node));
			if (successors.Count != 0)
			{
				bool successorIsUnlocked = false;
				foreach (Entity<XenoArtifactNodeComponent> item in successors)
				{
					if (!item.Comp.Locked)
					{
						successorIsUnlocked = true;
						break;
					}
				}
				if (successorIsUnlocked)
				{
					continue;
				}
			}
			NetEntity netEntity = ((EntitySystem)this).GetNetEntity(Entity<XenoArtifactNodeComponent>.op_Implicit(node), (MetaDataComponent)null);
			ent.Comp.CachedActiveNodes.Add(netEntity);
		}
		((EntitySystem)this).Dirty<XenoArtifactComponent>(ent, (MetaDataComponent)null);
	}

	public void RebuildCachedSegments(Entity<XenoArtifactComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return;
		}
		ent.Comp.CachedSegments.Clear();
		List<Entity<XenoArtifactNodeComponent>> entities = GetAllNodes(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), ent.Comp))).ToList();
		IEnumerable<List<NetEntity>> netEntities = from s in GetSegmentsFromNodes(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), ent.Comp)), entities)
			select s.Select((Entity<XenoArtifactNodeComponent> n) => ((EntitySystem)this).GetNetEntity(Entity<XenoArtifactNodeComponent>.op_Implicit(n), (MetaDataComponent)null)).ToList();
		ent.Comp.CachedSegments.AddRange(netEntities);
		((EntitySystem)this).Dirty<XenoArtifactComponent>(ent, (MetaDataComponent)null);
	}

	public IEnumerable<List<Entity<XenoArtifactNodeComponent>>> GetSegmentsFromNodes(Entity<XenoArtifactComponent> ent, List<Entity<XenoArtifactNodeComponent>> nodes)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		List<List<Entity<XenoArtifactNodeComponent>>> outSegments = new List<List<Entity<XenoArtifactNodeComponent>>>();
		foreach (Entity<XenoArtifactNodeComponent> node in nodes)
		{
			List<Entity<XenoArtifactNodeComponent>> segment = new List<Entity<XenoArtifactNodeComponent>>();
			GetSegmentNodesRecursive(ent, node, segment, outSegments);
			if (segment.Count != 0)
			{
				outSegments.Add(segment);
			}
		}
		return outSegments;
	}

	private void GetSegmentNodesRecursive(Entity<XenoArtifactComponent> ent, Entity<XenoArtifactNodeComponent> node, List<Entity<XenoArtifactNodeComponent>> segment, List<List<Entity<XenoArtifactNodeComponent>>> otherSegments)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if (otherSegments.Any((List<Entity<XenoArtifactNodeComponent>> list) => list.Contains(node)) || segment.Contains(node))
		{
			return;
		}
		segment.Add(node);
		foreach (Entity<XenoArtifactNodeComponent> p in GetDirectPredecessorNodes(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), Entity<XenoArtifactComponent>.op_Implicit(ent))), Entity<XenoArtifactNodeComponent>.op_Implicit(node)))
		{
			GetSegmentNodesRecursive(ent, p, segment, otherSegments);
		}
		foreach (Entity<XenoArtifactNodeComponent> s in GetDirectSuccessorNodes(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), Entity<XenoArtifactComponent>.op_Implicit(ent))), Entity<XenoArtifactNodeComponent>.op_Implicit(node)))
		{
			GetSegmentNodesRecursive(ent, s, segment, otherSegments);
		}
	}

	public void UpdateNodeResearchValue(Entity<XenoArtifactNodeComponent> node)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		XenoArtifactNodeComponent nodeComponent = Entity<XenoArtifactNodeComponent>.op_Implicit(node);
		if (!nodeComponent.Attached.HasValue)
		{
			nodeComponent.ResearchValue = 0;
			return;
		}
		Entity<XenoArtifactComponent> artifact = _xenoArtifactQuery.Get(((EntitySystem)this).GetEntity(nodeComponent.Attached.Value));
		List<Entity<XenoArtifactNodeComponent>> activeNodes = GetActiveNodes(artifact);
		float durabilityEffect = MathF.Pow((float)nodeComponent.Durability / (float)nodeComponent.MaxDurability, 2f);
		float durabilityMultiplier = (activeNodes.Contains(node) ? (1f - durabilityEffect) : (1f + durabilityEffect));
		HashSet<Entity<XenoArtifactNodeComponent>> predecessorNodes = GetPredecessorNodes(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(artifact), Entity<XenoArtifactComponent>.op_Implicit(artifact))), node);
		nodeComponent.ResearchValue = (int)(Math.Pow(1.25, Math.Pow(predecessorNodes.Count, 1.5)) * (double)nodeComponent.BasePointValue * (double)durabilityMultiplier);
	}

	private void InitializeUnlock()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_unlockingQuery = ((EntitySystem)this).GetEntityQuery<XenoArtifactUnlockingComponent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoArtifactUnlockingComponent, MapInitEvent>((EntityEventRefHandler<XenoArtifactUnlockingComponent, MapInitEvent>)OnUnlockingStarted, (Type[])null, (Type[])null);
	}

	private void UpdateUnlock(float _)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<XenoArtifactUnlockingComponent, XenoArtifactComponent> query = ((EntitySystem)this).EntityQueryEnumerator<XenoArtifactUnlockingComponent, XenoArtifactComponent>();
		EntityUid uid = default(EntityUid);
		XenoArtifactUnlockingComponent unlock = default(XenoArtifactUnlockingComponent);
		XenoArtifactComponent comp = default(XenoArtifactComponent);
		while (query.MoveNext(ref uid, ref unlock, ref comp))
		{
			if (!(_timing.CurTime < unlock.EndTime))
			{
				FinishUnlockingState(Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent>.op_Implicit((uid, unlock, comp)));
			}
		}
	}

	public bool CanUnlockNode(Entity<XenoArtifactNodeComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoArtifactNodeComponent>(Entity<XenoArtifactNodeComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return false;
		}
		EntityUid? artifact = ((EntitySystem)this).GetEntity(ent.Comp.Attached);
		XenoArtifactComponent artiComp = default(XenoArtifactComponent);
		if (!((EntitySystem)this).TryComp<XenoArtifactComponent>(artifact, ref artiComp))
		{
			return false;
		}
		if (artiComp.Suppressed)
		{
			return false;
		}
		if (!HasUnlockedPredecessor(Entity<XenoArtifactComponent>.op_Implicit((artifact.Value, artiComp)), Entity<XenoArtifactNodeComponent>.op_Implicit(ent)) || (!ent.Comp.Locked && GetSuccessorNodes(Entity<XenoArtifactComponent>.op_Implicit((artifact.Value, artiComp)), Entity<XenoArtifactNodeComponent>.op_Implicit((ent.Owner, ent.Comp))).Count == 0))
		{
			return false;
		}
		return true;
	}

	public void FinishUnlockingState(Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		XenoArtifactComponent artifactComponent = Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent>.op_Implicit(ent);
		XenoArtifactUnlockingComponent unlockingComponent = Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent>.op_Implicit(ent);
		string unlockAttemptResultMsg;
		SoundSpecifier soundEffect;
		if (TryGetNodeFromUnlockState(ent, out Entity<XenoArtifactNodeComponent>? node))
		{
			SetNodeUnlocked(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent>.op_Implicit(ent), artifactComponent)), node.Value);
			ActivateNode(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent>.op_Implicit(ent), Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent>.op_Implicit(ent))), Entity<XenoArtifactNodeComponent>.op_Implicit((Entity<XenoArtifactNodeComponent>.op_Implicit(node.Value), Entity<XenoArtifactNodeComponent>.op_Implicit(node.Value))), null, null, ((EntitySystem)this).Transform(Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent>.op_Implicit(ent)).Coordinates, consumeDurability: false);
			unlockAttemptResultMsg = "artifact-unlock-state-end-success";
			soundEffect = unlockingComponent.UnlockActivationSuccessfulSound;
		}
		else
		{
			unlockAttemptResultMsg = "artifact-unlock-state-end-failure";
			soundEffect = unlockingComponent.UnlockActivationFailedSound;
		}
		if (_net.IsServer)
		{
			_popup.PopupEntity(base.Loc.GetString(unlockAttemptResultMsg), Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent>.op_Implicit(ent));
			_audio.PlayPvs(soundEffect, ent.Owner, (AudioParams?)null);
		}
		((EntitySystem)this).RemComp(Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent>.op_Implicit(ent), (IComponent)(object)unlockingComponent);
		RaiseUnlockingFinished(ent, node);
		artifactComponent.NextUnlockTime = _timing.CurTime + artifactComponent.UnlockStateRefractory;
	}

	public void CancelUnlockingState(Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemComp(Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp1);
		RaiseUnlockingFinished(ent, null);
	}

	public bool TryGetNodeFromUnlockState(Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent> ent, [NotNullWhen(true)] out Entity<XenoArtifactNodeComponent>? node)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		node = null;
		ValueList<Entity<XenoArtifactNodeComponent>> potentialNodes = default(ValueList<Entity<XenoArtifactNodeComponent>>);
		XenoArtifactUnlockingComponent artifactUnlockingComponent = ent.Comp1;
		foreach (int nodeIndex in GetAllNodeIndices(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent>.op_Implicit(ent), Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent>.op_Implicit(ent)))))
		{
			XenoArtifactComponent artifactComponent = ent.Comp2;
			Entity<XenoArtifactNodeComponent> curNode = GetNode(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent>.op_Implicit(ent), artifactComponent)), nodeIndex);
			if (!curNode.Comp.Locked || !CanUnlockNode(Entity<XenoArtifactNodeComponent>.op_Implicit((Entity<XenoArtifactNodeComponent>.op_Implicit(curNode), Entity<XenoArtifactNodeComponent>.op_Implicit(curNode)))))
			{
				continue;
			}
			HashSet<int> requiredIndices = GetPredecessorNodes(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent>.op_Implicit(ent), artifactComponent)), nodeIndex);
			requiredIndices.Add(nodeIndex);
			if (!ent.Comp1.ArtifexiumApplied)
			{
				if (requiredIndices.Count == artifactUnlockingComponent.TriggeredNodeIndexes.Count && artifactUnlockingComponent.TriggeredNodeIndexes.All(requiredIndices.Contains))
				{
					node = curNode;
					return true;
				}
			}
			else if (artifactUnlockingComponent.TriggeredNodeIndexes.All(requiredIndices.Contains) && requiredIndices.Count - 1 == artifactUnlockingComponent.TriggeredNodeIndexes.Count)
			{
				potentialNodes.Add(curNode);
			}
		}
		if (potentialNodes.Count != 0)
		{
			node = RandomExtensions.Pick<Entity<XenoArtifactNodeComponent>>(RobustRandom, potentialNodes);
		}
		return node.HasValue;
	}

	private void OnUnlockingStarted(Entity<XenoArtifactUnlockingComponent> ent, ref MapInitEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		ArtifactUnlockingStartedEvent unlockingStartedEvent = default(ArtifactUnlockingStartedEvent);
		((EntitySystem)this).RaiseLocalEvent<ArtifactUnlockingStartedEvent>(ent.Owner, ref unlockingStartedEvent, false);
	}

	private void RaiseUnlockingFinished(Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent> ent, Entity<XenoArtifactNodeComponent>? node)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Entity<XenoArtifactNodeComponent>? val = node;
		ArtifactUnlockingFinishedEvent unlockingFinishedEvent = new ArtifactUnlockingFinishedEvent(val.HasValue ? new EntityUid?(Entity<XenoArtifactNodeComponent>.op_Implicit(val.GetValueOrDefault())) : ((EntityUid?)null));
		((EntitySystem)this).RaiseLocalEvent<ArtifactUnlockingFinishedEvent>(ent.Owner, ref unlockingFinishedEvent, false);
	}

	private void InitializeXAE()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoArtifactComponent, UseInHandEvent>((EntityEventRefHandler<XenoArtifactComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoArtifactComponent, AfterInteractEvent>((EntityEventRefHandler<XenoArtifactComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoArtifactComponent, ActivateInWorldEvent>((EntityEventRefHandler<XenoArtifactComponent, ActivateInWorldEvent>)OnActivateInWorld, (Type[])null, (Type[])null);
	}

	private void OnUseInHand(Entity<XenoArtifactComponent> ent, ref UseInHandEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = TryActivateXenoArtifact(ent, args.User, args.User, ((EntitySystem)this).Transform(args.User).Coordinates);
		}
	}

	private void OnAfterInteract(Entity<XenoArtifactComponent> ent, ref AfterInteractEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.CanReach)
		{
			((HandledEntityEventArgs)args).Handled = TryActivateXenoArtifact(ent, args.User, args.Target, args.ClickLocation);
		}
	}

	private void OnActivateInWorld(Entity<XenoArtifactComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Complex)
		{
			((HandledEntityEventArgs)args).Handled = TryActivateXenoArtifact(ent, args.User, args.Target, ((EntitySystem)this).Transform(args.Target).Coordinates);
		}
	}

	public bool TryActivateXenoArtifact(Entity<XenoArtifactComponent> artifact, EntityUid? user, EntityUid? target, EntityCoordinates coordinates, bool consumeDurability = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		XenoArtifactComponent xenoArtifactComponent = Entity<XenoArtifactComponent>.op_Implicit(artifact);
		if (xenoArtifactComponent.Suppressed)
		{
			return false;
		}
		UseDelayComponent delay = default(UseDelayComponent);
		if (((EntitySystem)this).TryComp<UseDelayComponent>(Entity<XenoArtifactComponent>.op_Implicit(artifact), ref delay) && !_useDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(artifact), delay)), checkDelayed: true))
		{
			return false;
		}
		bool success = false;
		foreach (Entity<XenoArtifactNodeComponent> node in GetActiveNodes(artifact))
		{
			success |= ActivateNode(artifact, node, user, target, coordinates, consumeDurability);
		}
		if (!success)
		{
			_popup.PopupClient(base.Loc.GetString("artifact-activation-fail"), Entity<XenoArtifactComponent>.op_Implicit(artifact), user);
			return false;
		}
		XenoArtifactActivatedEvent ev = new XenoArtifactActivatedEvent(artifact, user, target, coordinates);
		((EntitySystem)this).RaiseLocalEvent<XenoArtifactActivatedEvent>(Entity<XenoArtifactComponent>.op_Implicit(artifact), ref ev, false);
		if (user.HasValue)
		{
			_audio.PlayPredicted(xenoArtifactComponent.ForceActivationSoundSpecifier, Entity<XenoArtifactComponent>.op_Implicit(artifact), user, (AudioParams?)null);
		}
		else
		{
			_audio.PlayPvs(xenoArtifactComponent.ForceActivationSoundSpecifier, Entity<XenoArtifactComponent>.op_Implicit(artifact), (AudioParams?)null);
		}
		return true;
	}

	public bool ActivateNode(Entity<XenoArtifactComponent> artifact, Entity<XenoArtifactNodeComponent> node, EntityUid? user, EntityUid? target, EntityCoordinates coordinates, bool consumeDurability = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (node.Comp.Degraded)
		{
			return false;
		}
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(24, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(artifact.Owner)), "ToPrettyString(artifact.Owner)");
		handler.AppendLiteral(" node ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoArtifactNodeComponent>.op_Implicit(node), (MetaDataComponent)null), "ToPrettyString(node)");
		handler.AppendLiteral(" got activated at ");
		handler.AppendFormatted<EntityCoordinates>(coordinates, "coordinates");
		adminLogger.Add(LogType.ArtifactNode, LogImpact.Low, ref handler);
		if (consumeDurability)
		{
			AdjustNodeDurability(Entity<XenoArtifactNodeComponent>.op_Implicit((Entity<XenoArtifactNodeComponent>.op_Implicit(node), node.Comp)), -1);
		}
		XenoArtifactNodeActivatedEvent ev = new XenoArtifactNodeActivatedEvent(artifact, node, user, target, coordinates);
		((EntitySystem)this).RaiseLocalEvent<XenoArtifactNodeActivatedEvent>(Entity<XenoArtifactNodeComponent>.op_Implicit(node), ref ev, false);
		return true;
	}

	private void InitializeXAT()
	{
		XATRelayLocalEvent<DamageChangedEvent>();
		XATRelayLocalEvent<InteractUsingEvent>();
		XATRelayLocalEvent<PullStartedMessage>();
		XATRelayLocalEvent<AttackedEvent>();
		XATRelayLocalEvent<XATToolUseDoAfterEvent>();
		XATRelayLocalEvent<InteractHandEvent>();
		XATRelayLocalEvent<ReactionEntityEvent>();
		XATRelayLocalEvent<LandEvent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoArtifactComponent, ExaminedEvent>((EntityEventRefHandler<XenoArtifactComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	protected void XATRelayLocalEvent<T>() where T : notnull
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoArtifactComponent, T>((EntityEventRefHandler<XenoArtifactComponent, T>)RelayEventToNodes, (Type[])null, (Type[])null);
	}

	private void OnExamined(Entity<XenoArtifactComponent> ent, ref ExaminedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("XenoArtifactComponent"))
		{
			RelayEventToNodes(ent, ref args);
		}
	}

	protected void RelayEventToNodes<T>(Entity<XenoArtifactComponent> ent, ref T args) where T : notnull
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		XenoArchNodeRelayedEvent<T> ev = new XenoArchNodeRelayedEvent<T>(ent, args);
		foreach (Entity<XenoArtifactNodeComponent> node in GetAllNodes(ent))
		{
			((EntitySystem)this).RaiseLocalEvent<XenoArchNodeRelayedEvent<T>>(Entity<XenoArtifactNodeComponent>.op_Implicit(node), ref ev, false);
		}
	}

	public void TriggerXenoArtifact(Entity<XenoArtifactComponent> ent, Entity<XenoArtifactNodeComponent>? node, bool force = false)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		if (!force && _timing.CurTime < ent.Comp.NextUnlockTime)
		{
			return;
		}
		XenoArtifactUnlockingComponent unlockingComp = default(XenoArtifactUnlockingComponent);
		if (!_unlockingQuery.TryGetComponent(Entity<XenoArtifactComponent>.op_Implicit(ent), ref unlockingComp))
		{
			unlockingComp = ((EntitySystem)this).EnsureComp<XenoArtifactUnlockingComponent>(Entity<XenoArtifactComponent>.op_Implicit(ent));
			unlockingComp.EndTime = _timing.CurTime + ent.Comp.UnlockStateDuration;
			((EntitySystem)this).Log.Debug($"{((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoArtifactComponent>.op_Implicit(ent), (MetaDataComponent)null)} entered unlocking state");
			if (_net.IsServer)
			{
				_popup.PopupEntity(base.Loc.GetString("artifact-unlock-state-begin"), Entity<XenoArtifactComponent>.op_Implicit(ent));
			}
			((EntitySystem)this).Dirty<XenoArtifactComponent>(ent, (MetaDataComponent)null);
		}
		else if (node.HasValue)
		{
			int index = GetIndex(ent, Entity<XenoArtifactNodeComponent>.op_Implicit(node.Value));
			HashSet<int> predecessorNodeIndices = GetPredecessorNodes(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), Entity<XenoArtifactComponent>.op_Implicit(ent))), index);
			HashSet<int> successorNodeIndices = GetSuccessorNodes(Entity<XenoArtifactComponent>.op_Implicit((Entity<XenoArtifactComponent>.op_Implicit(ent), Entity<XenoArtifactComponent>.op_Implicit(ent))), index);
			if (unlockingComp.TriggeredNodeIndexes.Count == 0 || unlockingComp.TriggeredNodeIndexes.All((int x) => predecessorNodeIndices.Contains(x) || successorNodeIndices.Contains(x)))
			{
				unlockingComp.EndTime += ent.Comp.UnlockStateIncrementPerNode;
			}
		}
		if (node.HasValue && unlockingComp.TriggeredNodeIndexes.Add(GetIndex(ent, Entity<XenoArtifactNodeComponent>.op_Implicit(node.Value))))
		{
			((EntitySystem)this).Dirty(Entity<XenoArtifactComponent>.op_Implicit(ent), (IComponent)(object)unlockingComp, (MetaDataComponent)null);
		}
	}

	public void SetArtifexiumApplied(Entity<XenoArtifactUnlockingComponent> ent, bool val)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.ArtifexiumApplied = val;
		((EntitySystem)this).Dirty<XenoArtifactUnlockingComponent>(ent, (MetaDataComponent)null);
	}
}
