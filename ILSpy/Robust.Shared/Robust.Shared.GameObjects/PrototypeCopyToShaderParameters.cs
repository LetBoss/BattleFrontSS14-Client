using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class PrototypeCopyToShaderParameters : ISerializationGenerated<PrototypeCopyToShaderParameters>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string LayerKey;

	[DataField(null, false, 1, false, false, null)]
	public string? ParameterTexture;

	[DataField(null, false, 1, false, false, null)]
	public string? ParameterUV;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PrototypeCopyToShaderParameters target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			string target2 = null;
			if (LayerKey == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy(LayerKey, ref target2, hookCtx, hasHooks: false, context))
			{
				target2 = LayerKey;
			}
			target.LayerKey = target2;
			string target3 = null;
			if (!serialization.TryCustomCopy(ParameterTexture, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = ParameterTexture;
			}
			target.ParameterTexture = target3;
			string target4 = null;
			if (!serialization.TryCustomCopy(ParameterUV, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = ParameterUV;
			}
			target.ParameterUV = target4;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PrototypeCopyToShaderParameters target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PrototypeCopyToShaderParameters target2 = (PrototypeCopyToShaderParameters)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public PrototypeCopyToShaderParameters Instantiate()
	{
		return new PrototypeCopyToShaderParameters();
	}
}
