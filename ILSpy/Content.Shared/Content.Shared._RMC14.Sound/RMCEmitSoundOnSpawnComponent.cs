using System;
using Content.Shared.Sound.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Sound;

[RegisterComponent]
public sealed class RMCEmitSoundOnSpawnComponent : BaseEmitSoundComponent, ISerializationGenerated<RMCEmitSoundOnSpawnComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Entity;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCEmitSoundOnSpawnComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BaseEmitSoundComponent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCEmitSoundOnSpawnComponent)definitionCast;
		if (!serialization.TryCustomCopy<RMCEmitSoundOnSpawnComponent>(this, ref target, hookCtx, false, context))
		{
			EntityUid? EntityTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Entity, ref EntityTemp, hookCtx, false, context))
			{
				EntityTemp = serialization.CreateCopy<EntityUid?>(Entity, hookCtx, context, false);
			}
			target.Entity = EntityTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCEmitSoundOnSpawnComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref BaseEmitSoundComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCEmitSoundOnSpawnComponent cast = (RMCEmitSoundOnSpawnComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCEmitSoundOnSpawnComponent cast = (RMCEmitSoundOnSpawnComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCEmitSoundOnSpawnComponent def = (RMCEmitSoundOnSpawnComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCEmitSoundOnSpawnComponent Instantiate()
	{
		return new RMCEmitSoundOnSpawnComponent();
	}
}
