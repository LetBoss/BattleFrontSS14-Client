// Decompiled with JetBrains decompiler
// Type: Content.Shared.Random.RandomHelperSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using System.Numerics;

#nullable enable
namespace Content.Shared.Random;

public sealed class RandomHelperSystem : EntitySystem
{
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private IRobustRandom _random;

  public void RandomOffset(EntityUid entity, float minX, float maxX, float minY, float maxY)
  {
    Vector2 vector2 = new Vector2(this._random.NextFloat() * (maxX - minX) + minX, this._random.NextFloat() * (maxY - minY) + minY);
    TransformComponent xform = this.Transform(entity);
    this._transform.SetLocalPosition(entity, xform.LocalPosition + vector2, xform);
  }

  public void RandomOffset(EntityUid entity, float min, float max)
  {
    this.RandomOffset(entity, min, max, min, max);
  }

  public void RandomOffset(EntityUid entity, float value)
  {
    this.RandomOffset(entity, -value, value);
  }
}
