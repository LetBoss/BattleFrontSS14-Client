// Decompiled with JetBrains decompiler
// Type: Content.Shared.Toilet.Systems.SharedToiletSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Buckle.Components;
using Content.Shared.Interaction;
using Content.Shared.Plunger.Components;
using Content.Shared.Toilet.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Toilet.Systems;

public abstract class SharedToiletSystem : EntitySystem
{
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ToiletComponent, MapInitEvent>(new ComponentEventHandler<ToiletComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<ToiletComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<ToiletComponent, GetVerbsEvent<AlternativeVerb>>(this.OnToggleSeatVerb));
    this.SubscribeLocalEvent<ToiletComponent, ActivateInWorldEvent>(new ComponentEventHandler<ToiletComponent, ActivateInWorldEvent>(this.OnActivateInWorld));
  }

  private void OnMapInit(EntityUid uid, ToiletComponent component, MapInitEvent args)
  {
    if (this._random.Prob(0.5f))
      component.ToggleSeat = true;
    if (this._random.Prob(0.3f))
    {
      PlungerUseComponent comp;
      this.TryComp<PlungerUseComponent>(uid, out comp);
      if (comp == null)
        return;
      comp.NeedsPlunger = true;
    }
    this.UpdateAppearance(uid);
    this.Dirty(uid, (IComponent) component);
  }

  public bool CanToggle(EntityUid uid)
  {
    StrapComponent comp;
    return this.TryComp<StrapComponent>(uid, out comp) && comp.BuckledEntities.Count == 0;
  }

  private void OnToggleSeatVerb(
    EntityUid uid,
    ToiletComponent component,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanInteract || !args.CanAccess || !this.CanToggle(uid) || args.Hands == null)
      return;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Act = (Action) (() => this.ToggleToiletSeat(uid, new EntityUid?(args.User), component));
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    if (component.ToggleSeat)
    {
      alternativeVerb2.Text = this.Loc.GetString("toilet-seat-close");
      alternativeVerb2.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/close.svg.192dpi.png"));
    }
    else
    {
      alternativeVerb2.Text = this.Loc.GetString("toilet-seat-open");
      alternativeVerb2.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/open.svg.192dpi.png"));
    }
    args.Verbs.Add(alternativeVerb2);
  }

  private void OnActivateInWorld(EntityUid uid, ToiletComponent comp, ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex)
      return;
    args.Handled = true;
    this.ToggleToiletSeat(uid, new EntityUid?(args.User), comp);
  }

  public void ToggleToiletSeat(
    EntityUid uid,
    EntityUid? user = null,
    ToiletComponent? component = null,
    MetaDataComponent? meta = null)
  {
    if (!this.Resolve<ToiletComponent>(uid, ref component))
      return;
    component.ToggleSeat = !component.ToggleSeat;
    this._audio.PlayPredicted(component.SeatSound, uid, new EntityUid?(uid));
    this.UpdateAppearance(uid, component);
    this.Dirty(uid, (IComponent) component, meta);
  }

  private void UpdateAppearance(EntityUid uid, ToiletComponent? component = null)
  {
    if (!this.Resolve<ToiletComponent>(uid, ref component))
      return;
    this._appearance.SetData(uid, (Enum) ToiletVisuals.SeatVisualState, (object) (SeatVisualState) (component.ToggleSeat ? 0 : 1));
  }
}
