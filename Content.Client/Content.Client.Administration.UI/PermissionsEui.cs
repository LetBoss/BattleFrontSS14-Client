using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client.Administration.Managers;
using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;

namespace Content.Client.Administration.UI;

public sealed class PermissionsEui : BaseEui
{
	private sealed class Menu : DefaultWindow
	{
		private readonly PermissionsEui _ui;

		public readonly GridContainer AdminsList;

		public readonly GridContainer AdminRanksList;

		public readonly Button AddAdminButton;

		public readonly Button AddAdminRankButton;

		protected override Vector2 ContentsMinimumSize => new Vector2(600f, 400f);

		public Menu(PermissionsEui ui)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected O, but got Unknown
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Expected O, but got Unknown
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Expected O, but got Unknown
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Expected O, but got Unknown
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Expected O, but got Unknown
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Expected O, but got Unknown
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Expected O, but got Unknown
			_ui = ui;
			((DefaultWindow)this).Title = Loc.GetString("permissions-eui-menu-title");
			TabContainer val = new TabContainer();
			AddAdminButton = new Button
			{
				Text = Loc.GetString("permissions-eui-menu-add-admin-button"),
				HorizontalAlignment = (HAlignment)3
			};
			AddAdminRankButton = new Button
			{
				Text = Loc.GetString("permissions-eui-menu-add-admin-rank-button"),
				HorizontalAlignment = (HAlignment)3
			};
			AdminsList = new GridContainer
			{
				Columns = 5,
				VerticalExpand = true
			};
			BoxContainer val2 = new BoxContainer
			{
				Orientation = (LayoutOrientation)1
			};
			OrderedChildCollection children = ((Control)val2).Children;
			ScrollContainer val3 = new ScrollContainer
			{
				VerticalExpand = true
			};
			((Control)val3).Children.Add((Control)(object)AdminsList);
			children.Add((Control)val3);
			((Control)val2).Children.Add((Control)(object)AddAdminButton);
			BoxContainer val4 = val2;
			TabContainer.SetTabTitle((Control)(object)val4, Loc.GetString("permissions-eui-menu-admins-tab-title"));
			AdminRanksList = new GridContainer
			{
				Columns = 3,
				VerticalExpand = true
			};
			BoxContainer val5 = new BoxContainer
			{
				Orientation = (LayoutOrientation)1
			};
			OrderedChildCollection children2 = ((Control)val5).Children;
			ScrollContainer val6 = new ScrollContainer
			{
				VerticalExpand = true
			};
			((Control)val6).Children.Add((Control)(object)AdminRanksList);
			children2.Add((Control)val6);
			((Control)val5).Children.Add((Control)(object)AddAdminRankButton);
			BoxContainer val7 = val5;
			TabContainer.SetTabTitle((Control)(object)val7, Loc.GetString("permissions-eui-menu-admin-ranks-tab-title"));
			((Control)val).AddChild((Control)(object)val4);
			((Control)val).AddChild((Control)(object)val7);
			((DefaultWindow)this).Contents.AddChild((Control)(object)val);
		}
	}

	private sealed class EditAdminWindow : DefaultWindow
	{
		public readonly PermissionsEuiState.AdminData? SourceData;

		public readonly LineEdit? NameEdit;

		public readonly LineEdit TitleEdit;

		public readonly OptionButton RankButton;

		public readonly Button SaveButton;

		public readonly Button? RemoveButton;

		public readonly CheckBox SuspendedCheckbox;

		public readonly Dictionary<AdminFlags, (Button inherit, Button sub, Button plus)> FlagButtons = new Dictionary<AdminFlags, (Button, Button, Button)>();

		public unsafe EditAdminWindow(PermissionsEui ui, PermissionsEuiState.AdminData? data)
		{
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Expected O, but got Unknown
			//IL_00c3: Expected O, but got Unknown
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Expected O, but got Unknown
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Expected O, but got Unknown
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Expected O, but got Unknown
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Expected O, but got Unknown
			//IL_0147: Expected O, but got Unknown
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Expected O, but got Unknown
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Expected O, but got Unknown
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Expected O, but got Unknown
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Expected O, but got Unknown
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Expected O, but got Unknown
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Expected O, but got Unknown
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Expected O, but got Unknown
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Expected O, but got Unknown
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Expected O, but got Unknown
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Expected O, but got Unknown
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Expected O, but got Unknown
			((Control)this).MinSize = new Vector2(600f, 400f);
			SourceData = data;
			Control val;
			if (data.HasValue)
			{
				PermissionsEuiState.AdminData valueOrDefault = data.GetValueOrDefault();
				string text = valueOrDefault.UserName ?? ((object)(*(NetUserId*)(&valueOrDefault.UserId))/*cast due to constrained. prefix*/).ToString();
				((DefaultWindow)this).Title = Loc.GetString("permissions-eui-edit-admin-window-edit-admin-label", new(string, object)[1] { ("admin", text) });
				val = (Control)new Label
				{
					Text = text
				};
			}
			else
			{
				((DefaultWindow)this).Title = Loc.GetString("permissions-eui-menu-add-admin-button");
				LineEdit val2 = new LineEdit
				{
					PlaceHolder = Loc.GetString("permissions-eui-edit-admin-window-name-edit-placeholder")
				};
				LineEdit val3 = val2;
				NameEdit = val2;
				val = (Control)(object)val3;
			}
			TitleEdit = new LineEdit
			{
				PlaceHolder = Loc.GetString("permissions-eui-edit-admin-window-title-edit-placeholder")
			};
			RankButton = new OptionButton();
			SaveButton = new Button
			{
				Text = Loc.GetString("permissions-eui-edit-admin-window-save-button"),
				HorizontalAlignment = (HAlignment)3
			};
			SuspendedCheckbox = new CheckBox
			{
				Text = Loc.GetString("permissions-eui-edit-admin-window-suspended"),
				Pressed = (data?.Suspended ?? false)
			};
			RankButton.AddItem(Loc.GetString("permissions-eui-edit-admin-window-no-rank-button"), (int?)(-1));
			foreach (var (value, adminRankData2) in ui._ranks)
			{
				RankButton.AddItem(adminRankData2.Name, (int?)value);
			}
			RankButton.SelectId(data?.RankId ?? (-1));
			RankButton.OnItemSelected += RankSelected;
			GridContainer val4 = new GridContainer
			{
				Columns = 4,
				HSeparationOverride = 0,
				VSeparationOverride = 0
			};
			foreach (AdminFlags allFlag in AdminFlagsHelper.AllFlags)
			{
				bool disabled = !ui._adminManager.HasFlag(allFlag);
				string text2 = allFlag.ToString().ToUpper();
				ButtonGroup val5 = new ButtonGroup(true);
				Button val6 = new Button
				{
					Text = "I",
					StyleClasses = { "OpenRight" },
					Disabled = disabled,
					Group = val5
				};
				Button val7 = new Button
				{
					Text = "-",
					StyleClasses = { "OpenBoth" },
					Disabled = disabled,
					Group = val5
				};
				Button val8 = new Button
				{
					Text = "+",
					StyleClasses = { "OpenLeft" },
					Disabled = disabled,
					Group = val5
				};
				if (data.HasValue)
				{
					PermissionsEuiState.AdminData valueOrDefault2 = data.GetValueOrDefault();
					if ((valueOrDefault2.NegFlags & allFlag) != AdminFlags.None)
					{
						((BaseButton)val7).Pressed = true;
					}
					else if ((valueOrDefault2.PosFlags & allFlag) != AdminFlags.None)
					{
						((BaseButton)val8).Pressed = true;
					}
					else
					{
						((BaseButton)val6).Pressed = true;
					}
				}
				else
				{
					((BaseButton)val6).Pressed = true;
				}
				((Control)val4).AddChild((Control)new Label
				{
					Text = text2
				});
				((Control)val4).AddChild((Control)(object)val6);
				((Control)val4).AddChild((Control)(object)val7);
				((Control)val4).AddChild((Control)(object)val8);
				FlagButtons.Add(allFlag, (val6, val7, val8));
			}
			BoxContainer val9 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0
			};
			if (data.HasValue)
			{
				RemoveButton = new Button
				{
					Text = Loc.GetString("permissions-eui-edit-admin-window-remove-flag-button")
				};
				((Control)val9).AddChild((Control)(object)RemoveButton);
			}
			((Control)val9).AddChild((Control)(object)SaveButton);
			Control contents = ((DefaultWindow)this).Contents;
			BoxContainer val10 = new BoxContainer
			{
				Orientation = (LayoutOrientation)1
			};
			OrderedChildCollection children = ((Control)val10).Children;
			BoxContainer val11 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				SeparationOverride = 2
			};
			OrderedChildCollection children2 = ((Control)val11).Children;
			BoxContainer val12 = new BoxContainer
			{
				Orientation = (LayoutOrientation)1,
				HorizontalExpand = true
			};
			((Control)val12).Children.Add(val);
			((Control)val12).Children.Add((Control)(object)TitleEdit);
			((Control)val12).Children.Add((Control)(object)RankButton);
			((Control)val12).Children.Add((Control)(object)SuspendedCheckbox);
			children2.Add((Control)val12);
			((Control)val11).Children.Add((Control)(object)val4);
			((Control)val11).VerticalExpand = true;
			children.Add((Control)val11);
			((Control)val10).Children.Add((Control)(object)val9);
			contents.AddChild((Control)val10);
		}

		private void RankSelected(ItemSelectedEventArgs obj)
		{
			RankButton.SelectId(obj.Id);
		}

		public void CollectSetFlags(out AdminFlags pos, out AdminFlags neg)
		{
			pos = AdminFlags.None;
			neg = AdminFlags.None;
			foreach (KeyValuePair<AdminFlags, (Button, Button, Button)> flagButton in FlagButtons)
			{
				flagButton.Deconstruct(out var key, out var value);
				(Button, Button, Button) tuple = value;
				AdminFlags adminFlags = key;
				Button item = tuple.Item2;
				Button item2 = tuple.Item3;
				if (((BaseButton)item).Pressed)
				{
					neg |= adminFlags;
				}
				else if (((BaseButton)item2).Pressed)
				{
					pos |= adminFlags;
				}
			}
		}
	}

	private sealed class EditAdminRankWindow : DefaultWindow
	{
		public readonly int? SourceId;

		public readonly LineEdit NameEdit;

		public readonly Button SaveButton;

		public readonly Button? RemoveButton;

		public readonly Dictionary<AdminFlags, CheckBox> FlagCheckBoxes = new Dictionary<AdminFlags, CheckBox>();

		public EditAdminRankWindow(PermissionsEui ui, KeyValuePair<int, PermissionsEuiState.AdminRankData>? data)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Expected O, but got Unknown
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Expected O, but got Unknown
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Expected O, but got Unknown
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Expected O, but got Unknown
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Expected O, but got Unknown
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Expected O, but got Unknown
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Expected O, but got Unknown
			((DefaultWindow)this).Title = Loc.GetString("permissions-eui-edit-admin-rank-window-title");
			((Control)this).MinSize = new Vector2(600f, 400f);
			SourceId = data?.Key;
			NameEdit = new LineEdit
			{
				PlaceHolder = Loc.GetString("permissions-eui-edit-admin-rank-window-name-edit-placeholder")
			};
			if (data.HasValue)
			{
				NameEdit.Text = data.Value.Value.Name;
			}
			SaveButton = new Button
			{
				Text = Loc.GetString("permissions-eui-menu-save-admin-rank-button"),
				HorizontalAlignment = (HAlignment)3,
				HorizontalExpand = true
			};
			BoxContainer val = new BoxContainer
			{
				Orientation = (LayoutOrientation)1
			};
			foreach (AdminFlags allFlag in AdminFlagsHelper.AllFlags)
			{
				bool disabled = !ui._adminManager.HasFlag(allFlag);
				string text = allFlag.ToString().ToUpper();
				CheckBox val2 = new CheckBox
				{
					Disabled = disabled,
					Text = text
				};
				if (data.HasValue && (data.Value.Value.Flags & allFlag) != AdminFlags.None)
				{
					((BaseButton)val2).Pressed = true;
				}
				FlagCheckBoxes.Add(allFlag, val2);
				((Control)val).AddChild((Control)(object)val2);
			}
			BoxContainer val3 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0
			};
			if (data.HasValue)
			{
				RemoveButton = new Button
				{
					Text = Loc.GetString("permissions-eui-menu-remove-admin-rank-button")
				};
				((Control)val3).AddChild((Control)(object)RemoveButton);
			}
			((Control)val3).AddChild((Control)(object)SaveButton);
			Control contents = ((DefaultWindow)this).Contents;
			BoxContainer val4 = new BoxContainer
			{
				Orientation = (LayoutOrientation)1
			};
			((Control)val4).Children.Add((Control)(object)NameEdit);
			((Control)val4).Children.Add((Control)(object)val);
			((Control)val4).Children.Add((Control)(object)val3);
			contents.AddChild((Control)val4);
		}

		public AdminFlags CollectSetFlags()
		{
			AdminFlags adminFlags = AdminFlags.None;
			foreach (var (adminFlags3, val2) in FlagCheckBoxes)
			{
				if (((BaseButton)val2).Pressed)
				{
					adminFlags |= adminFlags3;
				}
			}
			return adminFlags;
		}
	}

	private const int NoRank = -1;

	[Dependency]
	private IClientAdminManager _adminManager;

	private readonly Menu _menu;

	private readonly List<DefaultWindow> _subWindows = new List<DefaultWindow>();

	private Dictionary<int, PermissionsEuiState.AdminRankData> _ranks = new Dictionary<int, PermissionsEuiState.AdminRankData>();

	public PermissionsEui()
	{
		IoCManager.InjectDependencies<PermissionsEui>(this);
		_menu = new Menu(this);
		((BaseButton)_menu.AddAdminButton).OnPressed += AddAdminPressed;
		((BaseButton)_menu.AddAdminRankButton).OnPressed += AddAdminRankPressed;
		((BaseWindow)_menu).OnClose += CloseEverything;
	}

	public override void Closed()
	{
		base.Closed();
		SendMessage(new CloseEuiMessage());
		CloseEverything();
	}

	private void CloseEverything()
	{
		DefaultWindow[] array = _subWindows.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			((BaseWindow)array[i]).Close();
		}
		((BaseWindow)_menu).Close();
	}

	private void AddAdminPressed(ButtonEventArgs obj)
	{
		OpenEditWindow(null);
	}

	private void AddAdminRankPressed(ButtonEventArgs obj)
	{
		OpenRankEditWindow(null);
	}

	private void OnEditPressed(PermissionsEuiState.AdminData admin)
	{
		OpenEditWindow(admin);
	}

	private void OpenEditWindow(PermissionsEuiState.AdminData? data)
	{
		EditAdminWindow window = new EditAdminWindow(this, data);
		((BaseButton)window.SaveButton).OnPressed += delegate
		{
			SaveAdminPressed(window);
		};
		((BaseWindow)window).OpenCentered();
		((BaseWindow)window).OnClose += delegate
		{
			_subWindows.Remove((DefaultWindow)(object)window);
		};
		if (data.HasValue)
		{
			((BaseButton)window.RemoveButton).OnPressed += delegate
			{
				RemoveButtonPressed(window);
			};
		}
		_subWindows.Add((DefaultWindow)(object)window);
	}

	private void OpenRankEditWindow(KeyValuePair<int, PermissionsEuiState.AdminRankData>? rank)
	{
		EditAdminRankWindow window = new EditAdminRankWindow(this, rank);
		((BaseButton)window.SaveButton).OnPressed += delegate
		{
			SaveAdminRankPressed(window);
		};
		((BaseWindow)window).OpenCentered();
		((BaseWindow)window).OnClose += delegate
		{
			_subWindows.Remove((DefaultWindow)(object)window);
		};
		if (rank.HasValue)
		{
			((BaseButton)window.RemoveButton).OnPressed += delegate
			{
				RemoveRankButtonPressed(window);
			};
		}
		_subWindows.Add((DefaultWindow)(object)window);
	}

	private void RemoveButtonPressed(EditAdminWindow window)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		SendMessage(new PermissionsEuiMsg.RemoveAdmin
		{
			UserId = window.SourceData.Value.UserId
		});
		((BaseWindow)window).Close();
	}

	private void RemoveRankButtonPressed(EditAdminRankWindow window)
	{
		SendMessage(new PermissionsEuiMsg.RemoveAdminRank
		{
			Id = window.SourceId.Value
		});
		((BaseWindow)window).Close();
	}

	private void SaveAdminPressed(EditAdminWindow popup)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		popup.CollectSetFlags(out var pos, out var neg);
		int? num = popup.RankButton.SelectedId;
		if (num == -1)
		{
			num = null;
		}
		string title = (string.IsNullOrWhiteSpace(popup.TitleEdit.Text) ? null : popup.TitleEdit.Text);
		bool pressed = ((BaseButton)popup.SuspendedCheckbox).Pressed;
		PermissionsEuiState.AdminData? sourceData = popup.SourceData;
		if (sourceData.HasValue)
		{
			PermissionsEuiState.AdminData valueOrDefault = sourceData.GetValueOrDefault();
			SendMessage(new PermissionsEuiMsg.UpdateAdmin
			{
				UserId = valueOrDefault.UserId,
				Title = title,
				PosFlags = pos,
				NegFlags = neg,
				RankId = num,
				Suspended = pressed
			});
		}
		else
		{
			SendMessage(new PermissionsEuiMsg.AddAdmin
			{
				UserNameOrId = popup.NameEdit.Text,
				Title = title,
				PosFlags = pos,
				NegFlags = neg,
				RankId = num,
				Suspended = pressed
			});
		}
		((BaseWindow)popup).Close();
	}

	private void SaveAdminRankPressed(EditAdminRankWindow popup)
	{
		AdminFlags flags = popup.CollectSetFlags();
		string text = popup.NameEdit.Text;
		int? sourceId = popup.SourceId;
		if (sourceId.HasValue)
		{
			int valueOrDefault = sourceId.GetValueOrDefault();
			SendMessage(new PermissionsEuiMsg.UpdateAdminRank
			{
				Id = valueOrDefault,
				Flags = flags,
				Name = text
			});
		}
		else
		{
			SendMessage(new PermissionsEuiMsg.AddAdminRank
			{
				Flags = flags,
				Name = text
			});
		}
		((BaseWindow)popup).Close();
	}

	public override void Opened()
	{
		((BaseWindow)_menu).OpenCentered();
	}

	public unsafe override void HandleState(EuiStateBase state)
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Expected O, but got Unknown
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Expected O, but got Unknown
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Expected O, but got Unknown
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Expected O, but got Unknown
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Expected O, but got Unknown
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Expected O, but got Unknown
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Expected O, but got Unknown
		PermissionsEuiState permissionsEuiState = (PermissionsEuiState)state;
		if (permissionsEuiState.IsLoading)
		{
			return;
		}
		_ranks = permissionsEuiState.AdminRanks;
		((Control)_menu.AdminsList).RemoveAllChildren();
		foreach (PermissionsEuiState.AdminData admin in permissionsEuiState.Admins.OrderBy((PermissionsEuiState.AdminData d) => d.UserName))
		{
			GridContainer adminsList = _menu.AdminsList;
			string? text = admin.UserName;
			if (text == null)
			{
				NetUserId userId = admin.UserId;
				text = ((object)(*(NetUserId*)(&userId))/*cast due to constrained. prefix*/).ToString();
			}
			string text2 = text;
			((Control)adminsList).AddChild((Control)new Label
			{
				Text = text2
			});
			Label val = new Label
			{
				Text = (admin.Title ?? Loc.GetString("permissions-eui-edit-admin-title-control-text").ToLowerInvariant())
			};
			if (admin.Title == null)
			{
				((Control)val).StyleClasses.Add("Italic");
			}
			((Control)adminsList).AddChild((Control)(object)val);
			AdminFlags adminFlags = admin.PosFlags;
			int? rankId = admin.RankId;
			bool flag;
			string text3;
			if (rankId.HasValue)
			{
				int valueOrDefault = rankId.GetValueOrDefault();
				flag = false;
				PermissionsEuiState.AdminRankData adminRankData = permissionsEuiState.AdminRanks[valueOrDefault];
				text3 = adminRankData.Name;
				adminFlags |= adminRankData.Flags;
			}
			else
			{
				flag = true;
				text3 = Loc.GetString("permissions-eui-edit-no-rank-text").ToLowerInvariant();
			}
			Label val2 = new Label
			{
				Text = text3
			};
			if (flag)
			{
				((Control)val2).StyleClasses.Add("Italic");
			}
			((Control)adminsList).AddChild((Control)(object)val2);
			string text4 = AdminFlagsHelper.PosNegFlagsText(admin.PosFlags, admin.NegFlags);
			((Control)adminsList).AddChild((Control)new Label
			{
				Text = text4,
				HorizontalExpand = true,
				HorizontalAlignment = (HAlignment)2
			});
			Button val3 = new Button
			{
				Text = Loc.GetString("permissions-eui-edit-title-button")
			};
			((BaseButton)val3).OnPressed += delegate
			{
				OnEditPressed(admin);
			};
			((Control)adminsList).AddChild((Control)(object)val3);
			if (!_adminManager.HasFlag(adminFlags))
			{
				((BaseButton)val3).Disabled = true;
				((Control)val3).ToolTip = Loc.GetString("permissions-eui-do-not-have-required-flags-to-edit-admin-tooltip");
			}
		}
		((Control)_menu.AdminRanksList).RemoveAllChildren();
		foreach (KeyValuePair<int, PermissionsEuiState.AdminRankData> kv in permissionsEuiState.AdminRanks)
		{
			PermissionsEuiState.AdminRankData value = kv.Value;
			string text5 = string.Join(' ', from f in AdminFlagsHelper.FlagsToNames(value.Flags)
				select "+" + f);
			((Control)_menu.AdminRanksList).AddChild((Control)new Label
			{
				Text = value.Name
			});
			((Control)_menu.AdminRanksList).AddChild((Control)new Label
			{
				Text = text5,
				HorizontalExpand = true,
				HorizontalAlignment = (HAlignment)2
			});
			Button val4 = new Button
			{
				Text = Loc.GetString("permissions-eui-edit-admin-rank-button")
			};
			((BaseButton)val4).OnPressed += delegate
			{
				OnEditRankPressed(kv);
			};
			((Control)_menu.AdminRanksList).AddChild((Control)(object)val4);
			if (!_adminManager.HasFlag(value.Flags))
			{
				((BaseButton)val4).Disabled = true;
				((Control)val4).ToolTip = Loc.GetString("permissions-eui-do-not-have-required-flags-to-edit-rank-tooltip");
			}
		}
	}

	private void OnEditRankPressed(KeyValuePair<int, PermissionsEuiState.AdminRankData> rank)
	{
		OpenRankEditWindow(rank);
	}
}
