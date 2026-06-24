using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.BroadPhase;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Robust.Shared.Physics;

[RegisterComponent]
public sealed class BroadphaseComponent : Component, ISerializationGenerated<BroadphaseComponent>, ISerializationGenerated
{
	public IBroadPhase DynamicTree = new DynamicTreeBroadPhase();

	public IBroadPhase StaticTree = new DynamicTreeBroadPhase();

	public DynamicTree<EntityUid> SundriesTree;

	public DynamicTree<EntityUid> StaticSundriesTree;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BroadphaseComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (BroadphaseComponent)target2;
		serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BroadphaseComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BroadphaseComponent target2 = (BroadphaseComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BroadphaseComponent target2 = (BroadphaseComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BroadphaseComponent target2 = (BroadphaseComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override BroadphaseComponent Instantiate()
	{
		return new BroadphaseComponent();
	}
}
