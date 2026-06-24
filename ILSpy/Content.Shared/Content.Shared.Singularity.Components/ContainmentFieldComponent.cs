using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Singularity.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ContainmentFieldComponent : Component, ISerializationGenerated<ContainmentFieldComponent>, ISerializationGenerated
{
	[DataField("throwForce", false, 1, false, false, null)]
	public float ThrowForce = 100f;

	[DataField("maxMass", false, 1, false, false, null)]
	public float MaxMass = 10000f;

	[DataField(null, false, 1, false, false, null)]
	public bool DestroyGarbage = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ContainmentFieldComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ContainmentFieldComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ContainmentFieldComponent>(this, ref target, hookCtx, false, context))
		{
			float ThrowForceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ThrowForce, ref ThrowForceTemp, hookCtx, false, context))
			{
				ThrowForceTemp = ThrowForce;
			}
			target.ThrowForce = ThrowForceTemp;
			float MaxMassTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxMass, ref MaxMassTemp, hookCtx, false, context))
			{
				MaxMassTemp = MaxMass;
			}
			target.MaxMass = MaxMassTemp;
			bool DestroyGarbageTemp = false;
			if (!serialization.TryCustomCopy<bool>(DestroyGarbage, ref DestroyGarbageTemp, hookCtx, false, context))
			{
				DestroyGarbageTemp = DestroyGarbage;
			}
			target.DestroyGarbage = DestroyGarbageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ContainmentFieldComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainmentFieldComponent cast = (ContainmentFieldComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainmentFieldComponent cast = (ContainmentFieldComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainmentFieldComponent def = (ContainmentFieldComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ContainmentFieldComponent Instantiate()
	{
		return new ContainmentFieldComponent();
	}
}
