// Decompiled with JetBrains decompiler
// Type: Content.Shared.Hands.HeldVisualsUpdatedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Hands;

public sealed class HeldVisualsUpdatedEvent : EntityEventArgs
{
  public readonly EntityUid User;
  public HashSet<string> RevealedLayers;

  public HeldVisualsUpdatedEvent(EntityUid user, HashSet<string> revealedLayers)
  {
    this.User = user;
    this.RevealedLayers = revealedLayers;
  }
}
