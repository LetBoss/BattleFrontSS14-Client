using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Preferences;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Roles;

[Serializable]
[ImplicitDataDefinitionForInheritors]
[NetSerializable]
public abstract class JobRequirement : ISerializationGenerated<JobRequirement>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Inverted;

	public abstract bool Check(IEntityManager entManager, IPrototypeManager protoManager, HumanoidCharacterProfile? profile, IReadOnlyDictionary<string, TimeSpan> playTimes, [NotNullWhen(false)] out FormattedMessage? reason);

	public JobRequirement()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref JobRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<JobRequirement>(this, ref target, hookCtx, false, context))
		{
			bool InvertedTemp = false;
			if (!serialization.TryCustomCopy<bool>(Inverted, ref InvertedTemp, hookCtx, false, context))
			{
				InvertedTemp = Inverted;
			}
			target.Inverted = InvertedTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref JobRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		JobRequirement cast = (JobRequirement)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual JobRequirement Instantiate()
	{
		throw new NotImplementedException();
	}
}
