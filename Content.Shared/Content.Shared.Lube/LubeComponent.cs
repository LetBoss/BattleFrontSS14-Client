using System;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Lube;

[RegisterComponent]
[NetworkedComponent]
public sealed class LubeComponent : Component, ISerializationGenerated<LubeComponent>, ISerializationGenerated
{
	[DataField("squeeze", false, 1, false, false, null)]
	public SoundSpecifier Squeeze = (SoundSpecifier)new SoundPathSpecifier("/Audio/Items/squeezebottle.ogg", (AudioParams?)null);

	[DataField("solution", false, 1, false, false, null)]
	public string Solution = "drink";

	[DataField("reagent", false, 1, false, false, typeof(PrototypeIdSerializer<ReagentPrototype>))]
	public string Reagent = "SpaceLube";

	[DataField("consumption", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public FixedPoint2 Consumption = FixedPoint2.New(3);

	[DataField("minSlips", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int MinSlips = 1;

	[DataField("maxSlips", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int MaxSlips = 6;

	[DataField("slipStrength", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int SlipStrength = 10;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref LubeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (LubeComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<LubeComponent>(this, ref target, hookCtx, false, context))
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
			FixedPoint2 ConsumptionTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(Consumption, ref ConsumptionTemp, hookCtx, false, context))
			{
				ConsumptionTemp = serialization.CreateCopy<FixedPoint2>(Consumption, hookCtx, context, false);
			}
			target.Consumption = ConsumptionTemp;
			int MinSlipsTemp = 0;
			if (!serialization.TryCustomCopy<int>(MinSlips, ref MinSlipsTemp, hookCtx, false, context))
			{
				MinSlipsTemp = MinSlips;
			}
			target.MinSlips = MinSlipsTemp;
			int MaxSlipsTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxSlips, ref MaxSlipsTemp, hookCtx, false, context))
			{
				MaxSlipsTemp = MaxSlips;
			}
			target.MaxSlips = MaxSlipsTemp;
			int SlipStrengthTemp = 0;
			if (!serialization.TryCustomCopy<int>(SlipStrength, ref SlipStrengthTemp, hookCtx, false, context))
			{
				SlipStrengthTemp = SlipStrength;
			}
			target.SlipStrength = SlipStrengthTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref LubeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LubeComponent cast = (LubeComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LubeComponent cast = (LubeComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LubeComponent def = (LubeComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override LubeComponent Instantiate()
	{
		return new LubeComponent();
	}
}
