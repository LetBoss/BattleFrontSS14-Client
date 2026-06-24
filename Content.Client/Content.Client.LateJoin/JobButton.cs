using Content.Shared.Roles;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Client.LateJoin;

internal sealed class JobButton : ContainerButton
{
	private bool _initialised;

	public Label JobLabel { get; }

	public string JobId { get; }

	public string JobLocalisedName { get; }

	public int? Amount { get; private set; }

	public JobButton(Label jobLabel, ProtoId<JobPrototype> jobId, string jobLocalisedName, int? amount)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		JobLabel = jobLabel;
		JobId = ProtoId<JobPrototype>.op_Implicit(jobId);
		JobLocalisedName = jobLocalisedName;
		RefreshLabel(amount);
		((Control)this).AddStyleClass("button");
		_initialised = true;
	}

	public void RefreshLabel(int? amount)
	{
		if (Amount != amount || !_initialised)
		{
			Amount = amount;
			JobLabel.Text = (Amount.HasValue ? Loc.GetString("late-join-gui-job-slot-capped", new(string, object)[2]
			{
				("jobName", JobLocalisedName),
				("amount", Amount)
			}) : Loc.GetString("late-join-gui-job-slot-uncapped", new(string, object)[1] { ("jobName", JobLocalisedName) }));
		}
	}
}
