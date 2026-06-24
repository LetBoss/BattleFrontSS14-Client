// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tools.Systems.SimpleToolUsageSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Tools.Components;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Shared.Tools.Systems;

public sealed class SimpleToolUsageSystem : EntitySystem
{
  [Dependency]
  private SharedDoAfterSystem _doAfterSystem;
  [Dependency]
  private SharedToolSystem _tools;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SimpleToolUsageComponent, AfterInteractUsingEvent>(new EntityEventRefHandler<SimpleToolUsageComponent, AfterInteractUsingEvent>(this.OnAfterInteract));
    this.SubscribeLocalEvent<SimpleToolUsageComponent, GetVerbsEvent<InteractionVerb>>(new EntityEventRefHandler<SimpleToolUsageComponent, GetVerbsEvent<InteractionVerb>>(this.OnGetInteractionVerbs));
  }

  private void OnAfterInteract(
    Entity<SimpleToolUsageComponent> ent,
    ref AfterInteractUsingEvent args)
  {
    if (!args.CanReach || args.Handled || !this._tools.HasQuality(args.Used, (string) ent.Comp.Quality))
      return;
    this.AttemptToolUsage(ent, args.User, args.Used);
  }

  public void OnGetInteractionVerbs(
    Entity<SimpleToolUsageComponent> ent,
    ref GetVerbsEvent<InteractionVerb> args)
  {
    if (!ent.Comp.UsageVerb.HasValue || !args.CanAccess || !args.CanInteract)
      return;
    bool flag = !args.Using.HasValue || !this._tools.HasQuality(args.Using.Value, (string) ent.Comp.Quality);
    EntityUid? used = args.Using;
    EntityUid user = args.User;
    InteractionVerb interactionVerb1 = new InteractionVerb();
    interactionVerb1.Act = (Action) (() =>
    {
      if (!used.HasValue)
        return;
      this.AttemptToolUsage(ent, user, used.Value);
    });
    interactionVerb1.Disabled = flag;
    interactionVerb1.Message = flag ? this.Loc.GetString((string) ent.Comp.BlockedMessage, ("quality", (object) ent.Comp.Quality)) : (string) null;
    ILocalizationManager loc = this.Loc;
    LocId? usageVerb = ent.Comp.UsageVerb;
    string valueOrDefault = usageVerb.HasValue ? (string) usageVerb.GetValueOrDefault() : (string) null;
    interactionVerb1.Text = loc.GetString(valueOrDefault);
    InteractionVerb interactionVerb2 = interactionVerb1;
    args.Verbs.Add(interactionVerb2);
  }

  private void AttemptToolUsage(
    Entity<SimpleToolUsageComponent> ent,
    EntityUid user,
    EntityUid tool)
  {
    AttemptSimpleToolUseEvent args = new AttemptSimpleToolUseEvent(user);
    this.RaiseLocalEvent<AttemptSimpleToolUseEvent>((EntityUid) ent, ref args);
    if (args.Cancelled)
      return;
    this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, ent.Comp.DoAfter, (DoAfterEvent) new SimpleToolDoAfterEvent(), new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), new EntityUid?(tool))
    {
      BreakOnDamage = true,
      BreakOnDropItem = true,
      BreakOnMove = true,
      BreakOnHandChange = true,
      NeedHand = true
    });
  }
}
