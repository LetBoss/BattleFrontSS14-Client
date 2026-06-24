// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.MetabolismMovespeedModifierSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chemistry;

public sealed class MetabolismMovespeedModifierSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private MovementSpeedModifierSystem _movespeed;
  private readonly List<Entity<MovespeedModifierMetabolismComponent>> _components = new List<Entity<MovespeedModifierMetabolismComponent>>();

  public virtual void Initialize()
  {
    base.Initialize();
    this.UpdatesOutsidePrediction = true;
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MovespeedModifierMetabolismComponent, ComponentStartup>(new EntityEventRefHandler<MovespeedModifierMetabolismComponent, ComponentStartup>((object) this, __methodptr(AddComponent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MovespeedModifierMetabolismComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<MovespeedModifierMetabolismComponent, RefreshMovementSpeedModifiersEvent>((object) this, __methodptr(OnRefreshMovespeed)), (Type[]) null, (Type[]) null);
  }

  private void OnRefreshMovespeed(
    EntityUid uid,
    MovespeedModifierMetabolismComponent component,
    RefreshMovementSpeedModifiersEvent args)
  {
    args.ModifySpeed(component.WalkSpeedModifier, component.SprintSpeedModifier);
  }

  private void AddComponent(
    Entity<MovespeedModifierMetabolismComponent> metabolism,
    ref ComponentStartup args)
  {
    this._components.Add(metabolism);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    TimeSpan curTime = this._gameTiming.CurTime;
    for (int index = this._components.Count - 1; index >= 0; --index)
    {
      Entity<MovespeedModifierMetabolismComponent> component = this._components[index];
      if (component.Comp.Deleted)
        this._components.RemoveAt(index);
      else if (!(component.Comp.ModifierTimer > curTime))
      {
        this._components.RemoveAt(index);
        this.RemComp<MovespeedModifierMetabolismComponent>(Entity<MovespeedModifierMetabolismComponent>.op_Implicit(component));
        this._movespeed.RefreshMovementSpeedModifiers(Entity<MovespeedModifierMetabolismComponent>.op_Implicit(component));
      }
    }
  }
}
