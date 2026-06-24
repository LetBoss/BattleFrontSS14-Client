using Content.Client.Chemistry.Components;
using Content.Client.Items.UI;
using Content.Client.Message;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Client.Chemistry.UI;

public sealed class SolutionStatusControl : PollingItemStatusControl<SolutionStatusControl.Data>
{
	public readonly record struct Data(FixedPoint2 Volume, FixedPoint2 MaxVolume, FixedPoint2? TransferVolume);

	private readonly Entity<SolutionItemStatusComponent> _parent;

	private readonly IEntityManager _entityManager;

	private readonly SharedSolutionContainerSystem _solutionContainers;

	private readonly RichTextLabel _label;

	public SolutionStatusControl(Entity<SolutionItemStatusComponent> parent, IEntityManager entityManager, SharedSolutionContainerSystem solutionContainers)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		_parent = parent;
		_entityManager = entityManager;
		_solutionContainers = solutionContainers;
		_label = new RichTextLabel
		{
			StyleClasses = { "ItemStatus" }
		};
		((Control)this).AddChild((Control)(object)_label);
	}

	protected override Data PollData()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!_solutionContainers.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(_parent.Owner), _parent.Comp.Solution, out Entity<SolutionComponent>? _, out Solution solution))
		{
			return default(Data);
		}
		FixedPoint2? transferVolume = null;
		SolutionTransferComponent solutionTransferComponent = default(SolutionTransferComponent);
		if (_entityManager.TryGetComponent<SolutionTransferComponent>(_parent.Owner, ref solutionTransferComponent))
		{
			transferVolume = solutionTransferComponent.TransferAmount;
		}
		return new Data(solution.Volume, solution.MaxVolume, transferVolume);
	}

	protected override void Update(in Data data)
	{
		string text = Loc.GetString("solution-status-volume", new(string, object)[2]
		{
			("currentVolume", data.Volume),
			("maxVolume", data.MaxVolume)
		});
		FixedPoint2? transferVolume = data.TransferVolume;
		if (transferVolume.HasValue)
		{
			FixedPoint2 valueOrDefault = transferVolume.GetValueOrDefault();
			text = text + "\n" + Loc.GetString("solution-status-transfer", new(string, object)[1] { ("volume", valueOrDefault) });
		}
		_label.SetMarkup(text);
	}
}
