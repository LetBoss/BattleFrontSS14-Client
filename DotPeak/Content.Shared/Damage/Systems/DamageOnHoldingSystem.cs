// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Systems.DamageOnHoldingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Damage.Systems;

public sealed class DamageOnHoldingSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private DamageableSystem _damageableSystem;
  [Dependency]
  private IGameTiming _timing;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamageOnHoldingComponent, MapInitEvent>(new ComponentEventHandler<DamageOnHoldingComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
  }

  public void SetEnabled(EntityUid uid, bool enabled, DamageOnHoldingComponent? component = null)
  {
    if (!this.Resolve<DamageOnHoldingComponent>(uid, ref component, true))
      return;
    component.Enabled = enabled;
    component.NextDamage = this._timing.CurTime;
  }

  private void OnMapInit(EntityUid uid, DamageOnHoldingComponent component, MapInitEvent args)
  {
    component.NextDamage = this._timing.CurTime;
  }

  public virtual void Update(float frameTime)
  {
    EntityQueryEnumerator<DamageOnHoldingComponent> entityQueryEnumerator = this.EntityQueryEnumerator<DamageOnHoldingComponent>();
    EntityUid entityUid;
    DamageOnHoldingComponent holdingComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref holdingComponent))
    {
      if (holdingComponent.Enabled && !(holdingComponent.NextDamage > this._timing.CurTime))
      {
        BaseContainer baseContainer;
        if (this._container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((entityUid, (TransformComponent) null, (MetaDataComponent) null)), ref baseContainer))
          this._damageableSystem.TryChangeDamage(new EntityUid?(baseContainer.Owner), holdingComponent.Damage, origin: new EntityUid?(entityUid));
        holdingComponent.NextDamage = this._timing.CurTime + TimeSpan.FromSeconds((double) holdingComponent.Interval);
      }
    }
  }
}
