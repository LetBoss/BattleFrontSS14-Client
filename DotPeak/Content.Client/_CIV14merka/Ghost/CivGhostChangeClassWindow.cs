// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Ghost.CivGhostChangeClassWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Ghost;

public sealed class CivGhostChangeClassWindow : DefaultWindow
{
  private readonly BoxContainer _content;

  public event Action<CivTdmClass>? ClassSelected;

  public event Action? RefreshRequested;

  public CivGhostChangeClassWindow()
  {
    this.Title = "Сменить класс";
    ((Control) this).MinSize = new Vector2(320f, 200f);
    ((BaseWindow) this).Resizable = false;
    BoxContainer boxContainer = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1,
      SeparationOverride = new int?(4)
    };
    Button button1 = new Button();
    button1.Text = "Обновить";
    ((Control) button1).HorizontalAlignment = (Control.HAlignment) 3;
    Button button2 = button1;
    ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action refreshRequested = this.RefreshRequested;
      if (refreshRequested == null)
        return;
      refreshRequested();
    });
    ((Control) boxContainer).AddChild((Control) button2);
    this._content = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1,
      SeparationOverride = new int?(4)
    };
    ((Control) boxContainer).AddChild((Control) this._content);
    this.Contents.AddChild((Control) boxContainer);
  }

  public void Populate(CivRosterStateEvent state)
  {
    ((Control) this._content).RemoveAllChildren();
    int? selectedTeamId = state.SelectedTeamId;
    if (selectedTeamId.HasValue)
    {
      int teamId = selectedTeamId.GetValueOrDefault();
      CivRosterTeamEntry civRosterTeamEntry = state.Teams.FirstOrDefault<CivRosterTeamEntry>((Func<CivRosterTeamEntry, bool>) (t => t.TeamId == teamId));
      if (civRosterTeamEntry == null || civRosterTeamEntry.Squads.Count == 0)
      {
        ((Control) this._content).AddChild((Control) new Label()
        {
          Text = "Нет отрядов"
        });
      }
      else
      {
        CivTdmClass[] civTdmClassArray = new CivTdmClass[4]
        {
          CivTdmClass.Rifleman,
          CivTdmClass.MachineGunner,
          CivTdmClass.Medic,
          CivTdmClass.Specialist
        };
        foreach (CivRosterSquadEntry rosterSquadEntry in (IEnumerable<CivRosterSquadEntry>) civRosterTeamEntry.Squads.OrderBy<CivRosterSquadEntry, int>((Func<CivRosterSquadEntry, int>) (s => s.SquadId)))
        {
          BoxContainer content = this._content;
          Label label = new Label();
          label.Text = $"Отряд #{rosterSquadEntry.SquadId}";
          label.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#aaaaaa", new Color?()));
          ((Control) label).Margin = new Thickness(0.0f, 6f, 0.0f, 0.0f);
          ((Control) content).AddChild((Control) label);
          foreach (CivTdmClass civTdmClass in civTdmClassArray)
          {
            string displayName = CivTdmClassHelper.GetDisplayName(civTdmClass);
            bool flag = true;
            (int Available, int Total) tuple;
            if (rosterSquadEntry.RoleTickets.TryGetValue(civTdmClass, out tuple))
            {
              displayName += $"  {tuple.Available}/{tuple.Total}";
              flag = tuple.Available > 0;
            }
            Button button1 = new Button();
            button1.Text = displayName;
            ((Control) button1).HorizontalExpand = true;
            ((BaseButton) button1).Disabled = !flag;
            Button button2 = button1;
            CivTdmClass captured = civTdmClass;
            ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
            {
              Action<CivTdmClass> classSelected = this.ClassSelected;
              if (classSelected != null)
                classSelected(captured);
              ((BaseWindow) this).Close();
            });
            ((Control) this._content).AddChild((Control) button2);
          }
        }
      }
    }
    else
      ((Control) this._content).AddChild((Control) new Label()
      {
        Text = "Нет команды"
      });
  }
}
