using System;
using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Marines.Skills;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class SkillWhitelist : ISerializationGenerated<SkillWhitelist>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public Dictionary<EntProtoId<SkillDefinitionComponent>, int> All = new Dictionary<EntProtoId<SkillDefinitionComponent>, int>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SkillWhitelist target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<SkillWhitelist>(this, ref target, hookCtx, false, context))
		{
			Dictionary<EntProtoId<SkillDefinitionComponent>, int> AllTemp = null;
			if (All == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(All, ref AllTemp, hookCtx, true, context))
			{
				AllTemp = serialization.CreateCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(All, hookCtx, context, false);
			}
			target.All = AllTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SkillWhitelist target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SkillWhitelist cast = (SkillWhitelist)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public SkillWhitelist Instantiate()
	{
		return new SkillWhitelist();
	}
}
