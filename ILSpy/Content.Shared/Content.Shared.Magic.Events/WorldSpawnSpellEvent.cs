using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Actions;
using Content.Shared.Storage;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Magic.Events;

public sealed class WorldSpawnSpellEvent : WorldTargetActionEvent, ISerializationGenerated<WorldSpawnSpellEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<EntitySpawnEntry> Prototypes = new List<EntitySpawnEntry>();

	[DataField(null, false, 1, false, false, null)]
	public Vector2 Offset;

	[DataField(null, false, 1, false, false, null)]
	public float? Lifetime;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref WorldSpawnSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		WorldTargetActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (WorldSpawnSpellEvent)definitionCast;
		if (!serialization.TryCustomCopy<WorldSpawnSpellEvent>(this, ref target, hookCtx, false, context))
		{
			List<EntitySpawnEntry> PrototypesTemp = null;
			if (Prototypes == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<EntitySpawnEntry>>(Prototypes, ref PrototypesTemp, hookCtx, true, context))
			{
				PrototypesTemp = serialization.CreateCopy<List<EntitySpawnEntry>>(Prototypes, hookCtx, context, false);
			}
			target.Prototypes = PrototypesTemp;
			Vector2 OffsetTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(Offset, ref OffsetTemp, hookCtx, false, context))
			{
				OffsetTemp = serialization.CreateCopy<Vector2>(Offset, hookCtx, context, false);
			}
			target.Offset = OffsetTemp;
			float? LifetimeTemp = null;
			if (!serialization.TryCustomCopy<float?>(Lifetime, ref LifetimeTemp, hookCtx, false, context))
			{
				LifetimeTemp = Lifetime;
			}
			target.Lifetime = LifetimeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref WorldSpawnSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref WorldTargetActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WorldSpawnSpellEvent cast = (WorldSpawnSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WorldSpawnSpellEvent cast = (WorldSpawnSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override WorldSpawnSpellEvent Instantiate()
	{
		return new WorldSpawnSpellEvent();
	}
}
