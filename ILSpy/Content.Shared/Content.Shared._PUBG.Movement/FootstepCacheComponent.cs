using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.Movement;

[RegisterComponent]
[NetworkedComponent]
public sealed class FootstepCacheComponent : Component, ISerializationGenerated<FootstepCacheComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Vector2i? LastTile;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? CachedSound;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan CacheTime;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan CacheDuration = TimeSpan.FromSeconds(30L);

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? CachedGridUid;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FootstepCacheComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FootstepCacheComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<FootstepCacheComponent>(this, ref target, hookCtx, false, context))
		{
			Vector2i? LastTileTemp = null;
			if (!serialization.TryCustomCopy<Vector2i?>(LastTile, ref LastTileTemp, hookCtx, false, context))
			{
				LastTileTemp = serialization.CreateCopy<Vector2i?>(LastTile, hookCtx, context, false);
			}
			target.LastTile = LastTileTemp;
			SoundSpecifier CachedSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(CachedSound, ref CachedSoundTemp, hookCtx, true, context))
			{
				CachedSoundTemp = serialization.CreateCopy<SoundSpecifier>(CachedSound, hookCtx, context, false);
			}
			target.CachedSound = CachedSoundTemp;
			TimeSpan CacheTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(CacheTime, ref CacheTimeTemp, hookCtx, false, context))
			{
				CacheTimeTemp = serialization.CreateCopy<TimeSpan>(CacheTime, hookCtx, context, false);
			}
			target.CacheTime = CacheTimeTemp;
			TimeSpan CacheDurationTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(CacheDuration, ref CacheDurationTemp, hookCtx, false, context))
			{
				CacheDurationTemp = serialization.CreateCopy<TimeSpan>(CacheDuration, hookCtx, context, false);
			}
			target.CacheDuration = CacheDurationTemp;
			EntityUid? CachedGridUidTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(CachedGridUid, ref CachedGridUidTemp, hookCtx, false, context))
			{
				CachedGridUidTemp = serialization.CreateCopy<EntityUid?>(CachedGridUid, hookCtx, context, false);
			}
			target.CachedGridUid = CachedGridUidTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FootstepCacheComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FootstepCacheComponent cast = (FootstepCacheComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FootstepCacheComponent cast = (FootstepCacheComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FootstepCacheComponent def = (FootstepCacheComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FootstepCacheComponent Instantiate()
	{
		return new FootstepCacheComponent();
	}
}
