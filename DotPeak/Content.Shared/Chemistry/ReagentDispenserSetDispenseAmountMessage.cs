// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.ReagentDispenserSetDispenseAmountMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Chemistry;

[NetSerializable]
[Serializable]
public sealed class ReagentDispenserSetDispenseAmountMessage : BoundUserInterfaceMessage
{
  public readonly ReagentDispenserDispenseAmount ReagentDispenserDispenseAmount;

  public ReagentDispenserSetDispenseAmountMessage(ReagentDispenserDispenseAmount amount)
  {
    this.ReagentDispenserDispenseAmount = amount;
  }

  public ReagentDispenserSetDispenseAmountMessage(string s)
  {
    if (s != null)
    {
      switch (s.Length)
      {
        case 1:
          switch (s[0])
          {
            case '1':
              this.ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U1;
              return;
            case '5':
              this.ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U5;
              return;
          }
          break;
        case 2:
          switch (s[0])
          {
            case '1':
              switch (s)
              {
                case "10":
                  this.ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U10;
                  return;
                case "15":
                  this.ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U15;
                  return;
              }
              break;
            case '2':
              switch (s)
              {
                case "20":
                  this.ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U20;
                  return;
                case "25":
                  this.ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U25;
                  return;
              }
              break;
            case '3':
              if (s == "30")
              {
                this.ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U30;
                return;
              }
              break;
            case '5':
              if (s == "50")
              {
                this.ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U50;
                return;
              }
              break;
          }
          break;
        case 3:
          if (s == "100")
          {
            this.ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U100;
            return;
          }
          break;
      }
    }
    throw new Exception($"Cannot convert the string `{s}` into a valid ReagentDispenser DispenseAmount");
  }
}
