// Decompiled with JetBrains decompiler
// Type: Content.Shared.Plunger.Systems.PlungerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Plunger.Components;
using Content.Shared.Popups;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

#nullable enable
namespace Content.Shared.Plunger.Systems;

public sealed class PlungerSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _proto;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PlungerComponent, AfterInteractEvent>(new ComponentEventHandler<PlungerComponent, AfterInteractEvent>(this.OnInteract));
    this.SubscribeLocalEvent<PlungerComponent, PlungerDoAfterEvent>(new ComponentEventHandler<PlungerComponent, PlungerDoAfterEvent>(this.OnDoAfter));
  }

  private void OnInteract(EntityUid uid, PlungerComponent component, AfterInteractEvent args)
  {
    if (args.Handled || !args.CanReach)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    PlungerUseComponent comp;
    if (!valueOrDefault.Valid || !this.TryComp<PlungerUseComponent>(args.Target, out comp) || comp.NeedsPlunger)
      return;
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, component.PlungeDuration, (DoAfterEvent) new PlungerDoAfterEvent(), new EntityUid?(uid), new EntityUid?(valueOrDefault), new EntityUid?(uid))
    {
      BreakOnMove = true,
      BreakOnDamage = true,
      MovementThreshold = 1f
    });
    args.Handled = true;
  }

  private void OnDoAfter(EntityUid uid, PlungerComponent component, DoAfterEvent args)
  {
    if (args.Cancelled || args.Handled || !args.Args.Target.HasValue)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    PlungerUseComponent comp;
    if (!valueOrDefault.Valid || !this.TryComp<PlungerUseComponent>(valueOrDefault, out comp))
      return;
    this._popup.PopupClient(this.Loc.GetString("plunger-unblock", ("target", (object) valueOrDefault)), args.User, new EntityUid?(args.User), PopupType.Medium);
    comp.Plunged = true;
    string prototype = this._proto.Index<WeightedRandomEntityPrototype>(comp.PlungerLoot).Pick(this._random);
    this._audio.PlayPredicted(comp.Sound, uid, new EntityUid?(uid));
    this.Spawn(prototype, this.Transform(valueOrDefault).Coordinates);
    this.RemComp<PlungerUseComponent>(valueOrDefault);
    this.Dirty(valueOrDefault, (IComponent) comp);
    args.Handled = true;
  }
}
