using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Ghost.Roles.Raffles;

[DataDefinition]
public sealed class GhostRoleRaffleSettings : ISerializationGenerated<GhostRoleRaffleSettings>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float RoundTimeRequirement;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField(null, false, 1, true, false, null)]
	public uint InitialDuration { get; set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField(null, false, 1, true, false, null)]
	public uint JoinExtendsDurationBy { get; set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField(null, false, 1, true, false, null)]
	public uint MaxDuration { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GhostRoleRaffleSettings target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<GhostRoleRaffleSettings>(this, ref target, hookCtx, false, context))
		{
			uint InitialDurationTemp = 0u;
			if (!serialization.TryCustomCopy<uint>(InitialDuration, ref InitialDurationTemp, hookCtx, false, context))
			{
				InitialDurationTemp = InitialDuration;
			}
			target.InitialDuration = InitialDurationTemp;
			uint JoinExtendsDurationByTemp = 0u;
			if (!serialization.TryCustomCopy<uint>(JoinExtendsDurationBy, ref JoinExtendsDurationByTemp, hookCtx, false, context))
			{
				JoinExtendsDurationByTemp = JoinExtendsDurationBy;
			}
			target.JoinExtendsDurationBy = JoinExtendsDurationByTemp;
			uint MaxDurationTemp = 0u;
			if (!serialization.TryCustomCopy<uint>(MaxDuration, ref MaxDurationTemp, hookCtx, false, context))
			{
				MaxDurationTemp = MaxDuration;
			}
			target.MaxDuration = MaxDurationTemp;
			float RoundTimeRequirementTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RoundTimeRequirement, ref RoundTimeRequirementTemp, hookCtx, false, context))
			{
				RoundTimeRequirementTemp = RoundTimeRequirement;
			}
			target.RoundTimeRequirement = RoundTimeRequirementTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GhostRoleRaffleSettings target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GhostRoleRaffleSettings cast = (GhostRoleRaffleSettings)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public GhostRoleRaffleSettings Instantiate()
	{
		return new GhostRoleRaffleSettings();
	}
}
