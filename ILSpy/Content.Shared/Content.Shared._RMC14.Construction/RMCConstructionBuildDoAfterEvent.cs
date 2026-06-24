using System;
using Content.Shared._RMC14.Construction.Prototypes;
using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Construction;

[Serializable]
[NetSerializable]
public sealed class RMCConstructionBuildDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<RMCConstructionBuildDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public RMCConstructionPrototype Prototype;

	[DataField(null, false, 1, true, false, null)]
	public int Amount;

	[DataField(null, false, 1, true, false, null)]
	public NetCoordinates Coordinates;

	[DataField(null, false, 1, true, false, null)]
	public Direction Direction;

	public RMCConstructionBuildDoAfterEvent(RMCConstructionPrototype prototype, int amount, NetCoordinates coordinates, Direction direction)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Prototype = prototype;
		Amount = amount;
		Coordinates = coordinates;
		Direction = direction;
	}

	public RMCConstructionBuildDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCConstructionBuildDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCConstructionBuildDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<RMCConstructionBuildDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			RMCConstructionPrototype PrototypeTemp = null;
			if (Prototype == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<RMCConstructionPrototype>(Prototype, ref PrototypeTemp, hookCtx, false, context))
			{
				PrototypeTemp = serialization.CreateCopy<RMCConstructionPrototype>(Prototype, hookCtx, context, false);
			}
			target.Prototype = PrototypeTemp;
			int AmountTemp = 0;
			if (!serialization.TryCustomCopy<int>(Amount, ref AmountTemp, hookCtx, false, context))
			{
				AmountTemp = Amount;
			}
			target.Amount = AmountTemp;
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
	public void Copy(ref RMCConstructionBuildDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCConstructionBuildDoAfterEvent cast = (RMCConstructionBuildDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCConstructionBuildDoAfterEvent cast = (RMCConstructionBuildDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCConstructionBuildDoAfterEvent Instantiate()
	{
		return new RMCConstructionBuildDoAfterEvent();
	}
}
