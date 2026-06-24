using Content.Client.Message;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Client.Chemistry.UI;

public sealed class InjectorStatusControl : Control
{
	private readonly Entity<InjectorComponent> _parent;

	private readonly SharedSolutionContainerSystem _solutionContainers;

	private readonly RichTextLabel _label;

	private FixedPoint2 PrevVolume;

	private FixedPoint2 PrevMaxVolume;

	private FixedPoint2 PrevTransferAmount;

	private InjectorToggleMode PrevToggleState;

	public InjectorStatusControl(Entity<InjectorComponent> parent, SharedSolutionContainerSystem solutionContainers)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		_parent = parent;
		_solutionContainers = solutionContainers;
		_label = new RichTextLabel
		{
			StyleClasses = { "ItemStatus" }
		};
		((Control)this).AddChild((Control)(object)_label);
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		if (_solutionContainers.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(_parent.Owner), _parent.Comp.SolutionName, out Entity<SolutionComponent>? _, out Solution solution) && (!(PrevVolume == solution.Volume) || !(PrevMaxVolume == solution.MaxVolume) || !(PrevTransferAmount == _parent.Comp.TransferAmount) || PrevToggleState != _parent.Comp.ToggleState))
		{
			PrevVolume = solution.Volume;
			PrevMaxVolume = solution.MaxVolume;
			PrevTransferAmount = _parent.Comp.TransferAmount;
			PrevToggleState = _parent.Comp.ToggleState;
			string item = Loc.GetString(_parent.Comp.ToggleState switch
			{
				InjectorToggleMode.Draw => "injector-draw-text", 
				InjectorToggleMode.Inject => "injector-inject-text", 
				_ => "injector-invalid-injector-toggle-mode", 
			});
			_label.SetMarkup(Loc.GetString("injector-volume-label", new(string, object)[4]
			{
				("currentVolume", solution.Volume),
				("totalVolume", solution.MaxVolume),
				("modeString", item),
				("transferVolume", _parent.Comp.TransferAmount)
			}));
		}
	}
}
