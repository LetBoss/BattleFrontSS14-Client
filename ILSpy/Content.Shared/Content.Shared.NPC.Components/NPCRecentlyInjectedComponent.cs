using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.NPC.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class NPCRecentlyInjectedComponent : Component, ISerializationGenerated<NPCRecentlyInjectedComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("accumulator", false, 1, false, false, null)]
	public float Accumulator;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("removeTime", false, 1, false, false, null)]
	public TimeSpan RemoveTime = TimeSpan.FromMinutes(1L);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NPCRecentlyInjectedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (NPCRecentlyInjectedComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<NPCRecentlyInjectedComponent>(this, ref target, hookCtx, false, context))
		{
			float AccumulatorTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Accumulator, ref AccumulatorTemp, hookCtx, false, context))
			{
				AccumulatorTemp = Accumulator;
			}
			target.Accumulator = AccumulatorTemp;
			TimeSpan RemoveTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(RemoveTime, ref RemoveTimeTemp, hookCtx, false, context))
			{
				RemoveTimeTemp = serialization.CreateCopy<TimeSpan>(RemoveTime, hookCtx, context, false);
			}
			target.RemoveTime = RemoveTimeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NPCRecentlyInjectedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NPCRecentlyInjectedComponent cast = (NPCRecentlyInjectedComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NPCRecentlyInjectedComponent cast = (NPCRecentlyInjectedComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NPCRecentlyInjectedComponent def = (NPCRecentlyInjectedComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NPCRecentlyInjectedComponent Instantiate()
	{
		return new NPCRecentlyInjectedComponent();
	}
}
