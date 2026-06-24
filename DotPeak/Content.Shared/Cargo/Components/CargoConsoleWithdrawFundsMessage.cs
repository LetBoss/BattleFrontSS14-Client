// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.Components.CargoConsoleWithdrawFundsMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Cargo.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Cargo.Components;

[NetSerializable]
[Serializable]
public sealed class CargoConsoleWithdrawFundsMessage : BoundUserInterfaceMessage
{
  public ProtoId<CargoAccountPrototype>? Account;
  public int Amount;

  public CargoConsoleWithdrawFundsMessage(ProtoId<CargoAccountPrototype>? account, int amount)
  {
    this.Account = account;
    this.Amount = amount;
  }
}
