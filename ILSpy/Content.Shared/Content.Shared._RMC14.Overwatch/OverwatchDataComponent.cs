using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Overwatch;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedOverwatchConsoleSystem) })]
public sealed class OverwatchDataComponent : Component, ISerializationGenerated<OverwatchDataComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public OverwatchMarine? Marine;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref OverwatchDataComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (OverwatchDataComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<OverwatchDataComponent>(this, ref target, hookCtx, false, context))
		{
			OverwatchMarine? MarineTemp = null;
			if (!serialization.TryCustomCopy<OverwatchMarine?>(Marine, ref MarineTemp, hookCtx, false, context))
			{
				MarineTemp = serialization.CreateCopy<OverwatchMarine?>(Marine, hookCtx, context, false);
			}
			target.Marine = MarineTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref OverwatchDataComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OverwatchDataComponent cast = (OverwatchDataComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OverwatchDataComponent cast = (OverwatchDataComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OverwatchDataComponent def = (OverwatchDataComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override OverwatchDataComponent Instantiate()
	{
		return new OverwatchDataComponent();
	}
}
