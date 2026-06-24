// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Doors.CMDoorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Power;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Access.Systems;
using Content.Shared.Directions;
using Content.Shared.Doors;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.GameTicking;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Prying.Components;
using Content.Shared.Tools.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Doors;

public sealed class CMDoorSystem : EntitySystem
{
  [Dependency]
  private AccessReaderSystem _accessReader;
  [Dependency]
  private SharedMarineAnnounceSystem _announce;
  [Dependency]
  private SharedDoorSystem _doors;
  [Dependency]
  private SharedGameTicker _gameTicker;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedAudioSystem _audioSystem;
  [Dependency]
  private SharedRMCPowerSystem _rmcPower;
  [Dependency]
  private IGameTiming _timing;
  private Robust.Shared.GameObjects.EntityQuery<DoorComponent> _doorQuery;
  private Robust.Shared.GameObjects.EntityQuery<CMDoubleDoorComponent> _doubleQuery;

  public override void Initialize()
  {
    this._doorQuery = this.GetEntityQuery<DoorComponent>();
    this._doubleQuery = this.GetEntityQuery<CMDoubleDoorComponent>();
    this.SubscribeLocalEvent<CMDoubleDoorComponent, DoorStateChangedEvent>(new EntityEventRefHandler<CMDoubleDoorComponent, DoorStateChangedEvent>(this.OnDoorStateChanged));
    this.SubscribeLocalEvent<RMCDoorButtonComponent, ActivateInWorldEvent>(new EntityEventRefHandler<RMCDoorButtonComponent, ActivateInWorldEvent>(this.OnButtonActivateInWorld));
    this.SubscribeLocalEvent<DoorComponent, RMCDoorPryEvent>(new EntityEventRefHandler<DoorComponent, RMCDoorPryEvent>(this.OnDoorPry));
    this.SubscribeLocalEvent<DoorComponent, RMCBeforePryEvent>(new EntityEventRefHandler<DoorComponent, RMCBeforePryEvent>(this.OnBeforePry));
    this.SubscribeLocalEvent<RMCPodDoorComponent, GetPryTimeModifierEvent>(new EntityEventRefHandler<RMCPodDoorComponent, GetPryTimeModifierEvent>(this.OnPodDoorGetPryTimeModifier));
    this.SubscribeLocalEvent<LayerChangeOnWeldComponent, DoorBoltsChangedEvent>(new EntityEventRefHandler<LayerChangeOnWeldComponent, DoorBoltsChangedEvent>(this.OnDoorBoltStateChanged));
    this.SubscribeLocalEvent<RMCOpenOnlyWhenUnanchoredComponent, BeforeDoorClosedEvent>(new EntityEventRefHandler<RMCOpenOnlyWhenUnanchoredComponent, BeforeDoorClosedEvent>(this.OnOpenOnlyWhenUnanchoredBeforeClosed));
  }

  private void OnDoorStateChanged(
    Entity<CMDoubleDoorComponent> door,
    ref DoorStateChangedEvent args)
  {
    switch (args.State)
    {
      case DoorState.Closing:
        this.Close(door);
        break;
      case DoorState.Opening:
        this.Open(door);
        break;
    }
  }

  private void OnButtonActivateInWorld(
    Entity<RMCDoorButtonComponent> button,
    ref ActivateInWorldEvent args)
  {
    EntityUid user = args.User;
    if (this.HasComp<XenoComponent>(user))
      return;
    if (!this._rmcPower.IsPowered((EntityUid) button))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-machines-unpowered"), (EntityUid) button, new EntityUid?(args.User), PopupType.SmallCaution);
    }
    else
    {
      TimeSpan? roundTimeToPress = button.Comp.MinimumRoundTimeToPress;
      if (roundTimeToPress.HasValue)
      {
        TimeSpan valueOrDefault = roundTimeToPress.GetValueOrDefault();
        if (this._gameTicker.RoundDuration() <= valueOrDefault)
        {
          int num = (int) (valueOrDefault.TotalMinutes - this._gameTicker.RoundDuration().TotalMinutes);
          this._popup.PopupClient(this.Loc.GetString((string) button.Comp.NoTimeMessage, ("minutes", (object) num)), user, new EntityUid?(user), PopupType.SmallCaution);
          return;
        }
      }
      if (button.Comp.Used && button.Comp.UseOnlyOnce)
        this._popup.PopupClient(this.Loc.GetString((string) button.Comp.AlreadyUsedMessage), (EntityUid) button, new EntityUid?(user), PopupType.SmallCaution);
      else if (!this._accessReader.IsAllowed(user, (EntityUid) button))
      {
        this._popup.PopupClient(this.Loc.GetString("cm-vending-machine-access-denied"), (EntityUid) button, new EntityUid?(user), PopupType.SmallCaution);
        this.DoPodDoorButtonAnimation((EntityUid) button, button.Comp.DeniedState);
      }
      else
      {
        TimeSpan curTime = this._timing.CurTime;
        if (curTime < button.Comp.LastUse + button.Comp.Cooldown)
          return;
        button.Comp.LastUse = curTime;
        button.Comp.Used = true;
        this.Dirty<RMCDoorButtonComponent>(button);
        string str1 = button.Comp.Id ?? this.Name((EntityUid) button);
        TransformComponent transformComponent = this.Transform((EntityUid) button);
        Robust.Shared.GameObjects.EntityQueryEnumerator<RMCPodDoorComponent, DoorComponent, TransformComponent, MetaDataComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCPodDoorComponent, DoorComponent, TransformComponent, MetaDataComponent>();
        EntityUid uid;
        RMCPodDoorComponent comp1;
        DoorComponent comp2;
        TransformComponent comp3;
        MetaDataComponent comp4;
        while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2, out comp3, out comp4))
        {
          if (!this.TerminatingOrDeleted(uid) && !(transformComponent.MapID != comp3.MapID))
          {
            string str2 = comp1.Id ?? comp4.EntityName;
            if (!(str1 != str2))
            {
              if (comp2.State == DoorState.Open)
                this._doors.StartClosing(uid);
              else
                this._doors.TryOpen(uid, comp2);
            }
          }
        }
        this._popup.PopupPredicted(this.Loc.GetString("rmc-door-button-pressed-self", (nameof (button), (object) button)), this.Loc.GetString("rmc-door-button-pressed-others", ("user", (object) user), (nameof (button), (object) button)), user, new EntityUid?(user));
        this.DoPodDoorButtonAnimation((EntityUid) button, button.Comp.OnState);
        if (!button.Comp.MarineAnnouncement.HasValue)
          return;
        ILocalizationManager loc = this.Loc;
        LocId? marineAnnouncement = button.Comp.MarineAnnouncement;
        string valueOrDefault = marineAnnouncement.HasValue ? (string) marineAnnouncement.GetValueOrDefault() : (string) null;
        this._announce.AnnounceHighCommand(loc.GetString(valueOrDefault), this.Loc.GetString((string) button.Comp.MarineAnnouncementAuthor));
      }
    }
  }

  public void DoPodDoorButtonAnimation(EntityUid button, string animState)
  {
    if (this._net.IsClient)
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new RMCPodDoorButtonPressedEvent(this.GetNetEntity(button), animState), Filter.PvsExcept(button));
  }

  private void OnBeforePry(Entity<DoorComponent> ent, ref RMCBeforePryEvent args)
  {
    DoorComponent comp;
    if (this.TryComp<DoorComponent>((EntityUid) ent, out comp) && comp.State != DoorState.Closed && (this.HasComp<RMCPodDoorComponent>((EntityUid) ent) || this.HasComp<XenoComponent>(args.User)))
      args.Cancelled = true;
    if (this.HasComp<XenoComponent>(args.User) && this.HasComp<AirlockComponent>((EntityUid) ent) || !this._rmcPower.IsPowered((EntityUid) ent))
      return;
    args.Cancelled = true;
  }

  private void OnDoorPry(Entity<DoorComponent> ent, ref RMCDoorPryEvent args)
  {
    if (args.Cancelled)
      this._audioSystem.Stop(ent.Comp.SoundEntity);
    if (!this.HasComp<XenoComponent>(args.User) || !this._net.IsServer || args.Cancelled)
      return;
    if (this.HasComp<RMCPodDoorComponent>(ent.Owner))
    {
      DoorComponent comp = ent.Comp;
      (EntityUid, AudioComponent)? nullable1 = this._audioSystem.PlayPredicted(ent.Comp.XenoPodDoorPrySound, ent.Owner, new EntityUid?(ent.Owner));
      ref (EntityUid, AudioComponent)? local = ref nullable1;
      EntityUid? nullable2 = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Item1) : new EntityUid?();
      comp.SoundEntity = nullable2;
    }
    else
    {
      DoorComponent comp = ent.Comp;
      (EntityUid, AudioComponent)? nullable3 = this._audioSystem.PlayPredicted(ent.Comp.XenoPrySound, ent.Owner, new EntityUid?(ent.Owner));
      ref (EntityUid, AudioComponent)? local = ref nullable3;
      EntityUid? nullable4 = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Item1) : new EntityUid?();
      comp.SoundEntity = nullable4;
    }
  }

  private void OnPodDoorGetPryTimeModifier(
    Entity<RMCPodDoorComponent> ent,
    ref GetPryTimeModifierEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.User))
      return;
    args.PryTimeModifier *= ent.Comp.XenoPodlockPryMultiplier;
  }

  private void OnDoorBoltStateChanged(
    Entity<LayerChangeOnWeldComponent> ent,
    ref DoorBoltsChangedEvent args)
  {
    FixturesComponent comp1;
    DoorComponent comp2;
    if (!this.TryComp<FixturesComponent>((EntityUid) ent, out comp1) || !this.TryComp<DoorComponent>((EntityUid) ent, out comp2))
      return;
    foreach (KeyValuePair<string, Fixture> fixture in comp1.Fixtures)
    {
      if (args.BoltsDown)
      {
        if ((CollisionGroup) fixture.Value.CollisionLayer == ent.Comp.UnWeldedLayer && comp2.State == DoorState.Closed)
          this._physics.SetCollisionLayer((EntityUid) ent, fixture.Key, fixture.Value, (int) ent.Comp.WeldedLayer);
      }
      else if ((CollisionGroup) fixture.Value.CollisionLayer == ent.Comp.WeldedLayer)
        this._physics.SetCollisionLayer((EntityUid) ent, fixture.Key, fixture.Value, (int) ent.Comp.UnWeldedLayer);
    }
  }

  private void OnOpenOnlyWhenUnanchoredBeforeClosed(
    Entity<RMCOpenOnlyWhenUnanchoredComponent> ent,
    ref BeforeDoorClosedEvent args)
  {
    TransformComponent comp;
    if (this.TryComp((EntityUid) ent, out comp) && comp.Anchored)
      return;
    args.Cancel();
  }

  private AnchoredEntitiesEnumerator? GetAdjacentEnumerator(Entity<CMDoubleDoorComponent> ent)
  {
    TransformComponent comp1;
    MapGridComponent comp2;
    if (!this.TryComp((EntityUid) ent, out comp1) || !this.TryComp<MapGridComponent>(comp1.GridUid, out comp2))
      return new AnchoredEntitiesEnumerator?();
    EntityCoordinates coordinates1 = comp1.Coordinates;
    Angle localRotation = comp1.LocalRotation;
    Direction cardinalDir = ((Angle) ref localRotation).GetCardinalDir();
    EntityCoordinates coordinates2 = coordinates1.Offset(cardinalDir);
    Vector2i tile = this._map.LocalToTile(comp1.GridUid.Value, comp2, coordinates2);
    return new AnchoredEntitiesEnumerator?(this._map.GetAnchoredEntitiesEnumerator(comp1.GridUid.Value, comp2, tile));
  }

  private bool AreFacing(EntityUid one, EntityUid two)
  {
    TransformComponent comp1;
    TransformComponent comp2;
    if (!this.TryComp(one, out comp1) || !this.TryComp(two, out comp2))
      return false;
    Angle localRotation1 = comp1.LocalRotation;
    Direction opposite = DirectionExtensions.GetOpposite(((Angle) ref localRotation1).GetCardinalDir());
    Angle localRotation2 = comp2.LocalRotation;
    Direction cardinalDir = ((Angle) ref localRotation2).GetCardinalDir();
    return opposite == cardinalDir;
  }

  private void Open(Entity<CMDoubleDoorComponent> ent)
  {
    AnchoredEntitiesEnumerator? adjacentEnumerator = this.GetAdjacentEnumerator(ent);
    if (!adjacentEnumerator.HasValue)
      return;
    AnchoredEntitiesEnumerator valueOrDefault = adjacentEnumerator.GetValueOrDefault();
    TimeSpan curTime = this._timing.CurTime;
    ent.Comp.LastOpeningAt = curTime;
    this.Dirty<CMDoubleDoorComponent>(ent);
    EntityUid? uid;
    while (valueOrDefault.MoveNext(out uid))
    {
      CMDoubleDoorComponent component1;
      DoorComponent component2;
      if (this._doubleQuery.TryGetComponent(uid, out component1) && component1.LastOpeningAt != curTime && this.AreFacing((EntityUid) ent, uid.Value) && this._doorQuery.TryGetComponent(uid, out component2) && component2.State != DoorState.Opening)
      {
        component1.LastOpeningAt = curTime;
        this.Dirty(uid.Value, (IComponent) component1);
        SoundSpecifier openSound = component2.OpenSound;
        component2.OpenSound = (SoundSpecifier) null;
        component2.Partial = false;
        this._doors.StartOpening(uid.Value, component2);
        component2.OpenSound = openSound;
      }
    }
  }

  private void Close(Entity<CMDoubleDoorComponent> ent)
  {
    AnchoredEntitiesEnumerator? adjacentEnumerator = this.GetAdjacentEnumerator(ent);
    if (!adjacentEnumerator.HasValue)
      return;
    AnchoredEntitiesEnumerator valueOrDefault = adjacentEnumerator.GetValueOrDefault();
    TimeSpan curTime = this._timing.CurTime;
    ent.Comp.LastClosingAt = curTime;
    this.Dirty<CMDoubleDoorComponent>(ent);
    EntityUid? uid;
    while (valueOrDefault.MoveNext(out uid))
    {
      CMDoubleDoorComponent component1;
      DoorComponent component2;
      if (this._doubleQuery.TryGetComponent(uid, out component1) && component1.LastClosingAt != curTime && this.AreFacing((EntityUid) ent, uid.Value) && this._doorQuery.TryGetComponent(uid, out component2) && component2.State != DoorState.Closing)
      {
        component1.LastClosingAt = curTime;
        this.Dirty(uid.Value, (IComponent) component1);
        SoundSpecifier closeSound = component2.CloseSound;
        component2.CloseSound = (SoundSpecifier) null;
        component2.Partial = false;
        this._doors.StartClosing(uid.Value, component2);
        component2.CloseSound = closeSound;
      }
    }
  }
}
