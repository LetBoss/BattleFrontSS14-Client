// Decompiled with JetBrains decompiler
// Type: Content.Shared.DoAfter.DoAfterAttemptEvent`1
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.DoAfter;

public sealed class DoAfterAttemptEvent<TEvent> : CancellableEntityEventArgs where TEvent : DoAfterEvent
{
  public readonly Content.Shared.DoAfter.DoAfter DoAfter;
  public readonly TEvent Event;

  public DoAfterAttemptEvent(Content.Shared.DoAfter.DoAfter doAfter, TEvent @event)
  {
    this.DoAfter = doAfter;
    this.Event = @event;
  }
}
