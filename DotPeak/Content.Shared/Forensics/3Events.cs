// Decompiled with JetBrains decompiler
// Type: Content.Shared.Forensics.TransferDnaEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Forensics;

[ByRefEvent]
public record struct TransferDnaEvent
{
  public EntityUid Donor;
  public EntityUid Recipient;
  public bool CanDnaBeCleaned;

  public TransferDnaEvent()
  {
    this.Donor = new EntityUid();
    this.Recipient = new EntityUid();
    this.CanDnaBeCleaned = true;
  }

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return (EqualityComparer<EntityUid>.Default.GetHashCode(this.Donor) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.Recipient)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.CanDnaBeCleaned);
  }

  [CompilerGenerated]
  public readonly bool Equals(TransferDnaEvent other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.Donor, other.Donor) && EqualityComparer<EntityUid>.Default.Equals(this.Recipient, other.Recipient) && EqualityComparer<bool>.Default.Equals(this.CanDnaBeCleaned, other.CanDnaBeCleaned);
  }
}
