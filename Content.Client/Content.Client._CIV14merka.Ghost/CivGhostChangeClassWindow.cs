using System;
using System.Linq;
using System.Numerics;
using Content.Shared._CIV14merka;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Maths;

namespace Content.Client._CIV14merka.Ghost;

public sealed class CivGhostChangeClassWindow : DefaultWindow
{
	private readonly BoxContainer _content;

	public event Action<CivTdmClass>? ClassSelected;

	public event Action? RefreshRequested;

	public CivGhostChangeClassWindow()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		((DefaultWindow)this).Title = "Сменить класс";
		((Control)this).MinSize = new Vector2(320f, 200f);
		((BaseWindow)this).Resizable = false;
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 4
		};
		Button val2 = new Button
		{
			Text = "Обновить",
			HorizontalAlignment = (HAlignment)3
		};
		((BaseButton)val2).OnPressed += delegate
		{
			this.RefreshRequested?.Invoke();
		};
		((Control)val).AddChild((Control)(object)val2);
		_content = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 4
		};
		((Control)val).AddChild((Control)(object)_content);
		((DefaultWindow)this).Contents.AddChild((Control)(object)val);
	}

	public void Populate(CivRosterStateEvent state)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Expected O, but got Unknown
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Expected O, but got Unknown
		((Control)_content).RemoveAllChildren();
		int? selectedTeamId = state.SelectedTeamId;
		if (selectedTeamId.HasValue)
		{
			int teamId = selectedTeamId.GetValueOrDefault();
			CivRosterTeamEntry civRosterTeamEntry = state.Teams.FirstOrDefault((CivRosterTeamEntry t) => t.TeamId == teamId);
			if (civRosterTeamEntry != null && civRosterTeamEntry.Squads.Count != 0)
			{
				CivTdmClass[] array = new CivTdmClass[4]
				{
					CivTdmClass.Rifleman,
					CivTdmClass.MachineGunner,
					CivTdmClass.Medic,
					CivTdmClass.Specialist
				};
				{
					foreach (CivRosterSquadEntry item in civRosterTeamEntry.Squads.OrderBy((CivRosterSquadEntry s) => s.SquadId))
					{
						((Control)_content).AddChild((Control)new Label
						{
							Text = $"Отряд #{item.SquadId}",
							FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#aaaaaa", (Color?)null),
							Margin = new Thickness(0f, 6f, 0f, 0f)
						});
						CivTdmClass[] array2 = array;
						foreach (CivTdmClass civTdmClass in array2)
						{
							string text = CivTdmClassHelper.GetDisplayName(civTdmClass);
							bool flag = true;
							if (item.RoleTickets.TryGetValue(civTdmClass, out (int, int) value))
							{
								text += $"  {value.Item1}/{value.Item2}";
								flag = value.Item1 > 0;
							}
							Button val = new Button
							{
								Text = text,
								HorizontalExpand = true,
								Disabled = !flag
							};
							CivTdmClass captured = civTdmClass;
							((BaseButton)val).OnPressed += delegate
							{
								this.ClassSelected?.Invoke(captured);
								((BaseWindow)this).Close();
							};
							((Control)_content).AddChild((Control)(object)val);
						}
					}
					return;
				}
			}
			((Control)_content).AddChild((Control)new Label
			{
				Text = "Нет отрядов"
			});
		}
		else
		{
			((Control)_content).AddChild((Control)new Label
			{
				Text = "Нет команды"
			});
		}
	}
}
