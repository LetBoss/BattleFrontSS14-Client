// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Map.RMCAnchoredEntitiesEnumerator
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Shared._RMC14.Map;

public struct RMCAnchoredEntitiesEnumerator(
  SharedTransformSystem transform,
  AnchoredEntitiesEnumerator enumerator,
  DirectionFlag facing = 0) : IDisposable
{
  private AnchoredEntitiesEnumerator _enumerator = enumerator;
  public static readonly RMCAnchoredEntitiesEnumerator Empty = new RMCAnchoredEntitiesEnumerator((SharedTransformSystem) null, AnchoredEntitiesEnumerator.Empty, (DirectionFlag) 0);

  public bool MoveNext(out EntityUid uid)
  {
    EntityUid? uid1;
    while (this._enumerator.MoveNext(out uid1))
    {
      if (facing == null)
      {
        uid = uid1.Value;
        return true;
      }
      Angle worldRotation = transform.GetWorldRotation(uid1.Value);
      if ((DirectionExtensions.AsFlag(((Angle) ref worldRotation).GetDir()) & facing) != null)
      {
        uid = uid1.Value;
        return true;
      }
    }
    uid = new EntityUid();
    return false;
  }

  public void Dispose() => this._enumerator.Dispose();
}
