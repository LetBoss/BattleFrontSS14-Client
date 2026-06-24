using System;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.RCD.Systems;

[Serializable]
[NetSerializable]
public sealed class RCDDoAfterEvent : DoAfterEvent, ISerializationGenerated<RCDDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public NetCoordinates Location { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public Direction Direction { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<RCDPrototype> StartingProtoId { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public int Cost { get; private set; } = 1;

	[DataField("fx", false, 1, false, false, null)]
	public NetEntity? Effect { get; private set; }

	private RCDDoAfterEvent()
	{
	}

	public RCDDoAfterEvent(NetCoordinates location, Direction direction, ProtoId<RCDPrototype> startingProtoId, int cost, NetEntity? effect = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Location = location;
		Direction = direction;
		StartingProtoId = startingProtoId;
		Cost = cost;
		Effect = effect;
	}

	public override DoAfterEvent Clone()
	{
		return this;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RCDDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RCDDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<RCDDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			NetCoordinates LocationTemp = default(NetCoordinates);
			if (!serialization.TryCustomCopy<NetCoordinates>(Location, ref LocationTemp, hookCtx, false, context))
			{
				LocationTemp = serialization.CreateCopy<NetCoordinates>(Location, hookCtx, context, false);
			}
			target.Location = LocationTemp;
			Direction DirectionTemp = (Direction)0;
			if (!serialization.TryCustomCopy<Direction>(Direction, ref DirectionTemp, hookCtx, false, context))
			{
				DirectionTemp = Direction;
			}
			target.Direction = DirectionTemp;
			ProtoId<RCDPrototype> StartingProtoIdTemp = default(ProtoId<RCDPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<RCDPrototype>>(StartingProtoId, ref StartingProtoIdTemp, hookCtx, false, context))
			{
				StartingProtoIdTemp = serialization.CreateCopy<ProtoId<RCDPrototype>>(StartingProtoId, hookCtx, context, false);
			}
			target.StartingProtoId = StartingProtoIdTemp;
			int CostTemp = 0;
			if (!serialization.TryCustomCopy<int>(Cost, ref CostTemp, hookCtx, false, context))
			{
				CostTemp = Cost;
			}
			target.Cost = CostTemp;
			NetEntity? EffectTemp = null;
			if (!serialization.TryCustomCopy<NetEntity?>(Effect, ref EffectTemp, hookCtx, false, context))
			{
				EffectTemp = serialization.CreateCopy<NetEntity?>(Effect, hookCtx, context, false);
			}
			target.Effect = EffectTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RCDDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RCDDoAfterEvent cast = (RCDDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RCDDoAfterEvent cast = (RCDDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RCDDoAfterEvent Instantiate()
	{
		return new RCDDoAfterEvent();
	}
}
