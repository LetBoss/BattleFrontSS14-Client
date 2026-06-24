using System;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(UtensilSystem) })]
public sealed class UtensilComponent : Component, ISerializationGenerated<UtensilComponent>, ISerializationGenerated
{
	[DataField("types", false, 1, false, false, null)]
	private UtensilType _types;

	[DataField("breakChance", false, 1, false, false, null)]
	public float BreakChance;

	[DataField("breakSound", false, 1, false, false, null)]
	public SoundSpecifier BreakSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Items/snap.ogg", (AudioParams?)null);

	[ViewVariables]
	public UtensilType Types
	{
		get
		{
			return _types;
		}
		set
		{
			if (!_types.Equals(value))
			{
				_types = value;
			}
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref UtensilComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (UtensilComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<UtensilComponent>(this, ref target, hookCtx, false, context))
		{
			UtensilType _typesTemp = UtensilType.None;
			if (!serialization.TryCustomCopy<UtensilType>(_types, ref _typesTemp, hookCtx, false, context))
			{
				_typesTemp = _types;
			}
			target._types = _typesTemp;
			float BreakChanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BreakChance, ref BreakChanceTemp, hookCtx, false, context))
			{
				BreakChanceTemp = BreakChance;
			}
			target.BreakChance = BreakChanceTemp;
			SoundSpecifier BreakSoundTemp = null;
			if (BreakSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(BreakSound, ref BreakSoundTemp, hookCtx, true, context))
			{
				BreakSoundTemp = serialization.CreateCopy<SoundSpecifier>(BreakSound, hookCtx, context, false);
			}
			target.BreakSound = BreakSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref UtensilComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UtensilComponent cast = (UtensilComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UtensilComponent cast = (UtensilComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UtensilComponent def = (UtensilComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override UtensilComponent Instantiate()
	{
		return new UtensilComponent();
	}
}
