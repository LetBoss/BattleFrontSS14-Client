// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mindshield.FakeMindShield.SharedFakeMindShieldSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Implants;
using Content.Shared.Mindshield.Components;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Mindshield.FakeMindShield;

public sealed class SharedFakeMindShieldSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private TagSystem _tag;
  [Dependency]
  private IGameTiming _timing;
  private static readonly ProtoId<TagPrototype> FakeMindShieldImplantTag = (ProtoId<TagPrototype>) "FakeMindShieldImplant";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<FakeMindShieldComponent, FakeMindShieldToggleEvent>(new ComponentEventHandler<FakeMindShieldComponent, FakeMindShieldToggleEvent>(this.OnToggleMindshield));
    this.SubscribeLocalEvent<FakeMindShieldComponent, ChameleonControllerOutfitSelectedEvent>(new ComponentEventHandler<FakeMindShieldComponent, ChameleonControllerOutfitSelectedEvent>(this.OnChameleonControllerOutfitSelected));
  }

  private void OnToggleMindshield(
    EntityUid uid,
    FakeMindShieldComponent comp,
    FakeMindShieldToggleEvent toggleEvent)
  {
    comp.IsEnabled = !comp.IsEnabled;
    this.Dirty(uid, (IComponent) comp);
  }

  private void OnChameleonControllerOutfitSelected(
    EntityUid uid,
    FakeMindShieldComponent component,
    ChameleonControllerOutfitSelectedEvent args)
  {
    ActionsComponent comp1;
    if (component.IsEnabled == args.ChameleonOutfit.HasMindShield || !this.TryComp<ActionsComponent>(uid, out comp1))
      return;
    bool flag = false;
    foreach (EntityUid action in comp1.Actions)
    {
      ActionComponent comp2;
      if (this._tag.HasTag(action, SharedFakeMindShieldSystem.FakeMindShieldImplantTag) && this.TryComp<ActionComponent>(action, out comp2))
      {
        flag = true;
        if (!this._actions.IsCooldownActive(comp2, new TimeSpan?(this._timing.CurTime)))
        {
          component.IsEnabled = args.ChameleonOutfit.HasMindShield;
          this.Dirty(uid, (IComponent) component);
          if (!comp2.UseDelay.HasValue)
            return;
          this._actions.SetCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) action), comp2.UseDelay.Value);
          return;
        }
      }
    }
    if (flag)
      return;
    component.IsEnabled = args.ChameleonOutfit.HasMindShield;
    this.Dirty(uid, (IComponent) component);
  }
}
