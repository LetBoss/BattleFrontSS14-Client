using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Damage.Components;

[NetworkedComponent]
[RegisterComponent]
public sealed class DamagedByContactComponent : Component, ISerializationGenerated<DamagedByContactComponent>, ISerializationGenerated
{
	[DataField("nextSecond", false, 1, false, false, typeof(TimeOffsetSerializer))]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan NextSecond = TimeSpan.Zero;

	[ViewVariables]
	public DamageSpecifier? Damage;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DamagedByContactComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DamagedByContactComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<DamagedByContactComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan NextSecondTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(NextSecond, ref NextSecondTemp, hookCtx, false, context))
			{
				NextSecondTemp = serialization.CreateCopy<TimeSpan>(NextSecond, hookCtx, context, false);
			}
			target.NextSecond = NextSecondTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DamagedByContactComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamagedByContactComponent cast = (DamagedByContactComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamagedByContactComponent cast = (DamagedByContactComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamagedByContactComponent def = (DamagedByContactComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DamagedByContactComponent Instantiate()
	{
		return new DamagedByContactComponent();
	}
}
