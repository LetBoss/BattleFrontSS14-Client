// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Flag.PlantableFlagSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Sprite;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.CombatMode;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Flag;

public sealed class PlantableFlagSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedCombatModeSystem _combatMode;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private SharedRMCSpriteSystem _rmcSprite;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private GunIFFSystem _gunIFF;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PlantableFlagComponent, UseInHandEvent>(new EntityEventRefHandler<PlantableFlagComponent, UseInHandEvent>(this.OnUseInHand));
    this.SubscribeLocalEvent<PlantableFlagComponent, PlantableFlagPlantDoAfterEvent>(new EntityEventRefHandler<PlantableFlagComponent, PlantableFlagPlantDoAfterEvent>(this.OnPlantDoAfter));
    this.SubscribeLocalEvent<PlantableFlagComponent, PlantableFlagRemoveDoAfterEvent>(new EntityEventRefHandler<PlantableFlagComponent, PlantableFlagRemoveDoAfterEvent>(this.OnRemoveDoAfter));
    this.SubscribeLocalEvent<PlantableFlagComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<PlantableFlagComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGetAlternativeVerbs));
  }

  private void OnUseInHand(Entity<PlantableFlagComponent> ent, ref UseInHandEvent args)
  {
    if (!this.CanPlantFlagPopup(ent, args.User, out EntityCoordinates? _))
      return;
    args.Handled = true;
    PlantableFlagPlantDoAfterEvent @event = new PlantableFlagPlantDoAfterEvent();
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, ent.Comp.Delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent))
    {
      BreakOnMove = true,
      NeedHand = true
    }))
      return;
    this._audio.PlayPredicted(ent.Comp.RaiseStartSound, (EntityUid) ent, new EntityUid?(args.User));
  }

  private void OnPlantDoAfter(
    Entity<PlantableFlagComponent> ent,
    ref PlantableFlagPlantDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    args.Handled = true;
    EntityCoordinates? target;
    if (!this.CanPlantFlagPopup(ent, args.User, out target))
      return;
    this._transform.SetCoordinates((EntityUid) ent, target.Value);
    this._transform.SetLocalRotation((EntityUid) ent, Angle.Zero);
    this._transform.AnchorEntity((EntityUid) ent);
    this._appearance.SetData((EntityUid) ent, (Enum) PlantableFlagVisuals.Planted, (object) true);
    if (ent.Comp.DeployOffset != Vector2.Zero)
      this._rmcSprite.SetOffset((EntityUid) ent, ent.Comp.DeployOffset);
    if (this._net.IsClient)
      return;
    SoundSpecifier sound = ent.Comp.RaiseEndSound;
    if (this._combatMode.IsInCombatMode(new EntityUid?(args.User)))
    {
      sound = ent.Comp.RaisedCombatSound;
      EntProtoId<IFFFactionComponent> faction;
      if (this._gunIFF.TryGetFaction((Entity<UserIFFComponent>) (args.User, this.CompOrNull<UserIFFComponent>(args.User)), out faction))
      {
        int num = 0;
        foreach (Entity<UserIFFComponent> entity in this._entityLookup.GetEntitiesInRange<UserIFFComponent>(args.User.ToCoordinates(), (float) ent.Comp.AlliesRange))
        {
          if (this._gunIFF.IsInFaction((Entity<UserIFFComponent>) (entity.Owner, entity.Comp), faction))
            ++num;
          if (num >= ent.Comp.AlliesRequired)
          {
            sound = ent.Comp.RaisedCombatAlliesSound;
            break;
          }
        }
      }
    }
    this._audio.PlayPvs(sound, (EntityUid) ent);
  }

  private void OnRemoveDoAfter(
    Entity<PlantableFlagComponent> ent,
    ref PlantableFlagRemoveDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    args.Handled = true;
    this._transform.Unanchor((EntityUid) ent);
    this._hands.TryPickupAnyHand(args.User, (EntityUid) ent);
    this._appearance.SetData((EntityUid) ent, (Enum) PlantableFlagVisuals.Planted, (object) false);
    this._rmcSprite.SetOffset((EntityUid) ent, Vector2.Zero);
  }

  private void OnGetAlternativeVerbs(
    Entity<PlantableFlagComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    TransformComponent comp;
    if (!this.TryComp((EntityUid) ent, out comp) || !comp.Anchored)
      return;
    EntityUid user = args.User;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = "Take Down";
    alternativeVerb.Act = (Action) (() =>
    {
      if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, ent.Comp.Delay, (DoAfterEvent) new PlantableFlagRemoveDoAfterEvent(), new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent))
      {
        BreakOnMove = true,
        NeedHand = true
      }))
        return;
      this._audio.PlayPredicted(ent.Comp.LowerStartSound, (EntityUid) ent, new EntityUid?(user));
    });
    verbs.Add(alternativeVerb);
  }

  private bool CanPlantFlagPopup(
    Entity<PlantableFlagComponent> ent,
    EntityUid user,
    [NotNullWhen(true)] out EntityCoordinates? target)
  {
    target = new EntityCoordinates?();
    TransformComponent comp;
    if (!this.TryComp(user, out comp))
      return false;
    (EntityCoordinates Coords, Angle worldRot) = this._transform.GetMoverCoordinateRotation(user, comp);
    target = new EntityCoordinates?(Coords.Offset(((Angle) ref worldRot).ToWorldVec()));
    if (!this._rmcMap.IsTileBlocked(target.Value))
      return true;
    this._popup.PopupClient($"You need a clear, open area to plant the {this.Name((EntityUid) ent)}, something is blocking the way in front of you!", user, new EntityUid?(user), PopupType.MediumCaution);
    return false;
  }
}
