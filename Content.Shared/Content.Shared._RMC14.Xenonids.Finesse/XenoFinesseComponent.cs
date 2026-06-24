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
public sealed class XenoFinesseComponent : Component, ISerializationGenerated<XenoFinesseComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public TimeSpan MarkedTime = TimeSpan.FromSeconds(3.5);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoFinesseComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoFinesseComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<XenoFinesseComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan MarkedTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(MarkedTime, ref MarkedTimeTemp, hookCtx, false, context))
			{
				MarkedTimeTemp = serialization.CreateCopy<TimeSpan>(MarkedTime, hookCtx, context, false);
			}
			target.MarkedTime = MarkedTimeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoFinesseComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoFinesseComponent cast = (XenoFinesseComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoFinesseComponent cast = (XenoFinesseComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoFinesseComponent def = (XenoFinesseComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoFinesseComponent Instantiate()
	{
		return new XenoFinesseComponent();
	}
}
