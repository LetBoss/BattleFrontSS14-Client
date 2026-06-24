// Decompiled with JetBrains decompiler
// Type: Content.Client.Salvage.UI.SalvageMagnetBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Message;
using Content.Shared.Salvage;
using Content.Shared.Salvage.Magnet;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Salvage.UI;

public sealed class SalvageMagnetBoundUserInterface : BoundUserInterface
{
  [Dependency]
  private IEntityManager _entManager;
  private OfferingWindow? _window;

  public SalvageMagnetBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    IoCManager.InjectDependencies<SalvageMagnetBoundUserInterface>(this);
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindowCenteredLeft<OfferingWindow>((BoundUserInterface) this);
    this._window.Title = Loc.GetString("salvage-magnet-window-title");
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is SalvageMagnetBoundUserInterfaceState userInterfaceState) || this._window == null)
      return;
    this._window.ClearOptions();
    SharedSalvageSystem sharedSalvageSystem = this._entManager.System<SharedSalvageSystem>();
    this._window.NextOffer = userInterfaceState.NextOffer;
    this._window.Progression = new TimeSpan?(userInterfaceState.EndTime ?? TimeSpan.Zero);
    this._window.Claimed = userInterfaceState.EndTime.HasValue;
    this._window.Cooldown = userInterfaceState.Cooldown;
    this._window.ProgressionCooldown = userInterfaceState.Duration;
    for (int index = 0; index < userInterfaceState.Offers.Count; ++index)
    {
      int offer = userInterfaceState.Offers[index];
      ISalvageMagnetOffering salvageOffering1 = sharedSalvageSystem.GetSalvageOffering(offer);
      OfferingWindowOption option = new OfferingWindowOption();
      ((Control) option).MinWidth = 210f;
      option.Disabled = userInterfaceState.EndTime.HasValue;
      option.Claimed = userInterfaceState.ActiveSeed == offer;
      int claimIndex = index;
      option.ClaimPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new MagnetClaimOfferEvent()
      {
        Index = claimIndex
      }));
      switch (salvageOffering1)
      {
        case AsteroidOffering asteroidOffering:
          option.Title = Loc.GetString("dungeon-config-proto-" + asteroidOffering.Id);
          List<string> list = asteroidOffering.MarkerLayers.Keys.ToList<string>();
          list.Sort();
          using (List<string>.Enumerator enumerator = list.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              string current = enumerator.Current;
              int markerLayer = asteroidOffering.MarkerLayers[current];
              BoxContainer boxContainer1 = new BoxContainer();
              boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
              ((Control) boxContainer1).HorizontalExpand = true;
              BoxContainer boxContainer2 = boxContainer1;
              Label label1 = new Label();
              label1.Text = Loc.GetString("salvage-magnet-resources", new (string, object)[1]
              {
                ("resource", (object) current)
              });
              ((Control) label1).HorizontalAlignment = (Control.HAlignment) 1;
              Label label2 = label1;
              Label label3 = new Label();
              label3.Text = Loc.GetString("salvage-magnet-resources-count", new (string, object)[1]
              {
                ("count", (object) markerLayer)
              });
              ((Control) label3).HorizontalAlignment = (Control.HAlignment) 3;
              ((Control) label3).HorizontalExpand = true;
              Label label4 = label3;
              ((Control) boxContainer2).AddChild((Control) label2);
              ((Control) boxContainer2).AddChild((Control) label4);
              option.AddContent((Control) boxContainer2);
            }
            break;
          }
        case DebrisOffering debrisOffering:
          option.Title = Loc.GetString("salvage-magnet-debris-" + debrisOffering.Id);
          break;
        case SalvageOffering salvageOffering2:
          option.Title = Loc.GetString("salvage-map-wreck");
          BoxContainer boxContainer3 = new BoxContainer();
          boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
          ((Control) boxContainer3).HorizontalExpand = true;
          BoxContainer boxContainer4 = boxContainer3;
          Label label5 = new Label();
          label5.Text = Loc.GetString("salvage-map-wreck-desc-size");
          ((Control) label5).HorizontalAlignment = (Control.HAlignment) 1;
          Label label6 = label5;
          RichTextLabel richTextLabel = new RichTextLabel();
          ((Control) richTextLabel).HorizontalAlignment = (Control.HAlignment) 3;
          ((Control) richTextLabel).HorizontalExpand = true;
          RichTextLabel label7 = richTextLabel;
          label7.SetMarkup(Loc.GetString(LocId.op_Implicit(salvageOffering2.SalvageMap.SizeString)));
          ((Control) boxContainer4).AddChild((Control) label6);
          ((Control) boxContainer4).AddChild((Control) label7);
          option.AddContent((Control) boxContainer4);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      this._window.AddOption(option);
    }
  }
}
