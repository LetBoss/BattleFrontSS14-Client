// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Events.RequestShootEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Events;

[NetSerializable]
[Serializable]
public sealed class RequestShootEvent : EntityEventArgs
{
  public NetEntity Gun;
  public NetCoordinates Coordinates;
  public NetEntity? Target;
  public List<int>? Shot;
  public GameTick LastRealTick;
}
