using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Finesse;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(XenoFinesseSystem) })]
public sealed class XenoMarkedComponent : Component, ISerializationGenerated<XenoMarkedComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public TimeSpan TimeAdded;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan WearOffAt;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoMarkedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoMarkedComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<XenoMarkedComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan TimeAddedTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(TimeAdded, ref TimeAddedTemp, hookCtx, false, context))
			{
				TimeAddedTemp = serialization.CreateCopy<TimeSpan>(TimeAdded, hookCtx, context, false);
			}
			target.TimeAdded = TimeAddedTemp;
			TimeSpan WearOffAtTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(WearOffAt, ref WearOffAtTemp, hookCtx, false, context))
			{
				WearOffAtTemp = serialization.CreateCopy<TimeSpan>(WearOffAt, hookCtx, context, false);
			}
			target.WearOffAt = WearOffAtTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoMarkedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoMarkedComponent cast = (XenoMarkedComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoMarkedComponent cast = (XenoMarkedComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoMarkedComponent def = (XenoMarkedComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoMarkedComponent Instantiate()
	{
		return new XenoMarkedComponent();
	}
}
