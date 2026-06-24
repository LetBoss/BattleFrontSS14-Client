// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.SharedAppearanceSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.GameObjects;

public abstract class SharedAppearanceSystem : EntitySystem
{
  [Dependency]
  private readonly IGameTiming _timing;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<AppearanceComponent, ComponentGetState>(new ComponentEventRefHandler<AppearanceComponent, ComponentGetState>(this.OnAppearanceGetState));
  }

  protected abstract void OnAppearanceGetState(
    EntityUid uid,
    AppearanceComponent component,
    ref ComponentGetState args);

  public virtual void QueueUpdate(EntityUid uid, AppearanceComponent component)
  {
  }

  private bool CheckIfApplyingState(AppearanceComponent component)
  {
    return this._timing.ApplyingState && component.NetSyncEnabled;
  }

  public void SetData(EntityUid uid, Enum key, object value, AppearanceComponent? component = null)
  {
    object obj;
    if (!this.Resolve<AppearanceComponent>(uid, ref component, false) || this.CheckIfApplyingState(component) || component.AppearanceData.TryGetValue(key, out obj) && obj.Equals(value))
      return;
    component.AppearanceData[key] = value;
    this.Dirty(uid, (IComponent) component);
    this.QueueUpdate(uid, component);
  }

  public void RemoveData(EntityUid uid, Enum key, AppearanceComponent? component = null)
  {
    if (!this.Resolve<AppearanceComponent>(uid, ref component, false) || this.CheckIfApplyingState(component))
      return;
    component.AppearanceData.Remove(key);
    this.Dirty(uid, (IComponent) component);
    this.QueueUpdate(uid, component);
  }

  public bool TryGetData<T>(EntityUid uid, Enum key, [NotNullWhen(true)] out T value, AppearanceComponent? component = null)
  {
    object obj1;
    if (this.Resolve<AppearanceComponent>(uid, ref component) && component.AppearanceData.TryGetValue(key, out obj1) && obj1 is T obj2)
    {
      value = obj2;
      return true;
    }
    value = default (T);
    return false;
  }

  public bool TryGetData(EntityUid uid, Enum key, [NotNullWhen(true)] out object? value, AppearanceComponent? component = null)
  {
    if (this.Resolve<AppearanceComponent>(uid, ref component))
      return component.AppearanceData.TryGetValue(key, out value);
    value = (object) null;
    return false;
  }

  public void CopyData(Entity<AppearanceComponent?> src, Entity<AppearanceComponent?> dest)
  {
    if (!this.Resolve<AppearanceComponent>((EntityUid) src, ref src.Comp, false))
      return;
    ref AppearanceComponent local = ref dest.Comp;
    if (local == null)
      local = this.EnsureComp<AppearanceComponent>((EntityUid) dest);
    dest.Comp.AppearanceData.Clear();
    foreach ((Enum key, object obj) in src.Comp.AppearanceData)
      dest.Comp.AppearanceData[key] = obj;
    this.Dirty((EntityUid) dest, (IComponent) dest.Comp);
    this.QueueUpdate((EntityUid) dest, dest.Comp);
  }

  public void AppendData(Entity<AppearanceComponent?> src, Entity<AppearanceComponent?> dest)
  {
    if (!this.Resolve<AppearanceComponent>((EntityUid) src, ref src.Comp, false))
      return;
    this.AppendData(src.Comp, dest);
  }

  public void AppendData(AppearanceComponent srcComp, Entity<AppearanceComponent?> dest)
  {
    ref AppearanceComponent local = ref dest.Comp;
    if (local == null)
      local = this.EnsureComp<AppearanceComponent>((EntityUid) dest);
    foreach ((Enum key, object obj) in srcComp.AppearanceData)
      dest.Comp.AppearanceData[key] = obj;
    this.Dirty((EntityUid) dest, (IComponent) dest.Comp);
    this.QueueUpdate((EntityUid) dest, dest.Comp);
  }
}
