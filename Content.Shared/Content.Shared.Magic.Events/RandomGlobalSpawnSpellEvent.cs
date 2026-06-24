using System;
using System.Collections.Generic;
using Content.Shared.Actions;
using Content.Shared.Storage;
using Robust.Shared.Audio;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Magic.Events;

public sealed class RandomGlobalSpawnSpellEvent : InstantActionEvent, ISerializationGenerated<RandomGlobalSpawnSpellEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<EntitySpawnEntry> Spawns = new List<EntitySpawnEntry>();

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier Sound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Magic/staff_animation.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public bool MakeSurvivorAntagonist;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RandomGlobalSpawnSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		InstantActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RandomGlobalSpawnSpellEvent)definitionCast;
		if (!serialization.TryCustomCopy<RandomGlobalSpawnSpellEvent>(this, ref target, hookCtx, false, context))
		{
			List<EntitySpawnEntry> SpawnsTemp = null;
			if (Spawns == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<EntitySpawnEntry>>(Spawns, ref SpawnsTemp, hookCtx, true, context))
			{
				SpawnsTemp = serialization.CreateCopy<List<EntitySpawnEntry>>(Spawns, hookCtx, context, false);
			}
			target.Spawns = SpawnsTemp;
			SoundSpecifier SoundTemp = null;
			if (Sound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(Sound, ref SoundTemp, hookCtx, true, context))
			{
				SoundTemp = serialization.CreateCopy<SoundSpecifier>(Sound, hookCtx, context, false);
			}
			target.Sound = SoundTemp;
			bool MakeSurvivorAntagonistTemp = false;
			if (!serialization.TryCustomCopy<bool>(MakeSurvivorAntagonist, ref MakeSurvivorAntagonistTemp, hookCtx, false, context))
			{
				MakeSurvivorAntagonistTemp = MakeSurvivorAntagonist;
			}
			target.MakeSurvivorAntagonist = MakeSurvivorAntagonistTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RandomGlobalSpawnSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomGlobalSpawnSpellEvent cast = (RandomGlobalSpawnSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomGlobalSpawnSpellEvent cast = (RandomGlobalSpawnSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RandomGlobalSpawnSpellEvent Instantiate()
	{
		return new RandomGlobalSpawnSpellEvent();
	}
}
