using System;
using System.Collections.Generic;
using Content.Shared.Construction.Steps;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Construction;

[Serializable]
[DataDefinition]
public sealed class ConstructionGraphEdge : ISerializationGenerated<ConstructionGraphEdge>, ISerializationGenerated
{
	[DataField("steps", false, 1, false, false, null)]
	private ConstructionGraphStep[] _steps = Array.Empty<ConstructionGraphStep>();

	[DataField("conditions", false, 1, false, true, null)]
	private IGraphCondition[] _conditions = Array.Empty<IGraphCondition>();

	[DataField("completed", false, 1, false, true, null)]
	private IGraphAction[] _completed = Array.Empty<IGraphAction>();

	[DataField("to", false, 1, true, false, null)]
	public string Target { get; private set; } = string.Empty;

	[ViewVariables]
	public IReadOnlyList<IGraphCondition> Conditions => _conditions;

	[ViewVariables]
	public IReadOnlyList<IGraphAction> Completed => _completed;

	[ViewVariables]
	public IReadOnlyList<ConstructionGraphStep> Steps => _steps;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ConstructionGraphEdge target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<ConstructionGraphEdge>(this, ref target, hookCtx, false, context))
		{
			ConstructionGraphStep[] _stepsTemp = null;
			if (_steps == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<ConstructionGraphStep[]>(_steps, ref _stepsTemp, hookCtx, true, context))
			{
				_stepsTemp = serialization.CreateCopy<ConstructionGraphStep[]>(_steps, hookCtx, context, false);
			}
			target._steps = _stepsTemp;
			IGraphCondition[] _conditionsTemp = null;
			if (_conditions == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<IGraphCondition[]>(_conditions, ref _conditionsTemp, hookCtx, true, context))
			{
				_conditionsTemp = serialization.CreateCopy<IGraphCondition[]>(_conditions, hookCtx, context, false);
			}
			target._conditions = _conditionsTemp;
			IGraphAction[] _completedTemp = null;
			if (_completed == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<IGraphAction[]>(_completed, ref _completedTemp, hookCtx, true, context))
			{
				_completedTemp = serialization.CreateCopy<IGraphAction[]>(_completed, hookCtx, context, false);
			}
			target._completed = _completedTemp;
			string TargetTemp = null;
			if (Target == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Target, ref TargetTemp, hookCtx, false, context))
			{
				TargetTemp = Target;
			}
			target.Target = TargetTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ConstructionGraphEdge target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ConstructionGraphEdge cast = (ConstructionGraphEdge)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ConstructionGraphEdge Instantiate()
	{
		return new ConstructionGraphEdge();
	}
}
