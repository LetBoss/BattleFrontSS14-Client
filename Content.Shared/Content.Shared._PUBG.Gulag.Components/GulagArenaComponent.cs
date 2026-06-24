using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.Gulag.Components;

[RegisterComponent]
public sealed class GulagArenaComponent : Component, ISerializationGenerated<GulagArenaComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public GulagArenaState State;

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Fighter1;

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Fighter2;

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? NPCInterference;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan? FightStartTime;

	[DataField(null, false, 1, false, false, null)]
	public float FightDuration = 20f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GulagArenaComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GulagArenaComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GulagArenaComponent>(this, ref target, hookCtx, false, context))
		{
			GulagArenaState StateTemp = GulagArenaState.Waiting;
			if (!serialization.TryCustomCopy<GulagArenaState>(State, ref StateTemp, hookCtx, false, context))
			{
				StateTemp = State;
			}
			target.State = StateTemp;
			EntityUid? Fighter1Temp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Fighter1, ref Fighter1Temp, hookCtx, false, context))
			{
				Fighter1Temp = serialization.CreateCopy<EntityUid?>(Fighter1, hookCtx, context, false);
			}
			target.Fighter1 = Fighter1Temp;
			EntityUid? Fighter2Temp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Fighter2, ref Fighter2Temp, hookCtx, false, context))
			{
				Fighter2Temp = serialization.CreateCopy<EntityUid?>(Fighter2, hookCtx, context, false);
			}
			target.Fighter2 = Fighter2Temp;
			EntityUid? NPCInterferenceTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(NPCInterference, ref NPCInterferenceTemp, hookCtx, false, context))
			{
				NPCInterferenceTemp = serialization.CreateCopy<EntityUid?>(NPCInterference, hookCtx, context, false);
			}
			target.NPCInterference = NPCInterferenceTemp;
			TimeSpan? FightStartTimeTemp = null;
			if (!serialization.TryCustomCopy<TimeSpan?>(FightStartTime, ref FightStartTimeTemp, hookCtx, false, context))
			{
				FightStartTimeTemp = serialization.CreateCopy<TimeSpan?>(FightStartTime, hookCtx, context, false);
			}
			target.FightStartTime = FightStartTimeTemp;
			float FightDurationTemp = 0f;
			if (!serialization.TryCustomCopy<float>(FightDuration, ref FightDurationTemp, hookCtx, false, context))
			{
				FightDurationTemp = FightDuration;
			}
			target.FightDuration = FightDurationTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GulagArenaComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GulagArenaComponent cast = (GulagArenaComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GulagArenaComponent cast = (GulagArenaComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GulagArenaComponent def = (GulagArenaComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GulagArenaComponent Instantiate()
	{
		return new GulagArenaComponent();
	}
}
