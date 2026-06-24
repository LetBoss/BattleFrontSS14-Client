using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Damage.Components;

[RegisterComponent]
public sealed class StaminaDamageOnCollideComponent : Component, ISerializationGenerated<StaminaDamageOnCollideComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("damage", false, 1, false, false, null)]
	public float Damage = 55f;

	[DataField("sound", false, 1, false, false, null)]
	public SoundSpecifier? Sound;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StaminaDamageOnCollideComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StaminaDamageOnCollideComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StaminaDamageOnCollideComponent>(this, ref target, hookCtx, false, context))
		{
			float DamageTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Damage, ref DamageTemp, hookCtx, false, context))
			{
				DamageTemp = Damage;
			}
			target.Damage = DamageTemp;
			SoundSpecifier SoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(Sound, ref SoundTemp, hookCtx, true, context))
			{
				SoundTemp = serialization.CreateCopy<SoundSpecifier>(Sound, hookCtx, context, false);
			}
			target.Sound = SoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StaminaDamageOnCollideComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StaminaDamageOnCollideComponent cast = (StaminaDamageOnCollideComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StaminaDamageOnCollideComponent cast = (StaminaDamageOnCollideComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StaminaDamageOnCollideComponent def = (StaminaDamageOnCollideComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StaminaDamageOnCollideComponent Instantiate()
	{
		return new StaminaDamageOnCollideComponent();
	}
}
