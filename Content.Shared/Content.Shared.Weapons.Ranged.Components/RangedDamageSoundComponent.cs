using System;
using System.Collections.Generic;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class RangedDamageSoundComponent : Component, ISerializationGenerated<RangedDamageSoundComponent>, ISerializationGenerated
{
	[DataField("soundGroups", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<SoundSpecifier, DamageGroupPrototype>))]
	public Dictionary<string, SoundSpecifier>? SoundGroups;

	[DataField("soundTypes", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<SoundSpecifier, DamageTypePrototype>))]
	public Dictionary<string, SoundSpecifier>? SoundTypes;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RangedDamageSoundComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RangedDamageSoundComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RangedDamageSoundComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, SoundSpecifier> SoundGroupsTemp = null;
			if (!serialization.TryCustomCopy<Dictionary<string, SoundSpecifier>>(SoundGroups, ref SoundGroupsTemp, hookCtx, true, context))
			{
				SoundGroupsTemp = serialization.CreateCopy<Dictionary<string, SoundSpecifier>>(SoundGroups, hookCtx, context, false);
			}
			target.SoundGroups = SoundGroupsTemp;
			Dictionary<string, SoundSpecifier> SoundTypesTemp = null;
			if (!serialization.TryCustomCopy<Dictionary<string, SoundSpecifier>>(SoundTypes, ref SoundTypesTemp, hookCtx, true, context))
			{
				SoundTypesTemp = serialization.CreateCopy<Dictionary<string, SoundSpecifier>>(SoundTypes, hookCtx, context, false);
			}
			target.SoundTypes = SoundTypesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RangedDamageSoundComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RangedDamageSoundComponent cast = (RangedDamageSoundComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RangedDamageSoundComponent cast = (RangedDamageSoundComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RangedDamageSoundComponent def = (RangedDamageSoundComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RangedDamageSoundComponent Instantiate()
	{
		return new RangedDamageSoundComponent();
	}
}
