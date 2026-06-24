// Decompiled with JetBrains decompiler
// Type: Content.Shared.Traits.Assorted.PainNumbnessSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Damage.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Events;
using Content.Shared.Mobs.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Shared.Traits.Assorted;

public sealed class PainNumbnessSystem : EntitySystem
{
  [Dependency]
  private MobThresholdSystem _mobThresholdSystem;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PainNumbnessComponent, ComponentInit>(new ComponentEventHandler<PainNumbnessComponent, ComponentInit>(this.OnComponentInit));
    this.SubscribeLocalEvent<PainNumbnessComponent, ComponentRemove>(new ComponentEventHandler<PainNumbnessComponent, ComponentRemove>(this.OnComponentRemove));
    this.SubscribeLocalEvent<PainNumbnessComponent, BeforeForceSayEvent>(new EntityEventRefHandler<PainNumbnessComponent, BeforeForceSayEvent>(this.OnChangeForceSay));
    this.SubscribeLocalEvent<PainNumbnessComponent, BeforeAlertSeverityCheckEvent>(new EntityEventRefHandler<PainNumbnessComponent, BeforeAlertSeverityCheckEvent>(this.OnAlertSeverityCheck));
  }

  private void OnComponentRemove(
    EntityUid uid,
    PainNumbnessComponent component,
    ComponentRemove args)
  {
    if (!this.HasComp<MobThresholdsComponent>(uid))
      return;
    this._mobThresholdSystem.VerifyThresholds(uid);
  }

  private void OnComponentInit(EntityUid uid, PainNumbnessComponent component, ComponentInit args)
  {
    if (!this.HasComp<MobThresholdsComponent>(uid))
      return;
    this._mobThresholdSystem.VerifyThresholds(uid);
  }

  private void OnChangeForceSay(Entity<PainNumbnessComponent> ent, ref BeforeForceSayEvent args)
  {
    args.Prefix = ent.Comp.ForceSayNumbDataset;
  }

  private void OnAlertSeverityCheck(
    Entity<PainNumbnessComponent> ent,
    ref BeforeAlertSeverityCheckEvent args)
  {
    if (!(args.CurrentAlert == (ProtoId<AlertPrototype>) "HumanHealth"))
      return;
    args.CancelUpdate = true;
  }
}
