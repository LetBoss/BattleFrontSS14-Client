// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceLinking.Events.NewLinkEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.DeviceLinking.Events;

public sealed class NewLinkEvent : EntityEventArgs
{
  public readonly EntityUid Source;
  public readonly EntityUid Sink;
  public readonly EntityUid? User;
  public readonly string SourcePort;
  public readonly string SinkPort;

  public NewLinkEvent(
    EntityUid? user,
    EntityUid source,
    string sourcePort,
    EntityUid sink,
    string sinkPort)
  {
    this.User = user;
    this.Source = source;
    this.SourcePort = sourcePort;
    this.Sink = sink;
    this.SinkPort = sinkPort;
  }
}
