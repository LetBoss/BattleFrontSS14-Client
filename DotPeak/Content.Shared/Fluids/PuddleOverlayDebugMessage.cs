// Decompiled with JetBrains decompiler
// Type: Content.Shared.Fluids.PuddleOverlayDebugMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Fluids;

[NetSerializable]
[Serializable]
public sealed class PuddleOverlayDebugMessage : EntityEventArgs
{
  public PuddleDebugOverlayData[] OverlayData { get; }

  public NetEntity GridUid { get; }

  public PuddleOverlayDebugMessage(NetEntity gridUid, PuddleDebugOverlayData[] overlayData)
  {
    this.GridUid = gridUid;
    this.OverlayData = overlayData;
  }
}
