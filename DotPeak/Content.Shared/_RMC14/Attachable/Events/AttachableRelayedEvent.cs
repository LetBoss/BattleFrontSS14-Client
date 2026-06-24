// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Events.AttachableRelayedEvent`1
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared._RMC14.Attachable.Events;

public sealed class AttachableRelayedEvent<TEvent> : EntityEventArgs
{
  public TEvent Args;
  public EntityUid Holder;

  public AttachableRelayedEvent(TEvent args, EntityUid holder)
  {
    this.Args = args;
    this.Holder = holder;
  }
}
