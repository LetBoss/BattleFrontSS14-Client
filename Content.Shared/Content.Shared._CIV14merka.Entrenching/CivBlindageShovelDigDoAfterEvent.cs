using System;
using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._CIV14merka.Entrenching;

[Serializable]
[NetSerializable]
public sealed class CivBlindageShovelDigDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<CivBlindageShovelDigDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public NetCoordinates Coordinates;

	public CivBlindageShovelDigDoAfterEvent(NetCoordinates coordinates)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Coordinates = coordinates;
	}

	public CivBlindageShovelDigDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CivBlindageShovelDigDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CivBlindageShovelDigDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<CivBlindageShovelDigDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			NetCoordinates CoordinatesTemp = default(NetCoordinates);
			if (!serialization.TryCustomCopy<NetCoordinates>(Coordinates, ref CoordinatesTemp, hookCtx, false, context))
			{
				CoordinatesTemp = serialization.CreateCopy<NetCoordinates>(Coordinates, hookCtx, context, false);
			}
			target.Coordinates = CoordinatesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CivBlindageShovelDigDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivBlindageShovelDigDoAfterEvent cast = (CivBlindageShovelDigDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivBlindageShovelDigDoAfterEvent cast = (CivBlindageShovelDigDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CivBlindageShovelDigDoAfterEvent Instantiate()
	{
		return new CivBlindageShovelDigDoAfterEvent();
	}
}
