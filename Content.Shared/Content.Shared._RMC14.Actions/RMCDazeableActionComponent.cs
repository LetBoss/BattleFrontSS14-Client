using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Actions;

[RegisterComponent]
[Access(new Type[] { typeof(SharedRMCActionsSystem) })]
public sealed class RMCDazeableActionComponent : Component, ISerializationGenerated<RMCDazeableActionComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float DurationMultiplier = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCDazeableActionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCDazeableActionComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCDazeableActionComponent>(this, ref target, hookCtx, false, context))
		{
			float DurationMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DurationMultiplier, ref DurationMultiplierTemp, hookCtx, false, context))
			{
				DurationMultiplierTemp = DurationMultiplier;
			}
			target.DurationMultiplier = DurationMultiplierTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCDazeableActionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCDazeableActionComponent cast = (RMCDazeableActionComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCDazeableActionComponent cast = (RMCDazeableActionComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCDazeableActionComponent def = (RMCDazeableActionComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCDazeableActionComponent Instantiate()
	{
		return new RMCDazeableActionComponent();
	}
}
