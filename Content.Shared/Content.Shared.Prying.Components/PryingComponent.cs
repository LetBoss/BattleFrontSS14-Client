using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Prying.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class PryingComponent : Component, ISerializationGenerated<PryingComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool PryPowered;

	[DataField(null, false, 1, false, false, null)]
	public bool Force;

	[DataField(null, false, 1, false, false, null)]
	public float SpeedModifier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? UseSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Items/crowbar.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public bool Enabled = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PryingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PryingComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PryingComponent>(this, ref target, hookCtx, false, context))
		{
			bool PryPoweredTemp = false;
			if (!serialization.TryCustomCopy<bool>(PryPowered, ref PryPoweredTemp, hookCtx, false, context))
			{
				PryPoweredTemp = PryPowered;
			}
			target.PryPowered = PryPoweredTemp;
			bool ForceTemp = false;
			if (!serialization.TryCustomCopy<bool>(Force, ref ForceTemp, hookCtx, false, context))
			{
				ForceTemp = Force;
			}
			target.Force = ForceTemp;
			float SpeedModifierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SpeedModifier, ref SpeedModifierTemp, hookCtx, false, context))
			{
				SpeedModifierTemp = SpeedModifier;
			}
			target.SpeedModifier = SpeedModifierTemp;
			SoundSpecifier UseSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(UseSound, ref UseSoundTemp, hookCtx, true, context))
			{
				UseSoundTemp = serialization.CreateCopy<SoundSpecifier>(UseSound, hookCtx, context, false);
			}
			target.UseSound = UseSoundTemp;
			bool EnabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Enabled, ref EnabledTemp, hookCtx, false, context))
			{
				EnabledTemp = Enabled;
			}
			target.Enabled = EnabledTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PryingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PryingComponent cast = (PryingComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PryingComponent cast = (PryingComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PryingComponent def = (PryingComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PryingComponent Instantiate()
	{
		return new PryingComponent();
	}
}
