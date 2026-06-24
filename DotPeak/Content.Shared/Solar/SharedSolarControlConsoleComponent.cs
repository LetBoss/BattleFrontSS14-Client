// Decompiled with JetBrains decompiler
// Type: Content.Shared.Solar.SolarControlConsoleBoundInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Solar;

[NetSerializable]
[Serializable]
public sealed class SolarControlConsoleBoundInterfaceState : BoundUserInterfaceState
{
  public Angle Rotation;
  public Angle AngularVelocity;
  public float OutputPower;
  public Angle TowardsSun;

  public SolarControlConsoleBoundInterfaceState(Angle r, Angle vm, float p, Angle tw)
  {
    this.Rotation = r;
    this.AngularVelocity = vm;
    this.OutputPower = p;
    this.TowardsSun = tw;
  }
}
