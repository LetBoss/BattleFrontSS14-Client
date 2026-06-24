using System;
using Content.Shared.DoAfter;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Deploy;

[Serializable]
[NetSerializable]
public sealed class RMCDeployDoAfterEvent : DoAfterEvent, ISerializationGenerated<RMCDeployDoAfterEvent>, ISerializationGenerated
{
	public Box2 Area;

	public RMCDeployDoAfterEvent(Box2 area)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Area = area;
	}

	public override DoAfterEvent Clone()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return new RMCDeployDoAfterEvent(Area);
	}

	public RMCDeployDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCDeployDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCDeployDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<RMCDeployDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCDeployDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCDeployDoAfterEvent cast = (RMCDeployDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCDeployDoAfterEvent cast = (RMCDeployDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCDeployDoAfterEvent Instantiate()
	{
		return new RMCDeployDoAfterEvent();
	}
}
