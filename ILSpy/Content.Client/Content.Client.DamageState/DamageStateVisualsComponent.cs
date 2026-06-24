using System;
using System.Collections.Generic;
using Content.Shared.Mobs;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client.DamageState;

[RegisterComponent]
public sealed class DamageStateVisualsComponent : Component, ISerializationGenerated<DamageStateVisualsComponent>, ISerializationGenerated
{
	public int? OriginalDrawDepth;

	[DataField("states", false, 1, false, false, null)]
	public Dictionary<MobState, Dictionary<DamageStateVisualLayers, string>> States = new Dictionary<MobState, Dictionary<DamageStateVisualLayers, string>>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DamageStateVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (DamageStateVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<DamageStateVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<MobState, Dictionary<DamageStateVisualLayers, string>> states = null;
			if (States == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<MobState, Dictionary<DamageStateVisualLayers, string>>>(States, ref states, hookCtx, true, context))
			{
				states = serialization.CreateCopy<Dictionary<MobState, Dictionary<DamageStateVisualLayers, string>>>(States, hookCtx, context, false);
			}
			target.States = states;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DamageStateVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageStateVisualsComponent target2 = (DamageStateVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageStateVisualsComponent target2 = (DamageStateVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageStateVisualsComponent target2 = (DamageStateVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DamageStateVisualsComponent Instantiate()
	{
		return new DamageStateVisualsComponent();
	}
}
