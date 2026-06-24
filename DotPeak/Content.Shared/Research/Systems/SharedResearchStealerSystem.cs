// Decompiled with JetBrains decompiler
// Type: Content.Shared.Research.Systems.SharedResearchStealerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Ninja.Systems;
using Content.Shared.Popups;
using Content.Shared.Research.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Research.Systems;

public abstract class SharedResearchStealerSystem : EntitySystem
{
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedNinjaGlovesSystem _gloves;
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ResearchStealerComponent, BeforeInteractHandEvent>(new ComponentEventHandler<ResearchStealerComponent, BeforeInteractHandEvent>(this.OnBeforeInteractHand));
  }

  private void OnBeforeInteractHand(
    EntityUid uid,
    ResearchStealerComponent comp,
    BeforeInteractHandEvent args)
  {
    EntityUid target1;
    TechnologyDatabaseComponent comp1;
    if (args.Handled || !this._gloves.AbilityCheck(uid, args, out target1) || !this.TryComp<TechnologyDatabaseComponent>(target1, out comp1) || this.HasComp<ResearchClientComponent>(target1))
      return;
    args.Handled = true;
    if (comp1.UnlockedTechnologies.Count == 0)
    {
      this._popup.PopupClient(this.Loc.GetString("ninja-download-fail"), uid, new EntityUid?(uid));
    }
    else
    {
      EntityManager entityManager = this.EntityManager;
      EntityUid user = uid;
      TimeSpan delay = comp.Delay;
      ResearchStealDoAfterEvent @event = new ResearchStealDoAfterEvent();
      EntityUid? nullable1 = new EntityUid?(target1);
      EntityUid? nullable2 = new EntityUid?(uid);
      EntityUid? eventTarget = new EntityUid?(uid);
      EntityUid? target2 = nullable1;
      EntityUid? used = nullable2;
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, user, delay, (DoAfterEvent) @event, eventTarget, target2, used)
      {
        BreakOnDamage = true,
        BreakOnMove = true,
        MovementThreshold = 0.5f
      });
    }
  }
}
