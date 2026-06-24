// Decompiled with JetBrains decompiler
// Type: Content.Shared.Beam.Components.BeamVisualizerEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Beam.Components;

[NetSerializable]
[Serializable]
public sealed class BeamVisualizerEvent : EntityEventArgs
{
  public readonly NetEntity Beam;
  public readonly float DistanceLength;
  public readonly Angle UserAngle;
  public readonly string? BodyState;
  public readonly string Shader = "unshaded";

  public BeamVisualizerEvent(
    NetEntity beam,
    float distanceLength,
    Angle userAngle,
    string? bodyState = null,
    string shader = "unshaded")
  {
    this.Beam = beam;
    this.DistanceLength = distanceLength;
    this.UserAngle = userAngle;
    this.BodyState = bodyState;
    this.Shader = shader;
  }
}
