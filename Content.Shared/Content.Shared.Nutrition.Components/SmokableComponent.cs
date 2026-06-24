using System;
using Content.Shared.FixedPoint;
using Content.Shared.Smoking;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class SmokableComponent : Component, ISerializationGenerated<SmokableComponent>, ISerializationGenerated
{
	[DataField("burntPrefix", false, 1, false, false, null)]
	public string BurntPrefix = "unlit";

	[DataField("litPrefix", false, 1, false, false, null)]
	public string LitPrefix = "lit";

	[DataField("unlitPrefix", false, 1, false, false, null)]
	public string UnlitPrefix = "unlit";

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? LightSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Effects/cig_light.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? SnuffSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Effects/cig_snuff.ogg", (AudioParams?)null);

	[DataField("solution", false, 1, false, false, null)]
	public string Solution { get; private set; } = "smokable";

	[DataField("inhaleAmount", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public FixedPoint2 InhaleAmount { get; private set; } = FixedPoint2.New(0.05f);

	[DataField("state", false, 1, false, false, null)]
	public SmokableState State { get; set; }

	[DataField("exposeTemperature", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float ExposeTemperature { get; set; }

	[DataField("exposeVolume", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float ExposeVolume { get; set; } = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SmokableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SmokableComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SmokableComponent>(this, ref target, hookCtx, false, context))
		{
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
			FixedPoint2 InhaleAmountTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(InhaleAmount, ref InhaleAmountTemp, hookCtx, false, context))
			{
				InhaleAmountTemp = serialization.CreateCopy<FixedPoint2>(InhaleAmount, hookCtx, context, false);
			}
			target.InhaleAmount = InhaleAmountTemp;
			SmokableState StateTemp = SmokableState.Unlit;
			if (!serialization.TryCustomCopy<SmokableState>(State, ref StateTemp, hookCtx, false, context))
			{
				StateTemp = State;
			}
			target.State = StateTemp;
			float ExposeTemperatureTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ExposeTemperature, ref ExposeTemperatureTemp, hookCtx, false, context))
			{
				ExposeTemperatureTemp = ExposeTemperature;
			}
			target.ExposeTemperature = ExposeTemperatureTemp;
			float ExposeVolumeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ExposeVolume, ref ExposeVolumeTemp, hookCtx, false, context))
			{
				ExposeVolumeTemp = ExposeVolume;
			}
			target.ExposeVolume = ExposeVolumeTemp;
			string BurntPrefixTemp = null;
			if (BurntPrefix == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(BurntPrefix, ref BurntPrefixTemp, hookCtx, false, context))
			{
				BurntPrefixTemp = BurntPrefix;
			}
			target.BurntPrefix = BurntPrefixTemp;
			string LitPrefixTemp = null;
			if (LitPrefix == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(LitPrefix, ref LitPrefixTemp, hookCtx, false, context))
			{
				LitPrefixTemp = LitPrefix;
			}
			target.LitPrefix = LitPrefixTemp;
			string UnlitPrefixTemp = null;
			if (UnlitPrefix == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(UnlitPrefix, ref UnlitPrefixTemp, hookCtx, false, context))
			{
				UnlitPrefixTemp = UnlitPrefix;
			}
			target.UnlitPrefix = UnlitPrefixTemp;
			SoundSpecifier LightSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(LightSound, ref LightSoundTemp, hookCtx, true, context))
			{
				LightSoundTemp = serialization.CreateCopy<SoundSpecifier>(LightSound, hookCtx, context, false);
			}
			target.LightSound = LightSoundTemp;
			SoundSpecifier SnuffSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(SnuffSound, ref SnuffSoundTemp, hookCtx, true, context))
			{
				SnuffSoundTemp = serialization.CreateCopy<SoundSpecifier>(SnuffSound, hookCtx, context, false);
			}
			target.SnuffSound = SnuffSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SmokableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SmokableComponent cast = (SmokableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SmokableComponent cast = (SmokableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SmokableComponent def = (SmokableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SmokableComponent Instantiate()
	{
		return new SmokableComponent();
	}
}
