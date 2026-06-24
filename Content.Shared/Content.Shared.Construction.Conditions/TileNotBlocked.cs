using System;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Conditions;

[DataDefinition]
public sealed class TileNotBlocked : IConstructionCondition, ISerializationGenerated<TileNotBlocked>, ISerializationGenerated
{
	[DataField("filterMobs", false, 1, false, false, null)]
	private bool _filterMobs;

	[DataField("failIfSpace", false, 1, false, false, null)]
	private bool _failIfSpace = true;

	[DataField("failIfNotSturdy", false, 1, false, false, null)]
	private bool _failIfNotSturdy = true;

	public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		TurfSystem turfSystem = default(TurfSystem);
		if (!IoCManager.Resolve<IEntityManager>().TrySystem<TurfSystem>(ref turfSystem))
		{
			return false;
		}
		if (!turfSystem.TryGetTileRef(location, out var tileRef))
		{
			return false;
		}
		if (turfSystem.IsSpace(tileRef.Value) && _failIfSpace)
		{
			return false;
		}
		if (!turfSystem.GetContentTileDefinition(tileRef.Value).Sturdy && _failIfNotSturdy)
		{
			return false;
		}
		return !turfSystem.IsTileBlocked(tileRef.Value, _filterMobs ? CollisionGroup.MobMask : CollisionGroup.Impassable);
	}

	public ConstructionGuideEntry GenerateGuideEntry()
	{
		return new ConstructionGuideEntry
		{
			Localization = "construction-step-condition-tile-not-blocked"
		};
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TileNotBlocked target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<TileNotBlocked>(this, ref target, hookCtx, false, context))
		{
			bool _filterMobsTemp = false;
			if (!serialization.TryCustomCopy<bool>(_filterMobs, ref _filterMobsTemp, hookCtx, false, context))
			{
				_filterMobsTemp = _filterMobs;
			}
			target._filterMobs = _filterMobsTemp;
			bool _failIfSpaceTemp = false;
			if (!serialization.TryCustomCopy<bool>(_failIfSpace, ref _failIfSpaceTemp, hookCtx, false, context))
			{
				_failIfSpaceTemp = _failIfSpace;
			}
			target._failIfSpace = _failIfSpaceTemp;
			bool _failIfNotSturdyTemp = false;
			if (!serialization.TryCustomCopy<bool>(_failIfNotSturdy, ref _failIfNotSturdyTemp, hookCtx, false, context))
			{
				_failIfNotSturdyTemp = _failIfNotSturdy;
			}
			target._failIfNotSturdy = _failIfNotSturdyTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TileNotBlocked target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TileNotBlocked cast = (TileNotBlocked)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public TileNotBlocked Instantiate()
	{
		return new TileNotBlocked();
	}
}
