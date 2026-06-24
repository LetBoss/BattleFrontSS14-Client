using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Weapons.Ranged.HoloTargeting;

[RegisterComponent]
[Access(new Type[] { typeof(RMCHoloTargetingSystem) })]
public sealed class HoloTargetingComponent : Component, ISerializationGenerated<HoloTargetingComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Stacks = 10f;

	[DataField(null, false, 1, false, false, null)]
	public float MaxStacks = 100f;

	[DataField(null, false, 1, false, false, null)]
	public float Decay = 5f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HoloTargetingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HoloTargetingComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<HoloTargetingComponent>(this, ref target, hookCtx, false, context))
		{
			float StacksTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Stacks, ref StacksTemp, hookCtx, false, context))
			{
				StacksTemp = Stacks;
			}
			target.Stacks = StacksTemp;
			float MaxStacksTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxStacks, ref MaxStacksTemp, hookCtx, false, context))
			{
				MaxStacksTemp = MaxStacks;
			}
			target.MaxStacks = MaxStacksTemp;
			float DecayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Decay, ref DecayTemp, hookCtx, false, context))
			{
				DecayTemp = Decay;
			}
			target.Decay = DecayTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HoloTargetingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HoloTargetingComponent cast = (HoloTargetingComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HoloTargetingComponent cast = (HoloTargetingComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HoloTargetingComponent def = (HoloTargetingComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HoloTargetingComponent Instantiate()
	{
		return new HoloTargetingComponent();
	}
}
