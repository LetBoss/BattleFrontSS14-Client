// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.CargoOrderData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Cargo.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Text;

#nullable enable
namespace Content.Shared.Cargo;

[DataDefinition]
[NetSerializable]
[Serializable]
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

  public CargoOrderData(
    int orderId,
    string productId,
    string productName,
    int price,
    int amount,
    string requester,
    string reason,
    ProtoId<CargoAccountPrototype> account)
  {
    this.OrderId = orderId;
    this.ProductId = productId;
    this.ProductName = productName;
    this.Price = price;
    this.OrderQuantity = amount;
    this.Requester = requester;
    this.Reason = reason;
    this.Account = account;
  }

  public void SetApproverData(string? approver) => this.Approver = approver;

  public void SetApproverData(string? fullName, string? jobTitle)
  {
    StringBuilder stringBuilder1 = new StringBuilder();
    StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler;
    if (!string.IsNullOrWhiteSpace(fullName))
    {
      StringBuilder stringBuilder2 = stringBuilder1;
      StringBuilder stringBuilder3 = stringBuilder2;
      interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(1, 1, stringBuilder2);
      interpolatedStringHandler.AppendFormatted(fullName);
      interpolatedStringHandler.AppendLiteral(" ");
      ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
      stringBuilder3.Append(ref local);
    }
    if (!string.IsNullOrWhiteSpace(jobTitle))
    {
      StringBuilder stringBuilder4 = stringBuilder1;
      StringBuilder stringBuilder5 = stringBuilder4;
      interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder4);
      interpolatedStringHandler.AppendLiteral("(");
      interpolatedStringHandler.AppendFormatted(jobTitle);
      interpolatedStringHandler.AppendLiteral(")");
      ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
      stringBuilder5.Append(ref local);
    }
    this.Approver = stringBuilder1.ToString();
  }

  public CargoOrderData()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CargoOrderData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<CargoOrderData>(this, ref target, hookCtx, false, context))
      return;
    int num1 = 0;
    if (!serialization.TryCustomCopy<int>(this.Price, ref num1, hookCtx, false, context))
      num1 = this.Price;
    target.Price = num1;
    int num2 = 0;
    if (!serialization.TryCustomCopy<int>(this.OrderId, ref num2, hookCtx, false, context))
      num2 = this.OrderId;
    target.OrderId = num2;
    string str1 = (string) null;
    if (this.ProductId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ProductId, ref str1, hookCtx, false, context))
      str1 = this.ProductId;
    target.ProductId = str1;
    string str2 = (string) null;
    if (this.ProductName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ProductName, ref str2, hookCtx, false, context))
      str2 = this.ProductName;
    target.ProductName = str2;
    int num3 = 0;
    if (!serialization.TryCustomCopy<int>(this.OrderQuantity, ref num3, hookCtx, false, context))
      num3 = this.OrderQuantity;
    target.OrderQuantity = num3;
    int num4 = 0;
    if (!serialization.TryCustomCopy<int>(this.NumDispatched, ref num4, hookCtx, false, context))
      num4 = this.NumDispatched;
    target.NumDispatched = num4;
    string str3 = (string) null;
    if (this.Requester == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Requester, ref str3, hookCtx, false, context))
      str3 = this.Requester;
    target.Requester = str3;
    string str4 = (string) null;
    if (this.Reason == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Reason, ref str4, hookCtx, false, context))
      str4 = this.Reason;
    target.Reason = str4;
    string str5 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Approver, ref str5, hookCtx, false, context))
      str5 = this.Approver;
    target.Approver = str5;
    ProtoId<CargoAccountPrototype> protoId = new ProtoId<CargoAccountPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<CargoAccountPrototype>>(this.Account, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<CargoAccountPrototype>>(this.Account, hookCtx, context, false);
    target.Account = protoId;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CargoOrderData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CargoOrderData target1 = (CargoOrderData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public CargoOrderData Instantiate() => new CargoOrderData();
}
