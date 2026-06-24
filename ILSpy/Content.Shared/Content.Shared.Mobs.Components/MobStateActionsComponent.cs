using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Mobs.Components;

[RegisterComponent]
public sealed class MobStateActionsComponent : Component, ISerializationGenerated<MobStateActionsComponent>, ISerializationGenerated
{
	[DataField("actions", false, 1, false, false, null)]
	public Dictionary<MobState, List<string>> Actions = new Dictionary<MobState, List<string>>();

	[DataField(null, false, 1, false, false, null)]
	public List<EntityUid> GrantedActions = new List<EntityUid>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MobStateActionsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MobStateActionsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<MobStateActionsComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<MobState, List<string>> ActionsTemp = null;
			if (Actions == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<MobState, List<string>>>(Actions, ref ActionsTemp, hookCtx, true, context))
			{
				ActionsTemp = serialization.CreateCopy<Dictionary<MobState, List<string>>>(Actions, hookCtx, context, false);
			}
			target.Actions = ActionsTemp;
			List<EntityUid> GrantedActionsTemp = null;
			if (GrantedActions == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<EntityUid>>(GrantedActions, ref GrantedActionsTemp, hookCtx, true, context))
			{
				GrantedActionsTemp = serialization.CreateCopy<List<EntityUid>>(GrantedActions, hookCtx, context, false);
			}
			target.GrantedActions = GrantedActionsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MobStateActionsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MobStateActionsComponent cast = (MobStateActionsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MobStateActionsComponent cast = (MobStateActionsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MobStateActionsComponent def = (MobStateActionsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MobStateActionsComponent Instantiate()
	{
		return new MobStateActionsComponent();
	}
}
