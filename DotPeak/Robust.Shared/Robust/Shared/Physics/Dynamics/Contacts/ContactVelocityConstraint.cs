// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Contacts.ContactVelocityConstraint
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Physics.Dynamics.Contacts;

internal struct ContactVelocityConstraint
{
  public int ContactIndex;
  public int IndexA;
  public int IndexB;
  public FixedArray2<VelocityConstraintPoint> Points;
  public Vector2 Normal;
  public Vector4 NormalMass;
  public Vector4 K;
  public float InvMassA;
  public float InvMassB;
  public float InvIA;
  public float InvIB;
  public float Friction;
  public float Restitution;
  public float TangentSpeed;
  public int PointCount;
}
