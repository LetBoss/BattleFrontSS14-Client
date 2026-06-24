using System;
using System.Text;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Cargo;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class CargoOrderData : ISerializationGenerated<CargoOrderData>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int Price;

	[DataField(null, false, 1, false, false, null)]
	public int OrderQuantity;

	[DataField(null, false, 1, false, false, null)]
	public int NumDispatched;

	public bool Approved;

	[DataField(null, false, 1, false, false, null)]
	public string? Approver;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<CargoAccountPrototype> Account;

	[DataField(null, false, 1, false, false, null)]
	public int OrderId { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public string ProductId { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public string ProductName { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public string Requester { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public string Reason { get; private set; }

	public CargoOrderData(int orderId, string productId, string productName, int price, int amount, string requester, string reason, ProtoId<CargoAccountPrototype> account)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		OrderId = orderId;
		ProductId = productId;
		ProductName = productName;
		Price = price;
		OrderQuantity = amount;
		Requester = requester;
		Reason = reason;
		Account = account;
	}

	public void SetApproverData(string? approver)
	{
		Approver = approver;
	}

	public void SetApproverData(string? fullName, string? jobTitle)
	{
		StringBuilder sb = new StringBuilder();
		if (!string.IsNullOrWhiteSpace(fullName))
		{
			StringBuilder stringBuilder = sb;
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 1, stringBuilder);
			handler.AppendFormatted(fullName);
			handler.AppendLiteral(" ");
			stringBuilder2.Append(ref handler);
		}
		if (!string.IsNullOrWhiteSpace(jobTitle))
		{
			StringBuilder stringBuilder = sb;
			StringBuilder stringBuilder3 = stringBuilder;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder);
			handler.AppendLiteral("(");
			handler.AppendFormatted(jobTitle);
			handler.AppendLiteral(")");
			stringBuilder3.Append(ref handler);
		}
		Approver = sb.ToString();
	}

	public CargoOrderData()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CargoOrderData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<CargoOrderData>(this, ref target, hookCtx, false, context))
		{
			int PriceTemp = 0;
			if (!serialization.TryCustomCopy<int>(Price, ref PriceTemp, hookCtx, false, context))
			{
				PriceTemp = Price;
			}
			target.Price = PriceTemp;
			int OrderIdTemp = 0;
			if (!serialization.TryCustomCopy<int>(OrderId, ref OrderIdTemp, hookCtx, false, context))
			{
				OrderIdTemp = OrderId;
			}
			target.OrderId = OrderIdTemp;
			string ProductIdTemp = null;
			if (ProductId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ProductId, ref ProductIdTemp, hookCtx, false, context))
			{
				ProductIdTemp = ProductId;
			}
			target.ProductId = ProductIdTemp;
			string ProductNameTemp = null;
			if (ProductName == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ProductName, ref ProductNameTemp, hookCtx, false, context))
			{
				ProductNameTemp = ProductName;
			}
			target.ProductName = ProductNameTemp;
			int OrderQuantityTemp = 0;
			if (!serialization.TryCustomCopy<int>(OrderQuantity, ref OrderQuantityTemp, hookCtx, false, context))
			{
				OrderQuantityTemp = OrderQuantity;
			}
			target.OrderQuantity = OrderQuantityTemp;
			int NumDispatchedTemp = 0;
			if (!serialization.TryCustomCopy<int>(NumDispatched, ref NumDispatchedTemp, hookCtx, false, context))
			{
				NumDispatchedTemp = NumDispatched;
			}
			target.NumDispatched = NumDispatchedTemp;
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
			string ApproverTemp = null;
			if (!serialization.TryCustomCopy<string>(Approver, ref ApproverTemp, hookCtx, false, context))
			{
				ApproverTemp = Approver;
			}
			target.Approver = ApproverTemp;
			ProtoId<CargoAccountPrototype> AccountTemp = default(ProtoId<CargoAccountPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<CargoAccountPrototype>>(Account, ref AccountTemp, hookCtx, false, context))
			{
				AccountTemp = serialization.CreateCopy<ProtoId<CargoAccountPrototype>>(Account, hookCtx, context, false);
			}
			target.Account = AccountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CargoOrderData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CargoOrderData cast = (CargoOrderData)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public CargoOrderData Instantiate()
	{
		return new CargoOrderData();
	}
}
