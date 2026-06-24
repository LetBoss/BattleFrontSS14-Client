using System;
using System.Collections.Generic;
using Content.Shared.Examine;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Construction.Steps;

[Serializable]
[ImplicitDataDefinitionForInheritors]
public abstract class ConstructionGraphStep : ISerializationGenerated<ConstructionGraphStep>, ISerializationGenerated
{
	[DataField("completed", false, 1, false, true, null)]
	private IGraphAction[] _completed = Array.Empty<IGraphAction>();

	[DataField("doAfter", false, 1, false, false, null)]
	public float DoAfter { get; private set; }

	public IReadOnlyList<IGraphAction> Completed => _completed;

	public abstract void DoExamine(ExaminedEvent examinedEvent);

	public abstract ConstructionGuideEntry GenerateGuideEntry();

	public ConstructionGraphStep()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref ConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<ConstructionGraphStep>(this, ref target, hookCtx, false, context))
		{
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
			float DoAfterTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DoAfter, ref DoAfterTemp, hookCtx, false, context))
			{
				DoAfterTemp = DoAfter;
			}
			target.DoAfter = DoAfterTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref ConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ConstructionGraphStep cast = (ConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual ConstructionGraphStep Instantiate()
	{
		throw new NotImplementedException();
	}
}
