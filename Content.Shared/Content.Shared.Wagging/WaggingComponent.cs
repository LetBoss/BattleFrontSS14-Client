using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Wagging;

[RegisterComponent]
[NetworkedComponent]
public sealed class WaggingComponent : Component, ISerializationGenerated<WaggingComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Action = EntProtoId.op_Implicit("ActionToggleWagging");

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? ActionEntity;

	public string Suffix = "Animated";

	[DataField(null, false, 1, false, false, null)]
	public bool Wagging;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref WaggingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
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
		target = (WaggingComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<WaggingComponent>(this, ref target, hookCtx, false, context))
		{
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
			bool WaggingTemp = false;
			if (!serialization.TryCustomCopy<bool>(Wagging, ref WaggingTemp, hookCtx, false, context))
			{
				WaggingTemp = Wagging;
			}
			target.Wagging = WaggingTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref WaggingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WaggingComponent cast = (WaggingComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WaggingComponent cast = (WaggingComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WaggingComponent def = (WaggingComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override WaggingComponent Instantiate()
	{
		return new WaggingComponent();
	}
}
