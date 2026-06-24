// Decompiled with JetBrains decompiler
// Type: Content.Shared.CriminalRecords.CriminalRecordSetStatusFilter
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Security;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.CriminalRecords;

[NetSerializable]
[Serializable]
public sealed class CriminalRecordSetStatusFilter : BoundUserInterfaceMessage
{
  public readonly SecurityStatus FilterStatus;

  public CriminalRecordSetStatusFilter(SecurityStatus newFilterStatus)
  {
    this.FilterStatus = newFilterStatus;
  }
}
