using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client._RMC14.Overwatch;

[RegisterComponent]
[Access(new Type[] { typeof(OverwatchConsoleSystem) })]
public sealed class OverwatchRelayedSoundComponent : Component, ISerializationGenerated<OverwatchRelayedSoundComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Relay;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref OverwatchRelayedSoundComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (OverwatchRelayedSoundComponent)(object)val;
		if (!serialization.TryCustomCopy<OverwatchRelayedSoundComponent>(this, ref target, hookCtx, false, context))
		{
			EntityUid? relay = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Relay, ref relay, hookCtx, false, context))
			{
				relay = serialization.CreateCopy<EntityUid?>(Relay, hookCtx, context, false);
			}
			target.Relay = relay;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref OverwatchRelayedSoundComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OverwatchRelayedSoundComponent target2 = (OverwatchRelayedSoundComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OverwatchRelayedSoundComponent target2 = (OverwatchRelayedSoundComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OverwatchRelayedSoundComponent target2 = (OverwatchRelayedSoundComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override OverwatchRelayedSoundComponent Instantiate()
	{
		return new OverwatchRelayedSoundComponent();
	}
}
