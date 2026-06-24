using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.CartridgeLoader.Cartridges;

[RegisterComponent]
public sealed class NanoTaskPrintedComponent : Component, ISerializationGenerated<NanoTaskPrintedComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public NanoTaskItem? Task;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NanoTaskPrintedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (NanoTaskPrintedComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<NanoTaskPrintedComponent>(this, ref target, hookCtx, false, context))
		{
			NanoTaskItem TaskTemp = null;
			if (!serialization.TryCustomCopy<NanoTaskItem>(Task, ref TaskTemp, hookCtx, false, context))
			{
				TaskTemp = serialization.CreateCopy<NanoTaskItem>(Task, hookCtx, context, false);
			}
			target.Task = TaskTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NanoTaskPrintedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NanoTaskPrintedComponent cast = (NanoTaskPrintedComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NanoTaskPrintedComponent cast = (NanoTaskPrintedComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NanoTaskPrintedComponent def = (NanoTaskPrintedComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NanoTaskPrintedComponent Instantiate()
	{
		return new NanoTaskPrintedComponent();
	}
}
