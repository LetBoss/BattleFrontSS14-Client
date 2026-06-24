using System;
using System.Collections.Generic;
using Content.Client.Popups;
using Content.Client.UserInterface.Controls;
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

namespace Content.Client.RCD;

public sealed class RCDMenuBoundUserInterface : BoundUserInterface
{
	private const string TopLevelActionCategory = "Main";

	private static readonly Dictionary<string, (string Tooltip, SpriteSpecifier Sprite)> PrototypesGroupingInfo = new Dictionary<string, (string, SpriteSpecifier)>
	{
		["WallsAndFlooring"] = ("rcd-component-walls-and-flooring", (SpriteSpecifier)new Texture(new ResPath("/Textures/Interface/Radial/RCD/walls_and_flooring.png"))),
		["WindowsAndGrilles"] = ("rcd-component-windows-and-grilles", (SpriteSpecifier)new Texture(new ResPath("/Textures/Interface/Radial/RCD/windows_and_grilles.png"))),
		["Airlocks"] = ("rcd-component-airlocks", (SpriteSpecifier)new Texture(new ResPath("/Textures/Interface/Radial/RCD/airlocks.png"))),
		["Electrical"] = ("rcd-component-electrical", (SpriteSpecifier)new Texture(new ResPath("/Textures/Interface/Radial/RCD/multicoil.png"))),
		["Lighting"] = ("rcd-component-lighting", (SpriteSpecifier)new Texture(new ResPath("/Textures/Interface/Radial/RCD/lighting.png")))
	};

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private ISharedPlayerManager _playerManager;

	private SimpleRadialMenu? _menu;

	public RCDMenuBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<RCDMenuBoundUserInterface>(this);
	}

	protected override void Open()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		RCDComponent rCDComponent = default(RCDComponent);
		if (base.EntMan.TryGetComponent<RCDComponent>(((BoundUserInterface)this).Owner, ref rCDComponent))
		{
			_menu = BoundUserInterfaceExt.CreateWindow<SimpleRadialMenu>((BoundUserInterface)(object)this);
			_menu.Track(((BoundUserInterface)this).Owner);
			IEnumerable<RadialMenuOption> models = ConvertToButtons(rCDComponent.AvailablePrototypes);
			_menu.SetButtons(models);
			_menu.OpenOverMouseScreenPosition();
		}
	}

	private IEnumerable<RadialMenuOption> ConvertToButtons(HashSet<ProtoId<RCDPrototype>> prototypes)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, List<RadialMenuActionOption>> dictionary = new Dictionary<string, List<RadialMenuActionOption>>();
		ValueList<RadialMenuActionOption> val = default(ValueList<RadialMenuActionOption>);
		foreach (ProtoId<RCDPrototype> prototype in prototypes)
		{
			RCDPrototype rCDPrototype = _prototypeManager.Index<RCDPrototype>(prototype);
			(string, SpriteSpecifier) value;
			if (rCDPrototype.Category == "Main")
			{
				RadialMenuActionOption<RCDPrototype> radialMenuActionOption = new RadialMenuActionOption<RCDPrototype>(HandleMenuOptionClick, rCDPrototype)
				{
					Sprite = rCDPrototype.Sprite,
					ToolTip = GetTooltip(rCDPrototype)
				};
				val.Add((RadialMenuActionOption)radialMenuActionOption);
			}
			else if (PrototypesGroupingInfo.TryGetValue(rCDPrototype.Category, out value))
			{
				if (!dictionary.TryGetValue(rCDPrototype.Category, out var value2))
				{
					value2 = new List<RadialMenuActionOption>();
					dictionary.Add(rCDPrototype.Category, value2);
				}
				RadialMenuActionOption<RCDPrototype> item = new RadialMenuActionOption<RCDPrototype>(HandleMenuOptionClick, rCDPrototype)
				{
					Sprite = rCDPrototype.Sprite,
					ToolTip = GetTooltip(rCDPrototype)
				};
				value2.Add(item);
			}
		}
		RadialMenuOption[] array = new RadialMenuOption[dictionary.Count + val.Count];
		int num = 0;
		foreach (KeyValuePair<string, List<RadialMenuActionOption>> item2 in dictionary)
		{
			item2.Deconstruct(out var key, out var value3);
			string key2 = key;
			List<RadialMenuActionOption> nested = value3;
			(string, SpriteSpecifier) tuple = PrototypesGroupingInfo[key2];
			array[num] = new RadialMenuNestedLayerOption(nested)
			{
				Sprite = tuple.Item2,
				ToolTip = Loc.GetString(tuple.Item1)
			};
			num++;
		}
		Enumerator<RadialMenuActionOption> enumerator3 = val.GetEnumerator();
		try
		{
			while (enumerator3.MoveNext())
			{
				RadialMenuActionOption current2 = enumerator3.Current;
				array[num] = current2;
				num++;
			}
			return array;
		}
		finally
		{
			((IDisposable)enumerator3/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private void HandleMenuOptionClick(RCDPrototype proto)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new RCDSystemMessage(ProtoId<RCDPrototype>.op_Implicit(proto.ID)));
		ICommonSession localSession = _playerManager.LocalSession;
		if (localSession == null || !localSession.AttachedEntity.HasValue)
		{
			return;
		}
		string message = Loc.GetString("rcd-component-change-mode", new(string, object)[1] { ("mode", Loc.GetString(proto.SetName)) });
		RcdMode mode = proto.Mode;
		if (mode - 2 <= RcdMode.Deconstruct)
		{
			string item = Loc.GetString(proto.SetName);
			EntityPrototype val = default(EntityPrototype);
			if (proto.Prototype != null && _prototypeManager.TryIndex(EntProtoId.op_Implicit(proto.Prototype), ref val))
			{
				item = val.Name;
			}
			message = Loc.GetString("rcd-component-change-build-mode", new(string, object)[1] { ("name", item) });
		}
		base.EntMan.System<PopupSystem>().PopupClient(message, ((BoundUserInterface)this).Owner, _playerManager.LocalSession.AttachedEntity);
	}

	private string GetTooltip(RCDPrototype proto)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		RcdMode mode = proto.Mode;
		bool flag = mode - 2 <= RcdMode.Deconstruct;
		EntityPrototype val = default(EntityPrototype);
		string text = ((!flag || proto.Prototype == null || !_prototypeManager.TryIndex(EntProtoId.op_Implicit(proto.Prototype), ref val)) ? Loc.GetString(proto.SetName) : Loc.GetString(val.Name));
		return OopsConcat(char.ToUpper(text[0]).ToString(), text.Remove(0, 1));
	}

	private static string OopsConcat(string a, string b)
	{
		return a + b;
	}
}
