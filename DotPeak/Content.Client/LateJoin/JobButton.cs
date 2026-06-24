// Decompiled with JetBrains decompiler
// Type: Content.Client.LateJoin.JobButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Roles;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Client.LateJoin;

internal sealed class JobButton : ContainerButton
{
  private bool _initialised;

  public Label JobLabel { get; }

  public string JobId { get; }

  public string JobLocalisedName { get; }

  public int? Amount { get; private set; }

  public JobButton(
    Label jobLabel,
    ProtoId<JobPrototype> jobId,
    string jobLocalisedName,
    int? amount)
  {
    this.JobLabel = jobLabel;
    this.JobId = ProtoId<JobPrototype>.op_Implicit(jobId);
    this.JobLocalisedName = jobLocalisedName;
    this.RefreshLabel(amount);
    ((Control) this).AddStyleClass("button");
    this._initialised = true;
  }

  public void RefreshLabel(int? amount)
  {
    int? amount1 = this.Amount;
    int? nullable = amount;
    if (amount1.GetValueOrDefault() == nullable.GetValueOrDefault() & amount1.HasValue == nullable.HasValue && this._initialised)
      return;
    this.Amount = amount;
    Label jobLabel = this.JobLabel;
    string str;
    if (!this.Amount.HasValue)
      str = Loc.GetString("late-join-gui-job-slot-uncapped", new (string, object)[1]
      {
        ("jobName", (object) this.JobLocalisedName)
      });
    else
      str = Loc.GetString("late-join-gui-job-slot-capped", new (string, object)[2]
      {
        ("jobName", (object) this.JobLocalisedName),
        (nameof (amount), (object) this.Amount)
      });
    jobLabel.Text = str;
  }
}
