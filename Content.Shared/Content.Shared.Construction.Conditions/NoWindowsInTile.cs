using System;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Conditions;

[DataDefinition]
public sealed class NoWindowsInTile : IConstructionCondition, ISerializationGenerated<NoWindowsInTile>, ISerializationGenerated
{
	private static readonly ProtoId<TagPrototype> WindowTag = ProtoId<TagPrototype>.op_Implicit("Window");

	public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		IEntitySystemManager entitySysManager = IoCManager.Resolve<IEntityManager>().EntitySysManager;
		TagSystem tagSystem = entitySysManager.GetEntitySystem<TagSystem>();
		foreach (EntityUid entity in entitySysManager.GetEntitySystem<EntityLookupSystem>().GetEntitiesIntersecting(location, (LookupFlags)4))
		{
			if (tagSystem.HasTag(entity, WindowTag))
			{
				return false;
			}
		}
		return true;
	}

	public ConstructionGuideEntry GenerateGuideEntry()
	{
		return new ConstructionGuideEntry
		{
			Localization = "construction-step-condition-no-windows-in-tile"
		};
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NoWindowsInTile target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<NoWindowsInTile>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NoWindowsInTile target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NoWindowsInTile cast = (NoWindowsInTile)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public NoWindowsInTile Instantiate()
	{
		return new NoWindowsInTile();
	}
}
