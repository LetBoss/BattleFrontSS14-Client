using System;
using Robust.Shared.ComponentTrees;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client._PUBG.FogOfWar;

[RegisterComponent]
public sealed class PubgFogOfWarOccludableTreeComponent : Component, IComponentTreeComponent<PubgFogOfWarOccludableComponent>, ISerializationGenerated<PubgFogOfWarOccludableTreeComponent>, ISerializationGenerated
{
	public DynamicTree<ComponentTreeEntry<PubgFogOfWarOccludableComponent>> Tree { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgFogOfWarOccludableTreeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (PubgFogOfWarOccludableTreeComponent)(object)val;
		serialization.TryCustomCopy<PubgFogOfWarOccludableTreeComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgFogOfWarOccludableTreeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgFogOfWarOccludableTreeComponent target2 = (PubgFogOfWarOccludableTreeComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgFogOfWarOccludableTreeComponent target2 = (PubgFogOfWarOccludableTreeComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgFogOfWarOccludableTreeComponent target2 = (PubgFogOfWarOccludableTreeComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgFogOfWarOccludableTreeComponent Instantiate()
	{
		return new PubgFogOfWarOccludableTreeComponent();
	}
}
