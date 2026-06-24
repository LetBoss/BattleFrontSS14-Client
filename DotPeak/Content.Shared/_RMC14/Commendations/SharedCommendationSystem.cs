// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Commendations.SharedCommendationSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared.Database;
using Content.Shared.GameTicking;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Commendations;

public abstract class SharedCommendationSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _config;
  protected readonly List<Commendation> RoundCommendations = new List<Commendation>();

  public int CharacterLimit { get; private set; }

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestartCleanup));
    this.SubscribeLocalEvent<CommendationReceiverComponent, PlayerAttachedEvent>(new EntityEventRefHandler<CommendationReceiverComponent, PlayerAttachedEvent>(this.OnCommendationReceiverPlayerAttached));
    this.Subs.CVar<int>(this._config, RMCCVars.RMCCommendationMaxLength, (Action<int>) (v => this.CharacterLimit = v), true);
  }

  private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
  {
    this.RoundCommendations.Clear();
  }

  private void OnCommendationReceiverPlayerAttached(
    Entity<CommendationReceiverComponent> ent,
    ref PlayerAttachedEvent args)
  {
    ent.Comp.LastPlayerId = args.Player.UserId.UserId.ToString();
  }

  public bool ValidCommendation(
    Entity<CommendationGiverComponent?, ActorComponent?> giver,
    Entity<CommendationReceiverComponent?> receiver,
    string text)
  {
    if (!this.Resolve<CommendationGiverComponent, ActorComponent>((EntityUid) giver, ref giver.Comp1, ref giver.Comp2, false) || !this.Resolve<CommendationReceiverComponent>((EntityUid) receiver, ref receiver.Comp, false) || receiver.Comp.LastPlayerId == null)
      return false;
    text = text.Trim();
    return !string.IsNullOrWhiteSpace(text);
  }

  public virtual void GiveCommendation(
    Entity<CommendationGiverComponent?, ActorComponent?> giver,
    Entity<CommendationReceiverComponent?> receiver,
    string name,
    string text,
    CommendationType type)
  {
  }

  public virtual void GiveCommendationByLastPlayerId(
    Entity<CommendationGiverComponent?, ActorComponent?> giver,
    string lastPlayerId,
    string receiverName,
    string name,
    string text,
    CommendationType type)
  {
  }

  public IReadOnlyList<Commendation> GetCommendations()
  {
    return (IReadOnlyList<Commendation>) this.RoundCommendations;
  }
}
