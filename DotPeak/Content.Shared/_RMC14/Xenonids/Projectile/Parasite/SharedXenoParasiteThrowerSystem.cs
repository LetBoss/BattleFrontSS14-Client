// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Projectile.Parasite.SharedXenoParasiteThrowerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Ghost;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Projectile.Parasite;

public abstract class SharedXenoParasiteThrowerSystem : EntitySystem
{
  [Dependency]
  protected SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedUserInterfaceSystem _ui;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoParasiteThrowerComponent, ExaminedEvent>(new EntityEventRefHandler<XenoParasiteThrowerComponent, ExaminedEvent>(this.OnParasiteThrowerExamine));
    this.SubscribeLocalEvent<XenoParasiteThrowerComponent, XenoChangeParasiteReserveMessage>(new EntityEventRefHandler<XenoParasiteThrowerComponent, XenoChangeParasiteReserveMessage>(this.OnParasiteReserveChange));
    this.SubscribeLocalEvent<XenoParasiteThrowerComponent, XenoReserveParasiteActionEvent>(new EntityEventRefHandler<XenoParasiteThrowerComponent, XenoReserveParasiteActionEvent>(this.OnSetReserve));
    this.SubscribeLocalEvent<XenoParasiteThrowerComponent, GetVerbsEvent<ActivationVerb>>(new EntityEventRefHandler<XenoParasiteThrowerComponent, GetVerbsEvent<ActivationVerb>>(this.OnGetVerbs));
  }

  private void OnParasiteThrowerExamine(
    Entity<XenoParasiteThrowerComponent> thrower,
    ref ExaminedEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.Examiner) && !this.HasComp<GhostComponent>(args.Examiner))
      return;
    if (this.HasComp<GhostComponent>(args.Examiner))
    {
      int num = Math.Max(thrower.Comp.CurParasites - thrower.Comp.ReservedParasites, 0);
      using (args.PushGroup("XenoParasiteThrowerComponent"))
        args.PushMarkup(this.Loc.GetString("rmc-xeno-throw-parasite-reserves", ("xeno", (object) thrower), ("rev_paras", (object) num)));
    }
    else
    {
      using (args.PushGroup("XenoParasiteThrowerComponent"))
        args.PushMarkup(this.Loc.GetString("rmc-xeno-throw-parasite-current", ("xeno", (object) thrower), ("cur_paras", (object) thrower.Comp.CurParasites), ("max_paras", (object) thrower.Comp.MaxParasites)));
    }
  }

  private void OnParasiteReserveChange(
    Entity<XenoParasiteThrowerComponent> thrower,
    ref XenoChangeParasiteReserveMessage args)
  {
    int num = Math.Clamp(args.NewReserve, 0, thrower.Comp.MaxParasites);
    thrower.Comp.ReservedParasites = num;
    this.Dirty<XenoParasiteThrowerComponent>(thrower);
  }

  private void OnSetReserve(
    Entity<XenoParasiteThrowerComponent> xeno,
    ref XenoReserveParasiteActionEvent args)
  {
    if (args.Handled)
      return;
    this._ui.OpenUi((Entity<UserInterfaceComponent>) xeno.Owner, (Enum) XenoReserveParasiteChangeUI.Key, new EntityUid?((EntityUid) xeno));
    args.Handled = true;
  }

  private void OnGetVerbs(
    Entity<XenoParasiteThrowerComponent> xeno,
    ref GetVerbsEvent<ActivationVerb> args)
  {
    EntityUid uid = args.User;
    if (!this.HasComp<ActorComponent>(uid) || !this.HasComp<GhostComponent>(uid) || xeno.Comp.CurParasites == 0 || xeno.Comp.ReservedParasites >= xeno.Comp.CurParasites)
      return;
    ActivationVerb activationVerb1 = new ActivationVerb();
    activationVerb1.Text = this.Loc.GetString("rmc-xeno-egg-ghost-verb");
    activationVerb1.Act = (Action) (() => this._ui.TryOpenUi((Entity<UserInterfaceComponent>) xeno.Owner, (Enum) XenoParasiteGhostUI.Key, uid));
    activationVerb1.Impact = LogImpact.High;
    ActivationVerb activationVerb2 = activationVerb1;
    args.Verbs.Add(activationVerb2);
  }
}
