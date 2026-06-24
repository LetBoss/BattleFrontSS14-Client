// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EntityQuery`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

public readonly struct EntityQuery<TComp1> where TComp1 : IComponent
{
  private readonly EntityManager _entMan;
  private readonly Dictionary<EntityUid, IComponent> _traitDict;

  internal EntityQuery(EntityManager entMan, Dictionary<EntityUid, IComponent> traitDict)
  {
    this._entMan = entMan;
    this._traitDict = traitDict;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public TComp1 GetComponent(EntityUid uid)
  {
    IComponent component;
    if (this._traitDict.TryGetValue(uid, out component) && !component.Deleted)
      return (TComp1) component;
    throw new KeyNotFoundException($"Entity {uid} does not have a component of type {typeof (TComp1)}");
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Entity<TComp1> Get(EntityUid uid)
  {
    IComponent comp;
    if (this._traitDict.TryGetValue(uid, out comp) && !comp.Deleted)
      return new Entity<TComp1>(uid, (TComp1) comp);
    throw new KeyNotFoundException($"Entity {uid} does not have a component of type {typeof (TComp1)}");
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool TryGetComponent([NotNullWhen(true)] EntityUid? uid, [NotNullWhen(true)] out TComp1? component)
  {
    if (uid.HasValue)
      return this.TryGetComponent(uid.Value, out component);
    component = default (TComp1);
    return false;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool TryGetComponent(EntityUid uid, [NotNullWhen(true)] out TComp1? component)
  {
    IComponent component1;
    if (this._traitDict.TryGetValue(uid, out component1) && !component1.Deleted)
    {
      component = (TComp1) component1;
      return true;
    }
    component = default (TComp1);
    return false;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool TryComp(EntityUid uid, [NotNullWhen(true)] out TComp1? component)
  {
    return this.TryGetComponent(uid, out component);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool TryComp([NotNullWhen(true)] EntityUid? uid, [NotNullWhen(true)] out TComp1? component)
  {
    return this.TryGetComponent(uid, out component);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool HasComp(EntityUid uid) => this.HasComponent(uid);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool HasComp([NotNullWhen(true)] EntityUid? uid) => this.HasComponent(uid);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool HasComponent(EntityUid uid)
  {
    IComponent component;
    return this._traitDict.TryGetValue(uid, out component) && !component.Deleted;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool HasComponent([NotNullWhen(true)] EntityUid? uid)
  {
    return uid.HasValue && this.HasComponent(uid.Value);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Resolve(EntityUid uid, [NotNullWhen(true)] ref TComp1? component, bool logMissing = true)
  {
    if ((object) component != null)
      return true;
    IComponent component1;
    if (this._traitDict.TryGetValue(uid, out component1) && !component1.Deleted)
    {
      component = (TComp1) component1;
      return true;
    }
    if (logMissing)
      this._entMan.ResolveSawmill.Error($"Can't resolve \"{typeof (TComp1)}\" on entity {this._entMan.ToPrettyString((Entity<MetaDataComponent>) uid)}!\n{Environment.StackTrace}");
    return false;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Resolve(ref Entity<TComp1?> entity, bool logMissing = true)
  {
    return this.Resolve(entity.Owner, ref entity.Comp, logMissing);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public TComp1? CompOrNull(EntityUid uid)
  {
    TComp1 component;
    return this.TryGetComponent(uid, out component) ? component : default (TComp1);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public TComp1 Comp(EntityUid uid) => this.GetComponent(uid);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal TComp1 GetComponentInternal(EntityUid uid)
  {
    IComponent componentInternal;
    if (this._traitDict.TryGetValue(uid, out componentInternal))
      return (TComp1) componentInternal;
    throw new KeyNotFoundException($"Entity {uid} does not have a component of type {typeof (TComp1)}");
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal bool TryGetComponentInternal([NotNullWhen(true)] EntityUid? uid, [NotNullWhen(true)] out TComp1? component)
  {
    if (uid.HasValue)
      return this.TryGetComponentInternal(uid.Value, out component);
    component = default (TComp1);
    return false;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal bool TryGetComponentInternal(EntityUid uid, [NotNullWhen(true)] out TComp1? component)
  {
    IComponent component1;
    if (this._traitDict.TryGetValue(uid, out component1))
    {
      component = (TComp1) component1;
      return true;
    }
    component = default (TComp1);
    return false;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal bool HasComponentInternal(EntityUid uid)
  {
    IComponent component;
    return this._traitDict.TryGetValue(uid, out component) && !component.Deleted;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal bool ResolveInternal(EntityUid uid, [NotNullWhen(true)] ref TComp1? component, bool logMissing = true)
  {
    if ((object) component != null)
      return true;
    IComponent component1;
    if (this._traitDict.TryGetValue(uid, out component1))
    {
      component = (TComp1) component1;
      return true;
    }
    if (logMissing)
      this._entMan.ResolveSawmill.Error($"Can't resolve \"{typeof (TComp1)}\" on entity {this._entMan.ToPrettyString((Entity<MetaDataComponent>) uid)}!\n{new StackTrace(1, true)}");
    return false;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal TComp1? CompOrNullInternal(EntityUid uid)
  {
    TComp1 component;
    return this.TryGetComponent(uid, out component) ? component : default (TComp1);
  }
}
