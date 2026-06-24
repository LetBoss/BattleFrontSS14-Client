using System;
using System.Collections.Immutable;
using Content.Client.Message;
using Content.Shared._RMC14.Dropship.Fabricator;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Dropship.Fabricator;

public sealed class DropshipFabricatorBui : BoundUserInterface
{
	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private IPrototypeManager _prototypes;

	[ViewVariables]
	private DropshipFabricatorWindow? _window;

	private readonly DropshipFabricatorSystem _system;

	public DropshipFabricatorBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<DropshipFabricatorBui>(this);
		_system = base.EntMan.System<DropshipFabricatorSystem>();
	}

	protected override void Open()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Expected O, but got Unknown
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Expected O, but got Unknown
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<DropshipFabricatorWindow>((BoundUserInterface)(object)this);
		_window.EquipmentLabel.SetMarkupPermissive(Loc.GetString("rmc-dropship-fabricator-equipment"));
		_window.AmmoLabel.SetMarkupPermissive(Loc.GetString("rmc-dropship-fabricator-ammo"));
		Refresh();
		ImmutableArray<EntProtoId<DropshipFabricatorPrintableComponent>>.Enumerator enumerator = _system.Printables.GetEnumerator();
		EntityPrototype val = default(EntityPrototype);
		DropshipFabricatorPrintableComponent dropshipFabricatorPrintableComponent = default(DropshipFabricatorPrintableComponent);
		while (enumerator.MoveNext())
		{
			EntProtoId<DropshipFabricatorPrintableComponent> id = enumerator.Current;
			if (_prototypes.TryIndex(EntProtoId<DropshipFabricatorPrintableComponent>.op_Implicit(id), ref val) && id.TryGet(ref dropshipFabricatorPrintableComponent, _prototypes, _compFactory))
			{
				RichTextLabel val2 = new RichTextLabel
				{
					Margin = new Thickness(4f, 2f),
					HorizontalExpand = false
				};
				val2.SetMarkupPermissive(val.Name);
				Button val3 = new Button();
				val3.Text = Loc.GetString("rmc-dropship-fabricator-fabricate", new(string, object)[1] { ("cost", dropshipFabricatorPrintableComponent.Cost) });
				((Control)val3).StyleClasses.Add("OpenBoth");
				((Control)val3).MinWidth = 120f;
				Button val4 = val3;
				((BaseButton)val4).OnPressed += delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipFabricatorPrintMsg(EntProtoId<DropshipFabricatorPrintableComponent>.op_Implicit(id)));
				};
				BoxContainer val5 = new BoxContainer
				{
					Orientation = (LayoutOrientation)0,
					Margin = new Thickness(0f, 4f)
				};
				((Control)val5).Children.Add((Control)(object)val2);
				((Control)val5).Children.Add(new Control
				{
					HorizontalExpand = true
				});
				((Control)val5).Children.Add((Control)(object)val4);
				((Control)val5).HorizontalExpand = true;
				BoxContainer val6 = val5;
				if (dropshipFabricatorPrintableComponent.Category == DropshipFabricatorPrintableComponent.CategoryType.Equipment)
				{
					((Control)_window.EquipmentContainer).AddChild((Control)(object)val6);
				}
				else
				{
					((Control)_window.AmmoContainer).AddChild((Control)(object)val6);
				}
			}
		}
	}

	public void Refresh()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		DropshipFabricatorWindow window = _window;
		DropshipFabricatorComponent dropshipFabricatorComponent = default(DropshipFabricatorComponent);
		if (window != null && !((Control)window).Disposed && base.EntMan.TryGetComponent<DropshipFabricatorComponent>(((BoundUserInterface)this).Owner, ref dropshipFabricatorComponent))
		{
			_window.PointsLabel.Text = Loc.GetString("rmc-dropship-fabricator-points", new(string, object)[1] { ("points", dropshipFabricatorComponent.Points) });
		}
	}
}
