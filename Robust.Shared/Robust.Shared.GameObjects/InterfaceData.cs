using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Robust.Shared.GameObjects;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class InterfaceData : ISerializationGenerated<InterfaceData>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float InteractionRange = 2f;

	[DataField(null, false, 1, false, false, null)]
	public bool RequireInputValidation = true;

	[DataField("type", false, 1, true, false, null)]
	public string ClientType { get; private set; }

	public InterfaceData(string clientType, float interactionRange = 2f, bool requireInputValidation = true)
	{
		ClientType = clientType;
		InteractionRange = interactionRange;
		RequireInputValidation = requireInputValidation;
	}

	public InterfaceData(InterfaceData data)
	{
		ClientType = data.ClientType;
		InteractionRange = data.InteractionRange;
		RequireInputValidation = data.RequireInputValidation;
	}

	public InterfaceData()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref InterfaceData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			string target2 = null;
			if (ClientType == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy(ClientType, ref target2, hookCtx, hasHooks: false, context))
			{
				target2 = ClientType;
			}
			target.ClientType = target2;
			float target3 = 0f;
			if (!serialization.TryCustomCopy(InteractionRange, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = InteractionRange;
			}
			target.InteractionRange = target3;
			bool target4 = false;
			if (!serialization.TryCustomCopy(RequireInputValidation, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = RequireInputValidation;
			}
			target.RequireInputValidation = target4;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref InterfaceData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InterfaceData target2 = (InterfaceData)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public InterfaceData Instantiate()
	{
		return new InterfaceData();
	}
}
