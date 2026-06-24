using System;
using Content.Shared.Clothing.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Clothing.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SkatesSystem) })]
public sealed class SkatesComponent : Component, ISerializationGenerated<SkatesComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Friction = 0.125f;

	[DataField(null, false, 1, false, false, null)]
	public float FrictionNoInput = 0.125f;

	[DataField(null, false, 1, false, false, null)]
	public float Acceleration = 0.25f;

	[DataField(null, false, 1, false, false, null)]
	public float MinimumSpeed = 3f;

	[DataField(null, false, 1, false, false, null)]
	public float StunSeconds = 3f;

	[DataField(null, false, 1, false, false, null)]
	public float DamageCooldown = 2f;

	[DataField(null, false, 1, false, false, null)]
	public float SpeedDamage = 1f;

	[ViewVariables]
	public float DefaultMinimumSpeed = 20f;

	[ViewVariables]
	public float DefaultStunSeconds = 1f;

	[ViewVariables]
	public float DefaultDamageCooldown = 2f;

	[ViewVariables]
	public float DefaultSpeedDamage = 0.5f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SkatesComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SkatesComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SkatesComponent>(this, ref target, hookCtx, false, context))
		{
			float FrictionTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Friction, ref FrictionTemp, hookCtx, false, context))
			{
				FrictionTemp = Friction;
			}
			target.Friction = FrictionTemp;
			float FrictionNoInputTemp = 0f;
			if (!serialization.TryCustomCopy<float>(FrictionNoInput, ref FrictionNoInputTemp, hookCtx, false, context))
			{
				FrictionNoInputTemp = FrictionNoInput;
			}
			target.FrictionNoInput = FrictionNoInputTemp;
			float AccelerationTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Acceleration, ref AccelerationTemp, hookCtx, false, context))
			{
				AccelerationTemp = Acceleration;
			}
			target.Acceleration = AccelerationTemp;
			float MinimumSpeedTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MinimumSpeed, ref MinimumSpeedTemp, hookCtx, false, context))
			{
				MinimumSpeedTemp = MinimumSpeed;
			}
			target.MinimumSpeed = MinimumSpeedTemp;
			float StunSecondsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(StunSeconds, ref StunSecondsTemp, hookCtx, false, context))
			{
				StunSecondsTemp = StunSeconds;
			}
			target.StunSeconds = StunSecondsTemp;
			float DamageCooldownTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DamageCooldown, ref DamageCooldownTemp, hookCtx, false, context))
			{
				DamageCooldownTemp = DamageCooldown;
			}
			target.DamageCooldown = DamageCooldownTemp;
			float SpeedDamageTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SpeedDamage, ref SpeedDamageTemp, hookCtx, false, context))
			{
				SpeedDamageTemp = SpeedDamage;
			}
			target.SpeedDamage = SpeedDamageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SkatesComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SkatesComponent cast = (SkatesComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SkatesComponent cast = (SkatesComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SkatesComponent def = (SkatesComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SkatesComponent Instantiate()
	{
		return new SkatesComponent();
	}
}
