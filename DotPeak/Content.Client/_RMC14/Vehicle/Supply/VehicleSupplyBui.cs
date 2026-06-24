// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Vehicle.Supply.VehicleSupplyBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Vehicle.Ui;
using Content.Shared._RMC14.Vehicle.Supply;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Vehicle.Supply;

public sealed class VehicleSupplyBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private VehicleSupplyWindow? _window;
  private string? _selectedVehicleId;
  private bool _suppressEvents;
  private readonly List<string> _availableVehicleIds = new List<string>();
  private readonly Dictionary<string, int> _availableCounts = new Dictionary<string, int>();
  private readonly Dictionary<string, int> _selectedCopyIndices = new Dictionary<string, int>();
  private readonly Dictionary<string, HardpointButton> _selectButtons = new Dictionary<string, HardpointButton>();
  private readonly Dictionary<string, HardpointButton> _copyToggleButtons = new Dictionary<string, HardpointButton>();
  private readonly Dictionary<string, BoxContainer> _copyContainers = new Dictionary<string, BoxContainer>();
  private readonly Dictionary<string, List<HardpointButton>> _copyButtons = new Dictionary<string, List<HardpointButton>>();
  private readonly HashSet<string> _copyExpanded = new HashSet<string>();

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<VehicleSupplyWindow>((BoundUserInterface) this);
    if (this._window == null)
      return;
    this._window.Title = string.Empty;
    ((BaseButton) this._window.RaiseButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new VehicleSupplyLiftMsg(true)));
    ((BaseButton) this._window.LowerButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new VehicleSupplyLiftMsg(false)));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is VehicleSupplyBuiState state1) || this._window == null)
      return;
    this._suppressEvents = true;
    this.UpdateStatus(state1);
    this.UpdateLists(state1);
    this._window.SetPreview(state1.Preview);
    this._suppressEvents = false;
  }

  private void UpdateStatus(VehicleSupplyBuiState state)
  {
    if (this._window == null)
      return;
    ref VehicleSupplyLiftMode? local = ref state.LiftMode;
    string str1 = (local.HasValue ? local.GetValueOrDefault().ToString() : (string) null) ?? "No lift";
    string str2 = string.IsNullOrWhiteSpace(state.ActiveVehicleId) ? "none" : state.ActiveVehicleId;
    string str3 = state.Busy ? "busy" : "idle";
    this._window.StatusLabel.Text = $"Lift: {str1} | Status: {str3} | Active: {str2}";
    bool flag1 = state.LiftMode.GetValueOrDefault() == VehicleSupplyLiftMode.Raising;
    bool flag2 = state.LiftMode.GetValueOrDefault() == VehicleSupplyLiftMode.Lowering;
    this._window.RaiseButton.Pulse = flag1;
    this._window.LowerButton.Pulse = flag2;
    this._window.SetLiftActivity(state.LiftMode, state.Busy);
  }

  private void UpdateLists(VehicleSupplyBuiState state)
  {
    if (this._window == null)
      return;
    this._availableVehicleIds.Clear();
    this._availableCounts.Clear();
    ((Control) this._window.AvailableRows).DisposeAllChildren();
    this._selectButtons.Clear();
    this._copyToggleButtons.Clear();
    this._copyContainers.Clear();
    this._copyButtons.Clear();
    if (state.Available.Count == 0)
    {
      this._selectedVehicleId = (string) null;
    }
    else
    {
      this._selectedVehicleId = state.SelectedVehicleId;
      bool flag = false;
      if (!string.IsNullOrWhiteSpace(this._selectedVehicleId))
      {
        foreach (VehicleSupplyEntryState supplyEntryState in state.Available)
        {
          if (supplyEntryState.Id == this._selectedVehicleId)
          {
            flag = true;
            break;
          }
        }
      }
      if (!flag && state.Available.Count > 0)
        this._selectedVehicleId = state.Available[0].Id;
      foreach (VehicleSupplyEntryState supplyEntryState in state.Available)
      {
        string str1;
        if (supplyEntryState.Count <= 1)
          str1 = supplyEntryState.Name;
        else
          str1 = $"{supplyEntryState.Name} x{supplyEntryState.Count}";
        string str2 = str1;
        this._availableVehicleIds.Add(supplyEntryState.Id);
        this._availableCounts[supplyEntryState.Id] = supplyEntryState.Count;
        BoxContainer boxContainer1 = new BoxContainer();
        boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
        boxContainer1.SeparationOverride = new int?(6);
        ((Control) boxContainer1).HorizontalExpand = true;
        BoxContainer boxContainer2 = boxContainer1;
        HardpointButton hardpointButton1 = new HardpointButton();
        hardpointButton1.LabelText = str2;
        ((Control) hardpointButton1).HorizontalExpand = true;
        HardpointButton button = hardpointButton1;
        string vehicleId = supplyEntryState.Id;
        ((BaseButton) button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
        {
          if (this._suppressEvents)
            return;
          int num;
          this.SelectVehicle(vehicleId, this._selectedCopyIndices.TryGetValue(vehicleId, out num) ? num : 0);
        });
        VehicleSupplyBui.ApplySelectionStyle(button, this._selectedVehicleId == vehicleId);
        ((Control) boxContainer2).AddChild((Control) button);
        this._selectButtons[vehicleId] = button;
        if (supplyEntryState.Count > 1)
        {
          HardpointButton hardpointButton2 = new HardpointButton();
          hardpointButton2.LabelText = this._copyExpanded.Contains(vehicleId) ? "Copies v" : "Copies >";
          ((Control) hardpointButton2).MinSize = new Vector2(110f, 0.0f);
          HardpointButton hardpointButton3 = hardpointButton2;
          BoxContainer boxContainer3 = new BoxContainer();
          boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
          ((Control) boxContainer3).Margin = new Thickness(12f, 0.0f, 0.0f, 0.0f);
          ((Control) boxContainer3).HorizontalExpand = true;
          ((Control) boxContainer3).Visible = this._copyExpanded.Contains(vehicleId);
          BoxContainer boxContainer4 = boxContainer3;
          for (int index = 0; index < supplyEntryState.Count; ++index)
          {
            int copyIndex = index;
            HardpointButton hardpointButton4 = new HardpointButton();
            hardpointButton4.LabelText = $"    #{index + 1}";
            ((Control) hardpointButton4).HorizontalExpand = true;
            HardpointButton hardpointButton5 = hardpointButton4;
            ((BaseButton) hardpointButton5).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
            {
              if (this._suppressEvents)
                return;
              this._selectedCopyIndices[vehicleId] = copyIndex;
              this.UpdateCopySelection(vehicleId);
              this.SelectVehicle(vehicleId, copyIndex);
            });
            ((Control) boxContainer4).AddChild((Control) hardpointButton5);
            List<HardpointButton> hardpointButtonList;
            if (!this._copyButtons.TryGetValue(vehicleId, out hardpointButtonList))
            {
              hardpointButtonList = new List<HardpointButton>();
              this._copyButtons[vehicleId] = hardpointButtonList;
            }
            hardpointButtonList.Add(hardpointButton5);
          }
          ((Control) boxContainer2).AddChild((Control) hardpointButton3);
          this._copyToggleButtons[vehicleId] = hardpointButton3;
          this._copyContainers[vehicleId] = boxContainer4;
          ((BaseButton) hardpointButton3).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
          {
            if (this._suppressEvents)
              return;
            if (this._copyExpanded.Contains(vehicleId))
              this._copyExpanded.Remove(vehicleId);
            else
              this._copyExpanded.Add(vehicleId);
            this.UpdateCopyExpanded(vehicleId);
          });
        }
        BoxContainer boxContainer5 = new BoxContainer();
        boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 1;
        boxContainer5.SeparationOverride = new int?(2);
        ((Control) boxContainer5).HorizontalExpand = true;
        BoxContainer boxContainer6 = boxContainer5;
        ((Control) boxContainer6).AddChild((Control) boxContainer2);
        BoxContainer boxContainer7;
        if (this._copyContainers.TryGetValue(vehicleId, out boxContainer7))
          ((Control) boxContainer6).AddChild((Control) boxContainer7);
        ((Control) this._window.AvailableRows).AddChild((Control) boxContainer6);
      }
      foreach ((string str, int num1) in this._availableCounts)
      {
        if (num1 > 1)
        {
          if (str == this._selectedVehicleId)
          {
            this._selectedCopyIndices[str] = state.SelectedCopyIndex;
          }
          else
          {
            int num2;
            if (!this._selectedCopyIndices.TryGetValue(str, out num2) || num2 < 0 || num2 >= num1)
              this._selectedCopyIndices[str] = 0;
          }
          this.UpdateCopySelection(str);
          this.UpdateCopyExpanded(str);
        }
      }
    }
  }

  private void SelectVehicle(string vehicleId, int copyIndex)
  {
    if (this._selectedVehicleId == vehicleId)
    {
      this.SendMessage((BoundUserInterfaceMessage) new VehicleSupplySelectMsg(vehicleId, copyIndex));
    }
    else
    {
      this._selectedVehicleId = vehicleId;
      this.UpdateSelectionVisuals();
      this.SendMessage((BoundUserInterfaceMessage) new VehicleSupplySelectMsg(vehicleId, copyIndex));
    }
  }

  private void UpdateSelectionVisuals()
  {
    foreach ((string key, HardpointButton button) in this._selectButtons)
      VehicleSupplyBui.ApplySelectionStyle(button, key == this._selectedVehicleId);
  }

  private void UpdateCopySelection(string vehicleId)
  {
    List<HardpointButton> hardpointButtonList;
    if (!this._copyButtons.TryGetValue(vehicleId, out hardpointButtonList))
      return;
    int num;
    if (!this._selectedCopyIndices.TryGetValue(vehicleId, out num))
      num = 0;
    for (int index = 0; index < hardpointButtonList.Count; ++index)
      VehicleSupplyBui.ApplySelectionStyle(hardpointButtonList[index], index == num);
  }

  private void UpdateCopyExpanded(string vehicleId)
  {
    BoxContainer boxContainer;
    HardpointButton hardpointButton;
    if (!this._copyContainers.TryGetValue(vehicleId, out boxContainer) || !this._copyToggleButtons.TryGetValue(vehicleId, out hardpointButton))
      return;
    bool flag = this._copyExpanded.Contains(vehicleId);
    ((Control) boxContainer).Visible = flag;
    hardpointButton.LabelText = flag ? "Copies v" : "Copies >";
  }

  private static void ApplySelectionStyle(HardpointButton button, bool selected)
  {
    button.Selected = selected;
    button.SelectedColor = HardpointButton.DefaultUnhoveredColor;
    button.UnhoveredColor = Color.FromHex((ReadOnlySpan<char>) "#1A3D5C", new Color?());
    button.HoveredColor = HardpointButton.DefaultHoveredColor;
    button.DisabledColor = HardpointButton.DefaultDisabledColor;
    button.TextColor = selected ? HardpointButton.DefaultTextColor : HardpointButton.DefaultUnselectedTextColor;
    button.DisabledTextColor = HardpointButton.DefaultDisabledTextColor;
    button.RefreshStyle();
  }
}
