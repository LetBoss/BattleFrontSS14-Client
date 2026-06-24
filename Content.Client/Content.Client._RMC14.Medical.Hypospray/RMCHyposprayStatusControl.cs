using Content.Client.Message;
using Content.Shared._RMC14.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.Medical.Hypospray;

public sealed class RMCHyposprayStatusControl : Control
{
	private readonly Entity<RMCHyposprayComponent> _parent;

	private readonly SharedSolutionContainerSystem _solutionContainers;

	private readonly RichTextLabel _label;

	private readonly SharedContainerSystem _container;

	private EntityUid? PrevVial;

	private FixedPoint2 PrevVolume;

	private FixedPoint2 PrevMaxVolume;

	private FixedPoint2 PrevTransferAmount;

	public RMCHyposprayStatusControl(Entity<RMCHyposprayComponent> parent, SharedSolutionContainerSystem solutionContainers, SharedContainerSystem containers)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		_parent = parent;
		_solutionContainers = solutionContainers;
		_container = containers;
		_label = new RichTextLabel
		{
			StyleClasses = { "ItemStatus" }
		};
		((Control)this).AddChild((Control)(object)_label);
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		BaseContainer val = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<RMCHyposprayComponent>.op_Implicit(_parent), _parent.Comp.SlotId, ref val, (ContainerManagerComponent)null))
		{
			return;
		}
		if (val.ContainedEntities.Count == 0)
		{
			if (!(PrevTransferAmount == _parent.Comp.TransferAmount) || PrevVial.HasValue)
			{
				PrevVial = null;
				PrevTransferAmount = _parent.Comp.TransferAmount;
				_label.SetMarkup(Loc.GetString("rmc-hypospray-label-novial", new(string, object)[1] { ("transferVolume", _parent.Comp.TransferAmount) }));
			}
			return;
		}
		EntityUid val2 = val.ContainedEntities[0];
		if (!_solutionContainers.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(val2), _parent.Comp.VialName, out Entity<SolutionComponent>? _, out Solution solution))
		{
			return;
		}
		if (PrevVolume == solution.Volume && PrevMaxVolume == solution.MaxVolume && PrevTransferAmount == _parent.Comp.TransferAmount)
		{
			EntityUid? prevVial = PrevVial;
			EntityUid val3 = val2;
			if (prevVial.HasValue && prevVial.GetValueOrDefault() == val3)
			{
				return;
			}
		}
		PrevVolume = solution.Volume;
		PrevMaxVolume = solution.MaxVolume;
		PrevTransferAmount = _parent.Comp.TransferAmount;
		PrevVial = val2;
		_label.SetMarkup(Loc.GetString("rmc-hypospray-label", new(string, object)[3]
		{
			("currentVolume", solution.Volume),
			("totalVolume", solution.MaxVolume),
			("transferVolume", _parent.Comp.TransferAmount)
		}));
	}
}
