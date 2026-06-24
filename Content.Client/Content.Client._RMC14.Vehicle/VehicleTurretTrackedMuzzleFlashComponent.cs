using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client._RMC14.Vehicle;

[RegisterComponent]
public sealed class VehicleTurretTrackedMuzzleFlashComponent : Component, ISerializationGenerated<VehicleTurretTrackedMuzzleFlashComponent>, ISerializationGenerated
{
	public EntityUid Weapon;

	public Vector2 Offset = Vector2.Zero;

	public Angle RotationOffset = Angle.Zero;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleTurretTrackedMuzzleFlashComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (VehicleTurretTrackedMuzzleFlashComponent)(object)val;
		serialization.TryCustomCopy<VehicleTurretTrackedMuzzleFlashComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleTurretTrackedMuzzleFlashComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleTurretTrackedMuzzleFlashComponent target2 = (VehicleTurretTrackedMuzzleFlashComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleTurretTrackedMuzzleFlashComponent target2 = (VehicleTurretTrackedMuzzleFlashComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleTurretTrackedMuzzleFlashComponent target2 = (VehicleTurretTrackedMuzzleFlashComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleTurretTrackedMuzzleFlashComponent Instantiate()
	{
		return new VehicleTurretTrackedMuzzleFlashComponent();
	}
}
