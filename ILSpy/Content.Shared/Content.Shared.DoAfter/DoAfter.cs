using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.DoAfter;

[Serializable]
[NetSerializable]
[DataDefinition]
[Access(new Type[] { typeof(SharedDoAfterSystem) })]
public sealed class DoAfter : ISerializationGenerated<DoAfter>, ISerializationGenerated
{
	[DataField("index", false, 1, true, false, null)]
	public ushort Index;

	[IncludeDataField(false, 1, false, null)]
	public DoAfterArgs Args;

	[DataField("startTime", false, 1, true, false, typeof(TimeOffsetSerializer))]
	public TimeSpan StartTime;

	[DataField("cancelledTime", false, 1, true, false, typeof(TimeOffsetSerializer))]
	public TimeSpan? CancelledTime;

	[DataField("lastEffectSpawnTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan? LastEffectSpawnTime;

	[DataField("completed", false, 1, false, false, null)]
	public bool Completed;

	[NonSerialized]
	[DataField("userPosition", false, 1, false, false, null)]
	public EntityCoordinates UserPosition;

	public NetCoordinates NetUserPosition;

	[DataField("targetDistance", false, 1, false, false, null)]
	public float TargetDistance;

	[DataField("activeHand", false, 1, false, false, null)]
	public string? InitialHand;

	[NonSerialized]
	[DataField("activeItem", false, 1, false, false, null)]
	public EntityUid? InitialItem;

	public NetEntity? NetInitialItem;

	[NonSerialized]
	public object? AttemptEvent;

	public DoAfterId Id => new DoAfterId(Args.User, Index);

	public bool Cancelled => CancelledTime.HasValue;

	private DoAfter()
	{
	}

	public DoAfter(ushort index, DoAfterArgs args, TimeSpan startTime)
	{
		Index = index;
		Args = args;
		StartTime = startTime;
	}

	public DoAfter(IEntityManager entManager, DoAfter other)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		Index = other.Index;
		Args = new DoAfterArgs(other.Args);
		StartTime = other.StartTime;
		CancelledTime = other.CancelledTime;
		Completed = other.Completed;
		UserPosition = other.UserPosition;
		TargetDistance = other.TargetDistance;
		InitialHand = other.InitialHand;
		InitialItem = other.InitialItem;
		NetUserPosition = other.NetUserPosition;
		NetInitialItem = other.NetInitialItem;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DoAfter target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<DoAfter>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		ushort IndexTemp = 0;
		if (!serialization.TryCustomCopy<ushort>(Index, ref IndexTemp, hookCtx, false, context))
		{
			IndexTemp = Index;
		}
		target.Index = IndexTemp;
		DoAfterArgs ArgsTemp = null;
		if (Args == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DoAfterArgs>(Args, ref ArgsTemp, hookCtx, false, context))
		{
			if (Args == null)
			{
				ArgsTemp = null;
			}
			else
			{
				serialization.CopyTo<DoAfterArgs>(Args, ref ArgsTemp, hookCtx, context, true);
			}
		}
		target.Args = ArgsTemp;
		TimeSpan StartTimeTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(StartTime, ref StartTimeTemp, hookCtx, false, context))
		{
			StartTimeTemp = serialization.CreateCopy<TimeSpan>(StartTime, hookCtx, context, false);
		}
		target.StartTime = StartTimeTemp;
		TimeSpan? CancelledTimeTemp = null;
		if (!serialization.TryCustomCopy<TimeSpan?>(CancelledTime, ref CancelledTimeTemp, hookCtx, false, context))
		{
			CancelledTimeTemp = serialization.CreateCopy<TimeSpan?>(CancelledTime, hookCtx, context, false);
		}
		target.CancelledTime = CancelledTimeTemp;
		TimeSpan? LastEffectSpawnTimeTemp = null;
		if (!serialization.TryCustomCopy<TimeSpan?>(LastEffectSpawnTime, ref LastEffectSpawnTimeTemp, hookCtx, false, context))
		{
			LastEffectSpawnTimeTemp = serialization.CreateCopy<TimeSpan?>(LastEffectSpawnTime, hookCtx, context, false);
		}
		target.LastEffectSpawnTime = LastEffectSpawnTimeTemp;
		bool CompletedTemp = false;
		if (!serialization.TryCustomCopy<bool>(Completed, ref CompletedTemp, hookCtx, false, context))
		{
			CompletedTemp = Completed;
		}
		target.Completed = CompletedTemp;
		EntityCoordinates UserPositionTemp = default(EntityCoordinates);
		if (!serialization.TryCustomCopy<EntityCoordinates>(UserPosition, ref UserPositionTemp, hookCtx, false, context))
		{
			UserPositionTemp = serialization.CreateCopy<EntityCoordinates>(UserPosition, hookCtx, context, false);
		}
		target.UserPosition = UserPositionTemp;
		float TargetDistanceTemp = 0f;
		if (!serialization.TryCustomCopy<float>(TargetDistance, ref TargetDistanceTemp, hookCtx, false, context))
		{
			TargetDistanceTemp = TargetDistance;
		}
		target.TargetDistance = TargetDistanceTemp;
		string InitialHandTemp = null;
		if (!serialization.TryCustomCopy<string>(InitialHand, ref InitialHandTemp, hookCtx, false, context))
		{
			InitialHandTemp = InitialHand;
		}
		target.InitialHand = InitialHandTemp;
		EntityUid? InitialItemTemp = null;
		if (!serialization.TryCustomCopy<EntityUid?>(InitialItem, ref InitialItemTemp, hookCtx, false, context))
		{
			InitialItemTemp = serialization.CreateCopy<EntityUid?>(InitialItem, hookCtx, context, false);
		}
		target.InitialItem = InitialItemTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DoAfter target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfter cast = (DoAfter)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public DoAfter Instantiate()
	{
		return new DoAfter();
	}
}
