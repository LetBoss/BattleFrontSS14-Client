using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Climbing.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ClimbableComponent : Component, ISerializationGenerated<ClimbableComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Range = 1.5f;

	[DataField(null, false, 1, false, false, null)]
	public bool Vaultable = true;

	[DataField("delay", false, 1, false, false, null)]
	public float ClimbDelay = 1.5f;

	[DataField(null, false, 1, false, false, null)]
	public float VaultPastDistance;

	[DataField("startClimbSound", false, 1, false, false, null)]
	public SoundSpecifier? StartClimbSound;

	[DataField("finishClimbSound", false, 1, false, false, null)]
	public SoundSpecifier? FinishClimbSound;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ClimbableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ClimbableComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ClimbableComponent>(this, ref target, hookCtx, false, context))
		{
			float RangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
			{
				RangeTemp = Range;
			}
			target.Range = RangeTemp;
			bool VaultableTemp = false;
			if (!serialization.TryCustomCopy<bool>(Vaultable, ref VaultableTemp, hookCtx, false, context))
			{
				VaultableTemp = Vaultable;
			}
			target.Vaultable = VaultableTemp;
			float ClimbDelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ClimbDelay, ref ClimbDelayTemp, hookCtx, false, context))
			{
				ClimbDelayTemp = ClimbDelay;
			}
			target.ClimbDelay = ClimbDelayTemp;
			float VaultPastDistanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(VaultPastDistance, ref VaultPastDistanceTemp, hookCtx, false, context))
			{
				VaultPastDistanceTemp = VaultPastDistance;
			}
			target.VaultPastDistance = VaultPastDistanceTemp;
			SoundSpecifier StartClimbSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(StartClimbSound, ref StartClimbSoundTemp, hookCtx, true, context))
			{
				StartClimbSoundTemp = serialization.CreateCopy<SoundSpecifier>(StartClimbSound, hookCtx, context, false);
			}
			target.StartClimbSound = StartClimbSoundTemp;
			SoundSpecifier FinishClimbSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(FinishClimbSound, ref FinishClimbSoundTemp, hookCtx, true, context))
			{
				FinishClimbSoundTemp = serialization.CreateCopy<SoundSpecifier>(FinishClimbSound, hookCtx, context, false);
			}
			target.FinishClimbSound = FinishClimbSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ClimbableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClimbableComponent cast = (ClimbableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClimbableComponent cast = (ClimbableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClimbableComponent def = (ClimbableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ClimbableComponent Instantiate()
	{
		return new ClimbableComponent();
	}
}
