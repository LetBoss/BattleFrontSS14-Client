using System;
using System.Collections.Generic;
using Content.Shared.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared.Construction.Conditions;

[DataDefinition]
public sealed class TileType : IConstructionCondition, ISerializationGenerated<TileType>, ISerializationGenerated
{
	[DataField("guideText", false, 1, false, false, null)]
	public string? GuideText;

	[DataField("guideIcon", false, 1, false, false, null)]
	public SpriteSpecifier? GuideIcon;

	[DataField("targets", false, 1, false, false, null)]
	public List<string> TargetTiles { get; private set; } = new List<string>();

	public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		TurfSystem turfSystem = default(TurfSystem);
		if (!IoCManager.Resolve<IEntityManager>().TrySystem<TurfSystem>(ref turfSystem))
		{
			return false;
		}
		if (!turfSystem.TryGetTileRef(location, out var tileFound))
		{
			return false;
		}
		ContentTileDefinition tile = turfSystem.GetContentTileDefinition(tileFound.Value);
		foreach (string targetTile in TargetTiles)
		{
			if (tile.ID == targetTile)
			{
				return true;
			}
		}
		return false;
	}

	public ConstructionGuideEntry? GenerateGuideEntry()
	{
		if (GuideText == null)
		{
			return null;
		}
		return new ConstructionGuideEntry
		{
			Localization = GuideText,
			Icon = GuideIcon
		};
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TileType target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<TileType>(this, ref target, hookCtx, false, context))
		{
			List<string> TargetTilesTemp = null;
			if (TargetTiles == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(TargetTiles, ref TargetTilesTemp, hookCtx, true, context))
			{
				TargetTilesTemp = serialization.CreateCopy<List<string>>(TargetTiles, hookCtx, context, false);
			}
			target.TargetTiles = TargetTilesTemp;
			string GuideTextTemp = null;
			if (!serialization.TryCustomCopy<string>(GuideText, ref GuideTextTemp, hookCtx, false, context))
			{
				GuideTextTemp = GuideText;
			}
			target.GuideText = GuideTextTemp;
			SpriteSpecifier GuideIconTemp = null;
			if (!serialization.TryCustomCopy<SpriteSpecifier>(GuideIcon, ref GuideIconTemp, hookCtx, true, context))
			{
				GuideIconTemp = serialization.CreateCopy<SpriteSpecifier>(GuideIcon, hookCtx, context, false);
			}
			target.GuideIcon = GuideIconTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TileType target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TileType cast = (TileType)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public TileType Instantiate()
	{
		return new TileType();
	}
}
