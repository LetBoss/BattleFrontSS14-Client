// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EntityManagerExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using Robust.Shared.Random;

#nullable enable
namespace Robust.Shared.GameObjects;

public static class EntityManagerExt
{
  public static T? GetComponentOrNull<T>(this IEntityManager entityManager, EntityUid entityUid) where T : IComponent
  {
    T component;
    return entityManager.TryGetComponent<T>(entityUid, out component) ? component : default (T);
  }

  public static T? GetComponentOrNull<T>(this IEntityManager entityManager, EntityUid? entityUid) where T : IComponent
  {
    T component;
    return entityUid.HasValue && entityManager.TryGetComponent<T>(entityUid.Value, out component) ? component : default (T);
  }

  public static bool TryGetRandom<TComp1>(
    this IEntityManager entManager,
    IRobustRandom random,
    out EntityUid entity,
    bool includePaused = false)
    where TComp1 : IComponent
  {
    ValueList<EntityUid> list = new ValueList<EntityUid>();
    TComp1 comp1;
    if (includePaused)
    {
      AllEntityQueryEnumerator<TComp1> entityQueryEnumerator = entManager.AllEntityQueryEnumerator<TComp1>();
      EntityUid uid;
      while (entityQueryEnumerator.MoveNext(out uid, out comp1))
        list.Add(uid);
    }
    else
    {
      EntityQueryEnumerator<TComp1> entityQueryEnumerator = entManager.EntityQueryEnumerator<TComp1>();
      EntityUid uid;
      while (entityQueryEnumerator.MoveNext(out uid, out comp1))
        list.Add(uid);
    }
    if (list.Count == 0)
    {
      entity = EntityUid.Invalid;
      return false;
    }
    entity = random.Pick<EntityUid>(list);
    return true;
  }

  public static bool TryGetRandom<TComp1, TComp2>(
    this IEntityManager entManager,
    IRobustRandom random,
    out EntityUid entity,
    bool includePaused = false)
    where TComp1 : IComponent
    where TComp2 : IComponent
  {
    ValueList<EntityUid> list = new ValueList<EntityUid>();
    TComp1 comp1;
    TComp2 comp2;
    if (includePaused)
    {
      AllEntityQueryEnumerator<TComp1, TComp2> entityQueryEnumerator = entManager.AllEntityQueryEnumerator<TComp1, TComp2>();
      EntityUid uid;
      while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
        list.Add(uid);
    }
    else
    {
      EntityQueryEnumerator<TComp1, TComp2> entityQueryEnumerator = entManager.EntityQueryEnumerator<TComp1, TComp2>();
      EntityUid uid;
      while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
        list.Add(uid);
    }
    if (list.Count == 0)
    {
      entity = EntityUid.Invalid;
      return false;
    }
    entity = random.Pick<EntityUid>(list);
    return true;
  }
}
