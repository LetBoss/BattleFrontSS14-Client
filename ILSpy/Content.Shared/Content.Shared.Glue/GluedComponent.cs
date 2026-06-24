using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Glue;

[RegisterComponent]
[Access(new Type[] { typeof(SharedGlueSystem) })]
public sealed class GluedComponent : Component, ISerializationGenerated<GluedComponent>, ISerializationGenerated
{
	[DataField("until", false, 1, false, false, typeof(TimeOffsetSerializer))]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan Until;

	[DataField("duration", false, 1, false, false, typeof(TimeOffsetSerializer))]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan Duration;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GluedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GluedComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GluedComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan UntilTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Until, ref UntilTemp, hookCtx, false, context))
			{
				UntilTemp = serialization.CreateCopy<TimeSpan>(Until, hookCtx, context, false);
			}
			target.Until = UntilTemp;
			TimeSpan DurationTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Duration, ref DurationTemp, hookCtx, false, context))
			{
				DurationTemp = serialization.CreateCopy<TimeSpan>(Duration, hookCtx, context, false);
			}
			target.Duration = DurationTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GluedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GluedComponent cast = (GluedComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GluedComponent cast = (GluedComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GluedComponent def = (GluedComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GluedComponent Instantiate()
	{
		return new GluedComponent();
	}
}
