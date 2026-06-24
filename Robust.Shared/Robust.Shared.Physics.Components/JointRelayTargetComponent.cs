using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Robust.Shared.Physics.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class JointRelayTargetComponent : Component, ISerializationGenerated<JointRelayTargetComponent>, ISerializationGenerated
{
	[DataField("relayTarget", false, 1, false, false, null)]
	public HashSet<EntityUid> Relayed = new HashSet<EntityUid>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref JointRelayTargetComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (JointRelayTargetComponent)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			HashSet<EntityUid> target3 = null;
			if (Relayed == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy(Relayed, ref target3, hookCtx, hasHooks: true, context))
			{
				target3 = serialization.CreateCopy(Relayed, hookCtx, context);
			}
			target.Relayed = target3;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref JointRelayTargetComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		JointRelayTargetComponent target2 = (JointRelayTargetComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		JointRelayTargetComponent target2 = (JointRelayTargetComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		JointRelayTargetComponent target2 = (JointRelayTargetComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override JointRelayTargetComponent Instantiate()
	{
		return new JointRelayTargetComponent();
	}
}
