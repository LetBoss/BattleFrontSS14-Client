using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Singularity.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class SingularityComponent : Component, ISerializationGenerated<SingularityComponent>, ISerializationGenerated
{
	[Access(/*Could not decode attribute arguments.*/)]
	[DataField("level", false, 1, false, false, null)]
	public byte Level = 1;

	[Access(/*Could not decode attribute arguments.*/)]
	[DataField("radsPerLevel", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float RadsPerLevel = 2f;

	[DataField("energy", false, 1, false, false, null)]
	public float Energy = 180f;

	[DataField("energyLoss", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float EnergyDrain;

	[DataField("ambientSound", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public SoundSpecifier? AmbientSound;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public EntityUid? AmbientSoundStream;

	[DataField("formationSound", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public SoundSpecifier? FormationSound;

	[DataField("dissipationSound", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public SoundSpecifier? DissipationSound;

	public SingularityComponent()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		AudioParams val = ((AudioParams)(ref AudioParams.Default)).WithVolume(5f);
		val = ((AudioParams)(ref val)).WithLoop(true);
		AmbientSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Effects/singularity_form.ogg", (AudioParams?)((AudioParams)(ref val)).WithMaxDistance(20f));
		DissipationSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Effects/singularity_collapse.ogg", (AudioParams?)AudioParams.Default);
		((Component)this)._002Ector();
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SingularityComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SingularityComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SingularityComponent>(this, ref target, hookCtx, false, context))
		{
			byte LevelTemp = 0;
			if (!serialization.TryCustomCopy<byte>(Level, ref LevelTemp, hookCtx, false, context))
			{
				LevelTemp = Level;
			}
			target.Level = LevelTemp;
			float RadsPerLevelTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RadsPerLevel, ref RadsPerLevelTemp, hookCtx, false, context))
			{
				RadsPerLevelTemp = RadsPerLevel;
			}
			target.RadsPerLevel = RadsPerLevelTemp;
			float EnergyTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Energy, ref EnergyTemp, hookCtx, false, context))
			{
				EnergyTemp = Energy;
			}
			target.Energy = EnergyTemp;
			float EnergyDrainTemp = 0f;
			if (!serialization.TryCustomCopy<float>(EnergyDrain, ref EnergyDrainTemp, hookCtx, false, context))
			{
				EnergyDrainTemp = EnergyDrain;
			}
			target.EnergyDrain = EnergyDrainTemp;
			SoundSpecifier AmbientSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(AmbientSound, ref AmbientSoundTemp, hookCtx, true, context))
			{
				AmbientSoundTemp = serialization.CreateCopy<SoundSpecifier>(AmbientSound, hookCtx, context, false);
			}
			target.AmbientSound = AmbientSoundTemp;
			SoundSpecifier FormationSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(FormationSound, ref FormationSoundTemp, hookCtx, true, context))
			{
				FormationSoundTemp = serialization.CreateCopy<SoundSpecifier>(FormationSound, hookCtx, context, false);
			}
			target.FormationSound = FormationSoundTemp;
			SoundSpecifier DissipationSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(DissipationSound, ref DissipationSoundTemp, hookCtx, true, context))
			{
				DissipationSoundTemp = serialization.CreateCopy<SoundSpecifier>(DissipationSound, hookCtx, context, false);
			}
			target.DissipationSound = DissipationSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SingularityComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SingularityComponent cast = (SingularityComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SingularityComponent cast = (SingularityComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SingularityComponent def = (SingularityComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SingularityComponent Instantiate()
	{
		return new SingularityComponent();
	}
}
