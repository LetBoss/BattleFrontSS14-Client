// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.ChemMasterCreatePillsMessage
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
public sealed class ChemMasterCreatePillsMessage : BoundUserInterfaceMessage
{
  public readonly uint Dosage;
  public readonly uint Number;
  public readonly string Label;

  public ChemMasterCreatePillsMessage(uint dosage, uint number, string label)
  {
    this.Dosage = dosage;
    this.Number = number;
    this.Label = label;
  }
}
