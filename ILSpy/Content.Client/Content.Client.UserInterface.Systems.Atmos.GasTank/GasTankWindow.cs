using System;
using System.Numerics;
using Content.Client.Message;
using Content.Client.Resources;
using Content.Client.Stylesheets;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Timing;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.Atmos.GasTank;

public sealed class GasTankWindow : BaseWindow
{
	[Dependency]
	private IEntityManager _entManager;

	[Dependency]
	private IResourceCache _cache;

	private readonly RichTextLabel _lblPressure;

	private readonly FloatSpinBox _spbPressure;

	private readonly RichTextLabel _lblInternals;

	private readonly Button _btnInternals;

	private readonly Label _topLabel;

	public EntityUid Entity;

	public event Action<float>? OnOutputPressure;

	public event Action? OnToggleInternals;

	public GasTankWindow()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Expected O, but got Unknown
		//IL_00ff: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_0126: Expected O, but got Unknown
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Expected O, but got Unknown
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Expected O, but got Unknown
		//IL_0208: Expected O, but got Unknown
		//IL_020a: Expected O, but got Unknown
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Expected O, but got Unknown
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Expected O, but got Unknown
		//IL_0267: Expected O, but got Unknown
		//IL_0269: Expected O, but got Unknown
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Expected O, but got Unknown
		//IL_02bb: Expected O, but got Unknown
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Expected O, but got Unknown
		//IL_030d: Expected O, but got Unknown
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Expected O, but got Unknown
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Expected O, but got Unknown
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Expected O, but got Unknown
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Expected O, but got Unknown
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Expected O, but got Unknown
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Expected O, but got Unknown
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Expected O, but got Unknown
		IoCManager.InjectDependencies<GasTankWindow>(this);
		LayoutContainer val = new LayoutContainer
		{
			Name = "GasTankRoot"
		};
		((Control)this).AddChild((Control)(object)val);
		((Control)this).MouseFilter = (MouseFilterMode)0;
		Texture texture = _cache.GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
		StyleBoxTexture val2 = new StyleBoxTexture
		{
			Texture = texture,
			Modulate = Color.FromHex((ReadOnlySpan<char>)"#25252A", (Color?)null)
		};
		val2.SetPatchMargin((Margin)15, 10f);
		PanelContainer val3 = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)val2,
			MouseFilter = (MouseFilterMode)1
		};
		LayoutContainer val4 = new LayoutContainer
		{
			Name = "BottomWrap"
		};
		((Control)val).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val4);
		LayoutContainer.SetAnchorPreset((Control)(object)val3, (LayoutPreset)15, false);
		LayoutContainer.SetMarginBottom((Control)(object)val3, -85f);
		LayoutContainer.SetAnchorPreset((Control)(object)val4, (LayoutPreset)13, false);
		LayoutContainer.SetGrowHorizontal((Control)(object)val4, (GrowDirection)2);
		BoxContainer val5 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		OrderedChildCollection children = ((Control)val5).Children;
		BoxContainer val6 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		BoxContainer val7 = val6;
		children.Add((Control)val6);
		((Control)val5).Children.Add(new Control
		{
			MinSize = new Vector2(0f, 110f)
		});
		BoxContainer val8 = val5;
		((Control)val).AddChild((Control)(object)val8);
		LayoutContainer.SetAnchorPreset((Control)(object)val8, (LayoutPreset)15, false);
		Font font = _cache.GetFont("/Fonts/Boxfont-round/Boxfont Round.ttf", 13);
		_topLabel = new Label
		{
			FontOverride = font,
			FontColorOverride = StyleNano.NanoGold,
			VerticalAlignment = (VAlignment)2,
			HorizontalExpand = true,
			HorizontalAlignment = (HAlignment)1,
			Margin = new Thickness(0f, 0f, 20f, 0f)
		};
		BoxContainer val9 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			Margin = new Thickness(4f, 2f, 12f, 2f)
		};
		((Control)val9).Children.Add((Control)(object)_topLabel);
		OrderedChildCollection children2 = ((Control)val9).Children;
		TextureButton val10 = new TextureButton
		{
			StyleClasses = { "windowCloseButton" },
			VerticalAlignment = (VAlignment)2
		};
		TextureButton val11 = val10;
		children2.Add((Control)val10);
		BoxContainer val12 = val9;
		PanelContainer val13 = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#202025", (Color?)null)
			}
		};
		OrderedChildCollection children3 = ((Control)val13).Children;
		BoxContainer val14 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			Margin = new Thickness(8f, 4f)
		};
		Control val15 = (Control)val14;
		children3.Add((Control)val14);
		PanelContainer val16 = val13;
		((Control)val7).AddChild((Control)(object)val12);
		((Control)val7).AddChild((Control)new PanelContainer
		{
			MinSize = new Vector2(0f, 2f),
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#525252ff", (Color?)null)
			}
		});
		((Control)val7).AddChild((Control)(object)val16);
		((Control)val7).AddChild((Control)new PanelContainer
		{
			MinSize = new Vector2(0f, 2f),
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#525252ff", (Color?)null)
			}
		});
		_lblPressure = new RichTextLabel();
		val15.AddChild((Control)(object)_lblPressure);
		_lblInternals = new RichTextLabel
		{
			MinSize = new Vector2(200f, 0f),
			VerticalAlignment = (VAlignment)2
		};
		_btnInternals = new Button
		{
			Text = Loc.GetString("gas-tank-window-internals-toggle-button")
		};
		BoxContainer val17 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			Margin = new Thickness(0f, 7f, 0f, 0f)
		};
		((Control)val17).Children.Add((Control)(object)_lblInternals);
		((Control)val17).Children.Add((Control)(object)_btnInternals);
		val15.AddChild((Control)val17);
		val15.AddChild(new Control
		{
			MinSize = new Vector2(0f, 10f)
		});
		val15.AddChild((Control)new Label
		{
			Text = Loc.GetString("gas-tank-window-output-pressure-label"),
			Align = (AlignMode)1
		});
		_spbPressure = new FloatSpinBox
		{
			IsValid = (float f) => f >= 0f || f <= 3000f,
			Margin = new Thickness(25f, 0f, 25f, 7f)
		};
		val15.AddChild((Control)(object)_spbPressure);
		_spbPressure.OnValueChanged += delegate(FloatSpinBoxEventArgs args)
		{
			this.OnOutputPressure?.Invoke(args.Value);
		};
		((BaseButton)_btnInternals).OnPressed += delegate
		{
			this.OnToggleInternals?.Invoke();
		};
		((BaseButton)val11).OnPressed += delegate
		{
			((BaseWindow)this).Close();
		};
	}

	public void SetTitle(string name)
	{
		_topLabel.Text = name;
	}

	public void UpdateState(GasTankBoundUserInterfaceState state)
	{
		_lblPressure.SetMarkup(Loc.GetString("gas-tank-window-tank-pressure-text", new(string, object)[1] { ("tankPressure", $"{state.TankPressure:0.##}") }));
	}

	public void Update(bool canConnectInternals, bool internalsConnected, float outputPressure)
	{
		((BaseButton)_btnInternals).Disabled = !canConnectInternals;
		_lblInternals.SetMarkup(Loc.GetString("gas-tank-window-internal-text", new(string, object)[1] { ("status", Loc.GetString(internalsConnected ? "gas-tank-window-internal-connected" : "gas-tank-window-internal-disconnected")) }));
		if (!((Control)_spbPressure).HasKeyboardFocus())
		{
			_spbPressure.Value = outputPressure;
		}
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		GasTankComponent item = default(GasTankComponent);
		if (_entManager.TryGetComponent<GasTankComponent>(Entity, ref item))
		{
			bool flag = _entManager.System<SharedGasTankSystem>().CanConnectToInternals(Entity<GasTankComponent>.op_Implicit((Entity, item)));
			((BaseButton)_btnInternals).Disabled = !flag;
		}
		if (!((BaseButton)_btnInternals).Disabled)
		{
			((BaseButton)_btnInternals).Disabled = _entManager.System<UseDelaySystem>().IsDelayed(Entity<UseDelayComponent>.op_Implicit(Entity), "gasTank");
		}
	}

	protected override DragMode GetDragModeFor(Vector2 relativeMousePos)
	{
		return (DragMode)1;
	}

	protected override bool HasPoint(Vector2 point)
	{
		return false;
	}
}
