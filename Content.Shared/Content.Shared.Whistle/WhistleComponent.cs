using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Whistle;

[RegisterComponent]
[NetworkedComponent]
public sealed class WhistleComponent : Component, ISerializationGenerated<WhistleComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Effect = EntProtoId.op_Implicit("WhistleExclamation");

	[DataField(null, false, 1, false, false, null)]
	public float Distance;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref WhistleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (WhistleComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<WhistleComponent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId EffectTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Effect, ref EffectTemp, hookCtx, false, context))
			{
				EffectTemp = serialization.CreateCopy<EntProtoId>(Effect, hookCtx, context, false);
			}
			target.Effect = EffectTemp;
			float DistanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Distance, ref DistanceTemp, hookCtx, false, context))
			{
				DistanceTemp = Distance;
			}
			target.Distance = DistanceTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref WhistleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WhistleComponent cast = (WhistleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WhistleComponent cast = (WhistleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WhistleComponent def = (WhistleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override WhistleComponent Instantiate()
	{
		return new WhistleComponent();
	}
}
