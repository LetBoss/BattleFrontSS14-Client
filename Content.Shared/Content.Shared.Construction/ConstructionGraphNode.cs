using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Construction.NodeEntities;
using Content.Shared.Construction.Serialization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Construction;

[Serializable]
[DataDefinition]
public sealed class ConstructionGraphNode : ISerializationGenerated<ConstructionGraphNode>, ISerializationGenerated
{
	[DataField("actions", false, 1, false, true, null)]
	private IGraphAction[] _actions = Array.Empty<IGraphAction>();

	[DataField("edges", false, 1, false, false, null)]
	private ConstructionGraphEdge[] _edges = Array.Empty<ConstructionGraphEdge>();

	[DataField("transform", false, 1, false, false, null)]
	public IGraphTransform[] TransformLogic = Array.Empty<IGraphTransform>();

	[DataField("doNotReplaceInheritingEntities", false, 1, false, false, null)]
	public bool DoNotReplaceInheritingEntities;

	[DataField("node", false, 1, true, false, null)]
	public string Name { get; private set; }

	[ViewVariables]
	public IReadOnlyList<ConstructionGraphEdge> Edges => _edges;

	[ViewVariables]
	public IReadOnlyList<IGraphAction> Actions => _actions;

	[DataField("entity", false, 1, false, false, typeof(GraphNodeEntitySerializer))]
	public IGraphNodeEntity Entity { get; private set; } = new NullNodeEntity();

	public ConstructionGraphEdge? GetEdge(string target)
	{
		ConstructionGraphEdge[] edges = _edges;
		foreach (ConstructionGraphEdge edge in edges)
		{
			if (edge.Target == target)
			{
				return edge;
			}
		}
		return null;
	}

	public int? GetEdgeIndex(string target)
	{
		for (int i = 0; i < _edges.Length; i++)
		{
			if (_edges[i].Target == target)
			{
				return i;
			}
		}
		return null;
	}

	public bool TryGetEdge(string target, [NotNullWhen(true)] out ConstructionGraphEdge? edge)
	{
		return (edge = GetEdge(target)) != null;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ConstructionGraphNode target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<ConstructionGraphNode>(this, ref target, hookCtx, false, context))
		{
			IGraphAction[] _actionsTemp = null;
			if (_actions == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<IGraphAction[]>(_actions, ref _actionsTemp, hookCtx, true, context))
			{
				_actionsTemp = serialization.CreateCopy<IGraphAction[]>(_actions, hookCtx, context, false);
			}
			target._actions = _actionsTemp;
			ConstructionGraphEdge[] _edgesTemp = null;
			if (_edges == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<ConstructionGraphEdge[]>(_edges, ref _edgesTemp, hookCtx, true, context))
			{
				_edgesTemp = serialization.CreateCopy<ConstructionGraphEdge[]>(_edges, hookCtx, context, false);
			}
			target._edges = _edgesTemp;
			string NameTemp = null;
			if (Name == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
			{
				NameTemp = Name;
			}
			target.Name = NameTemp;
			IGraphTransform[] TransformLogicTemp = null;
			if (TransformLogic == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<IGraphTransform[]>(TransformLogic, ref TransformLogicTemp, hookCtx, true, context))
			{
				TransformLogicTemp = serialization.CreateCopy<IGraphTransform[]>(TransformLogic, hookCtx, context, false);
			}
			target.TransformLogic = TransformLogicTemp;
			IGraphNodeEntity EntityTemp = null;
			if (Entity == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<IGraphNodeEntity>(Entity, ref EntityTemp, hookCtx, true, context))
			{
				EntityTemp = serialization.CreateCopy<IGraphNodeEntity>(Entity, hookCtx, context, false);
			}
			target.Entity = EntityTemp;
			bool DoNotReplaceInheritingEntitiesTemp = false;
			if (!serialization.TryCustomCopy<bool>(DoNotReplaceInheritingEntities, ref DoNotReplaceInheritingEntitiesTemp, hookCtx, false, context))
			{
				DoNotReplaceInheritingEntitiesTemp = DoNotReplaceInheritingEntities;
			}
			target.DoNotReplaceInheritingEntities = DoNotReplaceInheritingEntitiesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ConstructionGraphNode target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ConstructionGraphNode cast = (ConstructionGraphNode)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ConstructionGraphNode Instantiate()
	{
		return new ConstructionGraphNode();
	}
}
