// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.RayCastResults
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Physics;

public readonly struct RayCastResults(float distance, Vector2 hitPos, EntityUid hitEntity)
{
  public EntityUid HitEntity { get; } = hitEntity;

  public Vector2 HitPos { get; } = hitPos;

  public float Distance { get; } = distance;
}
