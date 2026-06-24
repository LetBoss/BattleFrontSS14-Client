// Decompiled with JetBrains decompiler
// Type: Content.Shared.Shuttles.UI.MapObjects.ShuttleExclusionObject
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Shuttles.UI.MapObjects;

[NetSerializable]
[Serializable]
public record struct ShuttleExclusionObject(NetCoordinates Coordinates, float Range, string Name = "") : 
  IMapObject
{
  public bool HideButton => false;
}
