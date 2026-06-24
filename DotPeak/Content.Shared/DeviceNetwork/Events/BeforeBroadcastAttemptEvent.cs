// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceNetwork.Events.BeforeBroadcastAttemptEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceNetwork.Components;
using Robust.Shared.GameObjects;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.DeviceNetwork.Events;

public sealed class BeforeBroadcastAttemptEvent : CancellableEntityEventArgs
{
  public readonly IReadOnlySet<DeviceNetworkComponent> Recipients;
  public HashSet<DeviceNetworkComponent>? ModifiedRecipients;

  public BeforeBroadcastAttemptEvent(IReadOnlySet<DeviceNetworkComponent> recipients)
  {
    this.Recipients = recipients;
  }
}
