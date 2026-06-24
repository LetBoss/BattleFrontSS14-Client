// Decompiled with JetBrains decompiler
// Type: Content.Shared.Radiation.Events.OnRadiationOverlayResistanceUpdateEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Radiation.Events;

[NetSerializable]
[Serializable]
public sealed class OnRadiationOverlayResistanceUpdateEvent : EntityEventArgs
{
  public readonly Dictionary<NetEntity, Dictionary<Vector2i, float>> Grids;

  public OnRadiationOverlayResistanceUpdateEvent(
    Dictionary<NetEntity, Dictionary<Vector2i, float>> grids)
  {
    this.Grids = grids;
  }
}
