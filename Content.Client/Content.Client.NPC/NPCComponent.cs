using System;
using Content.Shared.NPC;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.NPC;

public abstract class NPCComponent : SharedNPCComponent, ISerializationGenerated<NPCComponent>, ISerializationGenerated
{
	public NPCComponent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref NPCComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedNPCComponent target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (NPCComponent)target2;
		serialization.TryCustomCopy<NPCComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref NPCComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SharedNPCComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NPCComponent target2 = (NPCComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NPCComponent target2 = (NPCComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NPCComponent target2 = (NPCComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NPCComponent Instantiate()
	{
		throw new NotImplementedException();
	}
}
