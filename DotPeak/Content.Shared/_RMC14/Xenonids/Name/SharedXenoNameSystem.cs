// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Name.SharedXenoNameSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.IdentityManagement;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Players.PlayTimeTracking;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Name;

public abstract class SharedXenoNameSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private SharedMindSystem _mind;
  [Dependency]
  private NameModifierSystem _nameModifier;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private ISharedPlaytimeManager _playtime;
  [Dependency]
  private IPrototypeManager _prototype;
  private const string DefaultPrefix = "XX";
  private TimeSpan _xenoPrefixThreeTime;
  private TimeSpan _xenoPostfixTime;
  private TimeSpan _xenoPostfixTwoTime;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<NewXenoEvolvedEvent>(new EntityEventRefHandler<NewXenoEvolvedEvent>(this.OnNewXenoEvolved));
    this.SubscribeLocalEvent<XenoDevolvedEvent>(new EntityEventRefHandler<XenoDevolvedEvent>(this.OnXenoDevolved));
    this.SubscribeLocalEvent<XenoNameComponent, RefreshNameModifiersEvent>(new EntityEventRefHandler<XenoNameComponent, RefreshNameModifiersEvent>(this.OnRefreshNameModifiers));
    this.SubscribeLocalEvent<XenoNameComponent, MindAddedMessage>(new EntityEventRefHandler<XenoNameComponent, MindAddedMessage>(this.OnMindAdded));
    this.SubscribeLocalEvent<XenoNameComponent, RMCGetFixedIdentityEvent>(new EntityEventRefHandler<XenoNameComponent, RMCGetFixedIdentityEvent>(this.OnGetFixedIdentity));
    this.Subs.CVar<int>(this._config, RMCCVars.RMCPlaytimeXenoPrefixThreeTimeHours, (Action<int>) (v => this._xenoPrefixThreeTime = TimeSpan.FromHours(v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCPlaytimeXenoPostfixTimeHours, (Action<int>) (v => this._xenoPostfixTime = TimeSpan.FromHours(v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCPlaytimeXenoPostfixTwoTimeHours, (Action<int>) (v => this._xenoPostfixTwoTime = TimeSpan.FromHours(v)), true);
  }

  private void OnNewXenoEvolved(ref NewXenoEvolvedEvent ev)
  {
    this.TransferName((EntityUid) ev.OldXeno, ev.NewXeno);
  }

  private void OnXenoDevolved(ref XenoDevolvedEvent ev)
  {
    this.TransferName(ev.OldXeno, ev.NewXeno);
  }

  private void OnRefreshNameModifiers(
    Entity<XenoNameComponent> ent,
    ref RefreshNameModifiersEvent args)
  {
    string rank = ent.Comp.Rank;
    if (rank.Length > 0)
      rank += " ";
    string str1 = ent.Comp.Prefix;
    if (str1.Length == 0)
      str1 = "XX";
    string str2 = ent.Comp.Postfix;
    int number = ent.Comp.Number;
    if (this.HasComp<XenoOmitNumberComponent>((EntityUid) ent))
    {
      args.AddModifier((LocId) "rmc-xeno-name", 0, ("rank", (object) rank), ("prefix", (object) str1), ("postfix", (object) str2));
    }
    else
    {
      if (str2.Length > 0)
        str2 = "-" + str2;
      args.AddModifier((LocId) "rmc-xeno-name-number", 0, ("rank", (object) rank), ("prefix", (object) str1), ("number", (object) number), ("postfix", (object) str2));
    }
    MindComponent mind;
    if (!this._mind.TryGetMind((EntityUid) ent, out EntityUid _, out mind))
      return;
    mind.CharacterName = args.GetModifiedName();
  }

  private void OnMindAdded(Entity<XenoNameComponent> ent, ref MindAddedMessage args)
  {
    this.SetupName((EntityUid) ent);
  }

  private void OnGetFixedIdentity(Entity<XenoNameComponent> ent, ref RMCGetFixedIdentityEvent args)
  {
    if (this.HasComp<XenoOmitNumberComponent>((EntityUid) ent))
      args.Name = this.Loc.GetString("rmc-xeno-name", ("baseName", (object) args.Name), ("prefix", (object) "XX"), ("postfix", (object) string.Empty));
    else
      args.Name = this.Loc.GetString("rmc-xeno-name-number", ("baseName", (object) args.Name), ("prefix", (object) "XX"), ("number", (object) ent.Comp.Number), ("postfix", (object) string.Empty));
  }

  private TimeSpan GetXenoPlaytime(ICommonSession player)
  {
    TimeSpan zero = TimeSpan.Zero;
    try
    {
      foreach ((string str, TimeSpan timeSpan) in (IEnumerable<KeyValuePair<string, TimeSpan>>) this._playtime.GetPlayTimes(player))
      {
        PlayTimeTrackerPrototype prototype;
        if (this._prototype.TryIndex<PlayTimeTrackerPrototype>(str, out prototype) && prototype.IsXeno)
          zero += timeSpan;
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error reading total xeno playtime:\n{ex}");
    }
    return zero;
  }

  public int GetMaxXenoPrefixLength(ICommonSession player)
  {
    return !(this.GetXenoPlaytime(player) < this._xenoPrefixThreeTime) ? 3 : 2;
  }

  public int GetMaxXenoPostfixLength(ICommonSession player)
  {
    TimeSpan xenoPlaytime = this.GetXenoPlaytime(player);
    if (xenoPlaytime > this._xenoPostfixTwoTime)
      return 2;
    return xenoPlaytime > this._xenoPostfixTime ? 1 : 0;
  }

  private void TransferName(EntityUid oldXeno, EntityUid newXeno)
  {
    XenoNameComponent comp;
    if (this._net.IsClient || !this.TryComp<XenoNameComponent>(oldXeno, out comp))
      return;
    XenoNameComponent xenoNameComponent = this.EnsureComp<XenoNameComponent>(newXeno);
    xenoNameComponent.Rank = comp.Rank;
    xenoNameComponent.Prefix = comp.Prefix;
    xenoNameComponent.Number = comp.Number;
    xenoNameComponent.Postfix = comp.Postfix;
    this.Dirty(newXeno, (IComponent) xenoNameComponent);
    this.RemComp<AssignXenoNameComponent>(newXeno);
    this._nameModifier.RefreshNameModifiers((Entity<NameModifierComponent>) newXeno);
  }

  public virtual void SetupName(EntityUid xeno)
  {
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<AssignXenoNameComponent> entityQueryEnumerator = this.EntityQueryEnumerator<AssignXenoNameComponent>();
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out AssignXenoNameComponent _))
      this.SetupName(uid);
  }
}
