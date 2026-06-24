using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Stun;
using Content.Shared.Item;
using Content.Shared.Tag;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Whitelist;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class EntityWhitelist : ISerializationGenerated<EntityWhitelist>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string[]? Components;

	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<ItemSizePrototype>>? Sizes;

	[NonSerialized]
	[Access(new Type[] { typeof(EntityWhitelistSystem) })]
	public List<ComponentRegistration>? Registrations;

	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<TagPrototype>>? Tags;

	[DataField(null, false, 1, false, false, null)]
	public bool RequireAll;

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<EntProtoId<SkillDefinitionComponent>, int>? Skills;

	[DataField(null, false, 1, false, false, null)]
	public RMCSizes? MinMobSize;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EntityWhitelist target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<EntityWhitelist>(this, ref target, hookCtx, false, context))
		{
			string[] ComponentsTemp = null;
			if (!serialization.TryCustomCopy<string[]>(Components, ref ComponentsTemp, hookCtx, true, context))
			{
				ComponentsTemp = serialization.CreateCopy<string[]>(Components, hookCtx, context, false);
			}
			target.Components = ComponentsTemp;
			List<ProtoId<ItemSizePrototype>> SizesTemp = null;
			if (!serialization.TryCustomCopy<List<ProtoId<ItemSizePrototype>>>(Sizes, ref SizesTemp, hookCtx, true, context))
			{
				SizesTemp = serialization.CreateCopy<List<ProtoId<ItemSizePrototype>>>(Sizes, hookCtx, context, false);
			}
			target.Sizes = SizesTemp;
			List<ProtoId<TagPrototype>> TagsTemp = null;
			if (!serialization.TryCustomCopy<List<ProtoId<TagPrototype>>>(Tags, ref TagsTemp, hookCtx, true, context))
			{
				TagsTemp = serialization.CreateCopy<List<ProtoId<TagPrototype>>>(Tags, hookCtx, context, false);
			}
			target.Tags = TagsTemp;
			bool RequireAllTemp = false;
			if (!serialization.TryCustomCopy<bool>(RequireAll, ref RequireAllTemp, hookCtx, false, context))
			{
				RequireAllTemp = RequireAll;
			}
			target.RequireAll = RequireAllTemp;
			Dictionary<EntProtoId<SkillDefinitionComponent>, int> SkillsTemp = null;
			if (!serialization.TryCustomCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(Skills, ref SkillsTemp, hookCtx, true, context))
			{
				SkillsTemp = serialization.CreateCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(Skills, hookCtx, context, false);
			}
			target.Skills = SkillsTemp;
			RMCSizes? MinMobSizeTemp = null;
			if (!serialization.TryCustomCopy<RMCSizes?>(MinMobSize, ref MinMobSizeTemp, hookCtx, false, context))
			{
				MinMobSizeTemp = MinMobSize;
			}
			target.MinMobSize = MinMobSizeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EntityWhitelist target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityWhitelist cast = (EntityWhitelist)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public EntityWhitelist Instantiate()
	{
		return new EntityWhitelist();
	}
}
