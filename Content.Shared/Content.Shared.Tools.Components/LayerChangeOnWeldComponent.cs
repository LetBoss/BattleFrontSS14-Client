using System;
using Content.Shared.Physics;
using Content.Shared.Tools.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Tools.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(WeldableSystem) })]
public sealed class LayerChangeOnWeldComponent : Component, ISerializationGenerated<LayerChangeOnWeldComponent>, ISerializationGenerated
{
	[DataField("unWeldedLayer", false, 1, false, false, null)]
	[ViewVariables]
	public CollisionGroup UnWeldedLayer = CollisionGroup.AirlockLayer;

	[DataField("weldedLayer", false, 1, false, false, null)]
	[ViewVariables]
	public CollisionGroup WeldedLayer = CollisionGroup.WallLayer;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref LayerChangeOnWeldComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (LayerChangeOnWeldComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<LayerChangeOnWeldComponent>(this, ref target, hookCtx, false, context))
		{
			CollisionGroup UnWeldedLayerTemp = CollisionGroup.None;
			if (!serialization.TryCustomCopy<CollisionGroup>(UnWeldedLayer, ref UnWeldedLayerTemp, hookCtx, false, context))
			{
				UnWeldedLayerTemp = UnWeldedLayer;
			}
			target.UnWeldedLayer = UnWeldedLayerTemp;
			CollisionGroup WeldedLayerTemp = CollisionGroup.None;
			if (!serialization.TryCustomCopy<CollisionGroup>(WeldedLayer, ref WeldedLayerTemp, hookCtx, false, context))
			{
				WeldedLayerTemp = WeldedLayer;
			}
			target.WeldedLayer = WeldedLayerTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref LayerChangeOnWeldComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LayerChangeOnWeldComponent cast = (LayerChangeOnWeldComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LayerChangeOnWeldComponent cast = (LayerChangeOnWeldComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LayerChangeOnWeldComponent def = (LayerChangeOnWeldComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override LayerChangeOnWeldComponent Instantiate()
	{
		return new LayerChangeOnWeldComponent();
	}
}
