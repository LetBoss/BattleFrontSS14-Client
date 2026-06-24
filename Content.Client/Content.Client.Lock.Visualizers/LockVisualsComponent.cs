using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Lock.Visualizers;

[RegisterComponent]
[Access(new Type[] { typeof(LockVisualizerSystem) })]
public sealed class LockVisualsComponent : Component, ISerializationGenerated<LockVisualsComponent>, ISerializationGenerated
{
	[DataField("stateLocked", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string? StateLocked = "locked";

	[DataField("stateUnlocked", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string? StateUnlocked = "unlocked";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref LockVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (LockVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<LockVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			string stateLocked = null;
			if (!serialization.TryCustomCopy<string>(StateLocked, ref stateLocked, hookCtx, false, context))
			{
				stateLocked = StateLocked;
			}
			target.StateLocked = stateLocked;
			string stateUnlocked = null;
			if (!serialization.TryCustomCopy<string>(StateUnlocked, ref stateUnlocked, hookCtx, false, context))
			{
				stateUnlocked = StateUnlocked;
			}
			target.StateUnlocked = stateUnlocked;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref LockVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LockVisualsComponent target2 = (LockVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LockVisualsComponent target2 = (LockVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LockVisualsComponent target2 = (LockVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override LockVisualsComponent Instantiate()
	{
		return new LockVisualsComponent();
	}
}
