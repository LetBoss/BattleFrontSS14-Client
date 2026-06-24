// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Chemistry.Master.RMCChemMasterBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.UserInterface;
using Content.Client.Chemistry.Containers.EntitySystems;
using Content.Shared._RMC14.Chemistry.ChemMaster;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared._RMC14.Extensions;
using Content.Shared._RMC14.IconLabel;
using Content.Shared._RMC14.UserInterface;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.FixedPoint;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._RMC14.Chemistry.Master;

public sealed class RMCChemMasterBui : BoundUserInterface, IRefreshableBui
{
  [Dependency]
  private ILocalizationManager _localization;
  [Dependency]
  private RMCReagentSystem _reagent;
  private readonly ContainerSystem _container;
  private readonly ItemSlotsSystem _itemSlots;
  private readonly SolutionContainerSystem _solution;
  private readonly SpriteSystem _sprite;
  private RMCChemMasterWindow? _window;
  private RMCChemMasterPopupWindow? _colorPopup;
  private FixedPoint2? _lastBottleAmount;
  private readonly List<EntityUid> _lastPillBottleRows = new List<EntityUid>();
  private readonly Dictionary<EntityUid, RMCChemMasterPillBottleRow> _bottleRows = new Dictionary<EntityUid, RMCChemMasterPillBottleRow>();
  private readonly Dictionary<int, RMCChemMasterReagentRow> _beakerRows = new Dictionary<int, RMCChemMasterReagentRow>();
  private readonly Dictionary<int, RMCChemMasterReagentRow> _bufferRows = new Dictionary<int, RMCChemMasterReagentRow>();
  private readonly List<int> _toRemove = new List<int>();

  public RMCChemMasterBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._container = this.EntMan.System<ContainerSystem>();
    this._itemSlots = this.EntMan.System<ItemSlotsSystem>();
    this._solution = this.EntMan.System<SolutionContainerSystem>();
    this._sprite = this.EntMan.System<SpriteSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<RMCChemMasterWindow>((BoundUserInterface) this);
    MetaDataComponent metaDataComponent;
    if (this.EntMan.TryGetComponent<MetaDataComponent>(this.Owner, ref metaDataComponent))
      this._window.Title = metaDataComponent.EntityName;
    RMCChemMasterComponent chemMaster;
    if (!this.EntMan.TryGetComponent<RMCChemMasterComponent>(this.Owner, ref chemMaster))
      return;
    ((BaseButton) this._window.BeakerEjectButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterBeakerEjectMsg()));
    ((BaseButton) this._window.BeakerAllButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterBeakerTransferAllMsg()));
    ((BaseButton) this._window.BufferModeButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterBufferModeMsg(chemMaster.BufferTransferMode.NextWrap<RMCChemMasterBufferMode>())));
    ((BaseButton) this._window.BufferAllButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterBufferTransferAllMsg()));
    FloatSpinBox dialSpinBox = UIExtensions.CreateDialSpinBox(buttons: false, minWidth: 10);
    dialSpinBox.OnValueChanged += (Action<FloatSpinBox.FloatSpinBoxEventArgs>) (args => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterSetPillAmountMsg((int) args.Value)));
    this._window.PillAmountContainer.AddChild((Control) dialSpinBox);
    ButtonGroup buttonGroup = new ButtonGroup(true);
    for (int index = 0; index < chemMaster.PillTypes; ++index)
    {
      RMCChemMasterPillButton masterPillButton = new RMCChemMasterPillButton();
      ((BaseButton) masterPillButton.Button).Group = buttonGroup;
      int type = index + 1;
      SpriteSpecifier.Rsi rsi = new SpriteSpecifier.Rsi(chemMaster.PillRsi.RsiPath, $"pill{type}");
      masterPillButton.Texture.Texture = this._sprite.Frame0((SpriteSpecifier) rsi);
      ((BaseButton) masterPillButton.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterSetPillTypeMsg((uint) type)));
      ((Control) this._window.PillTypeContainer).AddChild((Control) masterPillButton);
    }
    ((BaseButton) this._window.CreatePillsButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterCreatePillsMsg()));
    this.Refresh();
  }

  public void Refresh()
  {
    RMCChemMasterComponent chemMasterComponent;
    if (this._window == null || !this.EntMan.TryGetComponent<RMCChemMasterComponent>(this.Owner, ref chemMasterComponent))
      return;
    FixedPoint2? lastBottleAmount = this._lastBottleAmount;
    FixedPoint2 bottleSize = chemMasterComponent.BottleSize;
    if ((lastBottleAmount.HasValue ? (lastBottleAmount.GetValueOrDefault() != bottleSize ? 1 : 0) : 1) != 0)
    {
      this._lastBottleAmount = new FixedPoint2?(chemMasterComponent.BottleSize);
      this._window.CreateBottleButton.Text = Loc.GetString("rmc-chem-master-create-bottle", new (string, object)[1]
      {
        ("amount", (object) chemMasterComponent.BottleSize)
      });
    }
    this.UpdateBeaker(Entity<RMCChemMasterComponent>.op_Implicit((this.Owner, chemMasterComponent)));
    this.UpdatePillBottles(Entity<RMCChemMasterComponent>.op_Implicit((this.Owner, chemMasterComponent)));
    this.UpdateBuffer(Entity<RMCChemMasterComponent>.op_Implicit((this.Owner, chemMasterComponent)));
    int num = (int) chemMasterComponent.SelectedType - 1;
    if ((long) chemMasterComponent.SelectedType < (long) ((Control) this._window.PillTypeContainer).ChildCount && ((Control) this._window.PillTypeContainer).GetChild(num) is RMCChemMasterPillButton child1)
      ((BaseButton) child1.Button).Pressed = true;
    foreach (Control child2 in this._window.PillAmountContainer.Children)
    {
      if (child2 is FloatSpinBox floatSpinBox)
        floatSpinBox.Value = (float) chemMasterComponent.PillAmount;
    }
    ((BaseButton) this._window.CreatePillsButton).Disabled = chemMasterComponent.SelectedBottles.Count <= 0;
  }

  private void UpdateBeaker(Entity<RMCChemMasterComponent> chemMaster)
  {
    if (this._window == null)
      return;
    ItemSlot itemSlot;
    if (this._itemSlots.TryGetSlot(this.Owner, chemMaster.Comp.BeakerSlot, out itemSlot))
    {
      EntityUid? containedEntity = (EntityUid?) itemSlot.ContainerSlot?.ContainedEntity;
      if (containedEntity.HasValue)
      {
        EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
        FitsInDispenserComponent dispenserComponent;
        Entity<SolutionComponent>? entity;
        if (this.EntMan.TryGetComponent<FitsInDispenserComponent>(valueOrDefault, ref dispenserComponent) && this._solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(valueOrDefault), dispenserComponent.Solution, out entity))
        {
          this._window.BeakerLabel.Text = Loc.GetString("rmc-chem-master-beaker-amount", new (string, object)[1]
          {
            ("amount", (object) entity.Value.Comp.Solution.Volume)
          });
          bool flag = entity.Value.Comp.Solution.Volume > FixedPoint2.Zero;
          ((Control) this._window.BeakerEmptyLabel).Visible = !flag;
          ((Control) this._window.BeakerAllButton).Visible = flag;
          for (int index = 0; index < entity.Value.Comp.Solution.Contents.Count; ++index)
          {
            ReagentQuantity content = entity.Value.Comp.Solution.Contents[index];
            RMCChemMasterReagentRow reagentRow;
            if (!this._beakerRows.TryGetValue(index, out reagentRow))
            {
              reagentRow = this.CreateReagentRow(Entity<RMCChemMasterComponent>.op_Implicit(chemMaster), content, (Action<FixedPoint2>) (setting => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterBeakerTransferMsg(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), setting))));
              this._beakerRows[index] = reagentRow;
              ((Control) this._window.BeakerContentsContainer).AddChild((Control) reagentRow);
            }
            this.UpdateReagentRow(reagentRow, content, (Action<FixedPoint2>) (setting => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterBeakerTransferMsg(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), setting))));
            reagentRow.AllButton.ClearOnPressed();
            reagentRow.AllButton.OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterBeakerTransferMsg(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), content.Quantity)));
            reagentRow.OnSubmit = (Action<LineEdit.LineEditEventArgs>) null;
            reagentRow.OnSubmit += (Action<LineEdit.LineEditEventArgs>) (args =>
            {
              double result;
              if (!double.TryParse(args.Text, out result))
                return;
              this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterBeakerTransferMsg(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), FixedPoint2.New(result)));
            });
          }
          this._toRemove.Clear();
          foreach ((int key, RMCChemMasterReagentRow masterReagentRow) in this._beakerRows)
          {
            if (key >= entity.Value.Comp.Solution.Contents.Count)
            {
              masterReagentRow.Orphan();
              this._toRemove.Add(key);
            }
          }
          using (List<int>.Enumerator enumerator = this._toRemove.GetEnumerator())
          {
            while (enumerator.MoveNext())
              this._beakerRows.Remove(enumerator.Current);
            return;
          }
        }
      }
    }
    this._window.BeakerLabel.Text = Loc.GetString("rmc-chem-master-beaker-none");
    ((Control) this._window.BeakerEmptyLabel).Visible = true;
    ((Control) this._window.BeakerAllButton).Visible = false;
    ((Control) this._window.BeakerContentsContainer).RemoveAllChildren();
    this._beakerRows.Clear();
  }

  private void UpdatePillBottles(Entity<RMCChemMasterComponent> chemMaster)
  {
    if (this._window == null)
      return;
    bool flag = chemMaster.Comp.SelectedBottles.Count > 0;
    BaseContainer baseContainer;
    if (((SharedContainerSystem) this._container).TryGetContainer(this.Owner, chemMaster.Comp.PillBottleContainer, ref baseContainer, (ContainerManagerComponent) null) && baseContainer.ContainedEntities.Count > 0)
    {
      ((Control) this._window.PillBottleColumnLabel).Margin = new Thickness(0.0f, 3f, 5f, 0.0f);
      ((Control) this._window.PillBottlesNoneLabel).Visible = false;
      foreach (Control child in ((Control) this._window.PillBottlesContainer).Children)
      {
        if (child is RMCChemMasterPillBottleRow masterPillBottleRow)
        {
          ((Control) masterPillBottleRow.LabelInput).Visible = flag;
          ((BaseButton) masterPillBottleRow.ColorButton).Disabled = !flag;
          break;
        }
      }
      if (this._lastPillBottleRows.SequenceEqual<EntityUid>((IEnumerable<EntityUid>) baseContainer.ContainedEntities))
      {
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) baseContainer.ContainedEntities)
        {
          RMCChemMasterPillBottleRow row;
          if (this._bottleRows.TryGetValue(containedEntity, out row))
          {
            this.UpdatePillBottleFill(row, containedEntity);
            this.UpdatePillBottleName(row, containedEntity);
          }
        }
      }
      else
      {
        this._lastPillBottleRows.Clear();
        this._lastPillBottleRows.AddRange((IEnumerable<EntityUid>) baseContainer.ContainedEntities);
        ((Control) this._window.PillBottlesContainer).RemoveChildExcept((Control) this._window.PillBottlesNoneLabel);
        this._bottleRows.Clear();
        for (int index1 = 0; index1 < baseContainer.ContainedEntities.Count; ++index1)
        {
          EntityUid containedEntity = baseContainer.ContainedEntities[index1];
          NetEntity? netContained;
          if (this.EntMan.TryGetNetEntity(containedEntity, ref netContained, (MetaDataComponent) null))
          {
            RMCChemMasterPillBottleRow row = new RMCChemMasterPillBottleRow();
            HashSet<EntityUid> selectedBottles = chemMaster.Comp.SelectedBottles;
            ((BaseButton) row.FillBottleButton).Pressed = selectedBottles.Contains(containedEntity);
            ((BaseButton) row.FillBottleButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (args => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterPillBottleFillMsg(netContained.Value, args.Button.Pressed)));
            this.UpdatePillBottleFill(row, containedEntity);
            this.UpdatePillBottleName(row, containedEntity);
            if (index1 == 0)
            {
              row.LabelInput.OnTextEntered += new Action<LineEdit.LineEditEventArgs>(this.OnLabelInputChanged);
              row.LabelInput.OnFocusExit += new Action<LineEdit.LineEditEventArgs>(this.OnLabelInputChanged);
              ((BaseButton) row.ColorButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_1 =>
              {
                if (this._colorPopup != null)
                {
                  ((BaseWindow) this._colorPopup).OpenCentered();
                }
                else
                {
                  this._colorPopup = new RMCChemMasterPopupWindow()
                  {
                    Title = Loc.GetString("rmc-chem-master-pill-bottle-window-title")
                  };
                  ((BaseWindow) this._colorPopup).OnClose += (Action) (() => this._colorPopup = (RMCChemMasterPopupWindow) null);
                  ((BaseWindow) this._colorPopup).OpenCentered();
                  for (int index2 = 0; index2 < chemMaster.Comp.PillCanisterTypes; ++index2)
                  {
                    RSI.State state = this._sprite.GetState(new SpriteSpecifier.Rsi(chemMaster.Comp.PillCanisterRsi, $"pill_canister{index2}"));
                    TextureButton textureButton = new TextureButton()
                    {
                      TextureNormal = state.Frame0
                    };
                    int type = index2;
                    ((BaseButton) textureButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_2 =>
                    {
                      this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterPillBottleColorMsg((RMCPillBottleColors) type));
                      ((BaseWindow) this._colorPopup).Close();
                    });
                    ((Control) this._colorPopup.Grid).AddChild((Control) textureButton);
                  }
                }
              });
              if (!flag)
              {
                ((Control) row.LabelInput).Visible = false;
                ((BaseButton) row.ColorButton).Disabled = true;
              }
              Control parent = ((Control) row.ColorView).Parent;
              if (parent != null)
                parent.Margin = new Thickness();
              ((Control) row.ColorView).Orphan();
              ((Control) row.ColorButton).AddChild((Control) row.ColorView);
            }
            else
            {
              ((Control) row.LabelInput).Visible = false;
              ((Control) row.ColorButton).Visible = false;
            }
            row.ColorView.SetEntity(new EntityUid?(containedEntity));
            ((BaseButton) row.TransferButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterPillBottleTransferMsg(netContained.Value)));
            ((BaseButton) row.EjectButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterPillBottleEjectMsg(netContained.Value)));
            ((Control) this._window.PillBottlesContainer).AddChild((Control) row);
            this._bottleRows[containedEntity] = row;
          }
        }
      }
    }
    else
    {
      this._lastPillBottleRows.Clear();
      ((Control) this._window.PillBottleColumnLabel).Margin = new Thickness(0.0f, 0.0f, 5f, 0.0f);
      ((Control) this._window.PillBottlesContainer).RemoveChildExcept((Control) this._window.PillBottlesNoneLabel);
      ((Control) this._window.PillBottlesNoneLabel).Visible = true;
    }
  }

  private void UpdateBuffer(Entity<RMCChemMasterComponent> chemMaster)
  {
    if (this._window == null)
      return;
    Button bufferModeButton = this._window.BufferModeButton;
    string text;
    switch (chemMaster.Comp.BufferTransferMode)
    {
      case RMCChemMasterBufferMode.ToBeaker:
        text = Loc.GetString("rmc-chem-master-buffer-to-beaker");
        break;
      case RMCChemMasterBufferMode.ToDisposal:
        text = Loc.GetString("rmc-chem-master-buffer-to-disposal");
        break;
      default:
        text = this._window.BufferModeButton.Text;
        break;
    }
    bufferModeButton.Text = text;
    Entity<SolutionComponent>? entity;
    if (!this._solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(this.Owner), chemMaster.Comp.BufferSolutionId, out entity) || entity.Value.Comp.Solution.Volume <= FixedPoint2.Zero)
    {
      ((Control) this._window.BufferEmptyLabel).Visible = true;
      ((Control) this._window.BufferAllButton).Visible = false;
      ((Control) this._window.BufferContainer).RemoveAllChildren();
      this._bufferRows.Clear();
    }
    else
    {
      ((Control) this._window.BufferEmptyLabel).Visible = false;
      ((Control) this._window.BufferAllButton).Visible = true;
      for (int index = 0; index < entity.Value.Comp.Solution.Contents.Count; ++index)
      {
        ReagentQuantity content = entity.Value.Comp.Solution.Contents[index];
        RMCChemMasterReagentRow reagentRow;
        if (!this._bufferRows.TryGetValue(index, out reagentRow))
        {
          reagentRow = this.CreateReagentRow(Entity<RMCChemMasterComponent>.op_Implicit(chemMaster), content, (Action<FixedPoint2>) (setting => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterBufferTransferMsg(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), setting))));
          this._bufferRows[index] = reagentRow;
          ((Control) this._window.BufferContainer).AddChild((Control) reagentRow);
        }
        this.UpdateReagentRow(reagentRow, content, (Action<FixedPoint2>) (setting => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterBufferTransferMsg(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), setting))));
        reagentRow.AllButton.ClearOnPressed();
        reagentRow.AllButton.OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterBufferTransferMsg(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), content.Quantity)));
        reagentRow.OnSubmit = (Action<LineEdit.LineEditEventArgs>) null;
        reagentRow.OnSubmit += (Action<LineEdit.LineEditEventArgs>) (args =>
        {
          double result;
          if (!double.TryParse(args.Text, out result))
            return;
          this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterBufferTransferMsg(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), FixedPoint2.New(result)));
        });
      }
      this._toRemove.Clear();
      foreach ((int key, RMCChemMasterReagentRow masterReagentRow) in this._bufferRows)
      {
        if (key >= entity.Value.Comp.Solution.Contents.Count)
        {
          masterReagentRow.Orphan();
          this._toRemove.Add(key);
        }
      }
      foreach (int key in this._toRemove)
        this._bufferRows.Remove(key);
    }
  }

  private RMCChemMasterReagentRow CreateReagentRow(
    RMCChemMasterComponent chemMaster,
    ReagentQuantity reagent,
    Action<FixedPoint2> onTransfer)
  {
    RMCChemMasterReagentRow row = new RMCChemMasterReagentRow();
    foreach (FixedPoint2 transferSetting in chemMaster.TransferSettings)
    {
      RMCChemMasterTransferButton masterTransferButton = new RMCChemMasterTransferButton()
      {
        Amount = transferSetting
      };
      ((Control) row.TransferSettingsContainer).AddChild((Control) masterTransferButton);
    }
    this.UpdateReagentRow(row, reagent, onTransfer);
    return row;
  }

  private void UpdateReagentRow(
    RMCChemMasterReagentRow row,
    ReagentQuantity reagent,
    Action<FixedPoint2> onTransfer)
  {
    string str = reagent.Reagent.Prototype;
    Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent1;
    if (this._reagent.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(str), out reagent1))
      str = reagent1.LocalizedName;
    row.ReagentLabel.Text = Loc.GetString("rmc-chem-master-reagent-amount", new (string, object)[2]
    {
      ("name", (object) str),
      ("amount", (object) reagent.Quantity)
    });
    foreach (Control child in ((Control) row.TransferSettingsContainer).Children)
    {
      if (child is RMCChemMasterTransferButton masterTransferButton)
      {
        masterTransferButton.Button.Text = masterTransferButton.Amount.ToString();
        masterTransferButton.OnPressed = (Action<FixedPoint2>) null;
        masterTransferButton.OnPressed += onTransfer;
      }
    }
  }

  private void OnLabelInputChanged(LineEdit.LineEditEventArgs args)
  {
    if (((Control) args.Control).Root == null)
      return;
    this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterPillBottleLabelMsg(args.Text));
  }

  private void UpdatePillBottleFill(RMCChemMasterPillBottleRow row, EntityUid contained)
  {
    StorageComponent storageComponent;
    if (!this.EntMan.TryGetComponent<StorageComponent>(contained, ref storageComponent))
      return;
    int num = 0;
    Box2i? nullable;
    if (Robust.Shared.Utility.Extensions.TryFirstOrNull<Box2i>((IEnumerable<Box2i>) storageComponent.Grid, ref nullable))
    {
      Box2i box2i = nullable.Value;
      num = ((Box2i) ref box2i).Width + 1;
    }
    row.PillAmountLabel.Text = Loc.GetString("rmc-chem-master-pill-bottle-pills", new (string, object)[2]
    {
      ("amount", (object) storageComponent.StoredItems.Count),
      ("total", (object) num)
    });
    row.ColorView.SetEntity(new EntityUid?(contained));
  }

  private void UpdatePillBottleName(RMCChemMasterPillBottleRow row, EntityUid contained)
  {
    IconLabelComponent iconLabelComponent;
    if (this.EntMan.TryGetComponent<IconLabelComponent>(contained, ref iconLabelComponent) && iconLabelComponent.LabelTextLocId.HasValue)
    {
      ILocalizationManager localization = this._localization;
      LocId? labelTextLocId = iconLabelComponent.LabelTextLocId;
      string str1 = labelTextLocId.HasValue ? LocId.op_Implicit(labelTextLocId.GetValueOrDefault()) : (string) null;
      string str2;
      ref string local = ref str2;
      (string, object)[] array = iconLabelComponent.LabelTextParams.ToArray();
      if (localization.TryGetString(str1, ref local, array) && str2.Length > 0)
      {
        if (str2.Length > 3)
          str2 = str2.Substring(0, 3);
        row.NameLabel.Text = $"({str2})";
        return;
      }
    }
    row.NameLabel.Text = string.Empty;
  }
}
