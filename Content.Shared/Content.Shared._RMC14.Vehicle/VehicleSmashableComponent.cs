using System;
using System.Collections.Generic;
using Content.Shared.Tag;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
public sealed class VehicleSmashableComponent : Component, ISerializationGenerated<VehicleSmashableComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool DeleteOnHit = true;

	[DataField(null, false, 1, false, false, null)]
	public HashSet<ProtoId<TagPrototype>> RequiredVehicleTags = new HashSet<ProtoId<TagPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public double DamageOnHit = 1000.0;

	[DataField(null, false, 1, false, false, null)]
	public float SlowdownMultiplier = 0.5f;

	[DataField(null, false, 1, false, false, null)]
	public float SlowdownDuration;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? SmashSound;

	[DataField(null, false, 1, false, false, null)]
	public bool RequiresDoorUnpowered;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleSmashableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleSmashableComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<VehicleSmashableComponent>(this, ref target, hookCtx, false, context))
		{
			bool DeleteOnHitTemp = false;
			if (!serialization.TryCustomCopy<bool>(DeleteOnHit, ref DeleteOnHitTemp, hookCtx, false, context))
			{
				DeleteOnHitTemp = DeleteOnHit;
			}
			target.DeleteOnHit = DeleteOnHitTemp;
			HashSet<ProtoId<TagPrototype>> RequiredVehicleTagsTemp = null;
			if (RequiredVehicleTags == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<ProtoId<TagPrototype>>>(RequiredVehicleTags, ref RequiredVehicleTagsTemp, hookCtx, true, context))
			{
				RequiredVehicleTagsTemp = serialization.CreateCopy<HashSet<ProtoId<TagPrototype>>>(RequiredVehicleTags, hookCtx, context, false);
			}
			target.RequiredVehicleTags = RequiredVehicleTagsTemp;
			double DamageOnHitTemp = 0.0;
			if (!serialization.TryCustomCopy<double>(DamageOnHit, ref DamageOnHitTemp, hookCtx, false, context))
			{
				DamageOnHitTemp = DamageOnHit;
			}
			target.DamageOnHit = DamageOnHitTemp;
			float SlowdownMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SlowdownMultiplier, ref SlowdownMultiplierTemp, hookCtx, false, context))
			{
				SlowdownMultiplierTemp = SlowdownMultiplier;
			}
			target.SlowdownMultiplier = SlowdownMultiplierTemp;
			float SlowdownDurationTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SlowdownDuration, ref SlowdownDurationTemp, hookCtx, false, context))
			{
				SlowdownDurationTemp = SlowdownDuration;
			}
			target.SlowdownDuration = SlowdownDurationTemp;
			SoundSpecifier SmashSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(SmashSound, ref SmashSoundTemp, hookCtx, true, context))
			{
				SmashSoundTemp = serialization.CreateCopy<SoundSpecifier>(SmashSound, hookCtx, context, false);
			}
			target.SmashSound = SmashSoundTemp;
			bool RequiresDoorUnpoweredTemp = false;
			if (!serialization.TryCustomCopy<bool>(RequiresDoorUnpowered, ref RequiresDoorUnpoweredTemp, hookCtx, false, context))
			{
				RequiresDoorUnpoweredTemp = RequiresDoorUnpowered;
			}
			target.RequiresDoorUnpowered = RequiresDoorUnpoweredTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleSmashableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleSmashableComponent cast = (VehicleSmashableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleSmashableComponent cast = (VehicleSmashableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleSmashableComponent def = (VehicleSmashableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleSmashableComponent Instantiate()
	{
		return new VehicleSmashableComponent();
	}
}
