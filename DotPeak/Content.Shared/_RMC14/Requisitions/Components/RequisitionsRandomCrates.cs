// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Requisitions.Components.RequisitionsRandomCrates
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Requisitions.Components;

[DataRecord]
[NetSerializable]
[Serializable]
public sealed class RequisitionsRandomCrates
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan Every;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int Minimum;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int MinimumFor;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public List<EntProtoId> Choices = new List<EntProtoId>();
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan Next;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int Given;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public double Fraction;
}
