using Content.Client.Items.UI;
using Content.Client.Message;
using Content.Shared.FixedPoint;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Client.Tools.UI;

public sealed class WelderStatusControl : PollingItemStatusControl<WelderStatusControl.Data>
{
	public record struct Data(FixedPoint2 Fuel, FixedPoint2 FuelCapacity, bool Lit);

	private readonly Entity<WelderComponent> _parent;

	private readonly IEntityManager _entityManager;

	private readonly SharedToolSystem _toolSystem;

	private readonly RichTextLabel _label;

	public WelderStatusControl(Entity<WelderComponent> parent, IEntityManager entityManager, SharedToolSystem toolSystem)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		_parent = parent;
		_entityManager = entityManager;
		_toolSystem = toolSystem;
		_label = new RichTextLabel
		{
			StyleClasses = { "ItemStatus" }
		};
		((Control)this).AddChild((Control)(object)_label);
		((Control)this).UpdateDraw();
	}

	protected override Data PollData()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		var (fuel, fuelCapacity) = _toolSystem.GetWelderFuelAndCapacity(Entity<WelderComponent>.op_Implicit(_parent), _parent.Comp);
		return new Data(fuel, fuelCapacity, _parent.Comp.Enabled);
	}

	protected override void Update(in Data data)
	{
		_label.SetMarkup(Loc.GetString("welder-component-on-examine-detailed-message", new(string, object)[4]
		{
			("colorName", (data.Fuel < data.FuelCapacity / 4f) ? "darkorange" : "orange"),
			("fuelLeft", data.Fuel),
			("fuelCapacity", data.FuelCapacity),
			("status", Loc.GetString(data.Lit ? "welder-component-on-examine-welder-lit-message" : "welder-component-on-examine-welder-not-lit-message"))
		}));
	}
}
