using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Explosion.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ExplosionVisualsComponent : Component, ISerializationGenerated<ExplosionVisualsComponent>, ISerializationGenerated
{
	public MapCoordinates Epicenter;

	public Dictionary<int, List<Vector2i>>? SpaceTiles;

	public Dictionary<EntityUid, Dictionary<int, List<Vector2i>>> Tiles = new Dictionary<EntityUid, Dictionary<int, List<Vector2i>>>();

	public List<float> Intensity = new List<float>();

	public string ExplosionType = string.Empty;

	public Matrix3x2 SpaceMatrix;

	public ushort SpaceTileSize;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ExplosionVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ExplosionVisualsComponent)(object)definitionCast;
		serialization.TryCustomCopy<ExplosionVisualsComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ExplosionVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExplosionVisualsComponent cast = (ExplosionVisualsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExplosionVisualsComponent cast = (ExplosionVisualsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExplosionVisualsComponent def = (ExplosionVisualsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ExplosionVisualsComponent Instantiate()
	{
		return new ExplosionVisualsComponent();
	}
}
