using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Tag;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Fluids.Components;

[RegisterComponent]
[Access(new Type[] { typeof(SharedDrainSystem) })]
public sealed class DrainComponent : Component, ISerializationGenerated<DrainComponent>, ISerializationGenerated
{
	public const string SolutionName = "drainBuffer";

	public static readonly ProtoId<TagPrototype> PlungerTag = ProtoId<TagPrototype>.op_Implicit("Plunger");

	[ViewVariables]
	public Entity<SolutionComponent>? Solution;

	[DataField(null, false, 1, false, false, null)]
	public float Accumulator;

	[DataField(null, false, 1, false, false, null)]
	public bool AutoDrain = true;

	[DataField(null, false, 1, false, false, null)]
	public float UnitsPerSecond = 6f;

	[DataField(null, false, 1, false, false, null)]
	public float UnitsDestroyedPerSecond = 3f;

	[DataField(null, false, 1, false, false, null)]
	public float Range = 2.5f;

	[DataField(null, false, 1, false, false, null)]
	public float DrainFrequency = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float UnclogDuration = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float UnclogProbability = 0.75f;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier ManualDrainSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Effects/Fluids/slosh.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier PlungerSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Items/Janitor/plunger.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier UnclogSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Effects/Fluids/glug.ogg", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DrainComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DrainComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<DrainComponent>(this, ref target, hookCtx, false, context))
		{
			float AccumulatorTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Accumulator, ref AccumulatorTemp, hookCtx, false, context))
			{
				AccumulatorTemp = Accumulator;
			}
			target.Accumulator = AccumulatorTemp;
			bool AutoDrainTemp = false;
			if (!serialization.TryCustomCopy<bool>(AutoDrain, ref AutoDrainTemp, hookCtx, false, context))
			{
				AutoDrainTemp = AutoDrain;
			}
			target.AutoDrain = AutoDrainTemp;
			float UnitsPerSecondTemp = 0f;
			if (!serialization.TryCustomCopy<float>(UnitsPerSecond, ref UnitsPerSecondTemp, hookCtx, false, context))
			{
				UnitsPerSecondTemp = UnitsPerSecond;
			}
			target.UnitsPerSecond = UnitsPerSecondTemp;
			float UnitsDestroyedPerSecondTemp = 0f;
			if (!serialization.TryCustomCopy<float>(UnitsDestroyedPerSecond, ref UnitsDestroyedPerSecondTemp, hookCtx, false, context))
			{
				UnitsDestroyedPerSecondTemp = UnitsDestroyedPerSecond;
			}
			target.UnitsDestroyedPerSecond = UnitsDestroyedPerSecondTemp;
			float RangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
			{
				RangeTemp = Range;
			}
			target.Range = RangeTemp;
			float DrainFrequencyTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DrainFrequency, ref DrainFrequencyTemp, hookCtx, false, context))
			{
				DrainFrequencyTemp = DrainFrequency;
			}
			target.DrainFrequency = DrainFrequencyTemp;
			float UnclogDurationTemp = 0f;
			if (!serialization.TryCustomCopy<float>(UnclogDuration, ref UnclogDurationTemp, hookCtx, false, context))
			{
				UnclogDurationTemp = UnclogDuration;
			}
			target.UnclogDuration = UnclogDurationTemp;
			float UnclogProbabilityTemp = 0f;
			if (!serialization.TryCustomCopy<float>(UnclogProbability, ref UnclogProbabilityTemp, hookCtx, false, context))
			{
				UnclogProbabilityTemp = UnclogProbability;
			}
			target.UnclogProbability = UnclogProbabilityTemp;
			SoundSpecifier ManualDrainSoundTemp = null;
			if (ManualDrainSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(ManualDrainSound, ref ManualDrainSoundTemp, hookCtx, true, context))
			{
				ManualDrainSoundTemp = serialization.CreateCopy<SoundSpecifier>(ManualDrainSound, hookCtx, context, false);
			}
			target.ManualDrainSound = ManualDrainSoundTemp;
			SoundSpecifier PlungerSoundTemp = null;
			if (PlungerSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(PlungerSound, ref PlungerSoundTemp, hookCtx, true, context))
			{
				PlungerSoundTemp = serialization.CreateCopy<SoundSpecifier>(PlungerSound, hookCtx, context, false);
			}
			target.PlungerSound = PlungerSoundTemp;
			SoundSpecifier UnclogSoundTemp = null;
			if (UnclogSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(UnclogSound, ref UnclogSoundTemp, hookCtx, true, context))
			{
				UnclogSoundTemp = serialization.CreateCopy<SoundSpecifier>(UnclogSound, hookCtx, context, false);
			}
			target.UnclogSound = UnclogSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DrainComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DrainComponent cast = (DrainComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DrainComponent cast = (DrainComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DrainComponent def = (DrainComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DrainComponent Instantiate()
	{
		return new DrainComponent();
	}
}
