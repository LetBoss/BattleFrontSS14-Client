using System;
using Content.Shared.Whitelist;
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
public sealed class EntityWhitelistCondition : IConstructionCondition, ISerializationGenerated<EntityWhitelistCondition>, ISerializationGenerated
{
	[DataField("conditionString", false, 1, false, false, null)]
	public string ConditionString = "construction-step-condition-entity-whitelist";

	[DataField("conditionIcon", false, 1, false, false, null)]
	public SpriteSpecifier? ConditionIcon;

	[DataField("whitelist", false, 1, true, false, null)]
	public EntityWhitelist Whitelist = new EntityWhitelist();

	public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return IoCManager.Resolve<IEntityManager>().System<EntityWhitelistSystem>().IsWhitelistPass(Whitelist, user);
	}

	public ConstructionGuideEntry GenerateGuideEntry()
	{
		return new ConstructionGuideEntry
		{
			Localization = ConditionString,
			Icon = ConditionIcon
		};
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EntityWhitelistCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<EntityWhitelistCondition>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		string ConditionStringTemp = null;
		if (ConditionString == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(ConditionString, ref ConditionStringTemp, hookCtx, false, context))
		{
			ConditionStringTemp = ConditionString;
		}
		target.ConditionString = ConditionStringTemp;
		SpriteSpecifier ConditionIconTemp = null;
		if (!serialization.TryCustomCopy<SpriteSpecifier>(ConditionIcon, ref ConditionIconTemp, hookCtx, true, context))
		{
			ConditionIconTemp = serialization.CreateCopy<SpriteSpecifier>(ConditionIcon, hookCtx, context, false);
		}
		target.ConditionIcon = ConditionIconTemp;
		EntityWhitelist WhitelistTemp = null;
		if (Whitelist == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, false, context))
		{
			if (Whitelist == null)
			{
				WhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, context, true);
			}
		}
		target.Whitelist = WhitelistTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EntityWhitelistCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityWhitelistCondition cast = (EntityWhitelistCondition)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public EntityWhitelistCondition Instantiate()
	{
		return new EntityWhitelistCondition();
	}
}
