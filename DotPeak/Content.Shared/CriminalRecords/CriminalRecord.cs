// Decompiled with JetBrains decompiler
// Type: Content.Shared.CriminalRecords.CriminalRecord
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Security;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.CriminalRecords;

[NetSerializable]
[DataRecord]
[Serializable]
public sealed record CriminalRecord
{
  [DataField(null, false, 1, false, false, null)]
  public SecurityStatus Status;
  [DataField(null, false, 1, false, false, null)]
  public string? Reason;
  [DataField(null, false, 1, false, false, null)]
  public string? InitiatorName;
  [DataField(null, false, 1, false, false, null)]
  public List<CrimeHistory> History = new List<CrimeHistory>();

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return (((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<SecurityStatus>.Default.GetHashCode(this.Status)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Reason)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.InitiatorName)) * -1521134295 + EqualityComparer<List<CrimeHistory>>.Default.GetHashCode(this.History);
  }

  [CompilerGenerated]
  public bool Equals(CriminalRecord? other)
  {
    if ((object) this == (object) other)
      return true;
    return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<SecurityStatus>.Default.Equals(this.Status, other.Status) && EqualityComparer<string>.Default.Equals(this.Reason, other.Reason) && EqualityComparer<string>.Default.Equals(this.InitiatorName, other.InitiatorName) && EqualityComparer<List<CrimeHistory>>.Default.Equals(this.History, other.History);
  }
}
