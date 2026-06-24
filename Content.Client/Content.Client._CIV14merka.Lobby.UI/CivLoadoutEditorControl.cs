using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

	private static readonly Color PanelBg = Color.FromHex((ReadOnlySpan<char>)"#1A1E24", (Color?)null);

	private static readonly Color PanelBgLight = Color.FromHex((ReadOnlySpan<char>)"#283040", (Color?)null);

	private static readonly Color BorderLight = Color.FromHex((ReadOnlySpan<char>)"#3A4553", (Color?)null);

	private static readonly Color Accent = Color.FromHex((ReadOnlySpan<char>)"#4CAF50", (Color?)null);

	private static readonly Color BonusAccent = Color.FromHex((ReadOnlySpan<char>)"#D9A441", (Color?)null);

	private static readonly Color TextPrimary = Color.FromHex((ReadOnlySpan<char>)"#FFFFFF", (Color?)null);

	private static readonly Color TextSecondary = Color.FromHex((ReadOnlySpan<char>)"#C0C6CF", (Color?)null);

	private static readonly Color TextMuted = Color.FromHex((ReadOnlySpan<char>)"#8A93A0", (Color?)null);

	private static readonly Color DisabledTint = Color.FromHex((ReadOnlySpan<char>)"#3A2222", (Color?)null);

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
		"jumpsuit", "head", "eyes", "mask", "outerClothing", "suitstorage", "back", "belt", "shoes", "gloves",
		"id", "ears", "pocket1", "pocket2"
	};

	private static readonly Dictionary<CivTdmClass, string> ClassIcons = new Dictionary<CivTdmClass, string>
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
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected O, but got Unknown
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Expected O, but got Unknown
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Expected O, but got Unknown
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Expected O, but got Unknown
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Expected O, but got Unknown
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Expected O, but got Unknown
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Expected O, but got Unknown
		IoCManager.InjectDependencies<CivLoadoutEditorControl>(this);
		_sprite = _entityManager.System<SpriteSystem>();
		_componentFactory = _entityManager.ComponentFactory;
		_system = _entityManager.System<CivLoadoutSystem>();
		((BoxContainer)this).Orientation = (LayoutOrientation)1;
		((BoxContainer)this).SeparationOverride = 8;
		((Control)this).HorizontalExpand = true;
		((Control)this).VerticalExpand = true;
		((Control)this).Margin = new Thickness(8f);
		foreach (CivFactionPrototype item in _prototype.EnumeratePrototypes<CivFactionPrototype>())
		{
			if (item.Loadouts.Count != 0 && !(item.ID == "Neutral"))
			{
				_factionIds.Add(item.ID);
			}
		}
		if (_factionIds.Count > 0)
		{
			_selectedFaction = _factionIds[0];
		}
		((Control)this).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-loadout-editor-faction"),
			FontColorOverride = TextMuted
		});
		_factionRow = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 8
		};
		((Control)this).AddChild((Control)(object)_factionRow);
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 10,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 6,
			MinSize = new Vector2(210f, 0f)
		};
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-loadout-editor-class"),
			FontColorOverride = TextMuted
		});
		_classList = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 4,
			HorizontalExpand = true
		};
		((Control)val2).AddChild((Control)(object)_classList);
		((Control)val).AddChild((Control)(object)val2);
		ScrollContainer val3 = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true
		};
		_content = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 4,
			HorizontalExpand = true
		};
		((Control)val3).AddChild((Control)(object)_content);
		((Control)val).AddChild((Control)(object)val3);
		((Control)this).AddChild((Control)(object)val);
		_system.StateUpdated += OnStateUpdated;
		Refresh();
	}

	private void OnStateUpdated(CivLoadoutStateEvent _)
	{
		RebuildContent();
	}

	private void Refresh()
	{
		BuildFactionRow();
		BuildClassList();
		RebuildContent();
	}

	private void BuildFactionRow()
	{
		((Control)_factionRow).RemoveAllChildren();
		CivFactionPrototype faction = default(CivFactionPrototype);
		foreach (string factionId in _factionIds)
		{
			if (_prototype.TryIndex<CivFactionPrototype>(factionId, ref faction))
			{
				ContainerButton val = MakeFactionTile(faction, factionId == _selectedFaction);
				string captured = factionId;
				((BaseButton)val).OnPressed += delegate
				{
					_selectedFaction = captured;
					Refresh();
				};
				((Control)_factionRow).AddChild((Control)(object)val);
			}
		}
	}

	private void BuildClassList()
	{
		((Control)_classList).RemoveAllChildren();
		CivTdmClass[] classOrder = ClassOrder;
		foreach (CivTdmClass civTdmClass in classOrder)
		{
			ContainerButton val = MakeClassRow(civTdmClass, civTdmClass == _selectedClass);
			CivTdmClass captured = civTdmClass;
			((BaseButton)val).OnPressed += delegate
			{
				_selectedClass = captured;
				Refresh();
			};
			((Control)_classList).AddChild((Control)(object)val);
		}
	}

	private void RebuildContent()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Expected O, but got Unknown
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Expected O, but got Unknown
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Expected O, but got Unknown
		((Control)_content).RemoveAllChildren();
		CivFactionPrototype civFactionPrototype = default(CivFactionPrototype);
		StartingGearPrototype gear = default(StartingGearPrototype);
		if (string.IsNullOrEmpty(_selectedFaction) || !_prototype.TryIndex<CivFactionPrototype>(_selectedFaction, ref civFactionPrototype) || !civFactionPrototype.Loadouts.TryGetValue(_selectedClass, out ProtoId<StartingGearPrototype> value) || !_prototype.TryIndex<StartingGearPrototype>(value, ref gear))
		{
			((Control)_content).AddChild((Control)new Label
			{
				Text = Loc.GetString("civ-loadout-editor-empty"),
				FontColorOverride = TextMuted
			});
			return;
		}
		((Control)_content).AddChild((Control)new Label
		{
			Text = civFactionPrototype.Name + " · " + CivTdmClassHelper.GetDisplayName(_selectedClass),
			FontColorOverride = TextPrimary,
			Margin = new Thickness(0f, 0f, 0f, 4f)
		});
		string key = CivLoadoutKeys.Combo(_selectedFaction, _selectedClass);
		CivLoadoutStateEvent state = _system.GetState();
		List<string> value2;
		HashSet<string> hashSet = (state.Disabled.TryGetValue(key, out value2) ? new HashSet<string>(value2) : new HashSet<string>());
		List<string> value3;
		Dictionary<string, string> dictionary = ParseSelections(state.Selections.TryGetValue(key, out value3) ? value3 : null);
		HashSet<string> hashSet2 = new HashSet<string>();
		List<(string, int)> list = new List<(string, int)>();
		foreach (string item5 in state.Owned)
		{
			if (CivLoadoutKeys.TryParseOwned(item5, out string faction, out int count, out string proto) && (string.IsNullOrEmpty(faction) || !(faction != _selectedFaction)))
			{
				if (count >= 1)
				{
					list.Add((proto, count));
				}
				else
				{
					hashSet2.Add(proto);
				}
			}
		}
		Dictionary<string, List<string>> effectiveSwaps = CivLoadoutManifest.GetEffectiveSwaps(gear, _selectedFaction, _selectedClass, hashSet2, _prototype, _componentFactory);
		List<CivLoadoutItemInfo> removableItems = CivLoadoutManifest.GetRemovableItems(gear, _prototype, _componentFactory, new HashSet<string>(effectiveSwaps.Keys));
		HashSet<string> hashSet3 = new HashSet<string>(removableItems.Select((CivLoadoutItemInfo r) => r.Key));
		if (effectiveSwaps.Count > 0)
		{
			((Control)_content).AddChild((Control)(object)MakeSectionLabel(Loc.GetString("civ-loadout-section-weapons")));
			foreach (string item6 in OrderedSlots(effectiveSwaps.Keys))
			{
				List<string> list2 = effectiveSwaps[item6];
				string value4;
				string current2 = ((dictionary.TryGetValue(item6, out value4) && list2.Contains(value4)) ? value4 : list2[0]);
				((Control)_content).AddChild(MakeSwapRow(item6, list2, current2));
			}
		}
		((Control)_content).AddChild((Control)(object)MakeSectionLabel(Loc.GetString("civ-loadout-section-gear")));
		GridContainer val = new GridContainer
		{
			Columns = 5
		};
		foreach (string item7 in OrderedSlots(gear.Equipment.Keys))
		{
			if (effectiveSwaps.ContainsKey(item7))
			{
				continue;
			}
			EntProtoId val2 = gear.Equipment[item7];
			string id = ((EntProtoId)(ref val2)).Id;
			if (!string.IsNullOrEmpty(id))
			{
				string item = CivLoadoutKeys.Item(item7, id);
				if (hashSet3.Contains(item))
				{
					((Control)val).AddChild(MakeItemCard(item7, id, 1, !hashSet.Contains(item), interactive: true));
				}
				else
				{
					((Control)val).AddChild(MakeItemCard(item7, id, 1, taken: true, interactive: false));
				}
			}
		}
		((Control)_content).AddChild((Control)(object)val);
		List<CivLoadoutItemInfo> list3 = removableItems.Where(delegate(CivLoadoutItemInfo it)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			string proto2 = it.Proto;
			EntProtoId val5 = gear.Equipment[it.Slot];
			return proto2 != ((EntProtoId)(ref val5)).Id;
		}).ToList();
		if (list3.Count > 0)
		{
			((Control)_content).AddChild((Control)(object)MakeSectionLabel(Loc.GetString("civ-loadout-section-carried")));
			GridContainer val3 = new GridContainer
			{
				Columns = 5
			};
			foreach (CivLoadoutItemInfo item8 in list3)
			{
				((Control)val3).AddChild(MakeItemCard(item8.Slot, item8.Proto, item8.Count, !hashSet.Contains(item8.Key), interactive: true));
			}
			((Control)_content).AddChild((Control)(object)val3);
		}
		if (list.Count <= 0)
		{
			return;
		}
		((Control)_content).AddChild((Control)(object)MakeSectionLabel(Loc.GetString("civ-loadout-section-bonus")));
		GridContainer val4 = new GridContainer
		{
			Columns = 5
		};
		foreach (var item9 in list)
		{
			string item2 = item9.Item1;
			int item3 = item9.Item2;
			string item4 = CivLoadoutKeys.Item("bonus", item2);
			((Control)val4).AddChild(MakeBonusCard(item2, item3, !hashSet.Contains(item4)));
		}
		((Control)_content).AddChild((Control)(object)val4);
	}

	private ContainerButton MakeFactionTile(CivFactionPrototype faction, bool selected)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Expected O, but got Unknown
		ContainerButton val = new ContainerButton
		{
			MinSize = new Vector2(130f, 46f)
		};
		PanelContainer val2 = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = (selected ? PanelBgLight : PanelBg),
				BorderColor = (selected ? faction.Color : BorderLight),
				BorderThickness = new Thickness((float)((!selected) ? 1 : 2))
			},
			HorizontalExpand = true,
			VerticalExpand = true
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 6,
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2,
			Margin = new Thickness(8f, 4f)
		};
		if (faction.Icon != null)
		{
			((Control)val3).AddChild((Control)new TextureRect
			{
				Texture = _sprite.Frame0(faction.Icon),
				Stretch = (StretchMode)7,
				MinSize = new Vector2(34f, 26f),
				VerticalAlignment = (VAlignment)2
			});
		}
		((Control)val3).AddChild((Control)new Label
		{
			Text = faction.Name,
			FontColorOverride = (selected ? TextPrimary : TextSecondary),
			VerticalAlignment = (VAlignment)2
		});
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		return val;
	}

	private ContainerButton MakeClassRow(CivTdmClass cls, bool selected)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Expected O, but got Unknown
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		ContainerButton val = new ContainerButton
		{
			HorizontalExpand = true,
			MinSize = new Vector2(0f, 50f)
		};
		PanelContainer val2 = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = (selected ? PanelBgLight : PanelBg),
				BorderColor = (selected ? Accent : BorderLight),
				BorderThickness = new Thickness((float)((!selected) ? 1 : 2))
			},
			HorizontalExpand = true,
			VerticalExpand = true
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 8,
			VerticalAlignment = (VAlignment)2,
			Margin = new Thickness(8f, 4f)
		};
		TextureResource val4 = default(TextureResource);
		if (ClassIcons.TryGetValue(cls, out string value) && _resourceCache.TryGetResource<TextureResource>(new ResPath("/Textures/_CIV14merka/Interface/Loadout/" + value + ".png"), ref val4))
		{
			((Control)val3).AddChild((Control)new TextureRect
			{
				Texture = val4.Texture,
				Stretch = (StretchMode)7,
				MinSize = new Vector2(40f, 40f),
				VerticalAlignment = (VAlignment)2
			});
		}
		((Control)val3).AddChild((Control)new Label
		{
			Text = CivTdmClassHelper.GetDisplayName(cls),
			FontColorOverride = (selected ? TextPrimary : TextSecondary),
			VerticalAlignment = (VAlignment)2
		});
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		return val;
	}

	private Control MakeSwapRow(string slot, List<string> options, string current)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Expected O, but got Unknown
		PanelContainer val = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = PanelBg,
				BorderColor = BorderLight,
				BorderThickness = new Thickness(1f)
			},
			HorizontalExpand = true
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 4,
			Margin = new Thickness(6f)
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 6
		};
		((Control)val3).AddChild((Control)new Label
		{
			Text = SlotName(slot),
			FontColorOverride = TextPrimary
		});
		((Control)val3).AddChild((Control)new Label
		{
			Text = $"({options.Count})",
			FontColorOverride = Accent
		});
		((Control)val2).AddChild((Control)(object)val3);
		BoxContainer val4 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 6
		};
		foreach (string option in options)
		{
			ContainerButton val5 = MakeOptionTile(option, option == current);
			string captured = option;
			((BaseButton)val5).OnPressed += delegate
			{
				_system.SetSlotChoice(_selectedFaction, _selectedClass, slot, captured);
				RebuildContent();
			};
			((Control)val4).AddChild((Control)(object)val5);
		}
		((Control)val2).AddChild((Control)(object)val4);
		((Control)val).AddChild((Control)(object)val2);
		return (Control)(object)val;
	}

	private ContainerButton MakeOptionTile(string proto, bool selected)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		ContainerButton val = new ContainerButton
		{
			MinSize = new Vector2(104f, 92f),
			ToolTip = ItemTooltip(proto)
		};
		PanelContainer val2 = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = (selected ? PanelBgLight : PanelBg),
				BorderColor = (selected ? Accent : BorderLight),
				BorderThickness = new Thickness((float)((!selected) ? 1 : 2))
			},
			HorizontalExpand = true,
			VerticalExpand = true
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalAlignment = (HAlignment)2,
			SeparationOverride = 2
		};
		((Control)val3).AddChild(MakeSprite(proto, bright: true));
		((Control)val3).AddChild((Control)new Label
		{
			Text = ProtoName(proto),
			FontColorOverride = (selected ? TextPrimary : TextSecondary),
			HorizontalAlignment = (HAlignment)2,
			ClipText = true,
			MaxWidth = 100f
		});
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		return val;
	}

	private Control MakeItemCard(string slot, string proto, int count, bool taken, bool interactive)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Expected O, but got Unknown
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Expected O, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalAlignment = (HAlignment)2,
			SeparationOverride = 2,
			Margin = new Thickness(4f)
		};
		((Control)val).AddChild(MakeSprite(proto, taken));
		string text = ProtoName(proto);
		if (count > 1)
		{
			text = Loc.GetString("civ-loadout-editor-item-count", new(string, object)[2]
			{
				("name", text),
				("count", count)
			});
		}
		((Control)val).AddChild((Control)new Label
		{
			Text = text,
			FontColorOverride = (taken ? TextSecondary : TextMuted),
			HorizontalAlignment = (HAlignment)2,
			ClipText = true,
			MaxWidth = 100f
		});
		StyleBoxFlat panelOverride = new StyleBoxFlat
		{
			BackgroundColor = (taken ? PanelBg : DisabledTint),
			BorderColor = BorderLight,
			BorderThickness = new Thickness(1f)
		};
		string toolTip = ItemTooltip(proto);
		if (!interactive)
		{
			PanelContainer val2 = new PanelContainer
			{
				PanelOverride = (StyleBox)(object)panelOverride,
				MinSize = new Vector2(108f, 96f),
				MouseFilter = (MouseFilterMode)0,
				ToolTip = toolTip
			};
			((Control)val2).AddChild((Control)(object)val);
			return (Control)val2;
		}
		ContainerButton val3 = new ContainerButton
		{
			MinSize = new Vector2(108f, 96f),
			ToolTip = toolTip
		};
		PanelContainer val4 = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)panelOverride,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((Control)val4).AddChild((Control)(object)val);
		((Control)val3).AddChild((Control)(object)val4);
		string key = CivLoadoutKeys.Item(slot, proto);
		bool wasTaken = taken;
		((BaseButton)val3).OnPressed += delegate
		{
			_system.SetItem(_selectedFaction, _selectedClass, key, wasTaken);
			RebuildContent();
		};
		return (Control)val3;
	}

	private Control MakeBonusCard(string proto, int count, bool taken)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalAlignment = (HAlignment)2,
			SeparationOverride = 2,
			Margin = new Thickness(2f)
		};
		((Control)val).AddChild(MakeSprite(proto, taken, 84f));
		string text = ProtoName(proto);
		if (count > 1)
		{
			text = Loc.GetString("civ-loadout-editor-item-count", new(string, object)[2]
			{
				("name", text),
				("count", count)
			});
		}
		((Control)val).AddChild((Control)new Label
		{
			Text = text,
			FontColorOverride = (taken ? BonusAccent : TextMuted),
			HorizontalAlignment = (HAlignment)2,
			ClipText = true,
			MaxWidth = 100f
		});
		StyleBoxFlat panelOverride = new StyleBoxFlat
		{
			BackgroundColor = (taken ? PanelBg : DisabledTint),
			BorderColor = (taken ? BonusAccent : BorderLight),
			BorderThickness = new Thickness(2f)
		};
		ContainerButton val2 = new ContainerButton
		{
			MinSize = new Vector2(108f, 96f),
			ToolTip = ItemTooltip(proto)
		};
		PanelContainer val3 = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)panelOverride,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((Control)val3).AddChild((Control)(object)val);
		((Control)val2).AddChild((Control)(object)val3);
		string key = CivLoadoutKeys.Item("bonus", proto);
		bool wasTaken = taken;
		((BaseButton)val2).OnPressed += delegate
		{
			_system.SetItem(_selectedFaction, _selectedClass, key, wasTaken);
			RebuildContent();
		};
		return (Control)val2;
	}

	private Control MakeSprite(string proto, bool bright, float size = 64f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		EntityPrototypeView val = new EntityPrototypeView((EntProtoId?)EntProtoId.op_Implicit(proto), _entityManager)
		{
			MinSize = new Vector2(size, size),
			Stretch = (StretchMode)1,
			HorizontalAlignment = (HAlignment)2
		};
		Color modulate;
		if (!bright)
		{
			Color white = Color.White;
			modulate = ((Color)(ref white)).WithAlpha(0.3f);
		}
		else
		{
			modulate = Color.White;
		}
		((Control)val).Modulate = modulate;
		return (Control)val;
	}

	private Label MakeSectionLabel(string text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		return new Label
		{
			Text = text,
			FontColorOverride = Accent,
			Margin = new Thickness(0f, 6f, 0f, 2f)
		};
	}

	private string SlotName(string slot)
	{
		return Loc.GetString("civ-loadout-slot-" + slot);
	}

	private string ProtoName(string proto)
	{
		EntityPrototype val = default(EntityPrototype);
		if (!_prototype.TryIndex<EntityPrototype>(proto, ref val))
		{
			return proto;
		}
		return val.Name;
	}

	private string ItemTooltip(string proto)
	{
		EntityPrototype val = default(EntityPrototype);
		if (!_prototype.TryIndex<EntityPrototype>(proto, ref val))
		{
			return proto;
		}
		if (!string.IsNullOrWhiteSpace(val.Description))
		{
			return val.Name + "\n\n" + val.Description;
		}
		return val.Name;
	}

	private static Dictionary<string, string> ParseSelections(List<string>? list)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (list == null)
		{
			return dictionary;
		}
		foreach (string item in list)
		{
			if (CivLoadoutKeys.TryParseSlotChoice(item, out string slot, out string proto))
			{
				dictionary[slot] = proto;
			}
		}
		return dictionary;
	}

	private static IEnumerable<string> OrderedSlots(IEnumerable<string> slots)
	{
		HashSet<string> set = new HashSet<string>(slots);
		string[] slotOrder = SlotOrder;
		foreach (string text in slotOrder)
		{
			if (set.Remove(text))
			{
				yield return text;
			}
		}
		foreach (string item in set)
		{
			yield return item;
		}
	}

	public void Cleanup()
	{
		_system.StateUpdated -= OnStateUpdated;
	}
}
