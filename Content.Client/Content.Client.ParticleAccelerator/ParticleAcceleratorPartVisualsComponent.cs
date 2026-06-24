using System;
using System.Collections.Generic;
using Content.Shared.Singularity.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Client.ParticleAccelerator;

[RegisterComponent]
[Access(new Type[] { typeof(ParticleAcceleratorPartVisualizerSystem) })]
public sealed class ParticleAcceleratorPartVisualsComponent : Component, ISerializationGenerated<ParticleAcceleratorPartVisualsComponent>, ISerializationGenerated
{
	[DataField("stateBase", false, 1, true, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string StateBase;

	[DataField("stateSuffixes", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Dictionary<ParticleAcceleratorVisualState, string> StatesSuffixes = new Dictionary<ParticleAcceleratorVisualState, string>
	{
		{
			ParticleAcceleratorVisualState.Powered,
			"p"
		},
		{
			ParticleAcceleratorVisualState.Level0,
			"p0"
		},
		{
			ParticleAcceleratorVisualState.Level1,
			"p1"
		},
		{
			ParticleAcceleratorVisualState.Level2,
			"p2"
		},
		{
			ParticleAcceleratorVisualState.Level3,
			"p3"
		}
	};

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ParticleAcceleratorPartVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (ParticleAcceleratorPartVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<ParticleAcceleratorPartVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			string stateBase = null;
			if (StateBase == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(StateBase, ref stateBase, hookCtx, false, context))
			{
				stateBase = StateBase;
			}
			target.StateBase = stateBase;
			Dictionary<ParticleAcceleratorVisualState, string> statesSuffixes = null;
			if (StatesSuffixes == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<ParticleAcceleratorVisualState, string>>(StatesSuffixes, ref statesSuffixes, hookCtx, true, context))
			{
				statesSuffixes = serialization.CreateCopy<Dictionary<ParticleAcceleratorVisualState, string>>(StatesSuffixes, hookCtx, context, false);
			}
			target.StatesSuffixes = statesSuffixes;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ParticleAcceleratorPartVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ParticleAcceleratorPartVisualsComponent target2 = (ParticleAcceleratorPartVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ParticleAcceleratorPartVisualsComponent target2 = (ParticleAcceleratorPartVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ParticleAcceleratorPartVisualsComponent target2 = (ParticleAcceleratorPartVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ParticleAcceleratorPartVisualsComponent Instantiate()
	{
		return new ParticleAcceleratorPartVisualsComponent();
	}
}
