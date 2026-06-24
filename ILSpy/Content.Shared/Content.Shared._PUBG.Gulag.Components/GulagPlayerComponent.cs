using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.Gulag.Components;

[RegisterComponent]
public sealed class GulagPlayerComponent : Component, ISerializationGenerated<GulagPlayerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public GulagPlayerState State;

	[DataField(null, false, 1, false, false, null)]
	public int? FrozenPlacement;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan? GhostProtectionExpiry;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GulagPlayerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GulagPlayerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GulagPlayerComponent>(this, ref target, hookCtx, false, context))
		{
			GulagPlayerState StateTemp = GulagPlayerState.Queued;
			if (!serialization.TryCustomCopy<GulagPlayerState>(State, ref StateTemp, hookCtx, false, context))
			{
				StateTemp = State;
			}
			target.State = StateTemp;
			int? FrozenPlacementTemp = null;
			if (!serialization.TryCustomCopy<int?>(FrozenPlacement, ref FrozenPlacementTemp, hookCtx, false, context))
			{
				FrozenPlacementTemp = FrozenPlacement;
			}
			target.FrozenPlacement = FrozenPlacementTemp;
			TimeSpan? GhostProtectionExpiryTemp = null;
			if (!serialization.TryCustomCopy<TimeSpan?>(GhostProtectionExpiry, ref GhostProtectionExpiryTemp, hookCtx, false, context))
			{
				GhostProtectionExpiryTemp = serialization.CreateCopy<TimeSpan?>(GhostProtectionExpiry, hookCtx, context, false);
			}
			target.GhostProtectionExpiry = GhostProtectionExpiryTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GulagPlayerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GulagPlayerComponent cast = (GulagPlayerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GulagPlayerComponent cast = (GulagPlayerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GulagPlayerComponent def = (GulagPlayerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GulagPlayerComponent Instantiate()
	{
		return new GulagPlayerComponent();
	}
}
