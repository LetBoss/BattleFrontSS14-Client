// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Admin.RMCAdminEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared._RMC14.Admin;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Strain;
using Content.Shared.Eui;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Prototypes;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Admin;

public sealed class RMCAdminEui : BaseEui
{
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private IPrototypeManager _prototypes;
  private static readonly Comparer<EntityPrototype> EntityComparer = Comparer<EntityPrototype>.Create((Comparison<EntityPrototype>) ((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal)));
  private static readonly Comparer<SpeciesPrototype> SpeciesComparer = Comparer<SpeciesPrototype>.Create((Comparison<SpeciesPrototype>) ((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal)));
  private RMCAdminWindow _adminWindow;
  private RMCCreateHiveWindow? _createHiveWindow;
  private bool _isFirstState = true;

  public override void Opened()
  {
    this._adminWindow = new RMCAdminWindow();
    this._adminWindow.XenoTab.HiveList.OnItemSelected += new Action<ItemList.ItemListSelectedEventArgs>(this.OnHiveSelected);
    ((BaseButton) this._adminWindow.XenoTab.CreateHiveButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnCreateHivePressed);
    SortedSet<SpeciesPrototype> sortedSet1 = new SortedSet<SpeciesPrototype>((IComparer<SpeciesPrototype>) RMCAdminEui.SpeciesComparer);
    foreach (SpeciesPrototype enumeratePrototype in this._prototypes.EnumeratePrototypes<SpeciesPrototype>())
    {
      if (enumeratePrototype.RoundStart)
        sortedSet1.Add(enumeratePrototype);
    }
    RMCTransformRow rmcTransformRow1 = new RMCTransformRow();
    rmcTransformRow1.Label.Text = Loc.GetString("rmc-ui-humanoid");
    ((Control) this._adminWindow.TransformTab.Container).AddChild((Control) rmcTransformRow1);
    int num = 0;
    foreach (SpeciesPrototype speciesPrototype in sortedSet1)
    {
      SpeciesPrototype species = speciesPrototype;
      if (num > 0 && num % 5 == 0)
      {
        ((Control) rmcTransformRow1.Container).Margin = new Thickness();
        rmcTransformRow1 = new RMCTransformRow();
        ((Control) rmcTransformRow1.Label).Visible = false;
        ((Control) rmcTransformRow1.Separator).Visible = false;
        ((Control) this._adminWindow.TransformTab.Container).AddChild((Control) rmcTransformRow1);
      }
      RMCTransformButton rmcTransformButton = new RMCTransformButton()
      {
        Type = TransformType.Humanoid
      };
      rmcTransformButton.TransformName.Text = Loc.GetString(species.Name);
      ((BaseButton) rmcTransformButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((EuiMessageBase) new RMCAdminTransformHumanoidMsg(species.ID)));
      ((Control) rmcTransformRow1.Container).AddChild((Control) rmcTransformButton);
      ++num;
    }
    SortedDictionary<int, SortedSet<EntityPrototype>> sortedDictionary = new SortedDictionary<int, SortedSet<EntityPrototype>>();
    foreach (EntityPrototype enumeratePrototype in this._prototypes.EnumeratePrototypes<EntityPrototype>())
    {
      XenoComponent xenoComponent;
      if (!enumeratePrototype.Abstract && enumeratePrototype.TryGetComponent<XenoComponent>(ref xenoComponent, this._compFactory) && !enumeratePrototype.HasComponent<XenoStrainComponent>(this._compFactory) && !enumeratePrototype.HasComponent<XenoHiddenComponent>(this._compFactory))
      {
        SortedSet<EntityPrototype> sortedSet2;
        if (!sortedDictionary.TryGetValue(xenoComponent.Tier, out sortedSet2))
        {
          sortedSet2 = new SortedSet<EntityPrototype>((IComparer<EntityPrototype>) RMCAdminEui.EntityComparer);
          sortedDictionary.Add(xenoComponent.Tier, sortedSet2);
        }
        sortedSet2.Add(enumeratePrototype);
      }
    }
    foreach ((int key, SortedSet<EntityPrototype> sortedSet3) in sortedDictionary)
    {
      RMCTransformRow rmcTransformRow2 = new RMCTransformRow();
      rmcTransformRow2.Label.Text = Loc.GetString("rmc-ui-tier", new (string, object)[1]
      {
        ("tier", (object) key)
      });
      foreach (EntityPrototype entityPrototype in sortedSet3)
      {
        EntityPrototype xeno = entityPrototype;
        RMCTransformButton rmcTransformButton = new RMCTransformButton()
        {
          Type = TransformType.Xeno
        };
        rmcTransformButton.TransformName.Text = xeno.Name;
        ((Control) rmcTransformRow2.Container).AddChild((Control) rmcTransformButton);
        ((BaseButton) rmcTransformButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((EuiMessageBase) new RMCAdminTransformXenoMsg(EntProtoId.op_Implicit(xeno.ID))));
      }
      ((Control) this._adminWindow.TransformTab.Container).AddChild((Control) rmcTransformRow2);
    }
    this._adminWindow.MarineTab.PointsSpinBox.ValueChanged += (Action<ValueChangedEventArgs>) (args => this.SendMessage((EuiMessageBase) new RMCAdminSetVendorPointsMsg(args.Value)));
    this._adminWindow.MarineTab.SpecialistPointsSpinBox.ValueChanged += (Action<ValueChangedEventArgs>) (args => this.SendMessage((EuiMessageBase) new RMCAdminSetSpecialistVendorPointsMsg(args.Value)));
    ((BaseWindow) this._adminWindow).OpenCentered();
  }

  private void OnHiveSelected(ItemList.ItemListSelectedEventArgs args)
  {
    this.SendMessage((EuiMessageBase) new RMCAdminChangeHiveMsg((Hive) ((ItemList.ItemListEventArgs) args).ItemList[args.ItemIndex].Metadata));
  }

  private void OnCreateHivePressed(BaseButton.ButtonEventArgs args)
  {
    if (this._createHiveWindow != null)
    {
      ((BaseWindow) this._createHiveWindow).RecenterWindow(new Vector2(0.5f, 0.5f));
    }
    else
    {
      this._createHiveWindow = new RMCCreateHiveWindow();
      ((BaseWindow) this._createHiveWindow).OnClose += new Action(this.OnCreateHiveClosed);
      this._createHiveWindow.HiveName.OnTextEntered += new Action<LineEdit.LineEditEventArgs>(this.OnCreateHiveEntered);
      ((BaseWindow) this._createHiveWindow).OpenCentered();
    }
  }

  private void OnCreateHiveClosed()
  {
    ((BaseWindow) this._createHiveWindow)?.Close();
    this._createHiveWindow = (RMCCreateHiveWindow) null;
  }

  private void OnCreateHiveEntered(LineEdit.LineEditEventArgs args)
  {
    this.SendMessage((EuiMessageBase) new RMCAdminCreateHiveMsg(args.Text));
    ((BaseWindow) this._createHiveWindow)?.Close();
  }

  public override void HandleState(EuiStateBase state)
  {
    if (!(state is RMCAdminEuiTargetState adminEuiTargetState))
      return;
    this._adminWindow.XenoTab.HiveList.Clear();
    foreach (Hive hive in adminEuiTargetState.Hives)
    {
      ItemList hiveList = this._adminWindow.XenoTab.HiveList;
      hiveList.Add(new ItemList.Item(hiveList)
      {
        Text = hive.Name,
        Metadata = (object) hive
      });
    }
    ((Control) this._adminWindow.MarineTab.SpecialistSkills).DisposeAllChildren();
    foreach ((_, _) in adminEuiTargetState.SpecialistSkills)
    {
      Button button1 = new Button();
      (string, bool) comp;
      button1.Text = comp.Item1;
      ((BaseButton) button1).ToggleMode = true;
      ((Control) button1).StyleClasses.Add("OpenBoth");
      Button button2 = button1;
      ((BaseButton) button2).Pressed = comp.Item2;
      ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (args =>
      {
        if (args.Button.Pressed)
          this.SendMessage((EuiMessageBase) new RMCAdminAddSpecSkillMsg(comp.Item1));
        else
          this.SendMessage((EuiMessageBase) new RMCAdminRemoveSpecSkillMsg(comp.Item1));
      });
      ((Control) this._adminWindow.MarineTab.SpecialistSkills).AddChild((Control) button2);
    }
    if (this._isFirstState)
    {
      this._adminWindow.MarineTab.PointsSpinBox.OverrideValue(adminEuiTargetState.Points);
      this._adminWindow.MarineTab.SpecialistPointsSpinBox.OverrideValue(adminEuiTargetState.ExtraPoints.GetValueOrDefault<string, int>("Specialist"));
    }
    ((Control) this._adminWindow.MarineTab.Squads).DisposeAllChildren();
    foreach (Squad squad1 in adminEuiTargetState.Squads)
    {
      Squad squad = squad1;
      RMCSquadRow rmcSquadRow1 = new RMCSquadRow();
      ((Control) rmcSquadRow1).HorizontalExpand = true;
      ((Control) rmcSquadRow1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 10f);
      RMCSquadRow rmcSquadRow2 = rmcSquadRow1;
      ((BaseButton) rmcSquadRow2.AddToSquadButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((EuiMessageBase) new RMCAdminAddToSquadMsg(squad.Id)));
      string name = string.Empty;
      Color color = Color.White;
      EntityPrototype entityPrototype;
      if (this._prototypes.TryIndex(squad.Id, ref entityPrototype))
      {
        name = entityPrototype.Name;
        SquadTeamComponent squadTeamComponent;
        if (entityPrototype.TryGetComponent<SquadTeamComponent>(ref squadTeamComponent, this._compFactory))
          color = squadTeamComponent.Color;
      }
      rmcSquadRow2.CreateSquadButton(squad.Exists, (Action) (() => this.SendMessage((EuiMessageBase) new RMCAdminCreateSquadMsg(squad.Id))), new int?(squad.Members), name, color);
      ((Control) this._adminWindow.MarineTab.Squads).AddChild((Control) rmcSquadRow2);
    }
    this._isFirstState = false;
  }

  public override void Closed() => ((BaseWindow) this._adminWindow).Close();
}
