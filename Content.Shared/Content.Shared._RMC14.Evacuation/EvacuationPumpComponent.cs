using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared._RMC14.Evacuation;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedEvacuationSystem) })]
public sealed class EvacuationPumpComponent : Component, ISerializationGenerated<EvacuationPumpComponent>, ISerializationGenerated
{
	[DataField("activeSound", false, 1, true, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public SoundSpecifier ActiveSound;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EvacuationPumpComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EvacuationPumpComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<EvacuationPumpComponent>(this, ref target, hookCtx, false, context))
		{
			SoundSpecifier ActiveSoundTemp = null;
			if (ActiveSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(ActiveSound, ref ActiveSoundTemp, hookCtx, true, context))
			{
				ActiveSoundTemp = serialization.CreateCopy<SoundSpecifier>(ActiveSound, hookCtx, context, false);
			}
			target.ActiveSound = ActiveSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EvacuationPumpComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EvacuationPumpComponent cast = (EvacuationPumpComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EvacuationPumpComponent cast = (EvacuationPumpComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EvacuationPumpComponent def = (EvacuationPumpComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EvacuationPumpComponent Instantiate()
	{
		return new EvacuationPumpComponent();
	}
}
