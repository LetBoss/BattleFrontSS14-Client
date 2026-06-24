using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Examine;

[RegisterComponent]
public sealed class ExaminerComponent : Component, ISerializationGenerated<ExaminerComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("skipChecks", false, 1, false, false, null)]
	public bool SkipChecks;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("checkInRangeUnOccluded", false, 1, false, false, null)]
	public bool CheckInRangeUnOccluded = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ExaminerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ExaminerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ExaminerComponent>(this, ref target, hookCtx, false, context))
		{
			bool SkipChecksTemp = false;
			if (!serialization.TryCustomCopy<bool>(SkipChecks, ref SkipChecksTemp, hookCtx, false, context))
			{
				SkipChecksTemp = SkipChecks;
			}
			target.SkipChecks = SkipChecksTemp;
			bool CheckInRangeUnOccludedTemp = false;
			if (!serialization.TryCustomCopy<bool>(CheckInRangeUnOccluded, ref CheckInRangeUnOccludedTemp, hookCtx, false, context))
			{
				CheckInRangeUnOccludedTemp = CheckInRangeUnOccluded;
			}
			target.CheckInRangeUnOccluded = CheckInRangeUnOccludedTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ExaminerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExaminerComponent cast = (ExaminerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExaminerComponent cast = (ExaminerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExaminerComponent def = (ExaminerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ExaminerComponent Instantiate()
	{
		return new ExaminerComponent();
	}
}
