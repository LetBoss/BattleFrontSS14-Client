using System;
using Content.Shared.Research.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Research.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedResearchStealerSystem) })]
public sealed class ResearchStealerComponent : Component, ISerializationGenerated<ResearchStealerComponent>, ISerializationGenerated
{
	[DataField("delay", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan Delay = TimeSpan.FromSeconds(20L);

	[DataField(null, false, 1, false, false, null)]
	public int MinToSteal = 4;

	[DataField(null, false, 1, false, false, null)]
	public int MaxToSteal = 8;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ResearchStealerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ResearchStealerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ResearchStealerComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan DelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Delay, ref DelayTemp, hookCtx, false, context))
			{
				DelayTemp = serialization.CreateCopy<TimeSpan>(Delay, hookCtx, context, false);
			}
			target.Delay = DelayTemp;
			int MinToStealTemp = 0;
			if (!serialization.TryCustomCopy<int>(MinToSteal, ref MinToStealTemp, hookCtx, false, context))
			{
				MinToStealTemp = MinToSteal;
			}
			target.MinToSteal = MinToStealTemp;
			int MaxToStealTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxToSteal, ref MaxToStealTemp, hookCtx, false, context))
			{
				MaxToStealTemp = MaxToSteal;
			}
			target.MaxToSteal = MaxToStealTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ResearchStealerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ResearchStealerComponent cast = (ResearchStealerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ResearchStealerComponent cast = (ResearchStealerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ResearchStealerComponent def = (ResearchStealerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ResearchStealerComponent Instantiate()
	{
		return new ResearchStealerComponent();
	}
}
