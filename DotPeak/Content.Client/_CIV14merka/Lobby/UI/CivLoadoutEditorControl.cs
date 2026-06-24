// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Lobby.UI.CivLoadoutEditorControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka;
using Content.Shared._CIV14merka.Factions;
using Content.Shared._CIV14merka.Loadout;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Roles;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Lobby.UI;

public sealed class CivLoadoutEditorControl : BoxContainer
{
  [Dependency]
  private readonly IEntityManager _entityManager;
  [Dependency]
  private readonly IResourceCache _resourceCache;
  [Dependency]
  private readonly IPrototypeManager _prototype;
  private readonly SpriteSystem _sprite;
  private readonly IComponentFactory _componentFactory;
  private readonly CivLoadoutSystem _system;
  private static readonly Color PanelBg = Color.FromHex((ReadOnlySpan<char>) "#1A1E24", new Color?());
  private static readonly Color PanelBgLight = Color.FromHex((ReadOnlySpan<char>) "#283040", new Color?());
  private static readonly Color BorderLight = Color.FromHex((ReadOnlySpan<char>) "#3A4553", new Color?());
  private static readonly Color Accent = Color.FromHex((ReadOnlySpan<char>) "#4CAF50", new Color?());
  private static readonly Color BonusAccent = Color.FromHex((ReadOnlySpan<char>) "#D9A441", new Color?());
  private static readonly Color TextPrimary = Color.FromHex((ReadOnlySpan<char>) "#FFFFFF", new Color?());
  private static readonly Color TextSecondary = Color.FromHex((ReadOnlySpan<char>) "#C0C6CF", new Color?());
  private static readonly Color TextMuted = Color.FromHex((ReadOnlySpan<char>) "#8A93A0", new Color?());
  private static readonly Color DisabledTint = Color.FromHex((ReadOnlySpan<char>) "#3A2222", new Color?());
  private static readonly CivTdmClass[] ClassOrder = new CivTdmClass[5]
  {
    CivTdmClass.Rifleman,
    CivTdmClass.MachineGunner,
    CivTdmClass.Specialist,
    CivTdmClass.Medic,
    CivTdmClass.SquadLeader
  };
  private static readonly string[] SlotOrder = new string[14]
  {
    "jumpsuit",
    "head",
    "eyes",
    "mask",
    "outerClothing",
    "suitstorage",
    "back",
    "belt",
    "shoes",
    "gloves",
    "id",
    "ears",
    "pocket1",
    "pocket2"
  };
  private static readonly Dictionary<CivTdmClass, string> ClassIcons = new Dictionary<CivTdmClass, string>()
  {
    [CivTdmClass.Rifleman] = "rifleman",
    [CivTdmClass.MachineGunner] = "machinegunner",
    [CivTdmClass.Specialist] = "specialist",
    [CivTdmClass.Medic] = "medic",
    [CivTdmClass.SquadLeader] = "squadleader"
  };
  private readonly BoxContainer _factionRow;
  private readonly BoxContainer _classList;
  private readonly BoxContainer _content;
  private readonly List<string> _factionIds = new List<string>();
  private string _selectedFaction = string.Empty;
  private CivTdmClass _selectedClass;

  public CivLoadoutEditorControl()
  {
    IoCManager.InjectDependencies<CivLoadoutEditorControl>(this);
    this._sprite = this._entityManager.System<SpriteSystem>();
    this._componentFactory = this._entityManager.ComponentFactory;
    this._system = this._entityManager.System<CivLoadoutSystem>();
    this.Orientation = (BoxContainer.LayoutOrientation) 1;
    this.SeparationOverride = new int?(8);
    ((Control) this).HorizontalExpand = true;
    ((Control) this).VerticalExpand = true;
    ((Control) this).Margin = new Thickness(8f);
    foreach (CivFactionPrototype enumeratePrototype in this._prototype.EnumeratePrototypes<CivFactionPrototype>())
    {
      if (enumeratePrototype.Loadouts.Count != 0 && !(enumeratePrototype.ID == "Neutral"))
        this._factionIds.Add(enumeratePrototype.ID);
    }
    if (this._factionIds.Count > 0)
      this._selectedFaction = this._factionIds[0];
    ((Control) this).AddChild((Control) new Label()
    {
      Text = Loc.GetString("civ-loadout-editor-faction"),
      FontColorOverride = new Color?(CivLoadoutEditorControl.TextMuted)
    });
    this._factionRow = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0,
      SeparationOverride = new int?(8)
    };
    ((Control) this).AddChild((Control) this._factionRow);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer1.SeparationOverride = new int?(10);
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer3.SeparationOverride = new int?(6);
    ((Control) boxContainer3).MinSize = new Vector2(210f, 0.0f);
    BoxContainer boxContainer4 = boxContainer3;
    ((Control) boxContainer4).AddChild((Control) new Label()
    {
      Text = Loc.GetString("civ-loadout-editor-class"),
      FontColorOverride = new Color?(CivLoadoutEditorControl.TextMuted)
    });
    BoxContainer boxContainer5 = new BoxContainer();
    boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer5.SeparationOverride = new int?(4);
    ((Control) boxContainer5).HorizontalExpand = true;
    this._classList = boxContainer5;
    ((Control) boxContainer4).AddChild((Control) this._classList);
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).HorizontalExpand = true;
    ((Control) scrollContainer1).VerticalExpand = true;
    ScrollContainer scrollContainer2 = scrollContainer1;
    BoxContainer boxContainer6 = new BoxContainer();
    boxContainer6.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer6.SeparationOverride = new int?(4);
    ((Control) boxContainer6).HorizontalExpand = true;
    this._content = boxContainer6;
    ((Control) scrollContainer2).AddChild((Control) this._content);
    ((Control) boxContainer2).AddChild((Control) scrollContainer2);
    ((Control) this).AddChild((Control) boxContainer2);
    this._system.StateUpdated += new Action<CivLoadoutStateEvent>(this.OnStateUpdated);
    this.Refresh();
  }

  private void OnStateUpdated(CivLoadoutStateEvent _) => this.RebuildContent();

  private void Refresh()
  {
    this.BuildFactionRow();
    this.BuildClassList();
    this.RebuildContent();
  }

  private void BuildFactionRow()
  {
    ((Control) this._factionRow).RemoveAllChildren();
    foreach (string factionId in this._factionIds)
    {
      CivFactionPrototype faction;
      if (this._prototype.TryIndex<CivFactionPrototype>(factionId, ref faction))
      {
        ContainerButton containerButton = this.MakeFactionTile(faction, factionId == this._selectedFaction);
        string captured = factionId;
        ((BaseButton) containerButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
        {
          this._selectedFaction = captured;
          this.Refresh();
        });
        ((Control) this._factionRow).AddChild((Control) containerButton);
      }
    }
  }

  private void BuildClassList()
  {
    ((Control) this._classList).RemoveAllChildren();
    foreach (CivTdmClass cls in CivLoadoutEditorControl.ClassOrder)
    {
      ContainerButton containerButton = this.MakeClassRow(cls, cls == this._selectedClass);
      CivTdmClass captured = cls;
      ((BaseButton) containerButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        this._selectedClass = captured;
        this.Refresh();
      });
      ((Control) this._classList).AddChild((Control) containerButton);
    }
  }

  private void RebuildContent()
  {
    ((Control) this._content).RemoveAllChildren();
    CivFactionPrototype factionPrototype;
    ProtoId<StartingGearPrototype> protoId;
    StartingGearPrototype gear;
    if (string.IsNullOrEmpty(this._selectedFaction) || !this._prototype.TryIndex<CivFactionPrototype>(this._selectedFaction, ref factionPrototype) || !factionPrototype.Loadouts.TryGetValue(this._selectedClass, out protoId) || !this._prototype.TryIndex<StartingGearPrototype>(protoId, ref gear))
    {
      ((Control) this._content).AddChild((Control) new Label()
      {
        Text = Loc.GetString("civ-loadout-editor-empty"),
        FontColorOverride = new Color?(CivLoadoutEditorControl.TextMuted)
      });
    }
    else
    {
      BoxContainer content = this._content;
      Label label = new Label();
      label.Text = $"{factionPrototype.Name} · {CivTdmClassHelper.GetDisplayName(this._selectedClass)}";
      label.FontColorOverride = new Color?(CivLoadoutEditorControl.TextPrimary);
      ((Control) label).Margin = new Thickness(0.0f, 0.0f, 0.0f, 4f);
      ((Control) content).AddChild((Control) label);
      string key = CivLoadoutKeys.Combo(this._selectedFaction, this._selectedClass);
      CivLoadoutStateEvent state = this._system.GetState();
      List<string> collection;
      HashSet<string> stringSet1 = state.Disabled.TryGetValue(key, out collection) ? new HashSet<string>((IEnumerable<string>) collection) : new HashSet<string>();
      List<string> stringList;
      Dictionary<string, string> selections = CivLoadoutEditorControl.ParseSelections(state.Selections.TryGetValue(key, out stringList) ? stringList : (List<string>) null);
      HashSet<string> owned = new HashSet<string>();
      List<(string, int)> valueTupleList = new List<(string, int)>();
      foreach (string entry in state.Owned)
      {
        string faction;
        int count;
        string proto;
        if (CivLoadoutKeys.TryParseOwned(entry, out faction, out count, out proto) && (string.IsNullOrEmpty(faction) || !(faction != this._selectedFaction)))
        {
          if (count >= 1)
            valueTupleList.Add((proto, count));
          else
            owned.Add(proto);
        }
      }
      Dictionary<string, List<string>> effectiveSwaps = CivLoadoutManifest.GetEffectiveSwaps(gear, this._selectedFaction, this._selectedClass, (IReadOnlySet<string>) owned, this._prototype, this._componentFactory);
      List<CivLoadoutItemInfo> removableItems = CivLoadoutManifest.GetRemovableItems(gear, this._prototype, this._componentFactory, new HashSet<string>((IEnumerable<string>) effectiveSwaps.Keys));
      HashSet<string> stringSet2 = new HashSet<string>(removableItems.Select<CivLoadoutItemInfo, string>((Func<CivLoadoutItemInfo, string>) (r => r.Key)));
      if (effectiveSwaps.Count > 0)
      {
        ((Control) this._content).AddChild((Control) this.MakeSectionLabel(Loc.GetString("civ-loadout-section-weapons")));
        foreach (string orderedSlot in CivLoadoutEditorControl.OrderedSlots((IEnumerable<string>) effectiveSwaps.Keys))
        {
          List<string> options = effectiveSwaps[orderedSlot];
          string str;
          string current = !selections.TryGetValue(orderedSlot, out str) || !options.Contains(str) ? options[0] : str;
          ((Control) this._content).AddChild(this.MakeSwapRow(orderedSlot, options, current));
        }
      }
      ((Control) this._content).AddChild((Control) this.MakeSectionLabel(Loc.GetString("civ-loadout-section-gear")));
      GridContainer gridContainer1 = new GridContainer()
      {
        Columns = 5
      };
      foreach (string orderedSlot in CivLoadoutEditorControl.OrderedSlots((IEnumerable<string>) gear.Equipment.Keys))
      {
        if (!effectiveSwaps.ContainsKey(orderedSlot))
        {
          EntProtoId entProtoId = gear.Equipment[orderedSlot];
          string id = ((EntProtoId) ref entProtoId).Id;
          if (!string.IsNullOrEmpty(id))
          {
            string str = CivLoadoutKeys.Item(orderedSlot, id);
            if (stringSet2.Contains(str))
              ((Control) gridContainer1).AddChild(this.MakeItemCard(orderedSlot, id, 1, !stringSet1.Contains(str), true));
            else
              ((Control) gridContainer1).AddChild(this.MakeItemCard(orderedSlot, id, 1, true, false));
          }
        }
      }
      ((Control) this._content).AddChild((Control) gridContainer1);
      List<CivLoadoutItemInfo> list = removableItems.Where<CivLoadoutItemInfo>((Func<CivLoadoutItemInfo, bool>) (it =>
      {
        string proto = it.Proto;
        EntProtoId entProtoId = gear.Equipment[it.Slot];
        string id = ((EntProtoId) ref entProtoId).Id;
        return proto != id;
      })).ToList<CivLoadoutItemInfo>();
      if (list.Count > 0)
      {
        ((Control) this._content).AddChild((Control) this.MakeSectionLabel(Loc.GetString("civ-loadout-section-carried")));
        GridContainer gridContainer2 = new GridContainer()
        {
          Columns = 5
        };
        foreach (CivLoadoutItemInfo civLoadoutItemInfo in list)
          ((Control) gridContainer2).AddChild(this.MakeItemCard(civLoadoutItemInfo.Slot, civLoadoutItemInfo.Proto, civLoadoutItemInfo.Count, !stringSet1.Contains(civLoadoutItemInfo.Key), true));
        ((Control) this._content).AddChild((Control) gridContainer2);
      }
      if (valueTupleList.Count <= 0)
        return;
      ((Control) this._content).AddChild((Control) this.MakeSectionLabel(Loc.GetString("civ-loadout-section-bonus")));
      GridContainer gridContainer3 = new GridContainer()
      {
        Columns = 5
      };
      foreach ((string proto, int count) in valueTupleList)
      {
        string str = CivLoadoutKeys.Item("bonus", proto);
        ((Control) gridContainer3).AddChild(this.MakeBonusCard(proto, count, !stringSet1.Contains(str)));
      }
      ((Control) this._content).AddChild((Control) gridContainer3);
    }
  }

  private ContainerButton MakeFactionTile(CivFactionPrototype faction, bool selected)
  {
    ContainerButton containerButton = new ContainerButton();
    ((Control) containerButton).MinSize = new Vector2(130f, 46f);
    PanelContainer panelContainer1 = new PanelContainer();
    panelContainer1.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = (selected ? CivLoadoutEditorControl.PanelBgLight : CivLoadoutEditorControl.PanelBg),
      BorderColor = (selected ? faction.Color : CivLoadoutEditorControl.BorderLight),
      BorderThickness = new Thickness(selected ? 2f : 1f)
    };
    ((Control) panelContainer1).HorizontalExpand = true;
    ((Control) panelContainer1).VerticalExpand = true;
    PanelContainer panelContainer2 = panelContainer1;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer1.SeparationOverride = new int?(6);
    ((Control) boxContainer1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) boxContainer1).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) boxContainer1).Margin = new Thickness(8f, 4f);
    BoxContainer boxContainer2 = boxContainer1;
    if (faction.Icon != null)
    {
      BoxContainer boxContainer3 = boxContainer2;
      TextureRect textureRect = new TextureRect();
      textureRect.Texture = this._sprite.Frame0(faction.Icon);
      textureRect.Stretch = (TextureRect.StretchMode) 7;
      ((Control) textureRect).MinSize = new Vector2(34f, 26f);
      ((Control) textureRect).VerticalAlignment = (Control.VAlignment) 2;
      ((Control) boxContainer3).AddChild((Control) textureRect);
    }
    BoxContainer boxContainer4 = boxContainer2;
    Label label = new Label();
    label.Text = faction.Name;
    label.FontColorOverride = new Color?(selected ? CivLoadoutEditorControl.TextPrimary : CivLoadoutEditorControl.TextSecondary);
    ((Control) label).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) boxContainer4).AddChild((Control) label);
    ((Control) panelContainer2).AddChild((Control) boxContainer2);
    ((Control) containerButton).AddChild((Control) panelContainer2);
    return containerButton;
  }

  private ContainerButton MakeClassRow(CivTdmClass cls, bool selected)
  {
    ContainerButton containerButton = new ContainerButton();
    ((Control) containerButton).HorizontalExpand = true;
    ((Control) containerButton).MinSize = new Vector2(0.0f, 50f);
    PanelContainer panelContainer1 = new PanelContainer();
    panelContainer1.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = (selected ? CivLoadoutEditorControl.PanelBgLight : CivLoadoutEditorControl.PanelBg),
      BorderColor = (selected ? CivLoadoutEditorControl.Accent : CivLoadoutEditorControl.BorderLight),
      BorderThickness = new Thickness(selected ? 2f : 1f)
    };
    ((Control) panelContainer1).HorizontalExpand = true;
    ((Control) panelContainer1).VerticalExpand = true;
    PanelContainer panelContainer2 = panelContainer1;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer1.SeparationOverride = new int?(8);
    ((Control) boxContainer1).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) boxContainer1).Margin = new Thickness(8f, 4f);
    BoxContainer boxContainer2 = boxContainer1;
    string str;
    TextureResource textureResource;
    if (CivLoadoutEditorControl.ClassIcons.TryGetValue(cls, out str) && this._resourceCache.TryGetResource<TextureResource>(new ResPath($"/Textures/_CIV14merka/Interface/Loadout/{str}.png"), ref textureResource))
    {
      BoxContainer boxContainer3 = boxContainer2;
      TextureRect textureRect = new TextureRect();
      textureRect.Texture = textureResource.Texture;
      textureRect.Stretch = (TextureRect.StretchMode) 7;
      ((Control) textureRect).MinSize = new Vector2(40f, 40f);
      ((Control) textureRect).VerticalAlignment = (Control.VAlignment) 2;
      ((Control) boxContainer3).AddChild((Control) textureRect);
    }
    BoxContainer boxContainer4 = boxContainer2;
    Label label = new Label();
    label.Text = CivTdmClassHelper.GetDisplayName(cls);
    label.FontColorOverride = new Color?(selected ? CivLoadoutEditorControl.TextPrimary : CivLoadoutEditorControl.TextSecondary);
    ((Control) label).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) boxContainer4).AddChild((Control) label);
    ((Control) panelContainer2).AddChild((Control) boxContainer2);
    ((Control) containerButton).AddChild((Control) panelContainer2);
    return containerButton;
  }

  private Control MakeSwapRow(string slot, List<string> options, string current)
  {
    PanelContainer panelContainer1 = new PanelContainer();
    panelContainer1.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = CivLoadoutEditorControl.PanelBg,
      BorderColor = CivLoadoutEditorControl.BorderLight,
      BorderThickness = new Thickness(1f)
    };
    ((Control) panelContainer1).HorizontalExpand = true;
    PanelContainer panelContainer2 = panelContainer1;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(4);
    ((Control) boxContainer1).Margin = new Thickness(6f);
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0,
      SeparationOverride = new int?(6)
    };
    ((Control) boxContainer3).AddChild((Control) new Label()
    {
      Text = this.SlotName(slot),
      FontColorOverride = new Color?(CivLoadoutEditorControl.TextPrimary)
    });
    BoxContainer boxContainer4 = boxContainer3;
    Label label = new Label();
    label.Text = $"({options.Count})";
    label.FontColorOverride = new Color?(CivLoadoutEditorControl.Accent);
    ((Control) boxContainer4).AddChild((Control) label);
    ((Control) boxContainer2).AddChild((Control) boxContainer3);
    BoxContainer boxContainer5 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0,
      SeparationOverride = new int?(6)
    };
    foreach (string option in options)
    {
      ContainerButton containerButton = this.MakeOptionTile(option, option == current);
      string captured = option;
      ((BaseButton) containerButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        this._system.SetSlotChoice(this._selectedFaction, this._selectedClass, slot, captured);
        this.RebuildContent();
      });
      ((Control) boxContainer5).AddChild((Control) containerButton);
    }
    ((Control) boxContainer2).AddChild((Control) boxContainer5);
    ((Control) panelContainer2).AddChild((Control) boxContainer2);
    return (Control) panelContainer2;
  }

  private ContainerButton MakeOptionTile(string proto, bool selected)
  {
    ContainerButton containerButton = new ContainerButton();
    ((Control) containerButton).MinSize = new Vector2(104f, 92f);
    ((Control) containerButton).ToolTip = this.ItemTooltip(proto);
    PanelContainer panelContainer1 = new PanelContainer();
    panelContainer1.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = (selected ? CivLoadoutEditorControl.PanelBgLight : CivLoadoutEditorControl.PanelBg),
      BorderColor = (selected ? CivLoadoutEditorControl.Accent : CivLoadoutEditorControl.BorderLight),
      BorderThickness = new Thickness(selected ? 2f : 1f)
    };
    ((Control) panelContainer1).HorizontalExpand = true;
    ((Control) panelContainer1).VerticalExpand = true;
    PanelContainer panelContainer2 = panelContainer1;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalAlignment = (Control.HAlignment) 2;
    boxContainer1.SeparationOverride = new int?(2);
    BoxContainer boxContainer2 = boxContainer1;
    ((Control) boxContainer2).AddChild(this.MakeSprite(proto, true));
    BoxContainer boxContainer3 = boxContainer2;
    Label label = new Label();
    label.Text = this.ProtoName(proto);
    label.FontColorOverride = new Color?(selected ? CivLoadoutEditorControl.TextPrimary : CivLoadoutEditorControl.TextSecondary);
    ((Control) label).HorizontalAlignment = (Control.HAlignment) 2;
    label.ClipText = true;
    ((Control) label).MaxWidth = 100f;
    ((Control) boxContainer3).AddChild((Control) label);
    ((Control) panelContainer2).AddChild((Control) boxContainer2);
    ((Control) containerButton).AddChild((Control) panelContainer2);
    return containerButton;
  }

  private Control MakeItemCard(
    string slot,
    string proto,
    int count,
    bool taken,
    bool interactive)
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalAlignment = (Control.HAlignment) 2;
    boxContainer1.SeparationOverride = new int?(2);
    ((Control) boxContainer1).Margin = new Thickness(4f);
    BoxContainer boxContainer2 = boxContainer1;
    ((Control) boxContainer2).AddChild(this.MakeSprite(proto, taken));
    string str1 = this.ProtoName(proto);
    if (count > 1)
      str1 = Loc.GetString("civ-loadout-editor-item-count", new (string, object)[2]
      {
        ("name", (object) str1),
        (nameof (count), (object) count)
      });
    BoxContainer boxContainer3 = boxContainer2;
    Label label = new Label();
    label.Text = str1;
    label.FontColorOverride = new Color?(taken ? CivLoadoutEditorControl.TextSecondary : CivLoadoutEditorControl.TextMuted);
    ((Control) label).HorizontalAlignment = (Control.HAlignment) 2;
    label.ClipText = true;
    ((Control) label).MaxWidth = 100f;
    ((Control) boxContainer3).AddChild((Control) label);
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat()
    {
      BackgroundColor = taken ? CivLoadoutEditorControl.PanelBg : CivLoadoutEditorControl.DisabledTint,
      BorderColor = CivLoadoutEditorControl.BorderLight,
      BorderThickness = new Thickness(1f)
    };
    string str2 = this.ItemTooltip(proto);
    if (!interactive)
    {
      PanelContainer panelContainer = new PanelContainer();
      panelContainer.PanelOverride = (StyleBox) styleBoxFlat;
      ((Control) panelContainer).MinSize = new Vector2(108f, 96f);
      ((Control) panelContainer).MouseFilter = (Control.MouseFilterMode) 0;
      ((Control) panelContainer).ToolTip = str2;
      ((Control) panelContainer).AddChild((Control) boxContainer2);
      return (Control) panelContainer;
    }
    ContainerButton containerButton = new ContainerButton();
    ((Control) containerButton).MinSize = new Vector2(108f, 96f);
    ((Control) containerButton).ToolTip = str2;
    PanelContainer panelContainer1 = new PanelContainer();
    panelContainer1.PanelOverride = (StyleBox) styleBoxFlat;
    ((Control) panelContainer1).HorizontalExpand = true;
    ((Control) panelContainer1).VerticalExpand = true;
    PanelContainer panelContainer2 = panelContainer1;
    ((Control) panelContainer2).AddChild((Control) boxContainer2);
    ((Control) containerButton).AddChild((Control) panelContainer2);
    string key = CivLoadoutKeys.Item(slot, proto);
    bool wasTaken = taken;
    ((BaseButton) containerButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this._system.SetItem(this._selectedFaction, this._selectedClass, key, wasTaken);
      this.RebuildContent();
    });
    return (Control) containerButton;
  }

  private Control MakeBonusCard(string proto, int count, bool taken)
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalAlignment = (Control.HAlignment) 2;
    boxContainer1.SeparationOverride = new int?(2);
    ((Control) boxContainer1).Margin = new Thickness(2f);
    BoxContainer boxContainer2 = boxContainer1;
    ((Control) boxContainer2).AddChild(this.MakeSprite(proto, taken, 84f));
    string str = this.ProtoName(proto);
    if (count > 1)
      str = Loc.GetString("civ-loadout-editor-item-count", new (string, object)[2]
      {
        ("name", (object) str),
        (nameof (count), (object) count)
      });
    BoxContainer boxContainer3 = boxContainer2;
    Label label = new Label();
    label.Text = str;
    label.FontColorOverride = new Color?(taken ? CivLoadoutEditorControl.BonusAccent : CivLoadoutEditorControl.TextMuted);
    ((Control) label).HorizontalAlignment = (Control.HAlignment) 2;
    label.ClipText = true;
    ((Control) label).MaxWidth = 100f;
    ((Control) boxContainer3).AddChild((Control) label);
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat()
    {
      BackgroundColor = taken ? CivLoadoutEditorControl.PanelBg : CivLoadoutEditorControl.DisabledTint,
      BorderColor = taken ? CivLoadoutEditorControl.BonusAccent : CivLoadoutEditorControl.BorderLight,
      BorderThickness = new Thickness(2f)
    };
    ContainerButton containerButton = new ContainerButton();
    ((Control) containerButton).MinSize = new Vector2(108f, 96f);
    ((Control) containerButton).ToolTip = this.ItemTooltip(proto);
    PanelContainer panelContainer1 = new PanelContainer();
    panelContainer1.PanelOverride = (StyleBox) styleBoxFlat;
    ((Control) panelContainer1).HorizontalExpand = true;
    ((Control) panelContainer1).VerticalExpand = true;
    PanelContainer panelContainer2 = panelContainer1;
    ((Control) panelContainer2).AddChild((Control) boxContainer2);
    ((Control) containerButton).AddChild((Control) panelContainer2);
    string key = CivLoadoutKeys.Item("bonus", proto);
    bool wasTaken = taken;
    ((BaseButton) containerButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this._system.SetItem(this._selectedFaction, this._selectedClass, key, wasTaken);
      this.RebuildContent();
    });
    return (Control) containerButton;
  }

  private Control MakeSprite(string proto, bool bright, float size = 64f)
  {
    EntityPrototypeView entityPrototypeView = new EntityPrototypeView(new EntProtoId?(EntProtoId.op_Implicit(proto)), this._entityManager);
    ((Control) entityPrototypeView).MinSize = new Vector2(size, size);
    ((SpriteView) entityPrototypeView).Stretch = (SpriteView.StretchMode) 1;
    ((Control) entityPrototypeView).HorizontalAlignment = (Control.HAlignment) 2;
    Color color;
    if (!bright)
    {
      Color white = Color.White;
      color = ((Color) ref white).WithAlpha(0.3f);
    }
    else
      color = Color.White;
    ((Control) entityPrototypeView).Modulate = color;
    return (Control) entityPrototypeView;
  }

  private Label MakeSectionLabel(string text)
  {
    Label label = new Label();
    label.Text = text;
    label.FontColorOverride = new Color?(CivLoadoutEditorControl.Accent);
    ((Control) label).Margin = new Thickness(0.0f, 6f, 0.0f, 2f);
    return label;
  }

  private string SlotName(string slot) => Loc.GetString("civ-loadout-slot-" + slot);

  private string ProtoName(string proto)
  {
    EntityPrototype entityPrototype;
    return !this._prototype.TryIndex<EntityPrototype>(proto, ref entityPrototype) ? proto : entityPrototype.Name;
  }

  private string ItemTooltip(string proto)
  {
    EntityPrototype entityPrototype;
    if (!this._prototype.TryIndex<EntityPrototype>(proto, ref entityPrototype))
      return proto;
    return !string.IsNullOrWhiteSpace(entityPrototype.Description) ? $"{entityPrototype.Name}\n\n{entityPrototype.Description}" : entityPrototype.Name;
  }

  private static Dictionary<string, string> ParseSelections(List<string>? list)
  {
    Dictionary<string, string> selections = new Dictionary<string, string>();
    if (list == null)
      return selections;
    foreach (string entry in list)
    {
      string slot;
      string proto;
      if (CivLoadoutKeys.TryParseSlotChoice(entry, out slot, out proto))
        selections[slot] = proto;
    }
    return selections;
  }

  private static IEnumerable<string> OrderedSlots(IEnumerable<string> slots)
  {
    HashSet<string> set = new HashSet<string>(slots);
    string[] strArray = CivLoadoutEditorControl.SlotOrder;
    for (int index = 0; index < strArray.Length; ++index)
    {
      string str = strArray[index];
      if (set.Remove(str))
        yield return str;
    }
    strArray = (string[]) null;
    foreach (string str in set)
      yield return str;
  }

  public void Cleanup()
  {
    this._system.StateUpdated -= new Action<CivLoadoutStateEvent>(this.OnStateUpdated);
  }
}
