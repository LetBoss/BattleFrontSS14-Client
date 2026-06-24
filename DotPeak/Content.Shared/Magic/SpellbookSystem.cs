// Decompiled with JetBrains decompiler
// Type: Content.Shared.Magic.SpellbookSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Interaction.Events;
using Content.Shared.Magic.Components;
using Content.Shared.Mind;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Magic;

public sealed class SpellbookSystem : EntitySystem
{
  [Dependency]
  private SharedChargesSystem _sharedCharges;
  [Dependency]
  private SharedMindSystem _mind;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private ActionContainerSystem _actionContainer;
  [Dependency]
  private INetManager _netManager;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<SpellbookComponent, MapInitEvent>(new EntityEventRefHandler<SpellbookComponent, MapInitEvent>(this.OnInit), new Type[1]
    {
      typeof (SharedMagicSystem)
    });
    this.SubscribeLocalEvent<SpellbookComponent, UseInHandEvent>(new EntityEventRefHandler<SpellbookComponent, UseInHandEvent>(this.OnUse));
    this.SubscribeLocalEvent<SpellbookComponent, SpellbookDoAfterEvent>(new EntityEventRefHandler<SpellbookComponent, SpellbookDoAfterEvent>(this.OnDoAfter<SpellbookDoAfterEvent>));
  }

  private void OnInit(Entity<SpellbookComponent> ent, ref MapInitEvent args)
  {
    foreach ((EntProtoId entProtoId, int? nullable1) in ent.Comp.SpellActions)
    {
      EntityUid? nullable2 = this._actionContainer.AddAction((EntityUid) ent, (string) entProtoId);
      if (nullable2.HasValue)
      {
        if (nullable1.HasValue)
        {
          int valueOrDefault = nullable1.GetValueOrDefault();
          this._sharedCharges.SetCharges((Entity<LimitedChargesComponent>) nullable2.Value, valueOrDefault);
        }
        ent.Comp.Spells.Add(nullable2.Value);
      }
    }
  }

  private void OnUse(Entity<SpellbookComponent> ent, ref UseInHandEvent args)
  {
    if (args.Handled)
      return;
    this.AttemptLearn(ent, args);
    args.Handled = true;
  }

  private void OnDoAfter<T>(Entity<SpellbookComponent> ent, ref T args) where T : DoAfterEvent
  {
    if (args.Handled || args.Cancelled)
      return;
    args.Handled = true;
    if (!ent.Comp.LearnPermanently)
    {
      this._actions.GrantActions((Entity<ActionsComponent>) args.Args.User, (IEnumerable<EntityUid>) ent.Comp.Spells, (Entity<ActionsContainerComponent>) ent.Owner);
    }
    else
    {
      EntityUid mindId;
      if (this._mind.TryGetMind(args.Args.User, out mindId, out MindComponent _))
      {
        ActionsContainerComponent newContainer = this.EnsureComp<ActionsContainerComponent>(mindId);
        if (this._netManager.IsServer)
          this._actionContainer.TransferAllActionsWithNewAttached((EntityUid) ent, mindId, args.Args.User, newContainer: newContainer);
      }
      else
      {
        foreach ((EntProtoId entProtoId, int? nullable) in ent.Comp.SpellActions)
        {
          EntityUid? actionId = new EntityUid?();
          if (this._actions.AddAction(args.Args.User, ref actionId, (string) entProtoId) && nullable.HasValue)
          {
            int valueOrDefault = nullable.GetValueOrDefault();
            this._sharedCharges.SetCharges((Entity<LimitedChargesComponent>) actionId.Value, valueOrDefault);
          }
        }
      }
      ent.Comp.SpellActions.Clear();
    }
  }

  private void AttemptLearn(Entity<SpellbookComponent> ent, UseInHandEvent args)
  {
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, ent.Comp.LearnTime, (DoAfterEvent) new SpellbookDoAfterEvent(), new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent))
    {
      BreakOnMove = true,
      BreakOnDamage = true,
      NeedHand = true
    });
  }
}
