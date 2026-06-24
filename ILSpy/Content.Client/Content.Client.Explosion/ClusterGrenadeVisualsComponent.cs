using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Explosion;

[RegisterComponent]
[Access(new Type[] { typeof(ClusterGrenadeVisualizerSystem) })]
public sealed class ClusterGrenadeVisualsComponent : Component, ISerializationGenerated<ClusterGrenadeVisualsComponent>, ISerializationGenerated
{
	[DataField("state", false, 1, false, false, null)]
	public string? State;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ClusterGrenadeVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (ClusterGrenadeVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<ClusterGrenadeVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			string state = null;
			if (!serialization.TryCustomCopy<string>(State, ref state, hookCtx, false, context))
			{
				state = State;
			}
			target.State = state;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ClusterGrenadeVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClusterGrenadeVisualsComponent target2 = (ClusterGrenadeVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClusterGrenadeVisualsComponent target2 = (ClusterGrenadeVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClusterGrenadeVisualsComponent target2 = (ClusterGrenadeVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ClusterGrenadeVisualsComponent Instantiate()
	{
		return new ClusterGrenadeVisualsComponent();
	}
}
