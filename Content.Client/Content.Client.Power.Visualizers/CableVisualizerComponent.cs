using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Power.Visualizers;

[RegisterComponent]
public sealed class CableVisualizerComponent : Component, ISerializationGenerated<CableVisualizerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string? StatePrefix;

	[DataField(null, false, 1, false, false, null)]
	public string? ExtraLayerPrefix;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CableVisualizerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (CableVisualizerComponent)(object)val;
		if (!serialization.TryCustomCopy<CableVisualizerComponent>(this, ref target, hookCtx, false, context))
		{
			string statePrefix = null;
			if (!serialization.TryCustomCopy<string>(StatePrefix, ref statePrefix, hookCtx, false, context))
			{
				statePrefix = StatePrefix;
			}
			target.StatePrefix = statePrefix;
			string extraLayerPrefix = null;
			if (!serialization.TryCustomCopy<string>(ExtraLayerPrefix, ref extraLayerPrefix, hookCtx, false, context))
			{
				extraLayerPrefix = ExtraLayerPrefix;
			}
			target.ExtraLayerPrefix = extraLayerPrefix;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CableVisualizerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CableVisualizerComponent target2 = (CableVisualizerComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CableVisualizerComponent target2 = (CableVisualizerComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CableVisualizerComponent target2 = (CableVisualizerComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CableVisualizerComponent Instantiate()
	{
		return new CableVisualizerComponent();
	}
}
