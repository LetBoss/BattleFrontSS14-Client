// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Common.UniqueActionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Input;
using Content.Shared.ActionBlocker;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Common;

public sealed class UniqueActionSystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private SharedHandsSystem _hands;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<UniqueActionComponent, GetVerbsEvent<InteractionVerb>>(new EntityEventRefHandler<UniqueActionComponent, GetVerbsEvent<InteractionVerb>>(this.OnGetVerbs));
    CommandBinds.Builder.Bind(CMKeyFunctions.CMUniqueAction, InputCmdHandler.FromDelegate((StateInputCmdDelegate) (session =>
    {
      EntityUid? attachedEntity = (EntityUid?) session?.AttachedEntity;
      if (!attachedEntity.HasValue)
        return;
      this.TryUniqueAction(attachedEntity.GetValueOrDefault());
    }), handle: false)).Register<UniqueActionSystem>();
  }

  public override void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<UniqueActionSystem>();
  }

  private void OnGetVerbs(
    Entity<UniqueActionComponent> ent,
    ref GetVerbsEvent<InteractionVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || !this._actionBlocker.CanInteract(args.User, new EntityUid?(args.Target)))
      return;
    EntityUid user = args.User;
    SortedSet<InteractionVerb> verbs = args.Verbs;
    InteractionVerb interactionVerb = new InteractionVerb();
    interactionVerb.Act = (Action) (() => this.TryUniqueAction(user, ent.Owner));
    interactionVerb.Text = "Unique action";
    verbs.Add(interactionVerb);
  }

  private void TryUniqueAction(EntityUid userUid)
  {
    EntityUid? nullable;
    if (!this._hands.TryGetActiveItem((Entity<HandsComponent>) userUid, out nullable))
      return;
    this.TryUniqueAction(userUid, nullable.Value);
  }

  private void TryUniqueAction(EntityUid userUid, EntityUid targetUid)
  {
    if (!this._actionBlocker.CanInteract(userUid, new EntityUid?(targetUid)))
      return;
    this.RaiseLocalEvent<UniqueActionEvent>(targetUid, new UniqueActionEvent(userUid));
  }
}
