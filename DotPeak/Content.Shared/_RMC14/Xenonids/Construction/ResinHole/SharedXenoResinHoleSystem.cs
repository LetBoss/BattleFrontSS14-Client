// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.ResinHole.SharedXenoResinHoleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Xenonids.Announce;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.ResinHole;

public abstract class SharedXenoResinHoleSystem : EntitySystem
{
  [Dependency]
  protected SharedAppearanceSystem _appearanceSystem;
  [Dependency]
  protected MobStateSystem _mobState;
  [Dependency]
  protected RMCHandsSystem _rmcHands;
  [Dependency]
  protected SharedXenoHiveSystem _hive;
  [Dependency]
  protected INetManager _net;
  [Dependency]
  protected SharedPopupSystem _popup;
  [Dependency]
  protected SharedDoAfterSystem _doAfter;
  [Dependency]
  private AreaSystem _areas;
  [Dependency]
  private SharedXenoAnnounceSystem _announce;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoResinHoleComponent, InteractUsingEvent>(new EntityEventRefHandler<XenoResinHoleComponent, InteractUsingEvent>(this.OnPlaceParasiteInXenoResinHole));
    this.SubscribeLocalEvent<XenoResinHoleComponent, ActivateInWorldEvent>(new EntityEventRefHandler<XenoResinHoleComponent, ActivateInWorldEvent>(this.OnActivateInWorldResinHole));
    this.SubscribeLocalEvent<XenoResinHoleComponent, XenoResinHoleActivationEvent>(new EntityEventRefHandler<XenoResinHoleComponent, XenoResinHoleActivationEvent>(this.OnResinHoleActivation));
    this.SubscribeLocalEvent<XenoResinHoleComponent, GettingAttackedAttemptEvent>(new EntityEventRefHandler<XenoResinHoleComponent, GettingAttackedAttemptEvent>(this.OnXenoResinHoleAttacked));
  }

  private void OnXenoResinHoleAttacked(
    Entity<XenoResinHoleComponent> resinHole,
    ref GettingAttackedAttemptEvent args)
  {
    if (!this._hive.FromSameHive((Entity<HiveMemberComponent>) args.Attacker, (Entity<HiveMemberComponent>) resinHole.Owner) || !resinHole.Comp.TrapPrototype.HasValue)
      return;
    args.Cancelled = true;
  }

  protected bool CanPlaceInHole(
    EntityUid uid,
    Entity<XenoResinHoleComponent> resinHole,
    EntityUid user)
  {
    if (!this.HasComp<XenoParasiteComponent>(uid) || this._mobState.IsDead(uid))
      return false;
    if (resinHole.Comp.TrapPrototype.HasValue)
    {
      this._popup.PopupEntity(this.Loc.GetString("cm-xeno-construction-resin-hole-full"), (EntityUid) resinHole, user);
      return false;
    }
    if (!this._rmcHands.IsPickupByAllowed((Entity<WhitelistPickupByComponent>) uid, (Entity<WhitelistPickupComponent>) user))
      return false;
    if (this.HasComp<ParasiteAIComponent>(uid))
      return true;
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-egg-awake-child", ("parasite", (object) uid)), user, user, PopupType.SmallCaution);
    return false;
  }

  private void OnPlaceParasiteInXenoResinHole(
    Entity<XenoResinHoleComponent> resinHole,
    ref InteractUsingEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    if (this._net.IsClient || !this.CanPlaceInHole(args.Used, resinHole, args.User))
      return;
    XenoPlaceParasiteInHoleDoAfterEvent @event = new XenoPlaceParasiteInHoleDoAfterEvent();
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, resinHole.Comp.AddParasiteDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) resinHole), new EntityUid?((EntityUid) resinHole), new EntityUid?(args.Used))
    {
      BreakOnDropItem = true,
      BreakOnMove = true,
      BreakOnHandChange = true
    });
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-construction-resin-hole-filling-parasite"), (EntityUid) resinHole, args.User);
  }

  private void OnActivateInWorldResinHole(
    Entity<XenoResinHoleComponent> resinHole,
    ref ActivateInWorldEvent args)
  {
    if (!this.HasComp<XenoParasiteComponent>(args.User))
      return;
    args.Handled = true;
    if (this._mobState.IsDead(args.User) || this._net.IsClient || resinHole.Comp.TrapPrototype.HasValue)
      return;
    this.SetTrapType(resinHole, "CMXenoParasite");
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-construction-resin-hole-enter-parasite", ("parasite", (object) args.User)), (EntityUid) resinHole);
    this._hive.SetSameHive((Entity<HiveMemberComponent>) args.User, (Entity<HiveMemberComponent>) resinHole.Owner);
    this.QueueDel(new EntityUid?(args.User));
    this._appearanceSystem.SetData(resinHole.Owner, (Enum) XenoResinHoleVisuals.Contained, (object) ContainedTrap.Parasite);
  }

  private void OnResinHoleActivation(
    Entity<XenoResinHoleComponent> ent,
    ref XenoResinHoleActivationEvent args)
  {
    Entity<HiveComponent>? hive1 = this._hive.GetHive((Entity<HiveMemberComponent>) ent.Owner);
    if (!hive1.HasValue)
      return;
    Entity<HiveComponent> valueOrDefault = hive1.GetValueOrDefault();
    string str1 = "Unknown";
    EntityPrototype areaPrototype;
    if (this._areas.TryGetArea((EntityUid) ent, out Entity<AreaComponent>? _, out areaPrototype))
      str1 = areaPrototype.Name;
    string str2 = this.Loc.GetString((string) args.message, ("location", (object) str1), ("type", (object) this.GetTrapTypeName(ent)));
    SharedXenoAnnounceSystem announce = this._announce;
    EntityUid owner = ent.Owner;
    EntityUid hive2 = (EntityUid) valueOrDefault;
    string message = str2;
    Color? nullable = new Color?(ent.Comp.MessageColor);
    PopupType? popup = new PopupType?();
    Color? color = nullable;
    announce.AnnounceToHive(owner, hive2, message, popup: popup, color: color);
  }

  protected void SetTrapType(Entity<XenoResinHoleComponent> resinHole, string? newTrapPrototype)
  {
    resinHole.Comp.TrapPrototype = (EntProtoId?) newTrapPrototype;
    XenoAnnounceStructureDestructionComponent comp;
    if (this.TryComp<XenoAnnounceStructureDestructionComponent>(resinHole.Owner, out comp))
      comp.StructureName = this.GetTrapTypeName(resinHole);
    this.Dirty<XenoResinHoleComponent>(resinHole);
  }

  public string GetTrapTypeName(Entity<XenoResinHoleComponent> resinHole)
  {
    EntProtoId? trapPrototype = resinHole.Comp.TrapPrototype;
    switch (trapPrototype.HasValue ? (string) trapPrototype.GetValueOrDefault() : (string) null)
    {
      case "CMXenoParasite":
        return this.Loc.GetString("rmc-xeno-construction-resin-hole-parasite-name");
      case "RMCSmokeAcid":
      case "RMCSmokeNeurotoxin":
        return this.Loc.GetString("rmc-xeno-construction-resin-hole-gas-name");
      case "XenoAcidSprayTrapWeak":
      case "XenoAcidSprayTrap":
      case "XenoAcidSprayTrapStrong":
        return this.Loc.GetString("rmc-xeno-construction-resin-hole-acid-name");
      default:
        return this.Loc.GetString("rmc-xeno-construction-resin-hole-empty-name");
    }
  }
}
