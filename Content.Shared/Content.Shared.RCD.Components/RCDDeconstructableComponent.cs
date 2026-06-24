using System;
using Content.Shared.RCD.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.RCD.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(RCDSystem) })]
public sealed class RCDDeconstructableComponent : Component, ISerializationGenerated<RCDDeconstructableComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int Cost = 1;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float Delay = 1f;

	[DataField("fx", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public EntProtoId? Effect;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool Deconstructable = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RCDDeconstructableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RCDDeconstructableComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RCDDeconstructableComponent>(this, ref target, hookCtx, false, context))
		{
			int CostTemp = 0;
			if (!serialization.TryCustomCopy<int>(Cost, ref CostTemp, hookCtx, false, context))
			{
				CostTemp = Cost;
			}
			target.Cost = CostTemp;
			float DelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Delay, ref DelayTemp, hookCtx, false, context))
			{
				DelayTemp = Delay;
			}
			target.Delay = DelayTemp;
			EntProtoId? EffectTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(Effect, ref EffectTemp, hookCtx, false, context))
			{
				EffectTemp = serialization.CreateCopy<EntProtoId?>(Effect, hookCtx, context, false);
			}
			target.Effect = EffectTemp;
			bool DeconstructableTemp = false;
			if (!serialization.TryCustomCopy<bool>(Deconstructable, ref DeconstructableTemp, hookCtx, false, context))
			{
				DeconstructableTemp = Deconstructable;
			}
			target.Deconstructable = DeconstructableTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RCDDeconstructableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RCDDeconstructableComponent cast = (RCDDeconstructableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RCDDeconstructableComponent cast = (RCDDeconstructableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RCDDeconstructableComponent def = (RCDDeconstructableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RCDDeconstructableComponent Instantiate()
	{
		return new RCDDeconstructableComponent();
	}
}
