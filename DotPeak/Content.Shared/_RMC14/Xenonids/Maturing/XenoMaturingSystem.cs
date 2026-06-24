// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Maturing.XenoMaturingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Examine;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Maturing;

public sealed class XenoMaturingSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private MobThresholdSystem _mobThreshold;
  [Dependency]
  private NameModifierSystem _nameModifier;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoMaturingComponent, MapInitEvent>(new EntityEventRefHandler<XenoMaturingComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<XenoMaturingComponent, ComponentRemove>(new EntityEventRefHandler<XenoMaturingComponent, ComponentRemove>(this.OnRemove));
    this.SubscribeLocalEvent<XenoMaturingComponent, RefreshNameModifiersEvent>(new EntityEventRefHandler<XenoMaturingComponent, RefreshNameModifiersEvent>(this.OnRefreshNameModifiers));
    this.SubscribeLocalEvent<XenoMaturingComponent, ExaminedEvent>(new EntityEventRefHandler<XenoMaturingComponent, ExaminedEvent>(this.OnExamined));
  }

  private void OnMapInit(Entity<XenoMaturingComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.MatureAt = this._timing.CurTime + ent.Comp.Delay;
    this.Dirty<XenoMaturingComponent>(ent);
    this._nameModifier.RefreshNameModifiers((Entity<NameModifierComponent>) ent.Owner);
  }

  private void OnRemove(Entity<XenoMaturingComponent> ent, ref ComponentRemove args)
  {
    if (this.TerminatingOrDeleted((EntityUid) ent))
      return;
    this._nameModifier.RefreshNameModifiers((Entity<NameModifierComponent>) ent.Owner);
  }

  private void OnRefreshNameModifiers(
    Entity<XenoMaturingComponent> ent,
    ref RefreshNameModifiersEvent args)
  {
    args.AddModifier((LocId) "rmc-xeno-immature-prefix");
  }

  private void OnExamined(Entity<XenoMaturingComponent> ent, ref ExaminedEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.Examiner))
      return;
    TimeSpan timeSpan = ent.Comp.MatureAt - this._timing.CurTime;
    if (timeSpan <= TimeSpan.Zero)
      return;
    using (args.PushGroup(nameof (XenoMaturingSystem)))
    {
      int totalMinutes = (int) timeSpan.TotalMinutes;
      int seconds = timeSpan.Seconds;
      if (totalMinutes > 0)
      {
        args.PushText(this.Loc.GetString("rmc-xeno-immature-matures-in-minutes", ("minutes", (object) totalMinutes), ("seconds", (object) seconds)));
      }
      else
      {
        if (seconds <= 0)
          return;
        args.PushText(this.Loc.GetString("rmc-xeno-immature-matures-in-seconds", ("seconds", (object) seconds)));
      }
    }
  }

  public void Mature(Entity<XenoMaturingComponent> maturing)
  {
    maturing.Comp.MatureAt = TimeSpan.Zero;
    this.Dirty<XenoMaturingComponent>(maturing);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoMaturingComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoMaturingComponent>();
    EntityUid uid;
    XenoMaturingComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1) && !(curTime < comp1.MatureAt))
    {
      this._mobThreshold.SetMobStateThreshold(uid, comp1.DeadThreshold, MobState.Dead);
      this._mobThreshold.SetMobStateThreshold(uid, comp1.CritThreshold, MobState.Critical);
      this.EntityManager.AddComponents(uid, comp1.AddComponents, true);
      foreach (EntProtoId addAction in comp1.AddActions)
        this._actions.AddAction(uid, (string) addAction);
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-immature-mature"), uid, uid, PopupType.Large);
      this.RemCompDeferred<XenoMaturingComponent>(uid);
    }
  }
}
