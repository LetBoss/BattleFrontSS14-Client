using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Revenant.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class RevenantOverloadedLightsComponent : Component, ISerializationGenerated<RevenantOverloadedLightsComponent>, ISerializationGenerated
{
	[ViewVariables]
	public EntityUid? Target;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float Accumulator;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float ZapDelay = 3f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float ZapRange = 4f;

	[DataField("zapBeamEntityId", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string ZapBeamEntityId = "LightningRevenant";

	public float? OriginalEnergy;

	public bool OriginalEnabled;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RevenantOverloadedLightsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RevenantOverloadedLightsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RevenantOverloadedLightsComponent>(this, ref target, hookCtx, false, context))
		{
			string ZapBeamEntityIdTemp = null;
			if (ZapBeamEntityId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ZapBeamEntityId, ref ZapBeamEntityIdTemp, hookCtx, false, context))
			{
				ZapBeamEntityIdTemp = ZapBeamEntityId;
			}
			target.ZapBeamEntityId = ZapBeamEntityIdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RevenantOverloadedLightsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RevenantOverloadedLightsComponent cast = (RevenantOverloadedLightsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RevenantOverloadedLightsComponent cast = (RevenantOverloadedLightsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RevenantOverloadedLightsComponent def = (RevenantOverloadedLightsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RevenantOverloadedLightsComponent Instantiate()
	{
		return new RevenantOverloadedLightsComponent();
	}
}
