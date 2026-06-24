using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Atmos.Components;

[RegisterComponent]
public sealed class MovedByPressureComponent : Component, ISerializationGenerated<MovedByPressureComponent>, ISerializationGenerated
{
	public const float MoveForcePushRatio = 1f;

	public const float MoveForceForcePushRatio = 1f;

	public const float ProbabilityOffset = 25f;

	public const float ProbabilityBasePercent = 10f;

	public const float ThrowForce = 100f;

	[DataField(null, false, 1, false, false, null)]
	public float Accumulator;

	[DataField(null, false, 1, false, false, null)]
	public HashSet<string> TableLayerRemoved = new HashSet<string>();

	[DataField(null, false, 1, false, false, null)]
	public bool Enabled { get; set; } = true;

	[DataField(null, false, 1, false, false, null)]
	public float PressureResistance { get; set; } = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float MoveResist { get; set; } = 100f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int LastHighPressureMovementAirCycle { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MovedByPressureComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MovedByPressureComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<MovedByPressureComponent>(this, ref target, hookCtx, false, context))
		{
			float AccumulatorTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Accumulator, ref AccumulatorTemp, hookCtx, false, context))
			{
				AccumulatorTemp = Accumulator;
			}
			target.Accumulator = AccumulatorTemp;
			bool EnabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Enabled, ref EnabledTemp, hookCtx, false, context))
			{
				EnabledTemp = Enabled;
			}
			target.Enabled = EnabledTemp;
			float PressureResistanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(PressureResistance, ref PressureResistanceTemp, hookCtx, false, context))
			{
				PressureResistanceTemp = PressureResistance;
			}
			target.PressureResistance = PressureResistanceTemp;
			float MoveResistTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MoveResist, ref MoveResistTemp, hookCtx, false, context))
			{
				MoveResistTemp = MoveResist;
			}
			target.MoveResist = MoveResistTemp;
			HashSet<string> TableLayerRemovedTemp = null;
			if (TableLayerRemoved == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<string>>(TableLayerRemoved, ref TableLayerRemovedTemp, hookCtx, true, context))
			{
				TableLayerRemovedTemp = serialization.CreateCopy<HashSet<string>>(TableLayerRemoved, hookCtx, context, false);
			}
			target.TableLayerRemoved = TableLayerRemovedTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MovedByPressureComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MovedByPressureComponent cast = (MovedByPressureComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MovedByPressureComponent cast = (MovedByPressureComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MovedByPressureComponent def = (MovedByPressureComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MovedByPressureComponent Instantiate()
	{
		return new MovedByPressureComponent();
	}
}
