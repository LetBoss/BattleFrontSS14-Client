using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Weapons.Ranged.Magazine;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(RMCMagazineSystem) })]
public sealed class RMCMagazineVisualsComponent : Component, ISerializationGenerated<RMCMagazineVisualsComponent>, ISerializationGenerated
{
	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCMagazineVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCMagazineVisualsComponent)(object)definitionCast;
		serialization.TryCustomCopy<RMCMagazineVisualsComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCMagazineVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCMagazineVisualsComponent cast = (RMCMagazineVisualsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCMagazineVisualsComponent cast = (RMCMagazineVisualsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCMagazineVisualsComponent def = (RMCMagazineVisualsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCMagazineVisualsComponent Instantiate()
	{
		return new RMCMagazineVisualsComponent();
	}
}
