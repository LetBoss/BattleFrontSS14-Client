using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client.Administration.Managers;
using Content.Client.ContextMenu.UI;
using Content.Client.Decals;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Client.Verbs;
using Content.Shared.Decals;
using Content.Shared.Input;
using Content.Shared.Maps;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Placement;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Mapping;

public sealed class MappingState : GameplayStateBase
{
	public enum CursorState
	{
		None,
		Pick,
		Delete
	}

	[Dependency]
	private IClientAdminManager _admin;

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IEntityNetworkManager _entityNetwork;

	[Dependency]
	private IInputManager _input;

	[Dependency]
	private ILogManager _log;

	[Dependency]
	private IMapManager _mapMan;

	[Dependency]
	private MappingManager _mapping;

	[Dependency]
	private IOverlayManager _overlays;

	[Dependency]
	private IPlacementManager _placement;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IResourceCache _resources;

	[Dependency]
	private IGameTiming _timing;

	private EntityMenuUIController _entityMenuController;

	private DecalPlacementSystem _decal;

	private SpriteSystem _sprite;

	private TransformSystem _transform;

	private VerbSystem _verbs;

	private readonly ISawmill _sawmill;

	private readonly GameplayStateLoadController _loadController;

	private bool _setup;

	private readonly List<MappingPrototype> _allPrototypes = new List<MappingPrototype>();

	private readonly Dictionary<IPrototype, MappingPrototype> _allPrototypesDict = new Dictionary<IPrototype, MappingPrototype>();

	private readonly Dictionary<Type, Dictionary<string, MappingPrototype>> _idDict = new Dictionary<Type, Dictionary<string, MappingPrototype>>();

	private readonly List<MappingPrototype> _prototypes = new List<MappingPrototype>();

	private (TimeSpan At, MappingSpawnButton Button)? _lastClicked;

	private Control? _scrollTo;

	private bool _updatePlacement;

	private bool _updateEraseDecal;

	private MappingScreen Screen => (MappingScreen)(object)UserInterfaceManager.ActiveScreen;

	private MainViewport Viewport => UserInterfaceManager.ActiveScreen.GetWidget<MainViewport>();

	public CursorState State { get; set; }

	public MappingState()
	{
		IoCManager.InjectDependencies<MappingState>(this);
		_sawmill = _log.GetSawmill("mapping");
		_loadController = UserInterfaceManager.GetUIController<GameplayStateLoadController>();
	}

	protected override void Startup()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Expected O, but got Unknown
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Expected O, but got Unknown
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Expected O, but got Unknown
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Expected O, but got Unknown
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Expected O, but got Unknown
		//IL_0287: Expected O, but got Unknown
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Expected O, but got Unknown
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Expected O, but got Unknown
		//IL_02af: Expected O, but got Unknown
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Expected O, but got Unknown
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Expected O, but got Unknown
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Expected O, but got Unknown
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Expected O, but got Unknown
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Expected O, but got Unknown
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Expected O, but got Unknown
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Expected O, but got Unknown
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Expected O, but got Unknown
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Expected O, but got Unknown
		EnsureSetup();
		base.Startup();
		UserInterfaceManager.LoadScreen<MappingScreen>();
		_loadController.LoadScreen();
		IInputCmdContext context = _input.Contexts.GetContext("common");
		context.AddFunction(ContentKeyFunctions.MappingUnselect);
		context.AddFunction(ContentKeyFunctions.SaveMap);
		context.AddFunction(ContentKeyFunctions.MappingEnablePick);
		context.AddFunction(ContentKeyFunctions.MappingEnableDelete);
		context.AddFunction(ContentKeyFunctions.MappingPick);
		context.AddFunction(ContentKeyFunctions.MappingRemoveDecal);
		context.AddFunction(ContentKeyFunctions.MappingCancelEraseDecal);
		context.AddFunction(ContentKeyFunctions.MappingOpenContextMenu);
		Screen.DecalSystem = _decal;
		Screen.Prototypes.SearchBar.OnTextChanged += OnSearch;
		((BaseButton)Screen.Prototypes.CollapseAllButton).OnPressed += OnCollapseAll;
		((BaseButton)Screen.Prototypes.ClearSearchButton).OnPressed += OnClearSearch;
		MappingPrototypeList prototypes = Screen.Prototypes;
		prototypes.GetPrototypeData = (Action<IPrototype, List<Texture>>)Delegate.Combine(prototypes.GetPrototypeData, new Action<IPrototype, List<Texture>>(OnGetData));
		Screen.Prototypes.SelectionChanged += OnSelected;
		Screen.Prototypes.CollapseToggled += OnCollapseToggled;
		((BaseButton)Screen.Pick).OnPressed += OnPickPressed;
		((BaseButton)Screen.Delete).OnPressed += OnDeletePressed;
		((BaseButton)Screen.EntityReplaceButton).OnToggled += OnEntityReplacePressed;
		Screen.EntityPlacementMode.OnItemSelected += OnEntityPlacementSelected;
		((BaseButton)Screen.EraseEntityButton).OnToggled += OnEraseEntityPressed;
		((BaseButton)Screen.EraseDecalButton).OnToggled += OnEraseDecalPressed;
		_placement.PlacementChanged += OnPlacementChanged;
		CommandBinds.Builder.Bind(ContentKeyFunctions.MappingUnselect, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleMappingUnselect), true, true)).Bind(ContentKeyFunctions.SaveMap, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleSaveMap), true, true)).Bind(ContentKeyFunctions.MappingEnablePick, (InputCmdHandler)new PointerStateInputCmdHandler(new PointerInputCmdDelegate(HandleEnablePick), new PointerInputCmdDelegate(HandleDisablePick), true))
			.Bind(ContentKeyFunctions.MappingEnableDelete, (InputCmdHandler)new PointerStateInputCmdHandler(new PointerInputCmdDelegate(HandleEnableDelete), new PointerInputCmdDelegate(HandleDisableDelete), true))
			.Bind(ContentKeyFunctions.MappingPick, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate(HandlePick), true, true))
			.Bind(ContentKeyFunctions.MappingRemoveDecal, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate(HandleEditorCancelPlace), true, true))
			.Bind(ContentKeyFunctions.MappingCancelEraseDecal, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleCancelEraseDecal), true, true))
			.Bind(ContentKeyFunctions.MappingOpenContextMenu, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleOpenContextMenu), true, true))
			.Register<MappingState>();
		_overlays.AddOverlay((Overlay)(object)new MappingOverlay(this));
		_prototypeManager.PrototypesReloaded += OnPrototypesReloaded;
		Screen.Prototypes.UpdateVisible(_prototypes);
	}

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs obj)
	{
		if (obj.WasModified<EntityPrototype>() || obj.WasModified<ContentTileDefinition>() || obj.WasModified<DecalPrototype>())
		{
			ReloadPrototypes();
		}
	}

	private bool HandleOpenContextMenu(in PointerInputCmdArgs args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Deselect();
		MapCoordinates targetPos = ((SharedTransformSystem)_transform).ToMapCoordinates(args.Coordinates, true);
		if (_verbs.TryGetEntityMenuEntities(targetPos, out List<EntityUid> entities))
		{
			_entityMenuController.OpenRootMenu(entities);
		}
		return true;
	}

	protected override void Shutdown()
	{
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		CommandBinds.Unregister<MappingState>();
		Screen.Prototypes.SearchBar.OnTextChanged -= OnSearch;
		((BaseButton)Screen.Prototypes.CollapseAllButton).OnPressed -= OnCollapseAll;
		((BaseButton)Screen.Prototypes.ClearSearchButton).OnPressed -= OnClearSearch;
		MappingPrototypeList prototypes = Screen.Prototypes;
		prototypes.GetPrototypeData = (Action<IPrototype, List<Texture>>)Delegate.Remove(prototypes.GetPrototypeData, new Action<IPrototype, List<Texture>>(OnGetData));
		Screen.Prototypes.SelectionChanged -= OnSelected;
		Screen.Prototypes.CollapseToggled -= OnCollapseToggled;
		((BaseButton)Screen.Pick).OnPressed -= OnPickPressed;
		((BaseButton)Screen.Delete).OnPressed -= OnDeletePressed;
		((BaseButton)Screen.EntityReplaceButton).OnToggled -= OnEntityReplacePressed;
		Screen.EntityPlacementMode.OnItemSelected -= OnEntityPlacementSelected;
		((BaseButton)Screen.EraseEntityButton).OnToggled -= OnEraseEntityPressed;
		((BaseButton)Screen.EraseDecalButton).OnToggled -= OnEraseDecalPressed;
		_placement.PlacementChanged -= OnPlacementChanged;
		_prototypeManager.PrototypesReloaded -= OnPrototypesReloaded;
		UserInterfaceManager.ClearWindows();
		_loadController.UnloadScreen();
		UserInterfaceManager.UnloadScreen();
		IInputCmdContext context = _input.Contexts.GetContext("common");
		context.RemoveFunction(ContentKeyFunctions.MappingUnselect);
		context.RemoveFunction(ContentKeyFunctions.SaveMap);
		context.RemoveFunction(ContentKeyFunctions.MappingEnablePick);
		context.RemoveFunction(ContentKeyFunctions.MappingEnableDelete);
		context.RemoveFunction(ContentKeyFunctions.MappingPick);
		context.RemoveFunction(ContentKeyFunctions.MappingRemoveDecal);
		context.RemoveFunction(ContentKeyFunctions.MappingCancelEraseDecal);
		context.RemoveFunction(ContentKeyFunctions.MappingOpenContextMenu);
		_overlays.RemoveOverlay<MappingOverlay>();
		base.Shutdown();
	}

	private void EnsureSetup()
	{
		if (!_setup)
		{
			_setup = true;
			_entityMenuController = UserInterfaceManager.GetUIController<EntityMenuUIController>();
			_decal = _entityManager.System<DecalPlacementSystem>();
			_sprite = _entityManager.System<SpriteSystem>();
			_transform = _entityManager.System<TransformSystem>();
			_verbs = _entityManager.System<VerbSystem>();
			ReloadPrototypes();
		}
	}

	private void ReloadPrototypes()
	{
		MappingPrototype mappingPrototype = new MappingPrototype(null, Loc.GetString("mapping-entities"))
		{
			Children = new List<MappingPrototype>()
		};
		_prototypes.Add(mappingPrototype);
		Dictionary<string, MappingPrototype> dictionary = new Dictionary<string, MappingPrototype>();
		foreach (EntityPrototype item in _prototypeManager.EnumeratePrototypes<EntityPrototype>())
		{
			Register<EntityPrototype>(item, item.ID, mappingPrototype);
		}
		Sort(dictionary, mappingPrototype);
		dictionary.Clear();
		MappingPrototype mappingPrototype2 = new MappingPrototype(null, Loc.GetString("mapping-tiles"))
		{
			Children = new List<MappingPrototype>()
		};
		_prototypes.Add(mappingPrototype2);
		foreach (ContentTileDefinition item2 in _prototypeManager.EnumeratePrototypes<ContentTileDefinition>())
		{
			Register(item2, item2.ID, mappingPrototype2);
		}
		Sort(dictionary, mappingPrototype2);
		dictionary.Clear();
		MappingPrototype mappingPrototype3 = new MappingPrototype(null, Loc.GetString("mapping-decals"))
		{
			Children = new List<MappingPrototype>()
		};
		_prototypes.Add(mappingPrototype3);
		foreach (DecalPrototype item3 in _prototypeManager.EnumeratePrototypes<DecalPrototype>())
		{
			Register(item3, item3.ID, mappingPrototype3);
		}
		Sort(dictionary, mappingPrototype3);
		dictionary.Clear();
	}

	private void Sort(Dictionary<string, MappingPrototype> prototypes, MappingPrototype topLevel)
	{
		if (topLevel.Children == null)
		{
			topLevel.Children = new List<MappingPrototype>();
		}
		foreach (MappingPrototype value in prototypes.Values)
		{
			if (value.Parents == null && value != topLevel)
			{
				value.Parents = new List<MappingPrototype> { topLevel };
				topLevel.Children.Add(value);
			}
			value.Parents?.Sort(Compare);
			value.Children?.Sort(Compare);
		}
		topLevel.Children.Sort(Compare);
		static int Compare(MappingPrototype a, MappingPrototype b)
		{
			return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
		}
	}

	private MappingPrototype? Register<T>(T? prototype, string id, MappingPrototype topLevel) where T : class, IPrototype, IInheritingPrototype
	{
		if (prototype == null && _prototypeManager.TryIndex<T>(id, ref prototype))
		{
			EntityPrototype val = (EntityPrototype)(object)((prototype is EntityPrototype) ? prototype : null);
			if (val != null && (val.HideSpawnMenu || val.Abstract))
			{
				prototype = null;
			}
		}
		if (prototype == null)
		{
			MappingDataNode val2 = default(MappingDataNode);
			if (!_prototypeManager.TryGetMapping(typeof(T), id, ref val2))
			{
				_sawmill.Error("No T found with id " + id);
				return null;
			}
			Dictionary<string, MappingPrototype> orNew = Extensions.GetOrNew<Type, Dictionary<string, MappingPrototype>>(_idDict, typeof(T));
			if (orNew.TryGetValue(id, out var value))
			{
				return value;
			}
			ValueDataNode val3 = default(ValueDataNode);
			string text = (val2.TryGet<ValueDataNode>("name", ref val3) ? val3.Value : id);
			ValueDataNode val4 = default(ValueDataNode);
			if (val2.TryGet<ValueDataNode>("suffix", ref val4))
			{
				text = text + " [" + val4.Value + "]";
			}
			value = new MappingPrototype((IPrototype?)(object)prototype, text);
			_allPrototypes.Add(value);
			orNew.Add(id, value);
			ValueDataNode val5 = default(ValueDataNode);
			SequenceDataNode source = default(SequenceDataNode);
			if (val2.TryGet<ValueDataNode>("parent", ref val5))
			{
				MappingPrototype mappingPrototype = Register<T>(null, val5.Value, topLevel);
				if (mappingPrototype != null)
				{
					MappingPrototype mappingPrototype2 = value;
					if (mappingPrototype2.Parents == null)
					{
						mappingPrototype2.Parents = new List<MappingPrototype>();
					}
					value.Parents.Add(mappingPrototype);
					mappingPrototype2 = mappingPrototype;
					if (mappingPrototype2.Children == null)
					{
						mappingPrototype2.Children = new List<MappingPrototype>();
					}
					mappingPrototype.Children.Add(value);
				}
			}
			else if (val2.TryGet<SequenceDataNode>("parent", ref source))
			{
				foreach (ValueDataNode item in ((IEnumerable)source).Cast<ValueDataNode>())
				{
					MappingPrototype mappingPrototype3 = Register<T>(null, item.Value, topLevel);
					if (mappingPrototype3 != null)
					{
						MappingPrototype mappingPrototype2 = value;
						if (mappingPrototype2.Parents == null)
						{
							mappingPrototype2.Parents = new List<MappingPrototype>();
						}
						value.Parents.Add(mappingPrototype3);
						mappingPrototype2 = mappingPrototype3;
						if (mappingPrototype2.Children == null)
						{
							mappingPrototype2.Children = new List<MappingPrototype>();
						}
						mappingPrototype3.Children.Add(value);
					}
				}
			}
			else
			{
				MappingPrototype mappingPrototype2 = topLevel;
				if (mappingPrototype2.Children == null)
				{
					mappingPrototype2.Children = new List<MappingPrototype>();
				}
				topLevel.Children.Add(value);
				mappingPrototype2 = value;
				if (mappingPrototype2.Parents == null)
				{
					mappingPrototype2.Parents = new List<MappingPrototype>();
				}
				value.Parents.Add(topLevel);
			}
			return value;
		}
		Dictionary<string, MappingPrototype> orNew2 = Extensions.GetOrNew<Type, Dictionary<string, MappingPrototype>>(_idDict, typeof(T));
		if (orNew2.TryGetValue(id, out var value2))
		{
			return value2;
		}
		EntityPrototype val6 = (EntityPrototype)(object)((prototype is EntityPrototype) ? prototype : null);
		string text2 = ((val6 != null) ? val6.Name : null) ?? ((IPrototype)prototype).ID;
		if (!string.IsNullOrWhiteSpace((val6 != null) ? val6.EditorSuffix : null))
		{
			text2 = text2 + " [" + val6.EditorSuffix + "]";
		}
		value2 = new MappingPrototype((IPrototype?)(object)prototype, text2);
		_allPrototypes.Add(value2);
		_allPrototypesDict.Add((IPrototype)(object)prototype, value2);
		orNew2.Add(((IPrototype)prototype).ID, value2);
		if (((IInheritingPrototype)prototype).Parents == null)
		{
			MappingPrototype mappingPrototype2 = topLevel;
			if (mappingPrototype2.Children == null)
			{
				mappingPrototype2.Children = new List<MappingPrototype>();
			}
			topLevel.Children.Add(value2);
			mappingPrototype2 = value2;
			if (mappingPrototype2.Parents == null)
			{
				mappingPrototype2.Parents = new List<MappingPrototype>();
			}
			value2.Parents.Add(topLevel);
			return value2;
		}
		string[] parents = ((IInheritingPrototype)prototype).Parents;
		foreach (string id2 in parents)
		{
			MappingPrototype mappingPrototype4 = Register<T>(null, id2, topLevel);
			if (mappingPrototype4 != null)
			{
				MappingPrototype mappingPrototype2 = value2;
				if (mappingPrototype2.Parents == null)
				{
					mappingPrototype2.Parents = new List<MappingPrototype>();
				}
				value2.Parents.Add(mappingPrototype4);
				mappingPrototype2 = mappingPrototype4;
				if (mappingPrototype2.Children == null)
				{
					mappingPrototype2.Children = new List<MappingPrototype>();
				}
				mappingPrototype4.Children.Add(value2);
			}
		}
		return value2;
	}

	private void OnPlacementChanged(object? sender, EventArgs e)
	{
		_updatePlacement = true;
	}

	protected override void OnKeyBindStateChanged(ViewportBoundKeyEventArgs args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		if (args.Viewport == null)
		{
			base.OnKeyBindStateChanged(new ViewportBoundKeyEventArgs(args.KeyEventArgs, (Control)(object)Viewport.Viewport));
		}
		else
		{
			base.OnKeyBindStateChanged(args);
		}
	}

	private void OnSearch(LineEditEventArgs args)
	{
		if (string.IsNullOrEmpty(args.Text))
		{
			((Control)Screen.Prototypes.PrototypeList).Visible = true;
			((Control)Screen.Prototypes.SearchList).Visible = false;
			return;
		}
		List<MappingPrototype> list = new List<MappingPrototype>();
		foreach (MappingPrototype allPrototype in _allPrototypes)
		{
			if (allPrototype.Name.Contains(args.Text, StringComparison.OrdinalIgnoreCase))
			{
				list.Add(allPrototype);
			}
		}
		list.Sort((MappingPrototype a, MappingPrototype b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
		((Control)Screen.Prototypes.PrototypeList).Visible = false;
		((Control)Screen.Prototypes.SearchList).Visible = true;
		Screen.Prototypes.Search(list);
	}

	private unsafe void OnCollapseAll(ButtonEventArgs args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Enumerator enumerator = ((Control)Screen.Prototypes.PrototypeList).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				if (((Enumerator)(ref enumerator)).Current is MappingSpawnButton button)
				{
					Collapse(button);
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		Screen.Prototypes.ScrollContainer.SetScrollValue(new Vector2(0f, 0f));
	}

	private void OnClearSearch(ButtonEventArgs obj)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		Screen.Prototypes.SearchBar.Text = string.Empty;
		OnSearch(new LineEditEventArgs(Screen.Prototypes.SearchBar, string.Empty));
	}

	private void OnGetData(IPrototype prototype, List<Texture> textures)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		EntityPrototype val = (EntityPrototype)(object)((prototype is EntityPrototype) ? prototype : null);
		if (val == null)
		{
			if (!(prototype is DecalPrototype decalPrototype))
			{
				if (prototype is ContentTileDefinition { Sprite: var sprite })
				{
					string text = (sprite.HasValue ? ((object)sprite.GetValueOrDefault()/*cast due to constrained. prefix*/).ToString() : null);
					if (text != null)
					{
						textures.Add(_resources.GetResource<TextureResource>(text, true).Texture);
					}
				}
			}
			else
			{
				textures.Add(_sprite.Frame0(decalPrototype.Sprite));
			}
		}
		else
		{
			textures.AddRange(from t in _sprite.GetPrototypeTextures(val)
				select t.Default);
		}
	}

	private unsafe void OnSelected(MappingPrototype mapping)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (mapping.Prototype == null)
		{
			return;
		}
		Stack<MappingPrototype> stack = new Stack<MappingPrototype>();
		stack.Push(mapping);
		for (MappingPrototype mappingPrototype = mapping.Parents?.FirstOrDefault(); mappingPrototype != null; mappingPrototype = mappingPrototype.Parents?.FirstOrDefault())
		{
			stack.Push(mappingPrototype);
		}
		_lastClicked = null;
		Control val = null;
		OrderedChildCollection children = ((Control)Screen.Prototypes.PrototypeList).Children;
		foreach (MappingPrototype item in stack)
		{
			Enumerator enumerator2 = children.GetEnumerator();
			try
			{
				while (((Enumerator)(ref enumerator2)).MoveNext())
				{
					Control current2 = ((Enumerator)(ref enumerator2)).Current;
					if (current2 is MappingSpawnButton mappingSpawnButton && mappingSpawnButton.Prototype == item)
					{
						UnCollapse(mappingSpawnButton);
						OnSelected(mappingSpawnButton, item.Prototype);
						children = ((Control)mappingSpawnButton.ChildrenPrototypes).Children;
						val = current2;
						break;
					}
				}
			}
			finally
			{
				((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
			}
		}
		if (val != null && ((Control)Screen.Prototypes.PrototypeList).Visible)
		{
			_scrollTo = val;
		}
	}

	private void OnSelected(MappingSpawnButton button, IPrototype? prototype)
	{
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Expected O, but got Unknown
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Expected O, but got Unknown
		TimeSpan curTime = _timing.CurTime;
		if (prototype is DecalPrototype)
		{
			Screen.SelectDecal(prototype.ID);
		}
		(TimeSpan, MappingSpawnButton)? lastClicked = _lastClicked;
		if (lastClicked.HasValue)
		{
			(TimeSpan, MappingSpawnButton) valueOrDefault = lastClicked.GetValueOrDefault();
			if (valueOrDefault.Item2 == button && valueOrDefault.Item1 > curTime - TimeSpan.FromSeconds(0.333) && string.IsNullOrEmpty(Screen.Prototypes.SearchBar.Text) && ((Control)button.CollapseButton).Visible)
			{
				((BaseButton)button.CollapseButton).Pressed = !((BaseButton)button.CollapseButton).Pressed;
				ToggleCollapse(button);
				((BaseButton)button.Button).Pressed = true;
				Screen.Prototypes.Selected = button;
				_lastClicked = null;
				return;
			}
		}
		if (!((BaseButton)button.Button).Pressed && button.Prototype?.Prototype != null && _lastClicked?.Button == button)
		{
			_lastClicked = null;
			Deselect();
			return;
		}
		_lastClicked = (curTime, button);
		if (button.Prototype == null)
		{
			return;
		}
		MappingSpawnButton selected = Screen.Prototypes.Selected;
		if (selected != null && selected != button)
		{
			Deselect();
		}
		((Control)Screen.EntityContainer).Visible = false;
		((Control)Screen.DecalContainer).Visible = false;
		EntityPrototype val = (EntityPrototype)(object)((prototype is EntityPrototype) ? prototype : null);
		if (val == null)
		{
			if (!(prototype is DecalPrototype decalPrototype))
			{
				if (prototype is ContentTileDefinition contentTileDefinition)
				{
					PlacementInformation val2 = new PlacementInformation
					{
						PlacementOption = "AlignTileAny",
						TileType = contentTileDefinition.TileId,
						IsTile = true
					};
					_decal.SetActive(active: false);
					_placement.BeginPlacing(val2, (PlacementHijack)null);
				}
				else
				{
					_placement.Clear();
				}
			}
			else
			{
				_placement.Clear();
				_decal.SetActive(active: true);
				_decal.UpdateDecalInfo(decalPrototype.ID, Color.White, 0f, snap: true, 0, cleanable: false);
				((Control)Screen.DecalContainer).Visible = true;
			}
		}
		else
		{
			int selectedId = Screen.EntityPlacementMode.SelectedId;
			PlacementInformation val3 = new PlacementInformation
			{
				PlacementOption = ((selectedId > 0) ? _placement.AllModeNames[selectedId] : val.PlacementMode),
				EntityType = val.ID,
				IsTile = false
			};
			((Control)Screen.EntityContainer).Visible = true;
			_decal.SetActive(active: false);
			_placement.BeginPlacing(val3, (PlacementHijack)null);
		}
		Screen.Prototypes.Selected = button;
		((BaseButton)button.Button).Pressed = true;
	}

	private void Deselect()
	{
		MappingSpawnButton selected = Screen.Prototypes.Selected;
		if (selected != null)
		{
			((BaseButton)selected.Button).Pressed = false;
			Screen.Prototypes.Selected = null;
			if (selected.Prototype?.Prototype is DecalPrototype)
			{
				_decal.SetActive(active: false);
				((Control)Screen.DecalContainer).Visible = false;
			}
			if (selected.Prototype?.Prototype is EntityPrototype)
			{
				_placement.Clear();
			}
			if (selected.Prototype?.Prototype is ContentTileDefinition)
			{
				_placement.Clear();
			}
		}
	}

	private void OnCollapseToggled(MappingSpawnButton button, ButtonToggledEventArgs args)
	{
		ToggleCollapse(button);
	}

	private void OnPickPressed(ButtonEventArgs args)
	{
		if (args.Button.Pressed)
		{
			EnablePick();
		}
		else
		{
			DisablePick();
		}
	}

	private void OnDeletePressed(ButtonEventArgs obj)
	{
		if (obj.Button.Pressed)
		{
			EnableDelete();
		}
		else
		{
			DisableDelete();
		}
	}

	private void OnEntityReplacePressed(ButtonToggledEventArgs args)
	{
		_placement.Replacement = args.Pressed;
	}

	private void OnEntityPlacementSelected(ItemSelectedEventArgs args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		Screen.EntityPlacementMode.SelectId(args.Id);
		if (_placement.CurrentMode != null)
		{
			PlacementInformation val = new PlacementInformation
			{
				PlacementOption = _placement.AllModeNames[args.Id],
				EntityType = _placement.CurrentPermission.EntityType,
				TileType = _placement.CurrentPermission.TileType,
				Range = 2,
				IsTile = _placement.CurrentPermission.IsTile
			};
			_placement.BeginPlacing(val, (PlacementHijack)null);
		}
	}

	private void OnEraseEntityPressed(ButtonEventArgs args)
	{
		if (args.Button.Pressed != _placement.Eraser)
		{
			if (args.Button.Pressed)
			{
				EnableEraser();
			}
			else
			{
				DisableEraser();
			}
		}
	}

	private void OnEraseDecalPressed(ButtonToggledEventArgs args)
	{
		_placement.Clear();
		Deselect();
		((BaseButton)Screen.EraseEntityButton).Pressed = false;
		_updatePlacement = true;
		_updateEraseDecal = args.Pressed;
	}

	private void EnableEraser()
	{
		if (!_placement.Eraser)
		{
			_placement.Clear();
			_placement.ToggleEraser();
			((BaseButton)Screen.EntityPlacementMode).Disabled = true;
			((BaseButton)Screen.EraseDecalButton).Pressed = false;
			Deselect();
		}
	}

	private void DisableEraser()
	{
		if (_placement.Eraser)
		{
			_placement.ToggleEraser();
			((BaseButton)Screen.EntityPlacementMode).Disabled = false;
		}
	}

	private void EnablePick()
	{
		Screen.UnPressActionsExcept((Control)(object)Screen.Pick);
		State = CursorState.Pick;
	}

	private void DisablePick()
	{
		((BaseButton)Screen.Pick).Pressed = false;
		State = CursorState.None;
	}

	private void EnableDelete()
	{
		Screen.UnPressActionsExcept((Control)(object)Screen.Delete);
		State = CursorState.Delete;
		EnableEraser();
	}

	private void DisableDelete()
	{
		((BaseButton)Screen.Delete).Pressed = false;
		State = CursorState.None;
		DisableEraser();
	}

	private bool HandleMappingUnselect(in PointerInputCmdArgs args)
	{
		MappingSpawnButton selected = Screen.Prototypes.Selected;
		if (selected != null)
		{
			MappingPrototype prototype = selected.Prototype;
			if (prototype != null && prototype.Prototype is DecalPrototype)
			{
				Deselect();
				return true;
			}
		}
		return false;
	}

	private bool HandleSaveMap(in PointerInputCmdArgs args)
	{
		return false;
	}

	private bool HandleEnablePick(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
	{
		EnablePick();
		return true;
	}

	private bool HandleDisablePick(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
	{
		DisablePick();
		return true;
	}

	private bool HandleEnableDelete(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
	{
		EnableDelete();
		return true;
	}

	private bool HandleDisableDelete(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
	{
		DisableDelete();
		return true;
	}

	private bool HandlePick(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		if (State != CursorState.Pick)
		{
			return false;
		}
		MappingPrototype value = null;
		if (!((EntityUid)(ref uid)).IsValid())
		{
			MapCoordinates val = ((SharedTransformSystem)_transform).ToMapCoordinates(coords, true);
			EntityUid val2 = default(EntityUid);
			MapGridComponent val3 = default(MapGridComponent);
			TileRef tile = default(TileRef);
			if (_mapMan.TryFindGridAt(val, ref val2, ref val3) && _entityManager.System<SharedMapSystem>().TryGetTileRef(val2, val3, coords, ref tile) && _allPrototypesDict.TryGetValue((IPrototype)(object)_entityManager.System<TurfSystem>().GetContentTileDefinition(tile), out value))
			{
				OnSelected(value);
				return true;
			}
		}
		if (value == null)
		{
			if (!(uid == EntityUid.Invalid))
			{
				MetaDataComponent componentOrNull = EntityManagerExt.GetComponentOrNull<MetaDataComponent>(_entityManager, uid);
				if (componentOrNull != null)
				{
					EntityPrototype entityPrototype = componentOrNull.EntityPrototype;
					if (entityPrototype != null && _allPrototypesDict.TryGetValue((IPrototype)(object)entityPrototype, out value))
					{
						OnSelected(value);
						IPlacementManager placement = _placement;
						Angle localRotation = _entityManager.GetComponent<TransformComponent>(uid).LocalRotation;
						placement.Direction = ((Angle)(ref localRotation)).GetDir();
						goto IL_00e5;
					}
				}
			}
			return true;
		}
		goto IL_00e5;
		IL_00e5:
		return true;
	}

	private bool HandleEditorCancelPlace(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!((BaseButton)Screen.EraseDecalButton).Pressed)
		{
			return false;
		}
		_entityNetwork.SendSystemNetworkMessage((EntityEventArgs)(object)new RequestDecalRemovalEvent(_entityManager.GetNetCoordinates(coords, (MetaDataComponent)null)), true);
		return true;
	}

	private bool HandleCancelEraseDecal(in PointerInputCmdArgs args)
	{
		if (!((BaseButton)Screen.EraseDecalButton).Pressed)
		{
			return false;
		}
		((BaseButton)Screen.EraseDecalButton).Pressed = false;
		return true;
	}

	private async void SaveMap()
	{
		await _mapping.SaveMap();
	}

	private void ToggleCollapse(MappingSpawnButton button)
	{
		if (((BaseButton)button.CollapseButton).Pressed)
		{
			if (button.Prototype?.Children != null)
			{
				foreach (MappingPrototype child in button.Prototype.Children)
				{
					Screen.Prototypes.Insert((Container)(object)button.ChildrenPrototypes, child, includeChildren: true);
				}
			}
			button.CollapseButton.Label.Text = "▼";
		}
		else
		{
			((Control)button.ChildrenPrototypes).DisposeAllChildren();
			button.CollapseButton.Label.Text = "▶";
		}
	}

	private void Collapse(MappingSpawnButton button)
	{
		if (((BaseButton)button.CollapseButton).Pressed)
		{
			((BaseButton)button.CollapseButton).Pressed = false;
			ToggleCollapse(button);
		}
	}

	private void UnCollapse(MappingSpawnButton button)
	{
		if (!((BaseButton)button.CollapseButton).Pressed)
		{
			((BaseButton)button.CollapseButton).Pressed = true;
			ToggleCollapse(button);
		}
	}

	public EntityUid? GetHoveredEntity()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		Control currentlyHovered = UserInterfaceManager.CurrentlyHovered;
		IViewportControl val = (IViewportControl)(object)((currentlyHovered is IViewportControl) ? currentlyHovered : null);
		if (val != null)
		{
			ScreenCoordinates mouseScreenPosition = _input.MouseScreenPosition;
			if (((ScreenCoordinates)(ref mouseScreenPosition)).IsValid)
			{
				MapCoordinates coordinates = val.PixelToMap(mouseScreenPosition.Position);
				return GetClickedEntity(coordinates);
			}
		}
		return null;
	}

	public override void FrameUpdate(FrameEventArgs e)
	{
		if (_updatePlacement)
		{
			_updatePlacement = false;
			if (!_placement.IsActive && _decal.GetActiveDecal().Decal == null)
			{
				Deselect();
			}
			((BaseButton)Screen.EraseEntityButton).Pressed = _placement.Eraser;
			((BaseButton)Screen.EraseDecalButton).Pressed = _updateEraseDecal;
			((BaseButton)Screen.EntityPlacementMode).Disabled = _placement.Eraser;
		}
		Control scrollTo = _scrollTo;
		if (scrollTo != null && scrollTo.Height > 0f && ((Control)Screen.Prototypes.PrototypeList).Visible)
		{
			float y = scrollTo.GlobalPosition.Y - ((Control)Screen.Prototypes.ScrollContainer).Height / 2f + scrollTo.Height;
			ScrollContainer scrollContainer = Screen.Prototypes.ScrollContainer;
			scrollContainer.SetScrollValue(scrollContainer.GetScrollValue(false) + new Vector2(0f, y));
			_scrollTo = null;
		}
	}
}
