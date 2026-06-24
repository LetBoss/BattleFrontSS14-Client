using System;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Cargo.Components;

[RegisterComponent]
public sealed class OverrideSellComponent : Component, ISerializationGenerated<OverrideSellComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<CargoAccountPrototype> OverrideAccount;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref OverrideSellComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
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
		target = (OverrideSellComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<OverrideSellComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<CargoAccountPrototype> OverrideAccountTemp = default(ProtoId<CargoAccountPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<CargoAccountPrototype>>(OverrideAccount, ref OverrideAccountTemp, hookCtx, false, context))
			{
				OverrideAccountTemp = serialization.CreateCopy<ProtoId<CargoAccountPrototype>>(OverrideAccount, hookCtx, context, false);
			}
			target.OverrideAccount = OverrideAccountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref OverrideSellComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OverrideSellComponent cast = (OverrideSellComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OverrideSellComponent cast = (OverrideSellComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OverrideSellComponent def = (OverrideSellComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override OverrideSellComponent Instantiate()
	{
		return new OverrideSellComponent();
	}
}
