// Decompiled with JetBrains decompiler
// Type: Content.Shared.MagicMirror.SharedMagicMirrorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Interaction;
using Content.Shared.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.MagicMirror;

public abstract class SharedMagicMirrorSystem : EntitySystem
{
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  protected SharedUserInterfaceSystem UISystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<MagicMirrorComponent, AfterInteractEvent>(new EntityEventRefHandler<MagicMirrorComponent, AfterInteractEvent>(this.OnMagicMirrorInteract));
    this.SubscribeLocalEvent<MagicMirrorComponent, BeforeActivatableUIOpenEvent>(new EntityEventRefHandler<MagicMirrorComponent, BeforeActivatableUIOpenEvent>(this.OnBeforeUIOpen));
    this.SubscribeLocalEvent<MagicMirrorComponent, ActivatableUIOpenAttemptEvent>(new ComponentEventRefHandler<MagicMirrorComponent, ActivatableUIOpenAttemptEvent>(this.OnAttemptOpenUI));
    this.SubscribeLocalEvent<MagicMirrorComponent, BoundUserInterfaceCheckRangeEvent>(new ComponentEventRefHandler<MagicMirrorComponent, BoundUserInterfaceCheckRangeEvent>(this.OnMirrorRangeCheck));
  }

  private void OnMagicMirrorInteract(
    Entity<MagicMirrorComponent> mirror,
    ref AfterInteractEvent args)
  {
    if (!args.CanReach)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid mirrorUid = (EntityUid) mirror;
    target = args.Target;
    EntityUid targetUid = target.Value;
    MagicMirrorComponent component = (MagicMirrorComponent) mirror;
    this.UpdateInterface(mirrorUid, targetUid, component);
    this.UISystem.TryOpenUi((Entity<UserInterfaceComponent>) mirror.Owner, (Enum) MagicMirrorUiKey.Key, args.User);
  }

  private void OnMirrorRangeCheck(
    EntityUid uid,
    MagicMirrorComponent component,
    ref BoundUserInterfaceCheckRangeEvent args)
  {
    if (args.Result == BoundUserInterfaceRangeResult.Fail)
      return;
    if (!component.Target.HasValue || !this.Exists(component.Target))
    {
      component.Target = new EntityUid?();
      args.Result = BoundUserInterfaceRangeResult.Fail;
    }
    else
    {
      if (this._interaction.InRangeUnobstructed((Entity<TransformComponent>) component.Target.Value, (Entity<TransformComponent>) uid))
        return;
      args.Result = BoundUserInterfaceRangeResult.Fail;
    }
  }

  private void OnAttemptOpenUI(
    EntityUid uid,
    MagicMirrorComponent component,
    ref ActivatableUIOpenAttemptEvent args)
  {
    if (this.HasComp<HumanoidAppearanceComponent>(component.Target ?? args.User))
      return;
    args.Cancel();
  }

  private void OnBeforeUIOpen(
    Entity<MagicMirrorComponent> ent,
    ref BeforeActivatableUIOpenEvent args)
  {
    this.UpdateInterface((EntityUid) ent, args.User, (MagicMirrorComponent) ent);
  }

  protected void UpdateInterface(
    EntityUid mirrorUid,
    EntityUid targetUid,
    MagicMirrorComponent component)
  {
    HumanoidAppearanceComponent comp;
    if (!this.TryComp<HumanoidAppearanceComponent>(targetUid, out comp))
      return;
    MagicMirrorComponent magicMirrorComponent = component;
    magicMirrorComponent.Target.GetValueOrDefault();
    if (!magicMirrorComponent.Target.HasValue)
    {
      EntityUid entityUid = targetUid;
      magicMirrorComponent.Target = new EntityUid?(entityUid);
    }
    IReadOnlyList<Marking> markings1;
    List<Marking> hair = comp.MarkingSet.TryGetCategory(MarkingCategories.Hair, out markings1) ? new List<Marking>((IEnumerable<Marking>) markings1) : new List<Marking>();
    IReadOnlyList<Marking> markings2;
    List<Marking> facialHair = comp.MarkingSet.TryGetCategory(MarkingCategories.FacialHair, out markings2) ? new List<Marking>((IEnumerable<Marking>) markings2) : new List<Marking>();
    MagicMirrorUiState state = new MagicMirrorUiState((string) comp.Species, hair, comp.MarkingSet.PointsLeft(MarkingCategories.Hair) + hair.Count, facialHair, comp.MarkingSet.PointsLeft(MarkingCategories.FacialHair) + facialHair.Count);
    component.Target = new EntityUid?(targetUid);
    this.UISystem.SetUiState((Entity<UserInterfaceComponent>) mirrorUid, (Enum) MagicMirrorUiKey.Key, (BoundUserInterfaceState) state);
    this.Dirty(mirrorUid, (IComponent) component);
  }
}
