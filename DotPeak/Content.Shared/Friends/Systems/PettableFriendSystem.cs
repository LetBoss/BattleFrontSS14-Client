// Decompiled with JetBrains decompiler
// Type: Content.Shared.Friends.Systems.PettableFriendSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Friends.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Systems;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Friends.Systems;

public sealed class PettableFriendSystem : EntitySystem
{
  [Dependency]
  private NpcFactionSystem _factionException;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private UseDelaySystem _useDelay;
  private Robust.Shared.GameObjects.EntityQuery<FactionExceptionComponent> _exceptionQuery;
  private Robust.Shared.GameObjects.EntityQuery<UseDelayComponent> _useDelayQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._exceptionQuery = this.GetEntityQuery<FactionExceptionComponent>();
    this._useDelayQuery = this.GetEntityQuery<UseDelayComponent>();
    this.SubscribeLocalEvent<PettableFriendComponent, UseInHandEvent>(new EntityEventRefHandler<PettableFriendComponent, UseInHandEvent>(this.OnUseInHand));
    this.SubscribeLocalEvent<PettableFriendComponent, GotRehydratedEvent>(new EntityEventRefHandler<PettableFriendComponent, GotRehydratedEvent>(this.OnRehydrated));
  }

  private void OnUseInHand(Entity<PettableFriendComponent> ent, ref UseInHandEvent args)
  {
    (EntityUid entityUid, PettableFriendComponent comp) = ent;
    EntityUid user = args.User;
    FactionExceptionComponent component1;
    if (args.Handled || !this._exceptionQuery.TryComp(entityUid, out component1))
      return;
    (EntityUid, FactionExceptionComponent) ent1 = (entityUid, component1);
    if (!this._factionException.IsIgnored((Entity<FactionExceptionComponent>) ent1, user))
    {
      this._popup.PopupClient(this.Loc.GetString((string) comp.SuccessString, ("target", (object) entityUid)), user, new EntityUid?(user));
      this._factionException.IgnoreEntity((Entity<FactionExceptionComponent>) ent1, (Entity<FactionExceptionTrackerComponent>) user);
      args.Handled = true;
    }
    else
    {
      UseDelayComponent component2;
      if (this._useDelayQuery.TryComp(entityUid, out component2) && !this._useDelay.TryResetDelay((Entity<UseDelayComponent>) (entityUid, component2), true))
        return;
      this._popup.PopupClient(this.Loc.GetString((string) comp.FailureString, ("target", (object) entityUid)), user, new EntityUid?(user));
    }
  }

  private void OnRehydrated(Entity<PettableFriendComponent> ent, ref GotRehydratedEvent args)
  {
    FactionExceptionComponent comp;
    if (!this.TryComp<FactionExceptionComponent>((EntityUid) ent, out comp))
      return;
    this._factionException.IgnoreEntities((Entity<FactionExceptionComponent>) args.Target, (IEnumerable<EntityUid>) comp.Ignored);
  }
}
