using System;
using System.Collections.Generic;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Singularity.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ContainmentFieldGeneratorComponent : Component, ISerializationGenerated<ContainmentFieldGeneratorComponent>, ISerializationGenerated
{
	private int _powerBuffer;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("powerMinimum", false, 1, false, false, null)]
	public int PowerMinimum = 6;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("power", false, 1, false, false, null)]
	public int PowerReceived = 3;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("powerLoss", false, 1, false, false, null)]
	public int PowerLoss = 2;

	[DataField("accumulator", false, 1, false, false, null)]
	public float Accumulator;

	[DataField("threshold", false, 1, false, false, null)]
	public float Threshold = 20f;

	[DataField("maxLength", false, 1, false, false, null)]
	public float MaxLength = 8f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("idTag", false, 1, false, false, typeof(PrototypeIdSerializer<TagPrototype>))]
	public string IDTag = "EmitterBolt";

	[DataField(null, false, 1, false, false, null)]
	public string SourceFixtureId = "projectile";

	[DataField(null, false, 1, false, false, null)]
	public bool Enabled;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool IsConnected;

	[DataField("collisionMask", false, 1, false, false, null)]
	public int CollisionMask = 31;

	[ViewVariables]
	public Dictionary<Direction, (Entity<ContainmentFieldGeneratorComponent>, List<EntityUid>)> Connections = new Dictionary<Direction, (Entity<ContainmentFieldGeneratorComponent>, List<EntityUid>)>();

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("createdField", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string CreatedField = "ContainmentField";

	[DataField("powerBuffer", false, 1, false, false, null)]
	public int PowerBuffer
	{
		get
		{
			return _powerBuffer;
		}
		set
		{
			_powerBuffer = Math.Clamp(value, 0, 25);
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ContainmentFieldGeneratorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ContainmentFieldGeneratorComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ContainmentFieldGeneratorComponent>(this, ref target, hookCtx, false, context))
		{
			int PowerBufferTemp = 0;
			if (!serialization.TryCustomCopy<int>(PowerBuffer, ref PowerBufferTemp, hookCtx, false, context))
			{
				PowerBufferTemp = PowerBuffer;
			}
			target.PowerBuffer = PowerBufferTemp;
			int PowerMinimumTemp = 0;
			if (!serialization.TryCustomCopy<int>(PowerMinimum, ref PowerMinimumTemp, hookCtx, false, context))
			{
				PowerMinimumTemp = PowerMinimum;
			}
			target.PowerMinimum = PowerMinimumTemp;
			int PowerReceivedTemp = 0;
			if (!serialization.TryCustomCopy<int>(PowerReceived, ref PowerReceivedTemp, hookCtx, false, context))
			{
				PowerReceivedTemp = PowerReceived;
			}
			target.PowerReceived = PowerReceivedTemp;
			int PowerLossTemp = 0;
			if (!serialization.TryCustomCopy<int>(PowerLoss, ref PowerLossTemp, hookCtx, false, context))
			{
				PowerLossTemp = PowerLoss;
			}
			target.PowerLoss = PowerLossTemp;
			float AccumulatorTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Accumulator, ref AccumulatorTemp, hookCtx, false, context))
			{
				AccumulatorTemp = Accumulator;
			}
			target.Accumulator = AccumulatorTemp;
			float ThresholdTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Threshold, ref ThresholdTemp, hookCtx, false, context))
			{
				ThresholdTemp = Threshold;
			}
			target.Threshold = ThresholdTemp;
			float MaxLengthTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxLength, ref MaxLengthTemp, hookCtx, false, context))
			{
				MaxLengthTemp = MaxLength;
			}
			target.MaxLength = MaxLengthTemp;
			string IDTagTemp = null;
			if (IDTag == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(IDTag, ref IDTagTemp, hookCtx, false, context))
			{
				IDTagTemp = IDTag;
			}
			target.IDTag = IDTagTemp;
			string SourceFixtureIdTemp = null;
			if (SourceFixtureId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(SourceFixtureId, ref SourceFixtureIdTemp, hookCtx, false, context))
			{
				SourceFixtureIdTemp = SourceFixtureId;
			}
			target.SourceFixtureId = SourceFixtureIdTemp;
			bool EnabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Enabled, ref EnabledTemp, hookCtx, false, context))
			{
				EnabledTemp = Enabled;
			}
			target.Enabled = EnabledTemp;
			int CollisionMaskTemp = 0;
			if (!serialization.TryCustomCopy<int>(CollisionMask, ref CollisionMaskTemp, hookCtx, false, context))
			{
				CollisionMaskTemp = CollisionMask;
			}
			target.CollisionMask = CollisionMaskTemp;
			string CreatedFieldTemp = null;
			if (CreatedField == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(CreatedField, ref CreatedFieldTemp, hookCtx, false, context))
			{
				CreatedFieldTemp = CreatedField;
			}
			target.CreatedField = CreatedFieldTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ContainmentFieldGeneratorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainmentFieldGeneratorComponent cast = (ContainmentFieldGeneratorComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainmentFieldGeneratorComponent cast = (ContainmentFieldGeneratorComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainmentFieldGeneratorComponent def = (ContainmentFieldGeneratorComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ContainmentFieldGeneratorComponent Instantiate()
	{
		return new ContainmentFieldGeneratorComponent();
	}
}
