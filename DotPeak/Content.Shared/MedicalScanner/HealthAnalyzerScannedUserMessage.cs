// Decompiled with JetBrains decompiler
// Type: Content.Shared.MedicalScanner.HealthAnalyzerScannedUserMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.MedicalScanner;

[NetSerializable]
[Serializable]
public sealed class HealthAnalyzerScannedUserMessage : BoundUserInterfaceMessage
{
  public readonly NetEntity? TargetEntity;
  public float Temperature;
  public float BloodLevel;
  public bool? ScanMode;
  public bool? Bleeding;
  public bool? Unrevivable;

  public HealthAnalyzerScannedUserMessage(
    NetEntity? targetEntity,
    float temperature,
    float bloodLevel,
    bool? scanMode,
    bool? bleeding,
    bool? unrevivable)
  {
    this.TargetEntity = targetEntity;
    this.Temperature = temperature;
    this.BloodLevel = bloodLevel;
    this.ScanMode = scanMode;
    this.Bleeding = bleeding;
    this.Unrevivable = unrevivable;
  }
}
