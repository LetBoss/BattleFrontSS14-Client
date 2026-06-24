using System;
using System.Collections.Generic;
using Content.Client.UserInterface.ControlExtensions;
using Content.Shared._RMC14.Telephone;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Client._RMC14.Telephone;

public sealed class RMCTelephoneBui : BoundUserInterface
{
	private static readonly List<string> TabOrder = new List<string> { "MP Dept.", "Almayer", "Command", "Offices", "ARES", "Dropship", "Marine" };

	private TelephoneWindow? _window;

	public RMCTelephoneBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<TelephoneWindow>((BoundUserInterface)(object)this);
		MetaDataComponent val = default(MetaDataComponent);
		if (base.EntMan.TryGetComponent<MetaDataComponent>(((BoundUserInterface)this).Owner, ref val))
		{
			((DefaultWindow)_window).Title = val.EntityName;
		}
		Refresh();
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		Refresh();
	}

	private unsafe void Refresh()
	{
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected O, but got Unknown
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Expected O, but got Unknown
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Expected O, but got Unknown
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Expected O, but got Unknown
		TelephoneWindow window = _window;
		if (window == null || !((BaseWindow)window).IsOpen || !(((BoundUserInterface)this).State is RMCTelephoneBuiState rMCTelephoneBuiState))
		{
			return;
		}
		((Control)_window.Tabs).DisposeAllChildren();
		Dictionary<string, BoxContainer> dictionary = new Dictionary<string, BoxContainer>();
		foreach (RMCPhone phone in rMCTelephoneBuiState.Phones)
		{
			if (!dictionary.TryGetValue(phone.Category, out var value))
			{
				value = new BoxContainer
				{
					Orientation = (LayoutOrientation)1
				};
				dictionary[phone.Category] = value;
				ScrollContainer val = new ScrollContainer
				{
					HScrollEnabled = false,
					VScrollEnabled = true,
					VerticalExpand = true
				};
				BoxContainer val2 = new BoxContainer
				{
					Orientation = (LayoutOrientation)1
				};
				((Control)val).AddChild((Control)(object)val2);
				LineEdit val3 = new LineEdit();
				((Control)value).AddChild((Control)(object)val3);
				((Control)value).AddChild((Control)(object)val);
				val3.OnTextChanged += delegate(LineEditEventArgs args)
				{
					//IL_003b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0040: Unknown result type (might be due to invalid IL or missing references)
					foreach (ScrollContainer item in ((Control)(object)_window.Tabs).GetControlOfType<ScrollContainer>())
					{
						foreach (BoxContainer item2 in ((Control)(object)item).GetControlOfType<BoxContainer>())
						{
							Enumerator enumerator8 = ((Control)item2).Children.GetEnumerator();
							try
							{
								while (((Enumerator)(ref enumerator8)).MoveNext())
								{
									Control current4 = ((Enumerator)(ref enumerator8)).Current;
									LineEdit val12 = (LineEdit)(object)((current4 is LineEdit) ? current4 : null);
									if (val12 != null)
									{
										val12.SetText(args.Text, false);
									}
									else
									{
										Button val13 = (Button)(object)((current4 is Button) ? current4 : null);
										if (val13 != null)
										{
											((Control)val13).Visible = val13.Text?.Contains(args.Text, StringComparison.OrdinalIgnoreCase) ?? false;
										}
									}
								}
							}
							finally
							{
								((IDisposable)(*(Enumerator*)(&enumerator8))/*cast due to constrained. prefix*/).Dispose();
							}
						}
					}
				};
			}
			Enumerator enumerator2 = ((Control)value).Children.GetEnumerator();
			try
			{
				while (((Enumerator)(ref enumerator2)).MoveNext())
				{
					Control current = ((Enumerator)(ref enumerator2)).Current;
					ScrollContainer val4 = (ScrollContainer)(object)((current is ScrollContainer) ? current : null);
					if (val4 == null)
					{
						continue;
					}
					Enumerator enumerator3 = ((Control)val4).Children.GetEnumerator();
					try
					{
						while (((Enumerator)(ref enumerator3)).MoveNext())
						{
							Control current2 = ((Enumerator)(ref enumerator3)).Current;
							BoxContainer val5 = (BoxContainer)(object)((current2 is BoxContainer) ? current2 : null);
							if (val5 != null)
							{
								Button val6 = new Button
								{
									Text = phone.Name,
									StyleClasses = { "OpenBoth" }
								};
								((BaseButton)val6).OnPressed += delegate
								{
									//IL_000c: Unknown result type (might be due to invalid IL or missing references)
									((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCTelephoneCallBuiMsg(phone.Id));
								};
								((Control)val5).AddChild((Control)(object)val6);
								break;
							}
						}
					}
					finally
					{
						((IDisposable)(*(Enumerator*)(&enumerator3))/*cast due to constrained. prefix*/).Dispose();
					}
				}
			}
			finally
			{
				((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
			}
		}
		foreach (string item3 in TabOrder)
		{
			if (dictionary.Remove(item3, out var value2))
			{
				((Control)_window.Tabs).AddChild((Control)(object)value2);
				TabContainer.SetTabTitle((Control)(object)value2, item3);
			}
		}
		foreach (var (text2, val8) in dictionary)
		{
			((Control)_window.Tabs).AddChild((Control)(object)val8);
			TabContainer.SetTabTitle((Control)(object)val8, text2);
		}
		((Control)_window.Buttons).DisposeAllChildren();
		if (rMCTelephoneBuiState.Dnd)
		{
			Button val9 = new Button
			{
				Text = Loc.GetString("phone-dnd-button"),
				StyleClasses = { "OpenBoth" },
				StyleClasses = { "Caution" },
				ToolTip = Loc.GetString("phone-dnd-tooltip-enabled")
			};
			((BaseButton)val9).OnPressed += delegate
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCTelephoneDndBuiMsg(dnd: false));
			};
			((Control)_window.Buttons).AddChild((Control)(object)val9);
		}
		else if (rMCTelephoneBuiState.CanDnd)
		{
			Button val10 = new Button
			{
				Text = Loc.GetString("phone-dnd-button"),
				StyleClasses = { "OpenBoth" },
				ToolTip = Loc.GetString("phone-dnd-tooltip-disabled")
			};
			((BaseButton)val10).OnPressed += delegate
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCTelephoneDndBuiMsg(dnd: true));
			};
			((Control)_window.Buttons).AddChild((Control)(object)val10);
		}
		else
		{
			Button val11 = new Button
			{
				Text = Loc.GetString("phone-dnd-button"),
				StyleClasses = { "OpenBoth" },
				ToolTip = Loc.GetString("phone-dnd-tooltip-locked"),
				Disabled = true
			};
			((Control)_window.Buttons).AddChild((Control)(object)val11);
		}
	}
}
