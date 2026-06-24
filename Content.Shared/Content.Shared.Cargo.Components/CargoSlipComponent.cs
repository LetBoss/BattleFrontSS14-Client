using System;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Cargo.Components;

[RegisterComponent]
public sealed class CargoSlipComponent : Component, ISerializationGenerated<CargoSlipComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<CargoProductPrototype> Product;

	[DataField(null, false, 1, false, false, null)]
	public string Requester;

	[DataField(null, false, 1, false, false, null)]
	public string Reason;

	[DataField(null, false, 1, false, false, null)]
	public int OrderQuantity;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<CargoAccountPrototype> Account;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CargoSlipComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CargoSlipComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CargoSlipComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<CargoProductPrototype> ProductTemp = default(ProtoId<CargoProductPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<CargoProductPrototype>>(Product, ref ProductTemp, hookCtx, false, context))
			{
				ProductTemp = serialization.CreateCopy<ProtoId<CargoProductPrototype>>(Product, hookCtx, context, false);
			}
			target.Product = ProductTemp;
			string RequesterTemp = null;
			if (Requester == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Requester, ref RequesterTemp, hookCtx, false, context))
			{
				RequesterTemp = Requester;
			}
			target.Requester = RequesterTemp;
			string ReasonTemp = null;
			if (Reason == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Reason, ref ReasonTemp, hookCtx, false, context))
			{
				ReasonTemp = Reason;
			}
			target.Reason = ReasonTemp;
			int OrderQuantityTemp = 0;
			if (!serialization.TryCustomCopy<int>(OrderQuantity, ref OrderQuantityTemp, hookCtx, false, context))
			{
				OrderQuantityTemp = OrderQuantity;
			}
			target.OrderQuantity = OrderQuantityTemp;
			ProtoId<CargoAccountPrototype> AccountTemp = default(ProtoId<CargoAccountPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<CargoAccountPrototype>>(Account, ref AccountTemp, hookCtx, false, context))
			{
				AccountTemp = serialization.CreateCopy<ProtoId<CargoAccountPrototype>>(Account, hookCtx, context, false);
			}
			target.Account = AccountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CargoSlipComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CargoSlipComponent cast = (CargoSlipComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CargoSlipComponent cast = (CargoSlipComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CargoSlipComponent def = (CargoSlipComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CargoSlipComponent Instantiate()
	{
		return new CargoSlipComponent();
	}
}
