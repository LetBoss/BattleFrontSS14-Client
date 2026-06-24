using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class DumpableSolutionComponent : Component, ISerializationGenerated<DumpableSolutionComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string Solution = "default";

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool Unlimited;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DumpableSolutionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DumpableSolutionComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<DumpableSolutionComponent>(this, ref target, hookCtx, false, context))
		{
			string SolutionTemp = null;
			if (Solution == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Solution, ref SolutionTemp, hookCtx, false, context))
			{
				SolutionTemp = Solution;
			}
			target.Solution = SolutionTemp;
			bool UnlimitedTemp = false;
			if (!serialization.TryCustomCopy<bool>(Unlimited, ref UnlimitedTemp, hookCtx, false, context))
			{
				UnlimitedTemp = Unlimited;
			}
			target.Unlimited = UnlimitedTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DumpableSolutionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DumpableSolutionComponent cast = (DumpableSolutionComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DumpableSolutionComponent cast = (DumpableSolutionComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DumpableSolutionComponent def = (DumpableSolutionComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DumpableSolutionComponent Instantiate()
	{
		return new DumpableSolutionComponent();
	}
}
