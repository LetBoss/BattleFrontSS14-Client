// Decompiled with JetBrains decompiler
// Type: Content.Shared.Pulling.Events.BeingPulledAttemptEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Shared.Pulling.Events;

public sealed class BeingPulledAttemptEvent : CancellableEntityEventArgs
{
  public BeingPulledAttemptEvent(EntityUid puller, EntityUid pulled)
  {
    this.Puller = puller;
    this.Pulled = pulled;
  }

  public EntityUid Puller { get; }

  public EntityUid Pulled { get; }
}
