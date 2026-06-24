// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Dropship.Weapon.DropshipWeaponsBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.TacticalMap;
using Content.Client._RMC14.UserInterface;
using Content.Client.Eye;
using Content.Client.UserInterface.ControlExtensions;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.ElectronicSystem;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Content.Shared._RMC14.Dropship.Utility.Systems;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.TacticalMap;
using Content.Shared.FixedPoint;
using Content.Shared.ParaDrop;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace Content.Client._RMC14.Dropship.Weapon;

public sealed class DropshipWeaponsBui : RMCPopOutBui<DropshipWeaponsWindow>
{
  private readonly ContainerSystem _container;
  private readonly SharedRMCEquipmentDeployerSystem _equipmentDeployer;
  private readonly EyeLerpingSystem _eyeLerping;
  private readonly DropshipSystem _system;
  private readonly DropshipWeaponSystem _weaponSystem;
  private readonly TacticalMapSystem _tacticalMapSystem;
  private EntityUid? _oldEye;
  private TacticalMapWrapper? _embeddedTacMapWrapperScreen1;
  private TacticalMapWrapper? _embeddedTacMapWrapperScreen2;

  protected override DropshipWeaponsWindow? Window { get; set; }

  public DropshipWeaponsBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._container = this.EntMan.System<ContainerSystem>();
    this._eyeLerping = this.EntMan.System<EyeLerpingSystem>();
    this._system = this.EntMan.System<DropshipSystem>();
    this._weaponSystem = this.EntMan.System<DropshipWeaponSystem>();
    this._tacticalMapSystem = this.EntMan.System<TacticalMapSystem>();
    this._equipmentDeployer = this.EntMan.System<SharedRMCEquipmentDeployerSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this.Window = this.CreatePopOutableWindow<DropshipWeaponsWindow>();
    this.Window.OffsetUpButton.Text = "^";
    ((BaseButton) this.Window.OffsetUpButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipTerminalWeaponsAdjustOffsetMsg((Direction) 4)));
    this.Window.OffsetLeftButton.Text = "<";
    ((BaseButton) this.Window.OffsetLeftButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipTerminalWeaponsAdjustOffsetMsg((Direction) 6)));
    this.Window.OffsetRightButton.Text = ">";
    ((BaseButton) this.Window.OffsetRightButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipTerminalWeaponsAdjustOffsetMsg((Direction) 2)));
    this.Window.OffsetDownButton.Text = "v";
    ((BaseButton) this.Window.OffsetDownButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipTerminalWeaponsAdjustOffsetMsg((Direction) 0)));
    ((BaseButton) this.Window.ResetOffsetButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipTerminalWeaponsResetOffsetMsg()));
    this.Window.ScreenOne.TopRow.Refresh();
    this.Window.ScreenOne.LeftRow.Refresh();
    this.Window.ScreenOne.RightRow.Refresh();
    this.Window.ScreenOne.BottomRow.Refresh();
    this.Window.ScreenTwo.TopRow.Refresh();
    this.Window.ScreenTwo.LeftRow.Refresh();
    this.Window.ScreenTwo.RightRow.Refresh();
    this.Window.ScreenTwo.BottomRow.Refresh();
    this.Refresh();
  }

  public void Refresh()
  {
    if (this.Window == null)
      return;
    DropshipTerminalWeaponsComponent weaponsComponent;
    if (this.EntMan.TryGetComponent<DropshipTerminalWeaponsComponent>(this.Owner, ref weaponsComponent))
    {
      this.SetScreen(true, weaponsComponent.ScreenOne);
      this.SetScreen(false, weaponsComponent.ScreenTwo);
    }
    this.RefreshButtons();
    if (this._embeddedTacMapWrapperScreen1 != null && weaponsComponent != null && weaponsComponent.ScreenOne.State == DropshipTerminalWeaponsScreen.TacMap)
      this.RefreshEmbeddedTacMap(this._embeddedTacMapWrapperScreen1);
    if (this._embeddedTacMapWrapperScreen2 == null || weaponsComponent == null || weaponsComponent.ScreenTwo.State != DropshipTerminalWeaponsScreen.TacMap)
      return;
    this.RefreshEmbeddedTacMap(this._embeddedTacMapWrapperScreen2);
  }

  private void RefreshButtons()
  {
    if (this.Window == null)
      return;
    foreach (DropshipWeaponsButton dropshipWeaponsButton in ((Control) this.Window.Panel).GetControlOfType<DropshipWeaponsButton>())
      dropshipWeaponsButton.Refresh();
  }

  private void SetScreen(bool first, DropshipTerminalWeaponsComponent.Screen compScreen)
  {
    DropshipTerminalWeaponsComponent terminal;
    if (this.Window == null || !this.EntMan.TryGetComponent<DropshipTerminalWeaponsComponent>(this.Owner, ref terminal))
      return;
    DropshipWeaponsScreen screen = first ? this.Window.ScreenOne : this.Window.ScreenTwo;
    screen.Viewport.RemoveAllChildren();
    screen.Viewport.Visible = false;
    DropshipWeaponsButtonData weaponsButtonData1 = Button("equip", DropshipTerminalWeaponsScreen.Equip);
    DropshipWeaponsButtonData weaponsButtonData2 = ButtonAction("exit", (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipTerminalWeaponsExitMsg(first))));
    DropshipWeaponsButtonData weaponsButtonData3 = Button("target", DropshipTerminalWeaponsScreen.Target);
    DropshipWeaponsButtonData weaponsButtonData4 = Button("cams", DropshipTerminalWeaponsScreen.Cams);
    DropshipWeaponsButtonData weaponsButtonData5 = Button("maps", DropshipTerminalWeaponsScreen.TacMap);
    DropshipWeaponsButtonData weaponsButtonData6 = ButtonAction("fire", (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipTerminalWeaponsFireMsg(first))));
    DropshipWeaponsButtonData weaponsButtonData7 = Button("strike", DropshipTerminalWeaponsScreen.Strike);
    DropshipWeaponsButtonData weaponsButtonData8 = ButtonAction("night-vision-on", (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipTerminalWeaponsNightVisionMsg(true))));
    DropshipWeaponsButtonData weaponsButtonData9 = ButtonAction("night-vision-off", (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipTerminalWeaponsNightVisionMsg(false))));
    DropshipWeaponsButtonData weaponsButtonData10 = ButtonAction("cancel", (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipTerminalWeaponsCancelMsg(first))));
    DropshipWeaponsButtonData weaponsButtonData11 = Button("weapon", DropshipTerminalWeaponsScreen.StrikeWeapon);
    DropshipWeaponsButtonData weaponsButtonData12 = ButtonAction("lock", (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropShipTerminalWeaponsParaDropTargetSelectMsg(true))));
    DropshipWeaponsButtonData weaponsButtonData13 = ButtonAction("clear", (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropShipTerminalWeaponsParaDropTargetSelectMsg(false))));
    DropshipWeaponsButtonData weaponsButtonData14 = ButtonAction("enable", (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropShipTerminalWeaponsSpotlightToggleMsg(true))));
    DropshipWeaponsButtonData weaponsButtonData15 = ButtonAction("disable", (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropShipTerminalWeaponsSpotlightToggleMsg(false))));
    DropshipWeaponsButtonData weaponsButtonData16 = ButtonAction("deploy", (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropShipTerminalWeaponsEquipmentDeployToggleMsg(true))));
    DropshipWeaponsButtonData weaponsButtonData17 = ButtonAction("retract", (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropShipTerminalWeaponsEquipmentDeployToggleMsg(false))));
    DropshipWeaponsButtonData weaponsButtonData18 = ButtonAction("auto-deploy", (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropShipTerminalWeaponsEquipmentAutoDeployToggleMsg(true))));
    DropshipWeaponsButtonData weaponsButtonData19 = ButtonAction("auto-deploy", (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropShipTerminalWeaponsEquipmentAutoDeployToggleMsg(false))));
    screen.ScreenLabel.Text = Loc.GetString("rmc-dropship-weapons-main-screen-text");
    ((Control) screen.ScreenLabel).VerticalAlignment = (Control.VAlignment) 0;
    ((Control) screen.ScreenLabel).Margin = new Thickness();
    ((Control) screen.ScreenLabel).Visible = true;
    this.ClearNames(screen);
    switch (compScreen.State)
    {
      case DropshipTerminalWeaponsScreen.Main:
        DropshipWeaponsButtonRow bottomRow1 = screen.BottomRow;
        DropshipWeaponsButtonData? nullable1 = new DropshipWeaponsButtonData?(weaponsButtonData5);
        DropshipWeaponsButtonData? nullable2 = new DropshipWeaponsButtonData?(weaponsButtonData4);
        DropshipWeaponsButtonData? one1 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? two1 = nullable1;
        DropshipWeaponsButtonData? three1 = nullable2;
        DropshipWeaponsButtonData? four1 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? five1 = new DropshipWeaponsButtonData?();
        bottomRow1.SetData(one1, two1, three1, four1, five1);
        DropshipWeaponsButtonRow topRow1 = screen.TopRow;
        DropshipWeaponsButtonData? one2 = new DropshipWeaponsButtonData?(weaponsButtonData1);
        nullable2 = new DropshipWeaponsButtonData?(weaponsButtonData3);
        DropshipWeaponsButtonData? two2 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? three2 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? four2 = nullable2;
        DropshipWeaponsButtonData? five2 = new DropshipWeaponsButtonData?();
        topRow1.SetData(one2, two2, three2, four2, five2);
        break;
      case DropshipTerminalWeaponsScreen.Equip:
        screen.BottomRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData2));
        DropshipWeaponsButtonData? one3;
        DropshipWeaponsButtonData? two3;
        DropshipWeaponsButtonData? three3;
        DropshipWeaponsButtonData? four3;
        DropshipWeaponsButtonData? utilityOne1;
        DropshipWeaponsButtonData? utilityTwo1;
        DropshipWeaponsButtonData? utilityThree;
        DropshipWeaponsButtonData? electronicSystemOne;
        DropshipWeaponsButtonData? electronicSystemTwo;
        this.TryGetWeapons(first, out one3, out two3, out three3, out four3, out utilityOne1, out utilityTwo1, out utilityThree, out electronicSystemOne, out electronicSystemTwo);
        screen.LeftRow.SetData(one3, two3, utilityOne1, utilityTwo1, utilityThree);
        screen.RightRow.SetData(three3, four3, electronicSystemOne, electronicSystemTwo);
        StringBuilder text = new StringBuilder();
        AddWeaponEntry(one3);
        AddWeaponEntry(two3);
        AddWeaponEntry(three3);
        AddWeaponEntry(four3);
        screen.ScreenLabel.Text = text.ToString();
        ((Control) screen.ScreenLabel).VerticalAlignment = (Control.VAlignment) 1;
        ((Control) screen.ScreenLabel).Margin = new Thickness(10f);
        break;

        void AddWeaponEntry(DropshipWeaponsButtonData? data)
        {
          NetEntity? weapon = (NetEntity?) data?.Weapon;
          EntityUid? nullable;
          if (!weapon.HasValue || !this.EntMan.TryGetEntity(weapon.GetValueOrDefault(), ref nullable))
            return;
          int weaponRounds = this._weaponSystem.GetWeaponRounds(Entity<DropshipWeaponComponent>.op_Implicit(nullable.Value));
          RMCEquipmentDeployerComponent deployerComponent;
          int? ammoCount;
          if (this.EntMan.TryGetComponent<RMCEquipmentDeployerComponent>(nullable.Value, ref deployerComponent) && deployerComponent.DeployEntity.HasValue && this._equipmentDeployer.TryGetDeployedAmmo(this.EntMan.GetEntity(deployerComponent.DeployEntity.Value), out ammoCount, out int? _))
            weaponRounds = ammoCount.Value;
          text.AppendLine(Loc.GetString("rmc-dropship-weapons-equip-weapon-ammo", new (string, object)[2]
          {
            ("weapon", (object) nullable),
            ("rounds", (object) weaponRounds)
          }));
          text.AppendLine();
        }
      case DropshipTerminalWeaponsScreen.Target:
        DropshipWeaponsButtonData? previous1;
        DropshipWeaponsButtonData? next1;
        AddTargets(out previous1, out next1);
        DropshipWeaponsButtonRow bottomRow2 = screen.BottomRow;
        DropshipWeaponsButtonData? one4 = new DropshipWeaponsButtonData?(weaponsButtonData2);
        DropshipWeaponsButtonData? nullable3 = next1;
        DropshipWeaponsButtonData? two4 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? three4 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? four4 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? five3 = nullable3;
        bottomRow2.SetData(one4, two4, three4, four4, five3);
        DropshipWeaponsButtonRow topRow2 = screen.TopRow;
        DropshipWeaponsButtonData? one5 = new DropshipWeaponsButtonData?(weaponsButtonData6);
        DropshipWeaponsButtonData? nullable4 = previous1;
        DropshipWeaponsButtonData? two5 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? three5 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? four5 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? five4 = nullable4;
        topRow2.SetData(one5, two5, three5, four5, five4);
        screen.LeftRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData7));
        screen.ScreenLabel.Text = TargetAcquisition();
        break;
      case DropshipTerminalWeaponsScreen.Strike:
        DropshipWeaponsButtonData? previous2;
        DropshipWeaponsButtonData? next2;
        AddTargets(out previous2, out next2);
        DropshipWeaponsButtonRow bottomRow3 = screen.BottomRow;
        DropshipWeaponsButtonData? one6 = new DropshipWeaponsButtonData?(weaponsButtonData2);
        DropshipWeaponsButtonData? nullable5 = next2;
        DropshipWeaponsButtonData? two6 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? three6 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? four6 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? five5 = nullable5;
        bottomRow3.SetData(one6, two6, three6, four6, five5);
        DropshipWeaponsButtonRow topRow3 = screen.TopRow;
        DropshipWeaponsButtonData? one7 = new DropshipWeaponsButtonData?(weaponsButtonData6);
        DropshipWeaponsButtonData? nullable6 = previous2;
        DropshipWeaponsButtonData? two7 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? three7 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? four7 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? five6 = nullable6;
        topRow3.SetData(one7, two7, three7, four7, five6);
        screen.LeftRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData10), new DropshipWeaponsButtonData?(weaponsButtonData11));
        screen.ScreenLabel.Text = TargetAcquisition();
        break;
      case DropshipTerminalWeaponsScreen.StrikeWeapon:
        DropshipWeaponsButtonData? previous3;
        DropshipWeaponsButtonData? next3;
        AddTargets(out previous3, out next3);
        DropshipWeaponsButtonRow bottomRow4 = screen.BottomRow;
        DropshipWeaponsButtonData? one8 = new DropshipWeaponsButtonData?(weaponsButtonData2);
        DropshipWeaponsButtonData? nullable7 = next3;
        DropshipWeaponsButtonData? two8 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? three8 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? four8 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? five7 = nullable7;
        bottomRow4.SetData(one8, two8, three8, four8, five7);
        DropshipWeaponsButtonRow topRow4 = screen.TopRow;
        DropshipWeaponsButtonData? one9 = new DropshipWeaponsButtonData?(weaponsButtonData6);
        DropshipWeaponsButtonData? utilityOne2 = previous3;
        DropshipWeaponsButtonData? two9 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? three9 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? utilityTwo2 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? four9 = utilityTwo2;
        DropshipWeaponsButtonData? five8 = utilityOne2;
        topRow4.SetData(one9, two9, three9, four9, five8);
        DropshipWeaponsButtonData? one10;
        DropshipWeaponsButtonData? two10;
        DropshipWeaponsButtonData? three10;
        DropshipWeaponsButtonData? four10;
        this.TryGetWeapons(first, out one10, out two10, out three10, out four10, out utilityOne2, out utilityTwo2, out DropshipWeaponsButtonData? _, out DropshipWeaponsButtonData? _, out DropshipWeaponsButtonData? _);
        screen.LeftRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData10), one10, two10, three10, four10);
        screen.ScreenLabel.Text = TargetAcquisition();
        break;
      case DropshipTerminalWeaponsScreen.Cams:
        screen.LeftRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData8), new DropshipWeaponsButtonData?(weaponsButtonData9));
        ((Control) screen.ScreenLabel).Visible = false;
        if (this._oldEye.HasValue)
        {
          EntityUid? oldEye = this._oldEye;
          EntityUid? target = terminal.Target;
          if ((oldEye.HasValue == target.HasValue ? (oldEye.HasValue ? (EntityUid.op_Inequality(oldEye.GetValueOrDefault(), target.GetValueOrDefault()) ? 1 : 0) : 0) : 1) != 0)
            this._eyeLerping.RemoveEye(this._oldEye.Value);
        }
        this._oldEye = terminal.Target;
        EntityUid eye;
        if (terminal.Target.HasValue && this._weaponSystem.TryGetTargetEye(Entity<DropshipTerminalWeaponsComponent>.op_Implicit((this.Owner, terminal)), Entity<DropshipTargetComponent>.op_Implicit(terminal.Target.Value), out eye))
        {
          if (!this.EntMan.HasComponent<LerpingEyeComponent>(eye))
            this._eyeLerping.AddEye(eye);
          EyeComponent eyeComponent;
          if (this.EntMan.TryGetComponent<EyeComponent>(eye, ref eyeComponent))
            screen.Viewport.Eye = (IEye) eyeComponent.Eye;
        }
        screen.Viewport.Visible = true;
        screen.BottomRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData2));
        break;
      case DropshipTerminalWeaponsScreen.SelectingWeapon:
        ((Control) screen.ScreenLabel).VerticalAlignment = (Control.VAlignment) 1;
        ((Control) screen.ScreenLabel).Margin = new Thickness(0.0f, 10f);
        EntityUid? nullable8;
        if (this.EntMan.TryGetEntity(compScreen.Weapon, ref nullable8))
        {
          Entity<DropshipAmmoComponent> ammo;
          if (this._weaponSystem.TryGetWeaponAmmo(Entity<DropshipWeaponComponent>.op_Implicit(nullable8.Value), out ammo))
            screen.ScreenLabel.Text = Loc.GetString("rmc-dropship-weapons-weapon-selected-ammo", new (string, object)[4]
            {
              ("weapon", (object) nullable8.Value),
              ("ammo", (object) ammo),
              ("rounds", (object) ammo.Comp.Rounds),
              ("maxRounds", (object) ammo.Comp.MaxRounds)
            });
          else
            screen.ScreenLabel.Text = Loc.GetString("rmc-dropship-weapons-weapon-selected", new (string, object)[1]
            {
              ("weapon", (object) nullable8.Value)
            });
        }
        screen.TopRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData1));
        screen.LeftRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData6));
        screen.BottomRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData2));
        break;
      case DropshipTerminalWeaponsScreen.Medevac:
        DropshipWeaponsButtonData? previous4;
        DropshipWeaponsButtonData? next4;
        AddButtons((Func<NetEntity, BoundUserInterfaceMessage>) (id => (BoundUserInterfaceMessage) new DropshipTerminalWeaponsMedevacSelectMsg(id)), (BoundUserInterfaceMessage) new DropshipTerminalWeaponsMedevacPreviousMsg(), (BoundUserInterfaceMessage) new DropshipTerminalWeaponsMedevacNextMsg(), screen.LeftRow, terminal.Medevacs, terminal.MedevacsPage, out previous4, out next4);
        screen.TopRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData1));
        screen.BottomRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData2));
        DropshipWeaponsButtonRow rightRow1 = screen.RightRow;
        DropshipWeaponsButtonData? one11 = previous4;
        DropshipWeaponsButtonData? nullable9 = next4;
        DropshipWeaponsButtonData? two11 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? three11 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? four11 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? five9 = nullable9;
        rightRow1.SetData(one11, two11, three11, four11, five9);
        screen.ScreenLabel.Text = Loc.GetString("rmc-dropship-medevac-system-screen-text");
        break;
      case DropshipTerminalWeaponsScreen.Fulton:
        DropshipWeaponsButtonData? previous5;
        DropshipWeaponsButtonData? next5;
        AddButtons((Func<NetEntity, BoundUserInterfaceMessage>) (id => (BoundUserInterfaceMessage) new DropshipTerminalWeaponsFultonSelectMsg(id)), (BoundUserInterfaceMessage) new DropshipTerminalWeaponsFultonPreviousMsg(), (BoundUserInterfaceMessage) new DropshipTerminalWeaponsFultonNextMsg(), screen.LeftRow, terminal.Fultons, terminal.FultonsPage, out previous5, out next5);
        screen.TopRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData1));
        screen.BottomRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData2));
        DropshipWeaponsButtonRow rightRow2 = screen.RightRow;
        DropshipWeaponsButtonData? one12 = previous5;
        DropshipWeaponsButtonData? nullable10 = next5;
        DropshipWeaponsButtonData? two12 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? three12 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? four12 = new DropshipWeaponsButtonData?();
        DropshipWeaponsButtonData? five10 = nullable10;
        rightRow2.SetData(one12, two12, three12, four12, five10);
        screen.ScreenLabel.Text = Loc.GetString("rmc-dropship-fulton-system-screen-text");
        break;
      case DropshipTerminalWeaponsScreen.Paradrop:
        string str1 = (string) null;
        Entity<DropshipComponent> dropship;
        ActiveParaDropComponent paraDropComponent;
        NetEntity? nullable11;
        if (terminal.Target.HasValue && this._system.TryGetGridDropship(this.Owner, out dropship) && this.EntMan.TryGetComponent<ActiveParaDropComponent>(Entity<DropshipComponent>.op_Implicit(dropship), ref paraDropComponent) && this._weaponSystem.TryGetNetEntity(paraDropComponent.DropTarget, ref nullable11, (MetaDataComponent) null))
        {
          foreach (DropshipTerminalWeaponsComponent.TargetEnt target in terminal.Targets)
          {
            NetEntity id = target.Id;
            NetEntity? nullable12 = nullable11;
            if ((nullable12.HasValue ? (NetEntity.op_Inequality(id, nullable12.GetValueOrDefault()) ? 1 : 0) : 1) == 0)
            {
              str1 = target.Name;
              break;
            }
          }
        }
        Label screenLabel = screen.ScreenLabel;
        (string, object)[] valueTupleArray = new (string, object)[1];
        string str2;
        if (str1 != null)
          str2 = Loc.GetString("rmc-dropship-paradrop-target-screen-target-targeting", new (string, object)[1]
          {
            ("dropTarget", (object) str1)
          });
        else
          str2 = Loc.GetString("rmc-dropship-paradrop-target-screen-target-none");
        valueTupleArray[0] = ("hasTarget", (object) str2);
        string str3 = Loc.GetString("rmc-dropship-paradrop-target-screen-text", valueTupleArray);
        screenLabel.Text = str3;
        screen.BottomRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData2));
        screen.LeftRow.SetData(new DropshipWeaponsButtonData?(str1 == null ? weaponsButtonData12 : weaponsButtonData13));
        screen.TopRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData1));
        break;
      case DropshipTerminalWeaponsScreen.Spotlight:
        screen.TopRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData1));
        screen.BottomRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData2));
        DropshipSpotlightComponent spotlightComponent;
        if (this.EntMan.TryGetComponent<DropshipSpotlightComponent>(this.EntMan.GetEntity(terminal.SelectedSystem), ref spotlightComponent))
        {
          screen.LeftRow.SetData(new DropshipWeaponsButtonData?(!spotlightComponent.Enabled ? weaponsButtonData14 : weaponsButtonData15));
          break;
        }
        break;
      case DropshipTerminalWeaponsScreen.TacMap:
        ((Control) screen.ScreenLabel).Visible = false;
        TacticalMapWrapper wrapper = first ? this._embeddedTacMapWrapperScreen1 : this._embeddedTacMapWrapperScreen2;
        if (wrapper == null)
        {
          wrapper = new TacticalMapWrapper();
          this.SetupEmbeddedTacMap(wrapper);
          if (first)
            this._embeddedTacMapWrapperScreen1 = wrapper;
          else
            this._embeddedTacMapWrapperScreen2 = wrapper;
        }
        screen.Viewport.AddChild((Control) wrapper);
        screen.Viewport.Visible = true;
        screen.BottomRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData2));
        this.RefreshEmbeddedTacMap(wrapper);
        break;
      case DropshipTerminalWeaponsScreen.EquipmentDeployer:
        EntityUid? entity1 = this.EntMan.GetEntity(terminal.SelectedSystem);
        screen.TopRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData1));
        screen.BottomRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData2));
        BaseContainer container;
        if (entity1.HasValue && this._equipmentDeployer.TryGetContainer(entity1.Value, out container))
        {
          EntityUid? nullable13 = new EntityUid?();
          if (container.Count > 0)
            nullable13 = new EntityUid?(container.ContainedEntities[0]);
          RMCEquipmentDeployerComponent deployerComponent;
          MetaDataComponent metaDataComponent;
          if (this.EntMan.TryGetComponent<RMCEquipmentDeployerComponent>(nullable13, ref deployerComponent) && this.EntMan.TryGetComponent<MetaDataComponent>(nullable13, ref metaDataComponent))
          {
            string str4 = "";
            EntityUid? entity2 = this.EntMan.GetEntity(deployerComponent.DeployEntity);
            string str5;
            if (entity2.HasValue && !this.EntMan.Deleted(entity2))
            {
              int? ammoCount;
              int? ammoCapacity;
              if (this._equipmentDeployer.TryGetDeployedAmmo(entity2.Value, out ammoCount, out ammoCapacity))
                str4 = Loc.GetString("rmc-dropship-equipment-deployer-ammo", new (string, object)[2]
                {
                  ("ammoCount", (object) ammoCount),
                  ("totalAmmoCount", (object) ammoCapacity)
                }) + "\n";
              str5 = Loc.GetString("rmc-dropship-equipment-deployer-health", new (string, object)[1]
              {
                ("status", this._equipmentDeployer.TryGetDeployedDamage(entity2.Value, out FixedPoint2 _) ? (object) Loc.GetString("rmc-dropship-equipment-damaged") : (object) Loc.GetString("rmc-dropship-equipment-operational"))
              }) + "\n";
            }
            else
              str5 = Loc.GetString("rmc-dropship-equipment-deployer-health", new (string, object)[1]
              {
                ("status", (object) Loc.GetString("rmc-dropship-equipment-destroyed"))
              }) + "\n";
            screen.ScreenLabel.Text = $"{Loc.GetString("rmc-dropship-equipment-deployer-text", new (string, object)[1]
            {
              ("deployName", (object) metaDataComponent.EntityName)
            })}\n{str5}{str4}{Loc.GetString("rmc-dropship-equipment-deployer-status", new (string, object)[1]
            {
              ("deployed", deployerComponent.IsDeployed ? (object) Loc.GetString("rmc-dropship-equipment-deployed") : (object) Loc.GetString("rmc-dropship-equipment-undeployed"))
            })}\n{Loc.GetString("rmc-dropship-equipment-deployer-auto-deploy", new (string, object)[1]
            {
              ("autoDeploy", deployerComponent.AutoDeploy ? (object) Loc.GetString("rmc-dropship-equipment-enabled") : (object) Loc.GetString("rmc-dropship-equipment-disabled"))
            })}";
            screen.LeftRow.SetData(new DropshipWeaponsButtonData?(!deployerComponent.IsDeployed ? weaponsButtonData16 : weaponsButtonData17), new DropshipWeaponsButtonData?(!deployerComponent.AutoDeploy ? weaponsButtonData18 : weaponsButtonData19));
            break;
          }
          break;
        }
        break;
      default:
        screen.BottomRow.SetData(new DropshipWeaponsButtonData?(weaponsButtonData2));
        break;
    }
    this.RefreshButtons();

    static DropshipWeaponsButtonData ButtonAction(
      string suffix,
      Action<BaseButton.ButtonEventArgs> onPressed)
    {
      return new DropshipWeaponsButtonData(LocId.op_Implicit("rmc-dropship-weapons-" + suffix), onPressed);
    }

    DropshipWeaponsButtonData Button(string suffix, DropshipTerminalWeaponsScreen change)
    {
      DropshipTerminalWeaponsChangeScreenMsg msg = new DropshipTerminalWeaponsChangeScreenMsg(first, change);
      return ButtonAction(suffix, new Action<BaseButton.ButtonEventArgs>(OnPressed));

      void OnPressed(BaseButton.ButtonEventArgs _)
      {
        this.SendPredictedMessage((BoundUserInterfaceMessage) msg);
      }
    }

    string TargetAcquisition()
    {
      EntityUid? entity = this.EntMan.GetEntity(compScreen.Weapon);
      return Loc.GetString("rmc-dropship-weapons-target-strike", new (string, object)[5]
      {
        ("mode", !compScreen.Weapon.HasValue ? (object) "NONE" : (object) "WEAPON"),
        ("weapon", !entity.HasValue ? (object) "" : (object) entity),
        ("target", !terminal.Target.HasValue ? (object) "NONE" : (object) terminal.Target.Value),
        ("xOffset", (object) terminal.Offset.X),
        ("yOffset", (object) terminal.Offset.Y)
      });
    }

    void AddButtons(
      Func<NetEntity, BoundUserInterfaceMessage> selectMsg,
      BoundUserInterfaceMessage previousMsg,
      BoundUserInterfaceMessage nextMsg,
      DropshipWeaponsButtonRow row,
      List<DropshipTerminalWeaponsComponent.TargetEnt> targets,
      int page,
      out DropshipWeaponsButtonData? previous,
      out DropshipWeaponsButtonData? next)
    {
      previous = new DropshipWeaponsButtonData?();
      next = new DropshipWeaponsButtonData?();
      int index = page * 5;
      if (targets.Count <= 5)
        index = 0;
      else if (index > targets.Count - 5)
        index = targets.Count - 5;
      if (index > 0)
        previous = new DropshipWeaponsButtonData?(ButtonAction(nameof (previous), (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage(previousMsg))));
      if (index + 4 < targets.Count - 1)
        next = new DropshipWeaponsButtonData?(ButtonAction(nameof (next), (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage(nextMsg))));
      DropshipWeaponsButtonData? targetData1 = GetTargetData(index);
      DropshipWeaponsButtonData? targetData2 = GetTargetData(index + 1);
      DropshipWeaponsButtonData? targetData3 = GetTargetData(index + 2);
      DropshipWeaponsButtonData? targetData4 = GetTargetData(index + 3);
      DropshipWeaponsButtonData? targetData5 = GetTargetData(index + 4);
      row.SetData(targetData1, targetData2, targetData3, targetData4, targetData5);

      DropshipWeaponsButtonData? GetTargetData(int index)
      {
        DropshipTerminalWeaponsComponent.TargetEnt targetEnt;
        if (!Extensions.TryGetValue<DropshipTerminalWeaponsComponent.TargetEnt>((IList<DropshipTerminalWeaponsComponent.TargetEnt>) targets, index, ref targetEnt))
          return new DropshipWeaponsButtonData?();
        BoundUserInterfaceMessage msg = selectMsg(targetEnt.Id);
        return new DropshipWeaponsButtonData?(new DropshipWeaponsButtonData(LocId.op_Implicit(targetEnt.Name), (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage(msg))));
      }
    }

    void AddTargets(out DropshipWeaponsButtonData? previous, out DropshipWeaponsButtonData? next)
    {
      AddButtons((Func<NetEntity, BoundUserInterfaceMessage>) (id => (BoundUserInterfaceMessage) new DropshipTerminalWeaponsTargetsSelectMsg(id)), (BoundUserInterfaceMessage) new DropshipTerminalWeaponsTargetsPreviousMsg(), (BoundUserInterfaceMessage) new DropshipTerminalWeaponsTargetsNextMsg(), screen.RightRow, terminal.Targets, terminal.TargetsPage, out previous, out next);
    }
  }

  private void SetupEmbeddedTacMap(TacticalMapWrapper wrapper)
  {
    TacticalMapComputerComponent computerComponent;
    if (wrapper == null || !this.EntMan.TryGetComponent<TacticalMapComputerComponent>(this.Owner, ref computerComponent))
      return;
    TabContainer.SetTabTitle((Control) wrapper.MapTab, Loc.GetString("rmc-dropship-weapons-maps"));
    TabContainer.SetTabVisible((Control) wrapper.MapTab, true);
    TabContainer.SetTabVisible((Control) wrapper.CanvasTab, false);
    AreaGridComponent areaGridComponent;
    if (computerComponent.Map.HasValue && this.EntMan.TryGetComponent<AreaGridComponent>(computerComponent.Map.Value, ref areaGridComponent))
      wrapper.UpdateTexture(Entity<AreaGridComponent>.op_Implicit((computerComponent.Map.Value, areaGridComponent)));
    int lineLimit = this._tacticalMapSystem.LineLimit;
    wrapper.SetLineLimit(lineLimit);
    wrapper.LastUpdateAt = computerComponent.LastAnnounceAt;
    wrapper.NextUpdateAt = computerComponent.NextAnnounceAt;
  }

  private void RefreshEmbeddedTacMap(TacticalMapWrapper wrapper)
  {
    TacticalMapComputerComponent computerComponent;
    if (wrapper == null || !this.EntMan.TryGetComponent<TacticalMapComputerComponent>(this.Owner, ref computerComponent))
      return;
    TacticalMapBlip[] blips = new TacticalMapBlip[computerComponent.Blips.Count];
    int num = 0;
    foreach (TacticalMapBlip tacticalMapBlip in computerComponent.Blips.Values)
      blips[num++] = tacticalMapBlip;
    wrapper.UpdateBlips(blips);
    wrapper.Map.Lines.Clear();
    TacticalMapLinesComponent componentOrNull1 = EntityManagerExt.GetComponentOrNull<TacticalMapLinesComponent>(this.EntMan, this.Owner);
    if (componentOrNull1 != null)
      wrapper.Map.Lines.AddRange((IEnumerable<TacticalMapLine>) componentOrNull1.MarineLines);
    TacticalMapLabelsComponent componentOrNull2 = EntityManagerExt.GetComponentOrNull<TacticalMapLabelsComponent>(this.EntMan, this.Owner);
    if (componentOrNull2 != null)
      wrapper.UpdateTacticalLabels(componentOrNull2.MarineLabels);
    wrapper.LastUpdateAt = computerComponent.LastAnnounceAt;
    wrapper.NextUpdateAt = computerComponent.NextAnnounceAt;
  }

  private void ClearNames(DropshipWeaponsScreen screen)
  {
    screen.TopRow.SetData();
    screen.LeftRow.SetData();
    screen.RightRow.SetData();
    screen.BottomRow.SetData();
  }

  private void TryGetWeapons(
    bool first,
    out DropshipWeaponsButtonData? one,
    out DropshipWeaponsButtonData? two,
    out DropshipWeaponsButtonData? three,
    out DropshipWeaponsButtonData? four,
    out DropshipWeaponsButtonData? utilityOne,
    out DropshipWeaponsButtonData? utilityTwo,
    out DropshipWeaponsButtonData? utilityThree,
    out DropshipWeaponsButtonData? electronicSystemOne,
    out DropshipWeaponsButtonData? electronicSystemTwo)
  {
    one = new DropshipWeaponsButtonData?();
    two = new DropshipWeaponsButtonData?();
    three = new DropshipWeaponsButtonData?();
    four = new DropshipWeaponsButtonData?();
    utilityOne = new DropshipWeaponsButtonData?();
    utilityTwo = new DropshipWeaponsButtonData?();
    utilityThree = new DropshipWeaponsButtonData?();
    electronicSystemOne = new DropshipWeaponsButtonData?();
    electronicSystemTwo = new DropshipWeaponsButtonData?();
    Entity<DropshipComponent> dropship;
    if (!this._system.TryGetGridDropship(this.Owner, out dropship))
      return;
    List<DropshipWeaponsButtonData?> nullableList1 = new List<DropshipWeaponsButtonData?>();
    List<DropshipWeaponsButtonData?> nullableList2 = new List<DropshipWeaponsButtonData?>();
    List<DropshipWeaponsButtonData?> nullableList3 = new List<DropshipWeaponsButtonData?>();
    foreach (EntityUid attachmentPoint in dropship.Comp.AttachmentPoints)
    {
      DropshipUtilityPointComponent utilityPointComponent;
      BaseContainer baseContainer1;
      if (this.EntMan.TryGetComponent<DropshipUtilityPointComponent>(attachmentPoint, ref utilityPointComponent) && ((SharedContainerSystem) this._container).TryGetContainer(attachmentPoint, utilityPointComponent.UtilitySlotId, ref baseContainer1, (ContainerManagerComponent) null) && baseContainer1.ContainedEntities.Count > 0)
      {
        EntityUid containedEntity = baseContainer1.ContainedEntities[0];
        string str;
        BoundUserInterfaceMessage msg;
        if (this.EntMan.HasComponent<MedevacComponent>(containedEntity))
        {
          str = "Medevac";
          msg = (BoundUserInterfaceMessage) new DropshipTerminalWeaponsChooseMedevacMsg(first);
        }
        else if (this.EntMan.HasComponent<RMCFultonComponent>(containedEntity))
        {
          str = "Fulton";
          msg = (BoundUserInterfaceMessage) new DropshipTerminalWeaponsChooseFultonMsg(first);
        }
        else if (this.EntMan.HasComponent<RMCParaDropComponent>(containedEntity))
        {
          str = "PDS";
          msg = (BoundUserInterfaceMessage) new DropshipTerminalWeaponsChooseParaDropMsg(first);
        }
        else
        {
          RMCEquipmentDeployerComponent deployerComponent;
          if (this.EntMan.TryGetComponent<RMCEquipmentDeployerComponent>(containedEntity, ref deployerComponent))
          {
            str = deployerComponent.DropShipWindowButtonText;
            msg = (BoundUserInterfaceMessage) new DropshipTerminalWeaponsChooseEquipmentDeployerMsg(first, this.EntMan.GetNetEntity(attachmentPoint, (MetaDataComponent) null));
          }
          else
            continue;
        }
        NetEntity netEntity = this.EntMan.GetNetEntity(containedEntity, (MetaDataComponent) null);
        DropshipWeaponsButtonData weaponsButtonData = new DropshipWeaponsButtonData(LocId.op_Implicit(str), (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage(msg)), new NetEntity?(netEntity));
        nullableList2.Add(new DropshipWeaponsButtonData?(weaponsButtonData));
      }
      DropshipElectronicSystemPointComponent systemPointComponent;
      BaseContainer baseContainer2;
      if (this.EntMan.TryGetComponent<DropshipElectronicSystemPointComponent>(attachmentPoint, ref systemPointComponent) && ((SharedContainerSystem) this._container).TryGetContainer(attachmentPoint, systemPointComponent.ContainerId, ref baseContainer2, (ContainerManagerComponent) null) && baseContainer2.ContainedEntities.Count > 0)
      {
        EntityUid containedEntity = baseContainer2.ContainedEntities[0];
        if (this.EntMan.HasComponent<DropshipSpotlightComponent>(containedEntity))
        {
          string str = "Spotlight";
          BoundUserInterfaceMessage msg = (BoundUserInterfaceMessage) new DropshipTerminalWeaponsChooseSpotlightMsg(first, this.EntMan.GetNetEntity(containedEntity, (MetaDataComponent) null));
          NetEntity netEntity = this.EntMan.GetNetEntity(containedEntity, (MetaDataComponent) null);
          DropshipWeaponsButtonData weaponsButtonData = new DropshipWeaponsButtonData(LocId.op_Implicit(str), (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage(msg)), new NetEntity?(netEntity));
          nullableList3.Add(new DropshipWeaponsButtonData?(weaponsButtonData));
        }
        else
          continue;
      }
      DropshipWeaponPointComponent weaponPointComponent;
      BaseContainer baseContainer3;
      if (this.EntMan.TryGetComponent<DropshipWeaponPointComponent>(attachmentPoint, ref weaponPointComponent) && ((SharedContainerSystem) this._container).TryGetContainer(attachmentPoint, weaponPointComponent.WeaponContainerSlotId, ref baseContainer3, (ContainerManagerComponent) null))
      {
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) baseContainer3.ContainedEntities)
        {
          DropshipWeaponComponent dropshipWeaponComponent;
          this.EntMan.TryGetComponent<DropshipWeaponComponent>(containedEntity, ref dropshipWeaponComponent);
          RMCEquipmentDeployerComponent deployerComponent;
          this.EntMan.TryGetComponent<RMCEquipmentDeployerComponent>(containedEntity, ref deployerComponent);
          if (dropshipWeaponComponent != null || deployerComponent != null)
          {
            string str = string.Empty;
            if (dropshipWeaponComponent != null)
              str = dropshipWeaponComponent.Abbreviation;
            else if (deployerComponent != null)
              str = deployerComponent.DropShipWindowButtonText;
            NetEntity netEntity = this.EntMan.GetNetEntity(containedEntity, (MetaDataComponent) null);
            DropshipTerminalWeaponsChooseWeaponMsg msg = new DropshipTerminalWeaponsChooseWeaponMsg(first, netEntity);
            DropshipWeaponsButtonData weaponsButtonData = new DropshipWeaponsButtonData(LocId.op_Implicit(str), (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) msg)), new NetEntity?(netEntity));
            nullableList1.Add(new DropshipWeaponsButtonData?(weaponsButtonData));
            if (nullableList1.Count >= 4)
              break;
          }
        }
      }
    }
    Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>) nullableList1, 0, ref one);
    Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>) nullableList1, 1, ref two);
    Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>) nullableList1, 2, ref three);
    Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>) nullableList1, 3, ref four);
    Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>) nullableList2, 0, ref utilityOne);
    Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>) nullableList2, 1, ref utilityTwo);
    Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>) nullableList2, 2, ref utilityThree);
    Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>) nullableList3, 0, ref electronicSystemOne);
    Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>) nullableList3, 1, ref electronicSystemTwo);
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      this._embeddedTacMapWrapperScreen1?.Orphan();
      this._embeddedTacMapWrapperScreen1 = (TacticalMapWrapper) null;
      this._embeddedTacMapWrapperScreen2?.Orphan();
      this._embeddedTacMapWrapperScreen2 = (TacticalMapWrapper) null;
      if (this._oldEye.HasValue)
        this._eyeLerping.RemoveEye(this._oldEye.Value);
    }
    base.Dispose(disposing);
  }
}
