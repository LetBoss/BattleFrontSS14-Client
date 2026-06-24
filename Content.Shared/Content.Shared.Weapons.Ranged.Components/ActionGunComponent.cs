using System;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(ActionGunSystem) })]
public sealed class ActionGunComponent : Component, ISerializationGenerated<ActionGunComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Action = EntProtoId.op_Implicit(string.Empty);

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? ActionEntity;

	[DataField(null, false, 1, true, false, null)]
	public EntProtoId GunProto = EntProtoId.op_Implicit(string.Empty);

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Gun;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ActionGunComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ActionGunComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ActionGunComponent>(this, ref target, hookCtx, false, context))
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
			EntProtoId GunProtoTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(GunProto, ref GunProtoTemp, hookCtx, false, context))
			{
				GunProtoTemp = serialization.CreateCopy<EntProtoId>(GunProto, hookCtx, context, false);
			}
			target.GunProto = GunProtoTemp;
			EntityUid? GunTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Gun, ref GunTemp, hookCtx, false, context))
			{
				GunTemp = serialization.CreateCopy<EntityUid?>(Gun, hookCtx, context, false);
			}
			target.Gun = GunTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ActionGunComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActionGunComponent cast = (ActionGunComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActionGunComponent cast = (ActionGunComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActionGunComponent def = (ActionGunComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ActionGunComponent Instantiate()
	{
		return new ActionGunComponent();
	}
}
