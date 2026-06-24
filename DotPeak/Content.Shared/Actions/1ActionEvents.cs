// Decompiled with JetBrains decompiler
// Type: Content.Shared.Actions.RequestPerformActionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;

#nullable disable
namespace Content.Shared.Actions;

[NetSerializable]
[Serializable]
public sealed class RequestPerformActionEvent : EntityEventArgs
{
  public readonly NetEntity Action;
  public readonly NetEntity? EntityTarget;
  public readonly NetCoordinates? EntityCoordinatesTarget;
  public readonly GameTick LastRealTick;

  public RequestPerformActionEvent(NetEntity action, GameTick lastRealTick)
  {
    this.Action = action;
    this.LastRealTick = lastRealTick;
  }

  public RequestPerformActionEvent(NetEntity action, NetEntity entityTarget, GameTick lastRealTick)
  {
    this.Action = action;
    this.EntityTarget = new NetEntity?(entityTarget);
    this.LastRealTick = lastRealTick;
  }

  public RequestPerformActionEvent(
    NetEntity action,
    NetCoordinates entityCoordinatesTarget,
    GameTick lastRealTick)
  {
    this.Action = action;
    this.EntityCoordinatesTarget = new NetCoordinates?(entityCoordinatesTarget);
    this.LastRealTick = lastRealTick;
  }

  public RequestPerformActionEvent(
    NetEntity action,
    NetEntity? entityTarget,
    NetCoordinates entityCoordinatesTarget,
    GameTick lastRealTick)
  {
    this.Action = action;
    this.EntityTarget = entityTarget;
    this.EntityCoordinatesTarget = new NetCoordinates?(entityCoordinatesTarget);
    this.LastRealTick = lastRealTick;
  }
}
