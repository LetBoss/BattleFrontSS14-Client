using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Physics;

[RegisterComponent]
[NetworkedComponent]
public sealed class JointComponent : Component, ISerializationGenerated<JointComponent>, ISerializationGenerated
{
	[DataField("relay", false, 1, false, false, null)]
	public EntityUid? Relay;

	[DataField("joints", false, 1, false, false, null)]
	internal Dictionary<string, Joint> Joints = new Dictionary<string, Joint>();

	[ViewVariables]
	public int JointCount => Joints.Count;

	public IReadOnlyDictionary<string, Joint> GetJoints => Joints;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref JointComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (JointComponent)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			EntityUid? target3 = null;
			if (!serialization.TryCustomCopy(Relay, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = serialization.CreateCopy(Relay, hookCtx, context);
			}
			target.Relay = target3;
			Dictionary<string, Joint> target4 = null;
			if (Joints == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy(Joints, ref target4, hookCtx, hasHooks: true, context))
			{
				target4 = serialization.CreateCopy(Joints, hookCtx, context);
			}
			target.Joints = target4;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref JointComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		JointComponent target2 = (JointComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		JointComponent target2 = (JointComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		JointComponent target2 = (JointComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override JointComponent Instantiate()
	{
		return new JointComponent();
	}
}
