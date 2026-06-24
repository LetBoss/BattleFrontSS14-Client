// Decompiled with JetBrains decompiler
// Type: Content.Shared.Contraband.ContrabandSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.CCVar;
using Content.Shared.Examine;
using Content.Shared.Localizations;
using Content.Shared.Roles;
using Content.Shared.Verbs;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Contraband;

public sealed class ContrabandSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _configuration;
  [Dependency]
  private IPrototypeManager _proto;
  [Dependency]
  private SharedIdCardSystem _id;
  [Dependency]
  private ExamineSystemShared _examine;
  private bool _contrabandExamineEnabled;
  private bool _contrabandExamineOnlyInHudEnabled;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ContrabandComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventRefHandler<ContrabandComponent, GetVerbsEvent<ExamineVerb>>((object) this, __methodptr(OnDetailedExamine)), (Type[]) null, (Type[]) null);
    EntitySystemSubscriptionExt.CVar<bool>(this.Subs, this._configuration, CCVars.ContrabandExamine, new Action<bool>(this.SetContrabandExamine), true);
    EntitySystemSubscriptionExt.CVar<bool>(this.Subs, this._configuration, CCVars.ContrabandExamineOnlyInHUD, new Action<bool>(this.SetContrabandExamineOnlyInHUD), true);
  }

  public void CopyDetails(EntityUid uid, ContrabandComponent other, ContrabandComponent? contraband = null)
  {
    if (!this.Resolve<ContrabandComponent>(uid, ref contraband, true))
      return;
    contraband.Severity = other.Severity;
    contraband.AllowedDepartments = other.AllowedDepartments;
    contraband.AllowedJobs = other.AllowedJobs;
    this.Dirty(uid, (IComponent) contraband, (MetaDataComponent) null);
  }

  private void OnDetailedExamine(
    EntityUid ent,
    ContrabandComponent component,
    ref GetVerbsEvent<ExamineVerb> args)
  {
    if (!this._contrabandExamineEnabled)
      return;
    if (this._contrabandExamineOnlyInHudEnabled)
    {
      GetContrabandDetailsEvent contrabandDetailsEvent = new GetContrabandDetailsEvent();
      this.RaiseLocalEvent<GetContrabandDetailsEvent>(args.User, ref contrabandDetailsEvent, false);
      if (!contrabandDetailsEvent.CanShowContraband)
        return;
    }
    if (!args.CanInteract)
      return;
    IEnumerable<string> first1 = component.AllowedDepartments.Select<ProtoId<DepartmentPrototype>, string>((Func<ProtoId<DepartmentPrototype>, string>) (p => this.Loc.GetString("contraband-department-plural", ("department", (object) this.Loc.GetString(LocId.op_Implicit(this._proto.Index<DepartmentPrototype>(p).Name))))));
    string[] array = component.AllowedJobs.Select<ProtoId<JobPrototype>, string>((Func<ProtoId<JobPrototype>, string>) (p => this._proto.Index<JobPrototype>(p).LocalizedName)).ToArray<string>();
    IEnumerable<string> second = ((IEnumerable<string>) array).Select<string, string>((Func<string, string>) (p => this.Loc.GetString("contraband-job-plural", ("job", (object) p))));
    ContrabandSeverityPrototype severityPrototype = this._proto.Index<ContrabandSeverityPrototype>(component.Severity);
    string deptMessage = !severityPrototype.ShowDepartmentsAndJobs ? this.Loc.GetString(LocId.op_Implicit(severityPrototype.ExamineText)) : this.Loc.GetString("contraband-examine-text-Restricted-department", ("departments", (object) ContentLocalizationManager.FormatList(first1.Concat<string>(second).ToList<string>())));
    List<ProtoId<DepartmentPrototype>> first2 = new List<ProtoId<DepartmentPrototype>>();
    string str = "";
    Entity<IdCardComponent> idCard;
    if (this._id.TryFindIdCard(args.User, out idCard))
    {
      first2 = idCard.Comp.JobDepartments;
      if (idCard.Comp.LocalizedJobTitle != null)
        str = idCard.Comp.LocalizedJobTitle;
    }
    string carryMessage = this.Loc.GetString("contraband-examine-text-avoid-carrying-around");
    string iconTexture = "/Textures/Interface/VerbIcons/lock-red.svg.192dpi.png";
    if (first2.Intersect<ProtoId<DepartmentPrototype>>((IEnumerable<ProtoId<DepartmentPrototype>>) component.AllowedDepartments).Any<ProtoId<DepartmentPrototype>>() || ((IEnumerable<string>) array).Contains<string>(str))
    {
      carryMessage = this.Loc.GetString("contraband-examine-text-in-the-clear");
      iconTexture = "/Textures/Interface/VerbIcons/unlock-green.svg.192dpi.png";
    }
    FormattedMessage contrabandExamine = this.GetContrabandExamine(deptMessage, carryMessage);
    this._examine.AddHoverExamineVerb(args, (Component) component, this.Loc.GetString("contraband-examinable-verb-text"), contrabandExamine.ToMarkup(), iconTexture);
  }

  private FormattedMessage GetContrabandExamine(string deptMessage, string carryMessage)
  {
    FormattedMessage contrabandExamine = new FormattedMessage();
    contrabandExamine.AddMarkupOrThrow(deptMessage);
    contrabandExamine.PushNewline();
    contrabandExamine.AddMarkupOrThrow(carryMessage);
    return contrabandExamine;
  }

  private void SetContrabandExamine(bool val) => this._contrabandExamineEnabled = val;

  private void SetContrabandExamineOnlyInHUD(bool val)
  {
    this._contrabandExamineOnlyInHudEnabled = val;
  }
}
