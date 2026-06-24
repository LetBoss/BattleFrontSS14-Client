// Decompiled with JetBrains decompiler
// Type: Content.Shared.CriminalRecords.Systems.SharedCriminalRecordsHackerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.CriminalRecords.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Ninja.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.CriminalRecords.Systems;

public abstract class SharedCriminalRecordsHackerSystem : EntitySystem
{
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedNinjaGlovesSystem _gloves;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CriminalRecordsHackerComponent, BeforeInteractHandEvent>(new EntityEventRefHandler<CriminalRecordsHackerComponent, BeforeInteractHandEvent>((object) this, __methodptr(OnBeforeInteractHand)), (Type[]) null, (Type[]) null);
  }

  private void OnBeforeInteractHand(
    Entity<CriminalRecordsHackerComponent> ent,
    ref BeforeInteractHandEvent args)
  {
    EntityUid target1;
    if (args.Handled || !this._gloves.AbilityCheck(Entity<CriminalRecordsHackerComponent>.op_Implicit(ent), args, out target1) || !this.HasComp<CriminalRecordsConsoleComponent>(target1))
      return;
    EntityManager entityManager = this.EntityManager;
    EntityUid user = Entity<CriminalRecordsHackerComponent>.op_Implicit(ent);
    TimeSpan delay = ent.Comp.Delay;
    CriminalRecordsHackDoAfterEvent @event = new CriminalRecordsHackDoAfterEvent();
    EntityUid? nullable1 = new EntityUid?(target1);
    EntityUid? nullable2 = new EntityUid?(Entity<CriminalRecordsHackerComponent>.op_Implicit(ent));
    EntityUid? eventTarget = new EntityUid?(Entity<CriminalRecordsHackerComponent>.op_Implicit(ent));
    EntityUid? target2 = nullable1;
    EntityUid? used = nullable2;
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, user, delay, (DoAfterEvent) @event, eventTarget, target2, used)
    {
      BreakOnDamage = true,
      BreakOnMove = true,
      MovementThreshold = 0.5f
    });
    args.Handled = true;
  }
}
