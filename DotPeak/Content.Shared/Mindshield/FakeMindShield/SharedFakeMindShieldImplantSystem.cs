// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mindshield.FakeMindShield.SharedFakeMindShieldImplantSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Implants;
using Content.Shared.Implants.Components;
using Content.Shared.Mindshield.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Mindshield.FakeMindShield;

public sealed class SharedFakeMindShieldImplantSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actionsSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SubdermalImplantComponent, FakeMindShieldToggleEvent>(new EntityEventRefHandler<SubdermalImplantComponent, FakeMindShieldToggleEvent>(this.OnFakeMindShieldToggle));
    this.SubscribeLocalEvent<FakeMindShieldImplantComponent, ImplantImplantedEvent>(new ComponentEventRefHandler<FakeMindShieldImplantComponent, ImplantImplantedEvent>(this.ImplantCheck));
    this.SubscribeLocalEvent<FakeMindShieldImplantComponent, EntGotRemovedFromContainerMessage>(new EntityEventRefHandler<FakeMindShieldImplantComponent, EntGotRemovedFromContainerMessage>(this.ImplantDraw));
  }

  private void OnFakeMindShieldToggle(
    Entity<SubdermalImplantComponent> entity,
    ref FakeMindShieldToggleEvent ev)
  {
    ev.Handled = true;
    EntityUid? implantedEntity = entity.Comp.ImplantedEntity;
    if (!implantedEntity.HasValue)
      return;
    EntityUid valueOrDefault = implantedEntity.GetValueOrDefault();
    FakeMindShieldComponent comp;
    if (!this.TryComp<FakeMindShieldComponent>(valueOrDefault, out comp))
      return;
    this._actionsSystem.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) ((EntityUid) ev.Action, (ActionComponent) ev.Action)), !comp.IsEnabled);
    this.RaiseLocalEvent<FakeMindShieldToggleEvent>(valueOrDefault, ev);
  }

  private void ImplantCheck(
    EntityUid uid,
    FakeMindShieldImplantComponent component,
    ref ImplantImplantedEvent ev)
  {
    if (!ev.Implanted.HasValue)
      return;
    this.EnsureComp<FakeMindShieldComponent>(ev.Implanted.Value);
  }

  private void ImplantDraw(
    Entity<FakeMindShieldImplantComponent> ent,
    ref EntGotRemovedFromContainerMessage ev)
  {
    this.RemComp<FakeMindShieldComponent>(ev.Container.Owner);
  }
}
