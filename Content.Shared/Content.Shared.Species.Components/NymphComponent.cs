using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Species.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class NymphComponent : Component, ISerializationGenerated<NymphComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public EntProtoId EntityPrototype;

	[DataField(null, false, 1, false, false, null)]
	public bool TransferMind;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NymphComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (NymphComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<NymphComponent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId EntityPrototypeTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(EntityPrototype, ref EntityPrototypeTemp, hookCtx, false, context))
			{
				EntityPrototypeTemp = serialization.CreateCopy<EntProtoId>(EntityPrototype, hookCtx, context, false);
			}
			target.EntityPrototype = EntityPrototypeTemp;
			bool TransferMindTemp = false;
			if (!serialization.TryCustomCopy<bool>(TransferMind, ref TransferMindTemp, hookCtx, false, context))
			{
				TransferMindTemp = TransferMind;
			}
			target.TransferMind = TransferMindTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NymphComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NymphComponent cast = (NymphComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NymphComponent cast = (NymphComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NymphComponent def = (NymphComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NymphComponent Instantiate()
	{
		return new NymphComponent();
	}
}
