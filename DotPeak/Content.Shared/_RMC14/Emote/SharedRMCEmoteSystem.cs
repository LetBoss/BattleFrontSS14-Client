// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Emote.SharedRMCEmoteSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Coordinates;
using Content.Shared.Interaction;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Emote;

public abstract class SharedRMCEmoteSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private RotateToFaceSystem _rotate;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedRMCMeleeWeaponSystem _melee;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedTransformSystem _transform;
  private TimeSpan _emoteCooldown;
  private readonly float _interactRange = 1f;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCHandEmotesComponent, InteractHandEvent>(new EntityEventRefHandler<RMCHandEmotesComponent, InteractHandEvent>(this.OnInteractHand));
    this.SubscribeLocalEvent<RMCHandEmotesComponent, GetVerbsEvent<InteractionVerb>>(new EntityEventRefHandler<RMCHandEmotesComponent, GetVerbsEvent<InteractionVerb>>(this.OnGetInteractionVerbs));
    this.SubscribeLocalEvent<RMCHandEmotesComponent, MoveInputEvent>(new EntityEventRefHandler<RMCHandEmotesComponent, MoveInputEvent>(this.OnMove));
    this.Subs.CVar<float>(this._config, RMCCVars.RMCEmoteCooldownSeconds, (Action<float>) (v => this._emoteCooldown = TimeSpan.FromSeconds((double) v)), true);
  }

  public virtual void TryEmoteWithChat(
    EntityUid source,
    ProtoId<EmotePrototype> emote,
    bool hideLog = false,
    string? nameOverride = null,
    bool ignoreActionBlocker = false,
    bool forceEmote = false,
    TimeSpan? cooldown = null)
  {
  }

  public bool TryEmote(Entity<EmoteCooldownComponent?> cooldown)
  {
    if (!this.Resolve<EmoteCooldownComponent>((EntityUid) cooldown, ref cooldown.Comp, false))
      return true;
    TimeSpan curTime = this._timing.CurTime;
    if (curTime < cooldown.Comp.NextEmote)
      return false;
    cooldown.Comp.NextEmote = curTime + this._emoteCooldown;
    this.Dirty<EmoteCooldownComponent>(cooldown);
    return true;
  }

  public void ResetCooldown(Entity<EmoteCooldownComponent?> cooldown)
  {
    if (!this.Resolve<EmoteCooldownComponent>((EntityUid) cooldown, ref cooldown.Comp, false))
      return;
    cooldown.Comp.NextEmote = this._timing.CurTime + this._emoteCooldown;
    this.Dirty<EmoteCooldownComponent>(cooldown);
  }

  private void OnInteractHand(Entity<RMCHandEmotesComponent> ent, ref InteractHandEvent args)
  {
    if (args.Handled)
      return;
    EntityUid user = args.User;
    RMCHandEmotesComponent comp;
    if (user == args.Target || !this.TryComp<RMCHandEmotesComponent>(user, out comp) || !ent.Comp.Active || comp.Active)
      return;
    EntityUid entityUid = user;
    EntityUid? target = ent.Comp.Target;
    if ((target.HasValue ? (entityUid != target.GetValueOrDefault() ? 1 : 0) : 1) != 0 || ent.Comp.State == RMCHandsEmoteState.Tailswipe && !this.HasComp<XenoComponent>(user))
      return;
    if (!this._interaction.InRangeUnobstructed((Entity<TransformComponent>) user, (Entity<TransformComponent>) args.Target, this._interactRange))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-hands-emotes-get-closer"), user, new EntityUid?(user));
    }
    else
    {
      args.Handled = true;
      this.PerformEmote(ent, (Entity<RMCHandEmotesComponent>) (user, comp));
    }
  }

  private void OnGetInteractionVerbs(
    Entity<RMCHandEmotesComponent> ent,
    ref GetVerbsEvent<InteractionVerb> args)
  {
    RMCHandEmotesComponent selfComp;
    if (!args.CanAccess || !args.CanInteract || args.Hands == null || !this.TryComp<RMCHandEmotesComponent>(args.User, out selfComp) || ent.Comp.Active || selfComp.Active || ent.Owner == args.User)
      return;
    EntityUid user = args.User;
    if (this.HasComp<XenoComponent>(user) && this.HasComp<XenoComponent>(ent.Owner))
    {
      SortedSet<InteractionVerb> verbs = args.Verbs;
      InteractionVerb interactionVerb = new InteractionVerb();
      interactionVerb.Act = (Action) (() => this.AttemptEmote((Entity<RMCHandEmotesComponent>) (user, selfComp), ent, RMCHandsEmoteState.Tailswipe));
      interactionVerb.Text = this.Loc.GetString("rmc-hands-emotes-tailswipe-perform");
      interactionVerb.Priority = -27;
      interactionVerb.Icon = (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("_RMC14/Effects/emotes.rsi"), "emote_tailswipe");
      verbs.Add(interactionVerb);
    }
    else
    {
      if (this.HasComp<XenoComponent>(user) || this.HasComp<XenoComponent>(ent.Owner))
        return;
      SortedSet<InteractionVerb> verbs1 = args.Verbs;
      InteractionVerb interactionVerb1 = new InteractionVerb();
      interactionVerb1.Act = (Action) (() => this.AttemptEmote((Entity<RMCHandEmotesComponent>) (user, selfComp), ent, RMCHandsEmoteState.Fistbump));
      interactionVerb1.Text = this.Loc.GetString("rmc-hands-emotes-fistbump-perform");
      interactionVerb1.Priority = -25;
      interactionVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("_RMC14/Effects/emotes.rsi"), "emote_fistbump");
      verbs1.Add(interactionVerb1);
      SortedSet<InteractionVerb> verbs2 = args.Verbs;
      InteractionVerb interactionVerb2 = new InteractionVerb();
      interactionVerb2.Act = (Action) (() => this.AttemptEmote((Entity<RMCHandEmotesComponent>) (user, selfComp), ent, RMCHandsEmoteState.Highfive));
      interactionVerb2.Text = this.Loc.GetString("rmc-hands-emotes-highfive-perform");
      interactionVerb2.Priority = -26;
      interactionVerb2.Icon = (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("_RMC14/Effects/emotes.rsi"), "emote_highfive");
      verbs2.Add(interactionVerb2);
      SortedSet<InteractionVerb> verbs3 = args.Verbs;
      InteractionVerb interactionVerb3 = new InteractionVerb();
      interactionVerb3.Act = (Action) (() => this.AttemptEmote((Entity<RMCHandEmotesComponent>) (user, selfComp), ent, RMCHandsEmoteState.Hug));
      interactionVerb3.Text = this.Loc.GetString("rmc-hands-emotes-hug-perform");
      interactionVerb3.Priority = -28;
      interactionVerb3.Icon = (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("_RMC14/Effects/emotes.rsi"), "emote_hug");
      verbs3.Add(interactionVerb3);
    }
  }

  private void OnMove(Entity<RMCHandEmotesComponent> ent, ref MoveInputEvent args)
  {
    if (!args.HasDirectionalMovement)
      return;
    this.CancelHandEmotes(ent);
  }

  public void AttemptEmote(
    Entity<RMCHandEmotesComponent> ent,
    Entity<RMCHandEmotesComponent> target,
    RMCHandsEmoteState state)
  {
    EntProtoId entProtoId;
    switch (state)
    {
      case RMCHandsEmoteState.Fistbump:
        entProtoId = ent.Comp.FistBumpEffect;
        break;
      case RMCHandsEmoteState.Highfive:
        entProtoId = ent.Comp.HighFiveEffect;
        break;
      case RMCHandsEmoteState.Tailswipe:
        entProtoId = ent.Comp.TailSwipeEffect;
        break;
      case RMCHandsEmoteState.Hug:
        entProtoId = ent.Comp.HugEffect;
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    EntProtoId prototype = entProtoId;
    string str1;
    switch (state)
    {
      case RMCHandsEmoteState.Fistbump:
        str1 = this.Loc.GetString("rmc-hands-emotes-fistbump-attempt", (nameof (ent), (object) ent), (nameof (target), (object) target));
        break;
      case RMCHandsEmoteState.Highfive:
        str1 = this.Loc.GetString("rmc-hands-emotes-highfive-attempt", (nameof (ent), (object) ent), (nameof (target), (object) target));
        break;
      case RMCHandsEmoteState.Tailswipe:
        str1 = this.Loc.GetString("rmc-hands-emotes-tailswipe-attempt", (nameof (ent), (object) ent), (nameof (target), (object) target));
        break;
      case RMCHandsEmoteState.Hug:
        str1 = this.Loc.GetString("rmc-hands-emotes-hug-attempt", (nameof (ent), (object) ent), (nameof (target), (object) target));
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    string othersMessage = str1;
    string str2;
    switch (state)
    {
      case RMCHandsEmoteState.Fistbump:
        str2 = this.Loc.GetString("rmc-hands-emotes-fistbump-attempt-self", (nameof (ent), (object) ent), (nameof (target), (object) target));
        break;
      case RMCHandsEmoteState.Highfive:
        str2 = this.Loc.GetString("rmc-hands-emotes-highfive-attempt-self", (nameof (ent), (object) ent), (nameof (target), (object) target));
        break;
      case RMCHandsEmoteState.Tailswipe:
        str2 = this.Loc.GetString("rmc-hands-emotes-tailswipe-attempt-self", (nameof (ent), (object) ent), (nameof (target), (object) target));
        break;
      case RMCHandsEmoteState.Hug:
        str2 = this.Loc.GetString("rmc-hands-emotes-hug-attempt-self", (nameof (ent), (object) ent), (nameof (target), (object) target));
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    string recipientMessage = str2;
    ent.Comp.Active = true;
    ent.Comp.Target = new EntityUid?(target.Owner);
    ent.Comp.LeaveHangingAt = this._timing.CurTime + ent.Comp.LeftHangingDelay;
    ent.Comp.State = state;
    if (this._net.IsServer)
      ent.Comp.SpawnedEffect = new EntityUid?(this.SpawnAttachedTo((string) prototype, ent.Owner.ToCoordinates(), rotation: new Angle()));
    this._popup.PopupPredicted(recipientMessage, othersMessage, ent.Owner, new EntityUid?(ent.Owner), PopupType.Medium);
    this.Dirty<RMCHandEmotesComponent>(ent);
  }

  public void CancelHandEmotes(Entity<RMCHandEmotesComponent> ent)
  {
    ent.Comp.Target = new EntityUid?();
    ent.Comp.Active = false;
    if (this._net.IsServer && ent.Comp.SpawnedEffect.HasValue)
      this.QueueDel(ent.Comp.SpawnedEffect);
    ent.Comp.SpawnedEffect = new EntityUid?();
    this.Dirty<RMCHandEmotesComponent>(ent);
  }

  public void PerformEmote(
    Entity<RMCHandEmotesComponent> ent,
    Entity<RMCHandEmotesComponent> target)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    EntityUid owner1 = ent.Owner;
    EntityUid owner2 = target.Owner;
    RMCHandsEmoteState state = ent.Comp.State;
    SoundSpecifier soundSpecifier;
    switch (state)
    {
      case RMCHandsEmoteState.Fistbump:
        soundSpecifier = ent.Comp.FistBumpSound;
        break;
      case RMCHandsEmoteState.Highfive:
        soundSpecifier = ent.Comp.HighFiveSound;
        break;
      case RMCHandsEmoteState.Tailswipe:
        soundSpecifier = ent.Comp.TailSwipeSound;
        break;
      case RMCHandsEmoteState.Hug:
        soundSpecifier = ent.Comp.HugSound;
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    SoundSpecifier sound = soundSpecifier;
    string str1;
    switch (state)
    {
      case RMCHandsEmoteState.Fistbump:
        str1 = this.Loc.GetString("rmc-hands-emotes-fistbump", (nameof (ent), (object) owner1), (nameof (target), (object) owner2));
        break;
      case RMCHandsEmoteState.Highfive:
        str1 = this.Loc.GetString("rmc-hands-emotes-highfive", (nameof (ent), (object) owner1), (nameof (target), (object) owner2));
        break;
      case RMCHandsEmoteState.Tailswipe:
        str1 = this.Loc.GetString("rmc-hands-emotes-tailswipe", (nameof (ent), (object) owner1), (nameof (target), (object) owner2));
        break;
      case RMCHandsEmoteState.Hug:
        str1 = this.Loc.GetString("rmc-hands-emotes-hug", (nameof (ent), (object) owner1), (nameof (target), (object) owner2));
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    string message1 = str1;
    string str2;
    switch (state)
    {
      case RMCHandsEmoteState.Fistbump:
        str2 = this.Loc.GetString("rmc-hands-emotes-fistbump-self", (nameof (ent), (object) owner1), (nameof (target), (object) owner2));
        break;
      case RMCHandsEmoteState.Highfive:
        str2 = this.Loc.GetString("rmc-hands-emotes-highfive-self", (nameof (ent), (object) owner1), (nameof (target), (object) owner2));
        break;
      case RMCHandsEmoteState.Tailswipe:
        str2 = this.Loc.GetString("rmc-hands-emotes-tailswipe-self", (nameof (ent), (object) owner1), (nameof (target), (object) owner2));
        break;
      case RMCHandsEmoteState.Hug:
        str2 = this.Loc.GetString("rmc-hands-emotes-hug-self", (nameof (ent), (object) owner1), (nameof (target), (object) owner2));
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    string message2 = str2;
    string str3;
    switch (state)
    {
      case RMCHandsEmoteState.Fistbump:
        str3 = this.Loc.GetString("rmc-hands-emotes-fistbump-self", (nameof (ent), (object) owner2), (nameof (target), (object) owner1));
        break;
      case RMCHandsEmoteState.Highfive:
        str3 = this.Loc.GetString("rmc-hands-emotes-highfive-self", (nameof (ent), (object) owner2), (nameof (target), (object) owner1));
        break;
      case RMCHandsEmoteState.Tailswipe:
        str3 = this.Loc.GetString("rmc-hands-emotes-tailswipe-self", (nameof (ent), (object) owner2), (nameof (target), (object) owner1));
        break;
      case RMCHandsEmoteState.Hug:
        str3 = this.Loc.GetString("rmc-hands-emotes-hug-self", (nameof (ent), (object) owner2), (nameof (target), (object) owner1));
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    string message3 = str3;
    this._popup.PopupClient(message2, owner1, new EntityUid?(owner1), PopupType.Medium);
    this._popup.PopupClient(message3, owner2, new EntityUid?(owner2), PopupType.Medium);
    this._melee.DoLunge(owner2, owner1);
    this._rotate.TryFaceCoordinates(owner1, this._transform.GetMapCoordinates(owner2).Position);
    this._rotate.TryFaceCoordinates(owner2, this._transform.GetMapCoordinates(owner1).Position);
    if (this._net.IsServer)
    {
      Filter filter = Filter.PvsExcept(owner1).RemovePlayerByAttachedEntity(owner2);
      this._popup.PopupEntity(message1, owner1, filter, true);
      this._audio.PlayPvs(sound, owner1);
      this._melee.DoLunge(owner1, owner2);
    }
    this.CancelHandEmotes(ent);
    this.CancelHandEmotes(target);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCHandEmotesComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCHandEmotesComponent, TransformComponent>();
    EntityUid uid;
    RMCHandEmotesComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out TransformComponent _))
    {
      if (comp1.Active && !(curTime < comp1.LeaveHangingAt))
      {
        this.CancelHandEmotes((Entity<RMCHandEmotesComponent>) (uid, comp1));
        this._popup.PopupEntity(this.Loc.GetString("rmc-hands-emotes-left-hanging"), uid, uid, PopupType.SmallCaution);
      }
    }
  }
}
