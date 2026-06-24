using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._PUBG.Gulag.Components;

[RegisterComponent]
public sealed class GulagQueueComponent : Component, ISerializationGenerated<GulagQueueComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<EntityUid> QueuedPlayers = new List<EntityUid>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<EntityUid, TimeSpan> QueueJoinTime = new Dictionary<EntityUid, TimeSpan>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GulagQueueComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GulagQueueComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GulagQueueComponent>(this, ref target, hookCtx, false, context))
		{
			List<EntityUid> QueuedPlayersTemp = null;
			if (QueuedPlayers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<EntityUid>>(QueuedPlayers, ref QueuedPlayersTemp, hookCtx, true, context))
			{
				QueuedPlayersTemp = serialization.CreateCopy<List<EntityUid>>(QueuedPlayers, hookCtx, context, false);
			}
			target.QueuedPlayers = QueuedPlayersTemp;
			Dictionary<EntityUid, TimeSpan> QueueJoinTimeTemp = null;
			if (QueueJoinTime == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<EntityUid, TimeSpan>>(QueueJoinTime, ref QueueJoinTimeTemp, hookCtx, true, context))
			{
				QueueJoinTimeTemp = serialization.CreateCopy<Dictionary<EntityUid, TimeSpan>>(QueueJoinTime, hookCtx, context, false);
			}
			target.QueueJoinTime = QueueJoinTimeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GulagQueueComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GulagQueueComponent cast = (GulagQueueComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GulagQueueComponent cast = (GulagQueueComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GulagQueueComponent def = (GulagQueueComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GulagQueueComponent Instantiate()
	{
		return new GulagQueueComponent();
	}
}
