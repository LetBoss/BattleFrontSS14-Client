using System;
using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Armor;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedArmorSystem) })]
public sealed class ArmorComponent : Component, ISerializationGenerated<ArmorComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public DamageModifierSet Modifiers;

	[DataField(null, false, 1, false, false, null)]
	public float PriceMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public bool ShowArmorOnExamine = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ArmorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ArmorComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<ArmorComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		DamageModifierSet ModifiersTemp = null;
		if (Modifiers == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageModifierSet>(Modifiers, ref ModifiersTemp, hookCtx, true, context))
		{
			if (Modifiers == null)
			{
				ModifiersTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageModifierSet>(Modifiers, ref ModifiersTemp, hookCtx, context, true);
			}
		}
		target.Modifiers = ModifiersTemp;
		float PriceMultiplierTemp = 0f;
		if (!serialization.TryCustomCopy<float>(PriceMultiplier, ref PriceMultiplierTemp, hookCtx, false, context))
		{
			PriceMultiplierTemp = PriceMultiplier;
		}
		target.PriceMultiplier = PriceMultiplierTemp;
		bool ShowArmorOnExamineTemp = false;
		if (!serialization.TryCustomCopy<bool>(ShowArmorOnExamine, ref ShowArmorOnExamineTemp, hookCtx, false, context))
		{
			ShowArmorOnExamineTemp = ShowArmorOnExamine;
		}
		target.ShowArmorOnExamine = ShowArmorOnExamineTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ArmorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ArmorComponent cast = (ArmorComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ArmorComponent cast = (ArmorComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ArmorComponent def = (ArmorComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ArmorComponent Instantiate()
	{
		return new ArmorComponent();
	}
}
