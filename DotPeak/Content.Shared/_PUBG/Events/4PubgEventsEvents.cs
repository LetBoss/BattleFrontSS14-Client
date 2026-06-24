// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Events.PubgEventClaimMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.Events;

[NetSerializable]
[Serializable]
public sealed class PubgEventClaimMessage : EntityEventArgs
{
  public string EventKey { get; }

  public string ClaimType { get; }

  public string ClaimId { get; }

  public PubgEventClaimMessage(string eventKey, string claimType, string claimId)
  {
    this.EventKey = eventKey;
    this.ClaimType = claimType;
    this.ClaimId = claimId;
  }
}
