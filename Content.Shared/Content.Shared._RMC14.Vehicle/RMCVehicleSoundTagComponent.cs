using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
public sealed class RMCVehicleSoundTagComponent : Component, ISerializationGenerated<RMCVehicleSoundTagComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public RMCVehicleSoundKind Kind;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCVehicleSoundTagComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCVehicleSoundTagComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCVehicleSoundTagComponent>(this, ref target, hookCtx, false, context))
		{
			RMCVehicleSoundKind KindTemp = RMCVehicleSoundKind.Run;
			if (!serialization.TryCustomCopy<RMCVehicleSoundKind>(Kind, ref KindTemp, hookCtx, false, context))
			{
				KindTemp = Kind;
			}
			target.Kind = KindTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCVehicleSoundTagComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCVehicleSoundTagComponent cast = (RMCVehicleSoundTagComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCVehicleSoundTagComponent cast = (RMCVehicleSoundTagComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCVehicleSoundTagComponent def = (RMCVehicleSoundTagComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCVehicleSoundTagComponent Instantiate()
	{
		return new RMCVehicleSoundTagComponent();
	}
}
