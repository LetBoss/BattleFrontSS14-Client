using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._CIV14merka.Aircraft;

[RegisterComponent]
public sealed class AircraftCannonComponent : Component, ISerializationGenerated<AircraftCannonComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int IgnoreWallsFromAltitude = 2;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AircraftCannonComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AircraftCannonComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<AircraftCannonComponent>(this, ref target, hookCtx, false, context))
		{
			int IgnoreWallsFromAltitudeTemp = 0;
			if (!serialization.TryCustomCopy<int>(IgnoreWallsFromAltitude, ref IgnoreWallsFromAltitudeTemp, hookCtx, false, context))
			{
				IgnoreWallsFromAltitudeTemp = IgnoreWallsFromAltitude;
			}
			target.IgnoreWallsFromAltitude = IgnoreWallsFromAltitudeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AircraftCannonComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AircraftCannonComponent cast = (AircraftCannonComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AircraftCannonComponent cast = (AircraftCannonComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AircraftCannonComponent def = (AircraftCannonComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AircraftCannonComponent Instantiate()
	{
		return new AircraftCannonComponent();
	}
}
