// Decompiled with JetBrains decompiler
// Type: Content.Client.RCD.RCDMenuBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Popups;
using Content.Client.UserInterface.Controls;
using Content.Shared.Popups;
using Content.Shared.RCD;
using Content.Shared.RCD.Components;
using Robust.Client.UserInterface;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.RCD;

public sealed class RCDMenuBoundUserInterface : BoundUserInterface
{
  private const string TopLevelActionCategory = "Main";
  private static readonly Dictionary<string, (string Tooltip, SpriteSpecifier Sprite)> PrototypesGroupingInfo = new Dictionary<string, (string, SpriteSpecifier)>()
  {
    ["WallsAndFlooring"] = ("rcd-component-walls-and-flooring", (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/Radial/RCD/walls_and_flooring.png"))),
    ["WindowsAndGrilles"] = ("rcd-component-windows-and-grilles", (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/Radial/RCD/windows_and_grilles.png"))),
    ["Airlocks"] = ("rcd-component-airlocks", (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/Radial/RCD/airlocks.png"))),
    ["Electrical"] = ("rcd-component-electrical", (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/Radial/RCD/multicoil.png"))),
    ["Lighting"] = ("rcd-component-lighting", (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/Radial/RCD/lighting.png")))
  };
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private ISharedPlayerManager _playerManager;
  private SimpleRadialMenu? _menu;

  public RCDMenuBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    IoCManager.InjectDependencies<RCDMenuBoundUserInterface>(this);
  }

  protected virtual void Open()
  {
    base.Open();
    RCDComponent rcdComponent;
    if (!this.EntMan.TryGetComponent<RCDComponent>(this.Owner, ref rcdComponent))
      return;
    this._menu = BoundUserInterfaceExt.CreateWindow<SimpleRadialMenu>((BoundUserInterface) this);
    this._menu.Track(this.Owner);
    this._menu.SetButtons(this.ConvertToButtons(rcdComponent.AvailablePrototypes));
    this._menu.OpenOverMouseScreenPosition();
  }

  private IEnumerable<RadialMenuOption> ConvertToButtons(HashSet<ProtoId<RCDPrototype>> prototypes)
  {
    Dictionary<string, List<RadialMenuActionOption>> dictionary = new Dictionary<string, List<RadialMenuActionOption>>();
    ValueList<RadialMenuActionOption> valueList = new ValueList<RadialMenuActionOption>();
    foreach (ProtoId<RCDPrototype> prototype in prototypes)
    {
      RCDPrototype rcdPrototype = this._prototypeManager.Index<RCDPrototype>(prototype);
      if (rcdPrototype.Category == "Main")
      {
        RadialMenuActionOption<RCDPrototype> menuActionOption1 = new RadialMenuActionOption<RCDPrototype>(new Action<RCDPrototype>(this.HandleMenuOptionClick), rcdPrototype);
        menuActionOption1.Sprite = rcdPrototype.Sprite;
        menuActionOption1.ToolTip = this.GetTooltip(rcdPrototype);
        RadialMenuActionOption<RCDPrototype> menuActionOption2 = menuActionOption1;
        valueList.Add((RadialMenuActionOption) menuActionOption2);
      }
      else if (RCDMenuBoundUserInterface.PrototypesGroupingInfo.TryGetValue(rcdPrototype.Category, out (string, SpriteSpecifier) _))
      {
        List<RadialMenuActionOption> menuActionOptionList;
        if (!dictionary.TryGetValue(rcdPrototype.Category, out menuActionOptionList))
        {
          menuActionOptionList = new List<RadialMenuActionOption>();
          dictionary.Add(rcdPrototype.Category, menuActionOptionList);
        }
        RadialMenuActionOption<RCDPrototype> menuActionOption3 = new RadialMenuActionOption<RCDPrototype>(new Action<RCDPrototype>(this.HandleMenuOptionClick), rcdPrototype);
        menuActionOption3.Sprite = rcdPrototype.Sprite;
        menuActionOption3.ToolTip = this.GetTooltip(rcdPrototype);
        RadialMenuActionOption<RCDPrototype> menuActionOption4 = menuActionOption3;
        menuActionOptionList.Add((RadialMenuActionOption) menuActionOption4);
      }
    }
    RadialMenuOption[] buttons = new RadialMenuOption[dictionary.Count + valueList.Count];
    int index1 = 0;
    foreach ((string key, List<RadialMenuActionOption> nested) in dictionary)
    {
      (string Tooltip, SpriteSpecifier Sprite) tuple = RCDMenuBoundUserInterface.PrototypesGroupingInfo[key];
      RadialMenuOption[] radialMenuOptionArray = buttons;
      int index2 = index1;
      RadialMenuNestedLayerOption nestedLayerOption = new RadialMenuNestedLayerOption((IReadOnlyCollection<RadialMenuOption>) nested);
      nestedLayerOption.Sprite = tuple.Sprite;
      nestedLayerOption.ToolTip = Loc.GetString(tuple.Tooltip);
      radialMenuOptionArray[index2] = (RadialMenuOption) nestedLayerOption;
      ++index1;
    }
    foreach (RadialMenuActionOption menuActionOption in valueList)
    {
      buttons[index1] = (RadialMenuOption) menuActionOption;
      ++index1;
    }
    return (IEnumerable<RadialMenuOption>) buttons;
  }

  private void HandleMenuOptionClick(RCDPrototype proto)
  {
    this.SendMessage((BoundUserInterfaceMessage) new RCDSystemMessage(ProtoId<RCDPrototype>.op_Implicit(proto.ID)));
    ICommonSession localSession = this._playerManager.LocalSession;
    if ((localSession != null ? (!localSession.AttachedEntity.HasValue ? 1 : 0) : 1) != 0)
      return;
    string message = Loc.GetString("rcd-component-change-mode", new (string, object)[1]
    {
      ("mode", (object) Loc.GetString(proto.SetName))
    });
    bool flag;
    switch (proto.Mode)
    {
      case RcdMode.ConstructTile:
      case RcdMode.ConstructObject:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    if (flag)
    {
      string name = Loc.GetString(proto.SetName);
      EntityPrototype entityPrototype;
      if (proto.Prototype != null && this._prototypeManager.TryIndex(EntProtoId.op_Implicit(proto.Prototype), ref entityPrototype))
        name = entityPrototype.Name;
      message = Loc.GetString("rcd-component-change-build-mode", new (string, object)[1]
      {
        ("name", (object) name)
      });
    }
    this.EntMan.System<PopupSystem>().PopupClient(message, this.Owner, this._playerManager.LocalSession.AttachedEntity, PopupType.Small);
  }

  private string GetTooltip(RCDPrototype proto)
  {
    bool flag;
    switch (proto.Mode)
    {
      case RcdMode.ConstructTile:
      case RcdMode.ConstructObject:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    EntityPrototype entityPrototype;
    string str = !flag || proto.Prototype == null || !this._prototypeManager.TryIndex(EntProtoId.op_Implicit(proto.Prototype), ref entityPrototype) ? Loc.GetString(proto.SetName) : Loc.GetString(entityPrototype.Name);
    return RCDMenuBoundUserInterface.OopsConcat(char.ToUpper(str[0]).ToString(), str.Remove(0, 1));
  }

  private static string OopsConcat(string a, string b) => a + b;
}
