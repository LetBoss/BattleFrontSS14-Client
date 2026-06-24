// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Examine.RMCMedicalExamineSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared._RMC14.Stun;
using Content.Shared.Body.Components;
using Content.Shared.Examine;
using Content.Shared.Mobs.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Shared._RMC14.Medical.Examine;

public sealed class RMCMedicalExamineSystem : EntitySystem
{
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private RMCSizeStunSystem _sizeStun;
  [Dependency]
  private RMCUnrevivableSystem _unrevivable;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCMedicalExamineComponent, ExaminedEvent>(new EntityEventRefHandler<RMCMedicalExamineComponent, ExaminedEvent>(this.OnExamined));
  }

  private void OnExamined(Entity<RMCMedicalExamineComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup(nameof (RMCMedicalExamineSystem), -1))
    {
      if (ent.Comp.Simple && this._mobState.IsDead(ent.Owner))
      {
        args.PushMarkup(this.Loc.GetString((string) ent.Comp.DeadText, ("victim", (object) ent.Owner)));
      }
      else
      {
        if (this.HasComp<RMCBlockMedicalExamineComponent>(args.Examiner))
          return;
        args.PushMessage(this.GetExamineText(ent));
      }
    }
  }

  public FormattedMessage GetExamineText(Entity<RMCMedicalExamineComponent> ent)
  {
    FormattedMessage examineText = new FormattedMessage();
    BloodstreamComponent comp;
    if (this.TryComp<BloodstreamComponent>((EntityUid) ent, out comp) && (double) comp.BleedAmount > 0.0)
      examineText.AddMarkupOrThrow(this.Loc.GetString((string) ent.Comp.BleedText, ("victim", (object) ent.Owner)));
    LocId? nullable1 = new LocId?();
    if (this._mobState.IsDead((EntityUid) ent))
      nullable1 = new LocId?(this._unrevivable.IsUnrevivable((EntityUid) ent) ? ent.Comp.UnrevivableText : ent.Comp.DeadText);
    else if (this._mobState.IsCritical((EntityUid) ent) || this._sizeStun.IsKnockedOut((EntityUid) ent))
      nullable1 = new LocId?(ent.Comp.CritText);
    if (nullable1.HasValue)
    {
      FormattedMessage formattedMessage = examineText;
      ILocalizationManager loc = this.Loc;
      LocId? nullable2 = nullable1;
      string valueOrDefault = nullable2.HasValue ? (string) nullable2.GetValueOrDefault() : (string) null;
      (string, object) valueTuple = ("victim", (object) ent.Owner);
      string markup = loc.GetString(valueOrDefault, valueTuple);
      formattedMessage.AddMarkupOrThrow(markup);
    }
    return examineText;
  }
}
