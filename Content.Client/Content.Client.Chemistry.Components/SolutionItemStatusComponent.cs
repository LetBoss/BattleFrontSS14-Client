using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Client.Chemistry.Components;

[RegisterComponent]
public sealed class SolutionItemStatusComponent : Component, ISerializationGenerated<SolutionItemStatusComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string Solution = "default";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SolutionItemStatusComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (SolutionItemStatusComponent)(object)val;
		if (!serialization.TryCustomCopy<SolutionItemStatusComponent>(this, ref target, hookCtx, false, context))
		{
			string solution = null;
			if (Solution == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Solution, ref solution, hookCtx, false, context))
			{
				solution = Solution;
			}
			target.Solution = solution;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SolutionItemStatusComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SolutionItemStatusComponent target2 = (SolutionItemStatusComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SolutionItemStatusComponent target2 = (SolutionItemStatusComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SolutionItemStatusComponent target2 = (SolutionItemStatusComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SolutionItemStatusComponent Instantiate()
	{
		return new SolutionItemStatusComponent();
	}
}
