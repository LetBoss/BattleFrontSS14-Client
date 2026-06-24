using System;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Map;
using Content.Shared.Construction;
using Content.Shared.Construction.Conditions;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Construction.Conditions;

[DataDefinition]
public sealed class TileBarricadeClear : IConstructionCondition, ISerializationGenerated<TileBarricadeClear>, ISerializationGenerated
{
	public ConstructionGuideEntry GenerateGuideEntry()
	{
		return new ConstructionGuideEntry
		{
			Localization = "rmc-construction-not-barricade-clear"
		};
	}

	public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		RMCMapSystem rMCMapSystem = IoCManager.Resolve<IEntityManager>().System<RMCMapSystem>();
		DirectionFlag facing = DirectionExtensions.AsFlag(direction);
		return !rMCMapSystem.HasAnchoredEntityEnumerator<BarricadeComponent>(location, (Direction?)null, facing);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TileBarricadeClear target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<TileBarricadeClear>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TileBarricadeClear target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TileBarricadeClear cast = (TileBarricadeClear)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public TileBarricadeClear Instantiate()
	{
		return new TileBarricadeClear();
	}
}
