using System;
using Robust.Shared.ComponentTrees;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client._PUBG.FogOfWar;

[RegisterComponent]
public sealed class PubgFogOfWarOccludableComponent : Component, IComponentTreeEntry<PubgFogOfWarOccludableComponent>, ISerializationGenerated<PubgFogOfWarOccludableComponent>, ISerializationGenerated
{
	public EntityUid? TreeUid { get; set; }

	public DynamicTree<ComponentTreeEntry<PubgFogOfWarOccludableComponent>>? Tree { get; set; }

	public bool AddToTree => true;

	public bool TreeUpdateQueued { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgFogOfWarOccludableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (PubgFogOfWarOccludableComponent)(object)val;
		serialization.TryCustomCopy<PubgFogOfWarOccludableComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgFogOfWarOccludableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgFogOfWarOccludableComponent target2 = (PubgFogOfWarOccludableComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgFogOfWarOccludableComponent target2 = (PubgFogOfWarOccludableComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgFogOfWarOccludableComponent target2 = (PubgFogOfWarOccludableComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgFogOfWarOccludableComponent Instantiate()
	{
		return new PubgFogOfWarOccludableComponent();
	}
}
