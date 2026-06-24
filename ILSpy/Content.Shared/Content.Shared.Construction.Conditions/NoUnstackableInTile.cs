using System;
using Content.Shared.Construction.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Conditions;

[DataDefinition]
public sealed class NoUnstackableInTile : IConstructionCondition, ISerializationGenerated<NoUnstackableInTile>, ISerializationGenerated
{
	public const string GuidebookString = "construction-step-condition-no-unstackable-in-tile";

	public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return !IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<AnchorableSystem>().AnyUnstackablesAnchoredAt(location);
	}

	public ConstructionGuideEntry GenerateGuideEntry()
	{
		return new ConstructionGuideEntry
		{
			Localization = "construction-step-condition-no-unstackable-in-tile"
		};
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NoUnstackableInTile target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<NoUnstackableInTile>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NoUnstackableInTile target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NoUnstackableInTile cast = (NoUnstackableInTile)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public NoUnstackableInTile Instantiate()
	{
		return new NoUnstackableInTile();
	}
}
