using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Shuttles.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class FTLSmashImmuneComponent : Component, ISerializationGenerated<FTLSmashImmuneComponent>, ISerializationGenerated
{
	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FTLSmashImmuneComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FTLSmashImmuneComponent)(object)definitionCast;
		serialization.TryCustomCopy<FTLSmashImmuneComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FTLSmashImmuneComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FTLSmashImmuneComponent cast = (FTLSmashImmuneComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FTLSmashImmuneComponent cast = (FTLSmashImmuneComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FTLSmashImmuneComponent def = (FTLSmashImmuneComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FTLSmashImmuneComponent Instantiate()
	{
		return new FTLSmashImmuneComponent();
	}
}
