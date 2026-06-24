using System;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Glue;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedGlueSystem) })]
public sealed class GlueComponent : Component, ISerializationGenerated<GlueComponent>, ISerializationGenerated
{
	[DataField("squeeze", false, 1, false, false, null)]
	public SoundSpecifier Squeeze = (SoundSpecifier)new SoundPathSpecifier("/Audio/Items/squeezebottle.ogg", (AudioParams?)null);

	[DataField("solution", false, 1, false, false, null)]
	public string Solution = "drink";

	[DataField("reagent", false, 1, false, false, typeof(PrototypeIdSerializer<ReagentPrototype>))]
	public string Reagent = "SpaceGlue";

	[DataField("consumptionUnit", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public FixedPoint2 ConsumptionUnit = FixedPoint2.New(5);

	[DataField("durationPerUnit", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan DurationPerUnit = TimeSpan.FromSeconds(6L);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GlueComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GlueComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GlueComponent>(this, ref target, hookCtx, false, context))
		{
			SoundSpecifier SqueezeTemp = null;
			if (Squeeze == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(Squeeze, ref SqueezeTemp, hookCtx, true, context))
			{
				SqueezeTemp = serialization.CreateCopy<SoundSpecifier>(Squeeze, hookCtx, context, false);
			}
			target.Squeeze = SqueezeTemp;
			string SolutionTemp = null;
			if (Solution == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Solution, ref SolutionTemp, hookCtx, false, context))
			{
				SolutionTemp = Solution;
			}
			target.Solution = SolutionTemp;
			string ReagentTemp = null;
			if (Reagent == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Reagent, ref ReagentTemp, hookCtx, false, context))
			{
				ReagentTemp = Reagent;
			}
			target.Reagent = ReagentTemp;
			FixedPoint2 ConsumptionUnitTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(ConsumptionUnit, ref ConsumptionUnitTemp, hookCtx, false, context))
			{
				ConsumptionUnitTemp = serialization.CreateCopy<FixedPoint2>(ConsumptionUnit, hookCtx, context, false);
			}
			target.ConsumptionUnit = ConsumptionUnitTemp;
			TimeSpan DurationPerUnitTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(DurationPerUnit, ref DurationPerUnitTemp, hookCtx, false, context))
			{
				DurationPerUnitTemp = serialization.CreateCopy<TimeSpan>(DurationPerUnit, hookCtx, context, false);
			}
			target.DurationPerUnit = DurationPerUnitTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GlueComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GlueComponent cast = (GlueComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GlueComponent cast = (GlueComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GlueComponent def = (GlueComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GlueComponent Instantiate()
	{
		return new GlueComponent();
	}
}
