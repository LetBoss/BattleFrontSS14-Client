using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Power.Generator;

[RegisterComponent]
[Access(new Type[] { typeof(SharedPortableGeneratorSystem) })]
public sealed class PortableGeneratorComponent : Component, ISerializationGenerated<PortableGeneratorComponent>, ISerializationGenerated
{
	[DataField("startChance", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float StartChance { get; set; } = 1f;

	[DataField("startTime", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan StartTime { get; set; } = TimeSpan.FromSeconds(2L);

	[DataField("startSound", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public SoundSpecifier? StartSound { get; set; }

	[DataField("startSoundEmpty", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public SoundSpecifier? StartSoundEmpty { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PortableGeneratorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PortableGeneratorComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PortableGeneratorComponent>(this, ref target, hookCtx, false, context))
		{
			float StartChanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(StartChance, ref StartChanceTemp, hookCtx, false, context))
			{
				StartChanceTemp = StartChance;
			}
			target.StartChance = StartChanceTemp;
			TimeSpan StartTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(StartTime, ref StartTimeTemp, hookCtx, false, context))
			{
				StartTimeTemp = serialization.CreateCopy<TimeSpan>(StartTime, hookCtx, context, false);
			}
			target.StartTime = StartTimeTemp;
			SoundSpecifier StartSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(StartSound, ref StartSoundTemp, hookCtx, true, context))
			{
				StartSoundTemp = serialization.CreateCopy<SoundSpecifier>(StartSound, hookCtx, context, false);
			}
			target.StartSound = StartSoundTemp;
			SoundSpecifier StartSoundEmptyTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(StartSoundEmpty, ref StartSoundEmptyTemp, hookCtx, true, context))
			{
				StartSoundEmptyTemp = serialization.CreateCopy<SoundSpecifier>(StartSoundEmpty, hookCtx, context, false);
			}
			target.StartSoundEmpty = StartSoundEmptyTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PortableGeneratorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PortableGeneratorComponent cast = (PortableGeneratorComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PortableGeneratorComponent cast = (PortableGeneratorComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PortableGeneratorComponent def = (PortableGeneratorComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PortableGeneratorComponent Instantiate()
	{
		return new PortableGeneratorComponent();
	}
}
