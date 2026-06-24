using System;
using System.Collections.Generic;
using Content.Shared.NodeContainer.NodeGroups;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.NodeContainer;

[ImplicitDataDefinitionForInheritors]
public abstract class Node : ISerializationGenerated<Node>, ISerializationGenerated
{
	[ViewVariables]
	public INodeGroup? NodeGroup;

	public bool Deleting;

	public readonly HashSet<Node> ReachableNodes = new HashSet<Node>();

	public int FloodGen;

	public int UndirectGen;

	public bool FlaggedForFlood;

	public int NetId;

	public string Name;

	[DataField("nodeGroupID", false, 1, false, false, null)]
	public NodeGroupID NodeGroupID { get; private set; }

	[ViewVariables]
	public EntityUid Owner { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public bool NeedAnchored { get; private set; } = true;

	public virtual bool Connectable(IEntityManager entMan, TransformComponent? xform = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (Deleting)
		{
			return false;
		}
		if (entMan.IsQueuedForDeletion(Owner))
		{
			return false;
		}
		if (!NeedAnchored)
		{
			return true;
		}
		if (xform == null)
		{
			xform = entMan.GetComponent<TransformComponent>(Owner);
		}
		return xform.Anchored;
	}

	public virtual void OnAnchorStateChanged(IEntityManager entityManager, bool anchored)
	{
	}

	public virtual void Initialize(EntityUid owner, IEntityManager entMan)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Owner = owner;
	}

	public abstract IEnumerable<Node> GetReachableNodes(TransformComponent xform, EntityQuery<NodeContainerComponent> nodeQuery, EntityQuery<TransformComponent> xformQuery, MapGridComponent? grid, IEntityManager entMan);

	public Node()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref Node target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<Node>(this, ref target, hookCtx, false, context))
		{
			NodeGroupID NodeGroupIDTemp = NodeGroupID.Default;
			if (!serialization.TryCustomCopy<NodeGroupID>(NodeGroupID, ref NodeGroupIDTemp, hookCtx, false, context))
			{
				NodeGroupIDTemp = NodeGroupID;
			}
			target.NodeGroupID = NodeGroupIDTemp;
			bool NeedAnchoredTemp = false;
			if (!serialization.TryCustomCopy<bool>(NeedAnchored, ref NeedAnchoredTemp, hookCtx, false, context))
			{
				NeedAnchoredTemp = NeedAnchored;
			}
			target.NeedAnchored = NeedAnchoredTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref Node target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Node cast = (Node)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual Node Instantiate()
	{
		throw new NotImplementedException();
	}
}
