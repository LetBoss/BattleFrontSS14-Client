using System;
using Content.Shared.Actions;
using Content.Shared.Maps;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Mapping;

public sealed class StartPlacementActionEvent : InstantActionEvent, ISerializationGenerated<StartPlacementActionEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? EntityType;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<ContentTileDefinition>? TileId;

	[DataField(null, false, 1, false, false, null)]
	public string? PlacementOption;

	[DataField(null, false, 1, false, false, null)]
	public bool Eraser;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StartPlacementActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstantActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StartPlacementActionEvent)definitionCast;
		if (!serialization.TryCustomCopy<StartPlacementActionEvent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId? EntityTypeTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(EntityType, ref EntityTypeTemp, hookCtx, false, context))
			{
				EntityTypeTemp = serialization.CreateCopy<EntProtoId?>(EntityType, hookCtx, context, false);
			}
			target.EntityType = EntityTypeTemp;
			ProtoId<ContentTileDefinition>? TileIdTemp = null;
			if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>?>(TileId, ref TileIdTemp, hookCtx, false, context))
			{
				TileIdTemp = serialization.CreateCopy<ProtoId<ContentTileDefinition>?>(TileId, hookCtx, context, false);
			}
			target.TileId = TileIdTemp;
			string PlacementOptionTemp = null;
			if (!serialization.TryCustomCopy<string>(PlacementOption, ref PlacementOptionTemp, hookCtx, false, context))
			{
				PlacementOptionTemp = PlacementOption;
			}
			target.PlacementOption = PlacementOptionTemp;
			bool EraserTemp = false;
			if (!serialization.TryCustomCopy<bool>(Eraser, ref EraserTemp, hookCtx, false, context))
			{
				EraserTemp = Eraser;
			}
			target.Eraser = EraserTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StartPlacementActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StartPlacementActionEvent cast = (StartPlacementActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StartPlacementActionEvent cast = (StartPlacementActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StartPlacementActionEvent Instantiate()
	{
		return new StartPlacementActionEvent();
	}
}
