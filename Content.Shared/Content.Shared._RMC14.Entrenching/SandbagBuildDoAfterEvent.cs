using System;
using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Entrenching;

[Serializable]
[NetSerializable]
public sealed class SandbagBuildDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<SandbagBuildDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public NetCoordinates Coordinates;

	[DataField(null, false, 1, true, false, null)]
	public Direction Direction;

	public SandbagBuildDoAfterEvent(NetCoordinates coordinates, Direction direction)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Coordinates = coordinates;
		Direction = direction;
	}

	public SandbagBuildDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SandbagBuildDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SandbagBuildDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<SandbagBuildDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			NetCoordinates CoordinatesTemp = default(NetCoordinates);
			if (!serialization.TryCustomCopy<NetCoordinates>(Coordinates, ref CoordinatesTemp, hookCtx, false, context))
			{
				CoordinatesTemp = serialization.CreateCopy<NetCoordinates>(Coordinates, hookCtx, context, false);
			}
			target.Coordinates = CoordinatesTemp;
			Direction DirectionTemp = (Direction)0;
			if (!serialization.TryCustomCopy<Direction>(Direction, ref DirectionTemp, hookCtx, false, context))
			{
				DirectionTemp = Direction;
			}
			target.Direction = DirectionTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SandbagBuildDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SandbagBuildDoAfterEvent cast = (SandbagBuildDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SandbagBuildDoAfterEvent cast = (SandbagBuildDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SandbagBuildDoAfterEvent Instantiate()
	{
		return new SandbagBuildDoAfterEvent();
	}
}
