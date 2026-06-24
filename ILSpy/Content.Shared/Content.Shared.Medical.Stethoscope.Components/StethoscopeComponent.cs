using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Medical.Stethoscope.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class StethoscopeComponent : Component, ISerializationGenerated<StethoscopeComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public TimeSpan Delay = TimeSpan.FromSeconds(1.75);

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2? LastMeasuredDamage;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Action = EntProtoId.op_Implicit("ActionStethoscope");

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? ActionEntity;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StethoscopeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StethoscopeComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StethoscopeComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan DelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Delay, ref DelayTemp, hookCtx, false, context))
			{
				DelayTemp = serialization.CreateCopy<TimeSpan>(Delay, hookCtx, context, false);
			}
			target.Delay = DelayTemp;
			FixedPoint2? LastMeasuredDamageTemp = null;
			if (!serialization.TryCustomCopy<FixedPoint2?>(LastMeasuredDamage, ref LastMeasuredDamageTemp, hookCtx, false, context))
			{
				LastMeasuredDamageTemp = serialization.CreateCopy<FixedPoint2?>(LastMeasuredDamage, hookCtx, context, false);
			}
			target.LastMeasuredDamage = LastMeasuredDamageTemp;
			EntProtoId ActionTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Action, ref ActionTemp, hookCtx, false, context))
			{
				ActionTemp = serialization.CreateCopy<EntProtoId>(Action, hookCtx, context, false);
			}
			target.Action = ActionTemp;
			EntityUid? ActionEntityTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(ActionEntity, ref ActionEntityTemp, hookCtx, false, context))
			{
				ActionEntityTemp = serialization.CreateCopy<EntityUid?>(ActionEntity, hookCtx, context, false);
			}
			target.ActionEntity = ActionEntityTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StethoscopeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StethoscopeComponent cast = (StethoscopeComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StethoscopeComponent cast = (StethoscopeComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StethoscopeComponent def = (StethoscopeComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StethoscopeComponent Instantiate()
	{
		return new StethoscopeComponent();
	}
}
