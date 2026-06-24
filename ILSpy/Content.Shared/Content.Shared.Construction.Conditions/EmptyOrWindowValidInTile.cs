using System;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Construction.Conditions;

[DataDefinition]
public sealed class EmptyOrWindowValidInTile : IConstructionCondition, ISerializationGenerated<EmptyOrWindowValidInTile>, ISerializationGenerated
{
	[DataField("tileNotBlocked", false, 1, false, false, null)]
	private TileNotBlocked _tileNotBlocked = new TileNotBlocked();

	public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		IEntityManager entManager = IoCManager.Resolve<IEntityManager>();
		EntityLookupSystem obj = entManager.System<EntityLookupSystem>();
		bool result = false;
		foreach (EntityUid entity in obj.GetEntitiesIntersecting(location, (LookupFlags)5))
		{
			if (entManager.HasComponent<SharedCanBuildWindowOnTopComponent>(entity))
			{
				result = true;
			}
		}
		if (!result)
		{
			result = _tileNotBlocked.Condition(user, location, direction);
		}
		return result;
	}

	public ConstructionGuideEntry GenerateGuideEntry()
	{
		return new ConstructionGuideEntry
		{
			Localization = "construction-guide-condition-empty-or-window-valid-in-tile"
		};
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EmptyOrWindowValidInTile target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<EmptyOrWindowValidInTile>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		TileNotBlocked _tileNotBlockedTemp = null;
		if (_tileNotBlocked == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<TileNotBlocked>(_tileNotBlocked, ref _tileNotBlockedTemp, hookCtx, false, context))
		{
			if (_tileNotBlocked == null)
			{
				_tileNotBlockedTemp = null;
			}
			else
			{
				serialization.CopyTo<TileNotBlocked>(_tileNotBlocked, ref _tileNotBlockedTemp, hookCtx, context, true);
			}
		}
		target._tileNotBlocked = _tileNotBlockedTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EmptyOrWindowValidInTile target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmptyOrWindowValidInTile cast = (EmptyOrWindowValidInTile)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public EmptyOrWindowValidInTile Instantiate()
	{
		return new EmptyOrWindowValidInTile();
	}
}
