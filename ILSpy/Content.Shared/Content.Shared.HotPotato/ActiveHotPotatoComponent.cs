using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.HotPotato;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedHotPotatoSystem) })]
public sealed class ActiveHotPotatoComponent : Component, ISerializationGenerated<ActiveHotPotatoComponent>, ISerializationGenerated
{
	[DataField("effectCooldown", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float EffectCooldown = 0.3f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan TargetTime = TimeSpan.Zero;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ActiveHotPotatoComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ActiveHotPotatoComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ActiveHotPotatoComponent>(this, ref target, hookCtx, false, context))
		{
			float EffectCooldownTemp = 0f;
			if (!serialization.TryCustomCopy<float>(EffectCooldown, ref EffectCooldownTemp, hookCtx, false, context))
			{
				EffectCooldownTemp = EffectCooldown;
			}
			target.EffectCooldown = EffectCooldownTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ActiveHotPotatoComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveHotPotatoComponent cast = (ActiveHotPotatoComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveHotPotatoComponent cast = (ActiveHotPotatoComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveHotPotatoComponent def = (ActiveHotPotatoComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ActiveHotPotatoComponent Instantiate()
	{
		return new ActiveHotPotatoComponent();
	}
}
