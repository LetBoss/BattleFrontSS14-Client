using System;
using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._PUBG.Emote;

public sealed class PubgEmoteActionEvent : InstantActionEvent, ISerializationGenerated<PubgEmoteActionEvent>, ISerializationGenerated
{
	public string EmoteId = string.Empty;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgEmoteActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstantActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PubgEmoteActionEvent)definitionCast;
		serialization.TryCustomCopy<PubgEmoteActionEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgEmoteActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgEmoteActionEvent cast = (PubgEmoteActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgEmoteActionEvent cast = (PubgEmoteActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgEmoteActionEvent Instantiate()
	{
		return new PubgEmoteActionEvent();
	}
}
