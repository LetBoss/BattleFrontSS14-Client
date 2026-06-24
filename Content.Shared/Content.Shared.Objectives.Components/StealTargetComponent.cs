using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Objectives.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class StealTargetComponent : Component, ISerializationGenerated<StealTargetComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<StealTargetGroupPrototype> StealGroup;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StealTargetComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
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
		target = (StealTargetComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StealTargetComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<StealTargetGroupPrototype> StealGroupTemp = default(ProtoId<StealTargetGroupPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<StealTargetGroupPrototype>>(StealGroup, ref StealGroupTemp, hookCtx, false, context))
			{
				StealGroupTemp = serialization.CreateCopy<ProtoId<StealTargetGroupPrototype>>(StealGroup, hookCtx, context, false);
			}
			target.StealGroup = StealGroupTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StealTargetComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StealTargetComponent cast = (StealTargetComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StealTargetComponent cast = (StealTargetComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StealTargetComponent def = (StealTargetComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StealTargetComponent Instantiate()
	{
		return new StealTargetComponent();
	}
}
