// Decompiled with JetBrains decompiler
// Type: Content.Shared.Interaction.InteractionPopupSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Bed.Sleep;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Interaction;

public sealed class InteractionPopupSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private MobStateSystem _mobStateSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private INetManager _netMan;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<InteractionPopupComponent, InteractHandEvent>(new ComponentEventHandler<InteractionPopupComponent, InteractHandEvent>(this.OnInteractHand));
    this.SubscribeLocalEvent<InteractionPopupComponent, ActivateInWorldEvent>(new ComponentEventHandler<InteractionPopupComponent, ActivateInWorldEvent>(this.OnActivateInWorld));
  }

  private void OnActivateInWorld(
    EntityUid uid,
    InteractionPopupComponent component,
    ActivateInWorldEvent args)
  {
    if (!args.Complex || !component.OnActivate)
      return;
    this.SharedInteract(uid, component, (HandledEntityEventArgs) args, args.Target, args.User);
  }

  private void OnInteractHand(
    EntityUid uid,
    InteractionPopupComponent component,
    InteractHandEvent args)
  {
    this.SharedInteract(uid, component, (HandledEntityEventArgs) args, args.Target, args.User);
  }

  private void SharedInteract(
    EntityUid uid,
    InteractionPopupComponent component,
    HandledEntityEventArgs args,
    EntityUid target,
    EntityUid user)
  {
    MobStateComponent comp;
    if (args.Handled || user == target || this.HasComp<SleepingComponent>(uid) || this.TryComp<MobStateComponent>(uid, out comp) && !this._mobStateSystem.IsAlive(uid, comp))
      return;
    args.Handled = true;
    TimeSpan curTime = this._gameTiming.CurTime;
    if (curTime < component.LastInteractTime + component.InteractDelay)
      return;
    component.LastInteractTime = curTime;
    string message = "";
    SoundSpecifier sound = (SoundSpecifier) null;
    float successChance = component.SuccessChance;
    bool flag = ((double) successChance == 0.0 || (double) successChance == 1.0) && !component.InteractSuccessSpawn.HasValue && !component.InteractFailureSpawn.HasValue;
    if (this._netMan.IsClient && !flag)
      return;
    if (this._random.Prob(component.SuccessChance))
    {
      if (component.InteractSuccessString != null)
        message = this.Loc.GetString(component.InteractSuccessString, (nameof (target), (object) Identity.Name(uid, (IEntityManager) this.EntityManager, new EntityUid?(user))));
      if (component.InteractSuccessSound != null)
        sound = component.InteractSuccessSound;
      if (component.InteractSuccessSpawn.HasValue)
      {
        EntProtoId? interactSuccessSpawn = component.InteractSuccessSpawn;
        this.Spawn(interactSuccessSpawn.HasValue ? (string) interactSuccessSpawn.GetValueOrDefault() : (string) null, this._transform.GetMapCoordinates(uid), rotation: new Angle());
      }
      InteractionSuccessEvent args1 = new InteractionSuccessEvent(user);
      this.RaiseLocalEvent<InteractionSuccessEvent>(target, ref args1);
    }
    else
    {
      if (component.InteractFailureString != null)
        message = this.Loc.GetString(component.InteractFailureString, (nameof (target), (object) Identity.Name(uid, (IEntityManager) this.EntityManager, new EntityUid?(user))));
      if (component.InteractFailureSound != null)
        sound = component.InteractFailureSound;
      if (component.InteractFailureSpawn.HasValue)
      {
        EntProtoId? interactFailureSpawn = component.InteractFailureSpawn;
        this.Spawn(interactFailureSpawn.HasValue ? (string) interactFailureSpawn.GetValueOrDefault() : (string) null, this._transform.GetMapCoordinates(uid), rotation: new Angle());
      }
      InteractionFailureEvent args2 = new InteractionFailureEvent(user);
      this.RaiseLocalEvent<InteractionFailureEvent>(target, ref args2);
    }
    if (!string.IsNullOrEmpty(component.MessagePerceivedByOthers))
    {
      foreach (ICommonSession recipient in Filter.PvsExcept(user, entityManager: (IEntityManager) this.EntityManager).Recipients)
      {
        EntityUid? attachedEntity = recipient.AttachedEntity;
        if (attachedEntity.HasValue)
        {
          EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
          this._popupSystem.PopupEntity(this.Loc.GetString(component.MessagePerceivedByOthers, (nameof (user), (object) Identity.Entity(user, (IEntityManager) this.EntityManager, new EntityUid?(valueOrDefault))), (nameof (target), (object) Identity.Name(uid, (IEntityManager) this.EntityManager, new EntityUid?(valueOrDefault)))), uid, valueOrDefault);
        }
      }
    }
    if (!flag)
    {
      this._popupSystem.PopupEntity(message, uid, user);
      if (component.SoundPerceivedByOthers)
        this._audio.PlayPvs(sound, target);
      else
        this._audio.PlayEntity(sound, Filter.Entities(user, target), target, false);
    }
    else
    {
      this._popupSystem.PopupClient(message, uid, new EntityUid?(user));
      if (sound == null)
        return;
      if (component.SoundPerceivedByOthers)
        this._audio.PlayPredicted(sound, target, new EntityUid?(user));
      else if (this._netMan.IsClient)
      {
        if (!this._gameTiming.IsFirstTimePredicted)
          return;
        this._audio.PlayEntity(sound, Filter.Local(), target, true);
      }
      else
        this._audio.PlayEntity(sound, Filter.Empty().FromEntities(target), target, false);
    }
  }

  public void SetInteractSuccessString(Entity<InteractionPopupComponent> ent, string str)
  {
    ent.Comp.InteractSuccessString = str;
  }

  public void SetInteractFailureString(Entity<InteractionPopupComponent> ent, string str)
  {
    ent.Comp.InteractFailureString = str;
  }
}
