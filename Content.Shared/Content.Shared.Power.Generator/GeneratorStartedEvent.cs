using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Power.Generator;

[Serializable]
[NetSerializable]
public sealed class GeneratorStartedEvent : DoAfterEvent, ISerializationGenerated<GeneratorStartedEvent>, ISerializationGenerated
{
	public override DoAfterEvent Clone()
	{
		return this;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GeneratorStartedEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GeneratorStartedEvent)definitionCast;
		serialization.TryCustomCopy<GeneratorStartedEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GeneratorStartedEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GeneratorStartedEvent cast = (GeneratorStartedEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GeneratorStartedEvent cast = (GeneratorStartedEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GeneratorStartedEvent Instantiate()
	{
		return new GeneratorStartedEvent();
	}
}
