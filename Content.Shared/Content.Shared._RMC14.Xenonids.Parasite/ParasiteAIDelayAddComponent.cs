using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Parasite;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedXenoParasiteSystem) })]
public sealed class ParasiteAIDelayAddComponent : Component, ISerializationGenerated<ParasiteAIDelayAddComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public TimeSpan DelayTime = TimeSpan.FromSeconds(420L);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan TimeToAI;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ParasiteAIDelayAddComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ParasiteAIDelayAddComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ParasiteAIDelayAddComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan DelayTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(DelayTime, ref DelayTimeTemp, hookCtx, false, context))
			{
				DelayTimeTemp = serialization.CreateCopy<TimeSpan>(DelayTime, hookCtx, context, false);
			}
			target.DelayTime = DelayTimeTemp;
			TimeSpan TimeToAITemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(TimeToAI, ref TimeToAITemp, hookCtx, false, context))
			{
				TimeToAITemp = serialization.CreateCopy<TimeSpan>(TimeToAI, hookCtx, context, false);
			}
			target.TimeToAI = TimeToAITemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ParasiteAIDelayAddComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ParasiteAIDelayAddComponent cast = (ParasiteAIDelayAddComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ParasiteAIDelayAddComponent cast = (ParasiteAIDelayAddComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ParasiteAIDelayAddComponent def = (ParasiteAIDelayAddComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ParasiteAIDelayAddComponent Instantiate()
	{
		return new ParasiteAIDelayAddComponent();
	}
}
