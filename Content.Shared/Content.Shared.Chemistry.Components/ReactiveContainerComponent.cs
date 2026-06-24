using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
public sealed class ReactiveContainerComponent : Component, ISerializationGenerated<ReactiveContainerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string Container;

	[DataField(null, false, 1, true, false, null)]
	public string Solution;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ReactiveContainerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ReactiveContainerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ReactiveContainerComponent>(this, ref target, hookCtx, false, context))
		{
			string ContainerTemp = null;
			if (Container == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Container, ref ContainerTemp, hookCtx, false, context))
			{
				ContainerTemp = Container;
			}
			target.Container = ContainerTemp;
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
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ReactiveContainerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReactiveContainerComponent cast = (ReactiveContainerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReactiveContainerComponent cast = (ReactiveContainerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReactiveContainerComponent def = (ReactiveContainerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ReactiveContainerComponent Instantiate()
	{
		return new ReactiveContainerComponent();
	}
}
