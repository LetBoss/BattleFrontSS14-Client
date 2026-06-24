// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Announce.SharedXenoAnnounceSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Bioscan;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Announce;

public abstract class SharedXenoAnnounceSystem : EntitySystem
{
  [Robust.Shared.IoC.Dependency]
  private AreaSystem _areas;
  [Robust.Shared.IoC.Dependency]
  private XenoEvolutionSystem _xenoEvolution;
  [Robust.Shared.IoC.Dependency]
  protected SharedXenoHiveSystem Hive;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoAnnounceDeathComponent, MobStateChangedEvent>(new EntityEventRefHandler<XenoAnnounceDeathComponent, MobStateChangedEvent>(this.OnAnnounceDeathMobStateChanged));
  }

  private void OnAnnounceDeathMobStateChanged(
    Entity<XenoAnnounceDeathComponent> ent,
    ref MobStateChangedEvent args)
  {
    if (args.NewMobState != MobState.Dead)
      return;
    string str = "Unknown";
    EntityPrototype areaPrototype;
    if (this._areas.TryGetArea((EntityUid) ent, out Entity<AreaComponent>? _, out areaPrototype))
      str = areaPrototype.Name;
    if (this.HasComp<ParasiteSpentComponent>((EntityUid) ent))
    {
      Entity<HiveMemberComponent> owner = (Entity<HiveMemberComponent>) ent.Owner;
      string message = this.Loc.GetString("rmc-xeno-parasite-announce-infect", ("xeno", (object) ent.Owner), ("location", (object) str));
      Color? nullable = new Color?(ent.Comp.Color);
      PopupType? popup = new PopupType?();
      Color? color = nullable;
      this.AnnounceSameHive(owner, message, popup: popup, color: color);
    }
    else
    {
      if (!this.HasComp<XenoEvolutionGranterComponent>((EntityUid) ent) && !this._xenoEvolution.HasLiving<XenoEvolutionGranterComponent>(1))
        return;
      Entity<HiveMemberComponent> owner = (Entity<HiveMemberComponent>) ent.Owner;
      string message = this.Loc.GetString((string) ent.Comp.Message, ("xeno", (object) ent.Owner), ("location", (object) str));
      Color? nullable = new Color?(ent.Comp.Color);
      PopupType? popup = new PopupType?();
      Color? color = nullable;
      this.AnnounceSameHive(owner, message, popup: popup, color: color);
    }
  }

  public string WrapHive(string message, Color? color = null)
  {
    color.GetValueOrDefault();
    if (!color.HasValue)
      color = new Color?(Color.FromHex((ReadOnlySpan<char>) "#921992", new Color?()));
    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(52, 2);
    interpolatedStringHandler.AppendLiteral("[color=");
    ref DefaultInterpolatedStringHandler local = ref interpolatedStringHandler;
    Color color1 = color.Value;
    string hex = ((Color) ref color1).ToHex();
    local.AppendFormatted(hex);
    interpolatedStringHandler.AppendLiteral("][font size=16][bold]");
    interpolatedStringHandler.AppendFormatted(message);
    interpolatedStringHandler.AppendLiteral("[/bold][/font][/color]\n\n");
    return interpolatedStringHandler.ToStringAndClear();
  }

  public virtual void Announce(
    EntityUid source,
    Filter filter,
    string message,
    string wrapped,
    SoundSpecifier? sound = null,
    PopupType? popup = null,
    bool needsQueen = false)
  {
  }

  public void AnnounceToHive(
    EntityUid source,
    EntityUid hive,
    string message,
    SoundSpecifier? sound = null,
    PopupType? popup = null,
    Color? color = null,
    bool needsQueen = false)
  {
    Filter filter = Filter.Empty().AddWhereAttachedEntity((Predicate<EntityUid>) (e => this.Hive.IsMember((Entity<HiveMemberComponent>) e, new EntityUid?(hive))));
    this.Announce(source, filter, message, this.WrapHive(message, color), sound, popup, needsQueen);
  }

  public void AnnounceSameHive(
    Entity<HiveMemberComponent?> xeno,
    string message,
    SoundSpecifier? sound = null,
    PopupType? popup = null,
    Color? color = null,
    bool needsQueen = false)
  {
    Entity<HiveComponent>? hive = this.Hive.GetHive(xeno);
    if (!hive.HasValue)
      return;
    Entity<HiveComponent> valueOrDefault = hive.GetValueOrDefault();
    this.AnnounceToHive((EntityUid) xeno, (EntityUid) valueOrDefault, message, sound, popup, color, needsQueen);
  }

  public void AnnounceSameHiveDefaultSound(
    Entity<HiveMemberComponent?> xeno,
    string message,
    PopupType? popup = null,
    Color? color = null,
    bool needsQueen = false)
  {
    SoundSpecifier xenoSound = new BioscanComponent().XenoSound;
    this.AnnounceSameHive(xeno, message, xenoSound, popup, color, needsQueen);
  }

  public void AnnounceAll(
    EntityUid source,
    string message,
    SoundSpecifier? sound = null,
    PopupType? popup = null,
    bool needsQueen = false)
  {
    this.Announce(source, Filter.Empty().AddWhereAttachedEntity(new Predicate<EntityUid>(((EntitySystem) this).HasComp<XenoComponent>)), message, message, sound, popup, needsQueen);
  }

  public void AnnounceQueenMother(string message)
  {
    SoundSpecifier xenoSound = new BioscanComponent().XenoSound;
    this.AnnounceAll(new EntityUid(), this.FormatQueenMother(message), xenoSound);
  }

  public string FormatQueenMother(string message)
  {
    return $"\n[bold][color=#7575F3][font size=24]Queen Mother Psychic Directive[/font][/color][/bold]\n\n[color=red][font size=14]{message}[/font][/color]\n\n";
  }
}
