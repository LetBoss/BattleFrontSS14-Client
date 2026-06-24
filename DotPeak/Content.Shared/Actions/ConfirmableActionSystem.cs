// Decompiled with JetBrains decompiler
// Type: Content.Shared.Actions.ConfirmableActionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions.Components;
using Content.Shared.Actions.Events;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Actions;

public sealed class ConfirmableActionSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedPopupSystem _popup;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ConfirmableActionComponent, ActionAttemptEvent>(new EntityEventRefHandler<ConfirmableActionComponent, ActionAttemptEvent>((object) this, __methodptr(OnAttempt)), (Type[]) null, (Type[]) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    TimeSpan curTime = this._timing.CurTime;
    EntityQueryEnumerator<ConfirmableActionComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ConfirmableActionComponent>();
    EntityUid entityUid;
    ConfirmableActionComponent confirmableActionComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref confirmableActionComponent))
    {
      TimeSpan? nextUnprime = confirmableActionComponent.NextUnprime;
      if (nextUnprime.HasValue)
      {
        TimeSpan valueOrDefault = nextUnprime.GetValueOrDefault();
        if (curTime >= valueOrDefault)
          this.Unprime(Entity<ConfirmableActionComponent>.op_Implicit((entityUid, confirmableActionComponent)));
      }
    }
  }

  private void OnAttempt(Entity<ConfirmableActionComponent> ent, ref ActionAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    TimeSpan? nextConfirm = ent.Comp.NextConfirm;
    if (nextConfirm.HasValue)
    {
      if (this._timing.CurTime < nextConfirm.GetValueOrDefault())
        args.Cancelled = true;
      else
        this.Unprime(ent);
    }
    else
    {
      this.Prime(ent, args.User);
      args.Cancelled = true;
    }
  }

  private void Prime(Entity<ConfirmableActionComponent> ent, EntityUid user)
  {
    EntityUid entityUid1;
    ConfirmableActionComponent confirmableActionComponent1;
    ent.Deconstruct(ref entityUid1, ref confirmableActionComponent1);
    EntityUid entityUid2 = entityUid1;
    ConfirmableActionComponent confirmableActionComponent2 = confirmableActionComponent1;
    confirmableActionComponent2.NextConfirm = new TimeSpan?(this._timing.CurTime + confirmableActionComponent2.ConfirmDelay);
    ConfirmableActionComponent confirmableActionComponent3 = confirmableActionComponent2;
    TimeSpan? nextConfirm = confirmableActionComponent2.NextConfirm;
    TimeSpan primeTime = confirmableActionComponent2.PrimeTime;
    TimeSpan? nullable = nextConfirm.HasValue ? new TimeSpan?(nextConfirm.GetValueOrDefault() + primeTime) : new TimeSpan?();
    confirmableActionComponent3.NextUnprime = nullable;
    this.Dirty(entityUid2, (IComponent) confirmableActionComponent2, (MetaDataComponent) null);
    this._popup.PopupClient(this.Loc.GetString(LocId.op_Implicit(confirmableActionComponent2.Popup)), user, new EntityUid?(user), PopupType.LargeCaution);
  }

  private void Unprime(Entity<ConfirmableActionComponent> ent)
  {
    EntityUid entityUid1;
    ConfirmableActionComponent confirmableActionComponent1;
    ent.Deconstruct(ref entityUid1, ref confirmableActionComponent1);
    EntityUid entityUid2 = entityUid1;
    ConfirmableActionComponent confirmableActionComponent2 = confirmableActionComponent1;
    confirmableActionComponent2.NextConfirm = new TimeSpan?();
    confirmableActionComponent2.NextUnprime = new TimeSpan?();
    this.Dirty(entityUid2, (IComponent) confirmableActionComponent2, (MetaDataComponent) null);
  }
}
