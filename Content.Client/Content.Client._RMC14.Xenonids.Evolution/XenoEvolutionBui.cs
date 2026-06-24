using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client._RMC14.Xenonids.UI;
using Content.Client.Message;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared._RMC14.Xenonids.Strain;
using Content.Shared.FixedPoint;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Xenonids.Evolution;

public sealed class XenoEvolutionBui : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _prototype;

	private readonly SpriteSystem _sprite;

	[ViewVariables]
	private XenoEvolutionWindow? _window;

	private readonly Dictionary<EntProtoId, XenoChoiceControl> _evolutionControls = new Dictionary<EntProtoId, XenoChoiceControl>();

	private readonly Dictionary<EntProtoId, XenoChoiceControl> _strainControls = new Dictionary<EntProtoId, XenoChoiceControl>();

	public XenoEvolutionBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_sprite = base.EntMan.System<SpriteSystem>();
	}

	protected override void Open()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<XenoEvolutionWindow>((BoundUserInterface)(object)this);
		((Control)_window.OvipositorNeededLabel).Visible = false;
		XenoEvolutionComponent xenoEvolutionComponent = default(XenoEvolutionComponent);
		if (base.EntMan.TryGetComponent<XenoEvolutionComponent>(((BoundUserInterface)this).Owner, ref xenoEvolutionComponent))
		{
			foreach (EntProtoId strain in xenoEvolutionComponent.Strains)
			{
				AddStrain(strain);
			}
		}
		((Control)_window.StrainsLabel).Visible = ((Control)_window.StrainsContainer).ChildCount > 0;
		Refresh();
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		Refresh();
	}

	private void AddEvolution(EntProtoId evolutionId)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		EntityPrototype val = default(EntityPrototype);
		if (!_prototype.TryIndex(evolutionId, ref val))
		{
			return;
		}
		if (!_evolutionControls.TryGetValue(evolutionId, out XenoChoiceControl value))
		{
			value = new XenoChoiceControl();
			value.Set(val.Name, _sprite.Frame0(val));
			((BaseButton)value.Button).Disabled = false;
			((BaseButton)value.Button).OnPressed += delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new XenoEvolveBuiMsg(evolutionId));
				((BoundUserInterface)this).Close();
			};
			_evolutionControls[evolutionId] = value;
			XenoEvolutionWindow? window = _window;
			if (window != null)
			{
				((Control)window.EvolutionsContainer).AddChild((Control)(object)value);
			}
		}
		((Control)value).Visible = true;
		((BaseButton)value.Button).Disabled = false;
	}

	private void AddStrain(EntProtoId strainId)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		XenoEvolutionWindow window = _window;
		EntityPrototype strain = default(EntityPrototype);
		if (window == null || !((BaseWindow)window).IsOpen || !_prototype.TryIndex(strainId, ref strain))
		{
			return;
		}
		if (!_strainControls.TryGetValue(strainId, out XenoChoiceControl value))
		{
			value = new XenoChoiceControl();
			string name = strain.Name;
			string description = null;
			XenoStrainComponent xenoStrainComponent = default(XenoStrainComponent);
			if (strain.TryGetComponent<XenoStrainComponent>(ref xenoStrainComponent, base.EntMan.ComponentFactory))
			{
				name = Loc.GetString(LocId.op_Implicit(xenoStrainComponent.Name)) + " " + name;
				LocId? description2 = xenoStrainComponent.Description;
				description = (description2.HasValue ? LocId.op_Implicit(description2.GetValueOrDefault()) : null);
			}
			value.Set(name, _sprite.Frame0(strain));
			((BaseButton)value.Button).Disabled = false;
			((BaseButton)value.Button).OnPressed += delegate
			{
				XenoStrainConfirmWindow confirmWindow = new XenoStrainConfirmWindow();
				confirmWindow.SetInfo(name, _sprite.Frame0(strain), description);
				confirmWindow.OnConfirm += delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new XenoStrainBuiMsg(strainId));
					((BaseWindow)confirmWindow).Close();
					((BoundUserInterface)this).Close();
				};
				((BaseWindow)confirmWindow).OpenCentered();
			};
			_strainControls[strainId] = value;
			((Control)_window.StrainsContainer).AddChild((Control)(object)value);
		}
		((Control)value).Visible = true;
		((BaseButton)value.Button).Disabled = false;
	}

	public void Refresh()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		XenoEvolutionComponent xenoEvolutionComponent = default(XenoEvolutionComponent);
		if (_window == null || !base.EntMan.TryGetComponent<XenoEvolutionComponent>(((BoundUserInterface)this).Owner, ref xenoEvolutionComponent))
		{
			return;
		}
		((Control)_window.PointsLabel).Visible = xenoEvolutionComponent.Max > FixedPoint2.Zero;
		foreach (XenoChoiceControl value in _evolutionControls.Values)
		{
			((Control)value).Visible = false;
		}
		foreach (EntProtoId evolvesToWithoutPoint in xenoEvolutionComponent.EvolvesToWithoutPoints)
		{
			AddEvolution(evolvesToWithoutPoint);
		}
		if (xenoEvolutionComponent.Points >= xenoEvolutionComponent.Max)
		{
			foreach (EntProtoId item in xenoEvolutionComponent.EvolvesTo)
			{
				AddEvolution(item);
			}
		}
		((Control)_window.Separator).Visible = ((IEnumerable<Control>)((Control)_window.EvolutionsContainer).Children).Any((Control child) => child.Visible) && ((IEnumerable<Control>)((Control)_window.StrainsContainer).Children).Any((Control child) => child.Visible);
		bool flag = ((BoundUserInterface)this).State is XenoEvolveBuiState xenoEvolveBuiState && xenoEvolveBuiState.LackingOvipositor;
		FixedPoint2 points = xenoEvolutionComponent.Points;
		_window.PointsLabel.Text = Loc.GetString("rmc-xeno-ui-evolution-points", new(string, object)[2]
		{
			("points", (int)Math.Floor(points.Double())),
			("maxPoints", xenoEvolutionComponent.Max)
		});
		if (flag && xenoEvolutionComponent.Max > FixedPoint2.Zero)
		{
			if (!((Control)_window.OvipositorNeededLabel).Visible)
			{
				_window.OvipositorNeededLabel.SetMarkupPermissive(Loc.GetString("rmc-xeno-ui-ovi-needed-label"));
				((Control)_window.OvipositorNeededLabel).Visible = true;
			}
		}
		else if (((Control)_window.OvipositorNeededLabel).Visible)
		{
			((Control)_window.OvipositorNeededLabel).Visible = false;
		}
	}
}
