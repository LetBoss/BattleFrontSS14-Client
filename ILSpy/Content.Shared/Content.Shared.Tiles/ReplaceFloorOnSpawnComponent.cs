using System;
using System.Collections.Generic;
using Content.Shared.Maps;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Tiles;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(ReplaceFloorOnSpawnSystem) })]
public sealed class ReplaceFloorOnSpawnComponent : Component, ISerializationGenerated<ReplaceFloorOnSpawnComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<ContentTileDefinition>>? ReplaceableTiles = new List<ProtoId<ContentTileDefinition>>();

	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<ContentTileDefinition>> ReplacementTiles = new List<ProtoId<ContentTileDefinition>>();

	[DataField(null, false, 1, false, false, null)]
	public bool ReplaceSpace = true;

	[DataField(null, false, 1, false, false, null)]
	public List<Vector2i> Offsets = new List<Vector2i>
	{
		Vector2i.Up,
		Vector2i.Down,
		Vector2i.Left,
		Vector2i.Right,
		Vector2i.Zero
	};

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ReplaceFloorOnSpawnComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ReplaceFloorOnSpawnComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ReplaceFloorOnSpawnComponent>(this, ref target, hookCtx, false, context))
		{
			List<ProtoId<ContentTileDefinition>> ReplaceableTilesTemp = null;
			if (!serialization.TryCustomCopy<List<ProtoId<ContentTileDefinition>>>(ReplaceableTiles, ref ReplaceableTilesTemp, hookCtx, true, context))
			{
				ReplaceableTilesTemp = serialization.CreateCopy<List<ProtoId<ContentTileDefinition>>>(ReplaceableTiles, hookCtx, context, false);
			}
			target.ReplaceableTiles = ReplaceableTilesTemp;
			List<ProtoId<ContentTileDefinition>> ReplacementTilesTemp = null;
			if (ReplacementTiles == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ProtoId<ContentTileDefinition>>>(ReplacementTiles, ref ReplacementTilesTemp, hookCtx, true, context))
			{
				ReplacementTilesTemp = serialization.CreateCopy<List<ProtoId<ContentTileDefinition>>>(ReplacementTiles, hookCtx, context, false);
			}
			target.ReplacementTiles = ReplacementTilesTemp;
			bool ReplaceSpaceTemp = false;
			if (!serialization.TryCustomCopy<bool>(ReplaceSpace, ref ReplaceSpaceTemp, hookCtx, false, context))
			{
				ReplaceSpaceTemp = ReplaceSpace;
			}
			target.ReplaceSpace = ReplaceSpaceTemp;
			List<Vector2i> OffsetsTemp = null;
			if (Offsets == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<Vector2i>>(Offsets, ref OffsetsTemp, hookCtx, true, context))
			{
				OffsetsTemp = serialization.CreateCopy<List<Vector2i>>(Offsets, hookCtx, context, false);
			}
			target.Offsets = OffsetsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ReplaceFloorOnSpawnComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReplaceFloorOnSpawnComponent cast = (ReplaceFloorOnSpawnComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReplaceFloorOnSpawnComponent cast = (ReplaceFloorOnSpawnComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReplaceFloorOnSpawnComponent def = (ReplaceFloorOnSpawnComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ReplaceFloorOnSpawnComponent Instantiate()
	{
		return new ReplaceFloorOnSpawnComponent();
	}
}
