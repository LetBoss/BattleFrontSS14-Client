using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Light.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class LightBulbComponent : Component, ISerializationGenerated<LightBulbComponent>, ISerializationGenerated
{
	[DataField("color", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Color Color = Color.White;

	[DataField("bulb", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public LightBulbType Type = LightBulbType.Tube;

	[DataField("startingState", false, 1, false, false, null)]
	public LightBulbState State;

	[DataField("BurningTemperature", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int BurningTemperature = 1400;

	[DataField("lightEnergy", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float LightEnergy = 0.8f;

	[DataField("lightRadius", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float LightRadius = 10f;

	[DataField("lightSoftness", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float LightSoftness = 1f;

	[DataField("PowerUse", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int PowerUse = 60;

	[DataField("breakSound", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public SoundSpecifier BreakSound = (SoundSpecifier)new SoundCollectionSpecifier("GlassBreak", (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(-6f));

	[DataField("normalSpriteState", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string NormalSpriteState = "normal";

	[DataField("brokenSpriteState", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string BrokenSpriteState = "broken";

	[DataField("burnedSpriteState", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string BurnedSpriteState = "burned";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref LightBulbComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (LightBulbComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<LightBulbComponent>(this, ref target, hookCtx, false, context))
		{
			Color ColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(Color, ref ColorTemp, hookCtx, false, context))
			{
				ColorTemp = serialization.CreateCopy<Color>(Color, hookCtx, context, false);
			}
			target.Color = ColorTemp;
			LightBulbType TypeTemp = LightBulbType.Bulb;
			if (!serialization.TryCustomCopy<LightBulbType>(Type, ref TypeTemp, hookCtx, false, context))
			{
				TypeTemp = Type;
			}
			target.Type = TypeTemp;
			LightBulbState StateTemp = LightBulbState.Normal;
			if (!serialization.TryCustomCopy<LightBulbState>(State, ref StateTemp, hookCtx, false, context))
			{
				StateTemp = State;
			}
			target.State = StateTemp;
			int BurningTemperatureTemp = 0;
			if (!serialization.TryCustomCopy<int>(BurningTemperature, ref BurningTemperatureTemp, hookCtx, false, context))
			{
				BurningTemperatureTemp = BurningTemperature;
			}
			target.BurningTemperature = BurningTemperatureTemp;
			float LightEnergyTemp = 0f;
			if (!serialization.TryCustomCopy<float>(LightEnergy, ref LightEnergyTemp, hookCtx, false, context))
			{
				LightEnergyTemp = LightEnergy;
			}
			target.LightEnergy = LightEnergyTemp;
			float LightRadiusTemp = 0f;
			if (!serialization.TryCustomCopy<float>(LightRadius, ref LightRadiusTemp, hookCtx, false, context))
			{
				LightRadiusTemp = LightRadius;
			}
			target.LightRadius = LightRadiusTemp;
			float LightSoftnessTemp = 0f;
			if (!serialization.TryCustomCopy<float>(LightSoftness, ref LightSoftnessTemp, hookCtx, false, context))
			{
				LightSoftnessTemp = LightSoftness;
			}
			target.LightSoftness = LightSoftnessTemp;
			int PowerUseTemp = 0;
			if (!serialization.TryCustomCopy<int>(PowerUse, ref PowerUseTemp, hookCtx, false, context))
			{
				PowerUseTemp = PowerUse;
			}
			target.PowerUse = PowerUseTemp;
			SoundSpecifier BreakSoundTemp = null;
			if (BreakSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(BreakSound, ref BreakSoundTemp, hookCtx, true, context))
			{
				BreakSoundTemp = serialization.CreateCopy<SoundSpecifier>(BreakSound, hookCtx, context, false);
			}
			target.BreakSound = BreakSoundTemp;
			string NormalSpriteStateTemp = null;
			if (NormalSpriteState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(NormalSpriteState, ref NormalSpriteStateTemp, hookCtx, false, context))
			{
				NormalSpriteStateTemp = NormalSpriteState;
			}
			target.NormalSpriteState = NormalSpriteStateTemp;
			string BrokenSpriteStateTemp = null;
			if (BrokenSpriteState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(BrokenSpriteState, ref BrokenSpriteStateTemp, hookCtx, false, context))
			{
				BrokenSpriteStateTemp = BrokenSpriteState;
			}
			target.BrokenSpriteState = BrokenSpriteStateTemp;
			string BurnedSpriteStateTemp = null;
			if (BurnedSpriteState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(BurnedSpriteState, ref BurnedSpriteStateTemp, hookCtx, false, context))
			{
				BurnedSpriteStateTemp = BurnedSpriteState;
			}
			target.BurnedSpriteState = BurnedSpriteStateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref LightBulbComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LightBulbComponent cast = (LightBulbComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LightBulbComponent cast = (LightBulbComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LightBulbComponent def = (LightBulbComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override LightBulbComponent Instantiate()
	{
		return new LightBulbComponent();
	}
}
