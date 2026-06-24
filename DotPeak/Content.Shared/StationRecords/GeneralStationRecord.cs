// Decompiled with JetBrains decompiler
// Type: Content.Shared.StationRecords.GeneralStationRecord
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Enums;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.StationRecords;

[NetSerializable]
[Serializable]
public sealed record GeneralStationRecord
{
  [DataField(null, false, 1, false, false, null)]
  public string Name = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public int Age;
  [DataField(null, false, 1, false, false, null)]
  public string JobTitle = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public string JobIcon = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public string JobPrototype = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public string? Squad;
  [DataField(null, false, 1, false, false, null)]
  public string Species = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public Gender Gender = Gender.Epicene;
  [DataField(null, false, 1, false, false, null)]
  public int DisplayPriority;
  [DataField(null, false, 1, false, false, null)]
  public string? Fingerprint;
  [DataField(null, false, 1, false, false, null)]
  public string? DNA;

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return ((((((((((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Name)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.Age)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.JobTitle)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.JobIcon)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.JobPrototype)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Squad)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Species)) * -1521134295 + EqualityComparer<Gender>.Default.GetHashCode(this.Gender)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.DisplayPriority)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Fingerprint)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.DNA);
  }

  [CompilerGenerated]
  public bool Equals(GeneralStationRecord? other)
  {
    if ((object) this == (object) other)
      return true;
    return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.Name, other.Name) && EqualityComparer<int>.Default.Equals(this.Age, other.Age) && EqualityComparer<string>.Default.Equals(this.JobTitle, other.JobTitle) && EqualityComparer<string>.Default.Equals(this.JobIcon, other.JobIcon) && EqualityComparer<string>.Default.Equals(this.JobPrototype, other.JobPrototype) && EqualityComparer<string>.Default.Equals(this.Squad, other.Squad) && EqualityComparer<string>.Default.Equals(this.Species, other.Species) && EqualityComparer<Gender>.Default.Equals(this.Gender, other.Gender) && EqualityComparer<int>.Default.Equals(this.DisplayPriority, other.DisplayPriority) && EqualityComparer<string>.Default.Equals(this.Fingerprint, other.Fingerprint) && EqualityComparer<string>.Default.Equals(this.DNA, other.DNA);
  }
}
