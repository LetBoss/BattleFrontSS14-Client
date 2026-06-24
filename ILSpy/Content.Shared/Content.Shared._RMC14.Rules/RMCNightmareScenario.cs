using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Rules;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed record RMCNightmareScenario : ISerializationGenerated<RMCNightmareScenario>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string ScenarioName = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public float ScenarioProbability = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCNightmareScenario target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<RMCNightmareScenario>(this, ref target, hookCtx, false, context))
		{
			string ScenarioNameTemp = null;
			if (ScenarioName == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ScenarioName, ref ScenarioNameTemp, hookCtx, false, context))
			{
				ScenarioNameTemp = ScenarioName;
			}
			target.ScenarioName = ScenarioNameTemp;
			float ScenarioProbabilityTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ScenarioProbability, ref ScenarioProbabilityTemp, hookCtx, false, context))
			{
				ScenarioProbabilityTemp = ScenarioProbability;
			}
			target.ScenarioProbability = ScenarioProbabilityTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCNightmareScenario target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCNightmareScenario cast = (RMCNightmareScenario)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RMCNightmareScenario Instantiate()
	{
		return new RMCNightmareScenario();
	}
}
