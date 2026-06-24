// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Contacts.ContactPositionConstraint
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Physics.Collision;
using Robust.Shared.Utility;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Physics.Dynamics.Contacts;

internal struct ContactPositionConstraint
{
  internal FixedArray2<Vector2> LocalPoints;
  public Vector2 LocalNormal;
  public Vector2 LocalPoint;
  public float InvMassA;
  public float InvMassB;
  public Vector2 LocalCenterA;
  public Vector2 LocalCenterB;
  public float InvIA;
  public float InvIB;
  public ManifoldType Type;
  public float RadiusA;
  public float RadiusB;
  public int PointCount;

  public int IndexA { get; set; }

  public int IndexB { get; set; }
}
