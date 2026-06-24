// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.BodyType
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Robust.Shared.Physics;

[NetSerializable]
[Flags]
[Serializable]
public enum BodyType : byte
{
  Kinematic = 0,
  KinematicController = 2,
  Static = 4,
  Dynamic = 8,
}
