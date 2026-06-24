using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class SolutionComponent : Component, ISerializationGenerated<SolutionComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Solution Solution = new Solution();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SolutionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SolutionComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<SolutionComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		Solution SolutionTemp = null;
		if (Solution == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Solution>(Solution, ref SolutionTemp, hookCtx, true, context))
		{
			if (Solution == null)
			{
				SolutionTemp = null;
			}
			else
			{
				serialization.CopyTo<Solution>(Solution, ref SolutionTemp, hookCtx, context, true);
			}
		}
		target.Solution = SolutionTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SolutionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SolutionComponent cast = (SolutionComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SolutionComponent cast = (SolutionComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SolutionComponent def = (SolutionComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SolutionComponent Instantiate()
	{
		return new SolutionComponent();
	}
}
