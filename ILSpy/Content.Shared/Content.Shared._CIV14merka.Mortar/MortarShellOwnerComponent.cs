using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._CIV14merka.Mortar;

[RegisterComponent]
public sealed class MortarShellOwnerComponent : Component, ISerializationGenerated<MortarShellOwnerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityUid? User;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? HitSound;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan HitDelay = TimeSpan.FromSeconds(0.35);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MortarShellOwnerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MortarShellOwnerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<MortarShellOwnerComponent>(this, ref target, hookCtx, false, context))
		{
			EntityUid? UserTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(User, ref UserTemp, hookCtx, false, context))
			{
				UserTemp = serialization.CreateCopy<EntityUid?>(User, hookCtx, context, false);
			}
			target.User = UserTemp;
			SoundSpecifier HitSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(HitSound, ref HitSoundTemp, hookCtx, true, context))
			{
				HitSoundTemp = serialization.CreateCopy<SoundSpecifier>(HitSound, hookCtx, context, false);
			}
			target.HitSound = HitSoundTemp;
			TimeSpan HitDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(HitDelay, ref HitDelayTemp, hookCtx, false, context))
			{
				HitDelayTemp = serialization.CreateCopy<TimeSpan>(HitDelay, hookCtx, context, false);
			}
			target.HitDelay = HitDelayTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MortarShellOwnerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MortarShellOwnerComponent cast = (MortarShellOwnerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MortarShellOwnerComponent cast = (MortarShellOwnerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MortarShellOwnerComponent def = (MortarShellOwnerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MortarShellOwnerComponent Instantiate()
	{
		return new MortarShellOwnerComponent();
	}
}
