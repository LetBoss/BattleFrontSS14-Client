// Decompiled with JetBrains decompiler
// Type: Content.Client.Mapping.MappingState
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.Mapping;

public sealed class MappingState : GameplayStateBase
{
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

  private MappingScreen Screen => (MappingScreen) this.UserInterfaceManager.ActiveScreen;

  private MainViewport Viewport => this.UserInterfaceManager.ActiveScreen.GetWidget<MainViewport>();

  public MappingState.CursorState State { get; set; }

  public MappingState()
  {
    IoCManager.InjectDependencies<MappingState>(this);
    this._sawmill = this._log.GetSawmill("mapping");
    this._loadController = this.UserInterfaceManager.GetUIController<GameplayStateLoadController>();
  }

  protected override void Startup()
  {
    this.EnsureSetup();
    base.Startup();
    this.UserInterfaceManager.LoadScreen<MappingScreen>();
    this._loadController.LoadScreen();
    IInputCmdContext context = this._input.Contexts.GetContext("common");
    context.AddFunction(ContentKeyFunctions.MappingUnselect);
    context.AddFunction(ContentKeyFunctions.SaveMap);
    context.AddFunction(ContentKeyFunctions.MappingEnablePick);
    context.AddFunction(ContentKeyFunctions.MappingEnableDelete);
    context.AddFunction(ContentKeyFunctions.MappingPick);
    context.AddFunction(ContentKeyFunctions.MappingRemoveDecal);
    context.AddFunction(ContentKeyFunctions.MappingCancelEraseDecal);
    context.AddFunction(ContentKeyFunctions.MappingOpenContextMenu);
    this.Screen.DecalSystem = this._decal;
    this.Screen.Prototypes.SearchBar.OnTextChanged += new Action<LineEdit.LineEditEventArgs>(this.OnSearch);
    ((BaseButton) this.Screen.Prototypes.CollapseAllButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnCollapseAll);
    ((BaseButton) this.Screen.Prototypes.ClearSearchButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnClearSearch);
    this.Screen.Prototypes.GetPrototypeData += new Action<IPrototype, List<Texture>>(this.OnGetData);
    this.Screen.Prototypes.SelectionChanged += new Action<MappingSpawnButton, IPrototype>(this.OnSelected);
    this.Screen.Prototypes.CollapseToggled += new Action<MappingSpawnButton, BaseButton.ButtonToggledEventArgs>(this.OnCollapseToggled);
    ((BaseButton) this.Screen.Pick).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnPickPressed);
    ((BaseButton) this.Screen.Delete).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnDeletePressed);
    ((BaseButton) this.Screen.EntityReplaceButton).OnToggled += new Action<BaseButton.ButtonToggledEventArgs>(this.OnEntityReplacePressed);
    this.Screen.EntityPlacementMode.OnItemSelected += new Action<OptionButton.ItemSelectedEventArgs>(this.OnEntityPlacementSelected);
    ((BaseButton) this.Screen.EraseEntityButton).OnToggled += new Action<BaseButton.ButtonToggledEventArgs>(this.OnEraseEntityPressed);
    ((BaseButton) this.Screen.EraseDecalButton).OnToggled += new Action<BaseButton.ButtonToggledEventArgs>(this.OnEraseDecalPressed);
    this._placement.PlacementChanged += new EventHandler(this.OnPlacementChanged);
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(ContentKeyFunctions.MappingUnselect, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleMappingUnselect)), true, true)).Bind(ContentKeyFunctions.SaveMap, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleSaveMap)), true, true)).Bind(ContentKeyFunctions.MappingEnablePick, (InputCmdHandler) new PointerStateInputCmdHandler(new PointerInputCmdDelegate((object) this, __methodptr(HandleEnablePick)), new PointerInputCmdDelegate((object) this, __methodptr(HandleDisablePick)), true)).Bind(ContentKeyFunctions.MappingEnableDelete, (InputCmdHandler) new PointerStateInputCmdHandler(new PointerInputCmdDelegate((object) this, __methodptr(HandleEnableDelete)), new PointerInputCmdDelegate((object) this, __methodptr(HandleDisableDelete)), true)).Bind(ContentKeyFunctions.MappingPick, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate((object) this, __methodptr(HandlePick)), true, true)).Bind(ContentKeyFunctions.MappingRemoveDecal, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate((object) this, __methodptr(HandleEditorCancelPlace)), true, true)).Bind(ContentKeyFunctions.MappingCancelEraseDecal, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleCancelEraseDecal)), true, true)).Bind(ContentKeyFunctions.MappingOpenContextMenu, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleOpenContextMenu)), true, true)).Register<MappingState>();
    this._overlays.AddOverlay((Overlay) new MappingOverlay(this));
    this._prototypeManager.PrototypesReloaded += new Action<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded);
    this.Screen.Prototypes.UpdateVisible(this._prototypes);
  }

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs obj)
  {
    if (!obj.WasModified<EntityPrototype>() && !obj.WasModified<ContentTileDefinition>() && !obj.WasModified<DecalPrototype>())
      return;
    this.ReloadPrototypes();
  }

  private bool HandleOpenContextMenu(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    this.Deselect();
    List<EntityUid> entities;
    if (this._verbs.TryGetEntityMenuEntities(((SharedTransformSystem) this._transform).ToMapCoordinates(args.Coordinates, true), out entities))
      this._entityMenuController.OpenRootMenu(entities);
    return true;
  }

  protected override void Shutdown()
  {
    CommandBinds.Unregister<MappingState>();
    this.Screen.Prototypes.SearchBar.OnTextChanged -= new Action<LineEdit.LineEditEventArgs>(this.OnSearch);
    ((BaseButton) this.Screen.Prototypes.CollapseAllButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnCollapseAll);
    ((BaseButton) this.Screen.Prototypes.ClearSearchButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnClearSearch);
    this.Screen.Prototypes.GetPrototypeData -= new Action<IPrototype, List<Texture>>(this.OnGetData);
    this.Screen.Prototypes.SelectionChanged -= new Action<MappingSpawnButton, IPrototype>(this.OnSelected);
    this.Screen.Prototypes.CollapseToggled -= new Action<MappingSpawnButton, BaseButton.ButtonToggledEventArgs>(this.OnCollapseToggled);
    ((BaseButton) this.Screen.Pick).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnPickPressed);
    ((BaseButton) this.Screen.Delete).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnDeletePressed);
    ((BaseButton) this.Screen.EntityReplaceButton).OnToggled -= new Action<BaseButton.ButtonToggledEventArgs>(this.OnEntityReplacePressed);
    this.Screen.EntityPlacementMode.OnItemSelected -= new Action<OptionButton.ItemSelectedEventArgs>(this.OnEntityPlacementSelected);
    ((BaseButton) this.Screen.EraseEntityButton).OnToggled -= new Action<BaseButton.ButtonToggledEventArgs>(this.OnEraseEntityPressed);
    ((BaseButton) this.Screen.EraseDecalButton).OnToggled -= new Action<BaseButton.ButtonToggledEventArgs>(this.OnEraseDecalPressed);
    this._placement.PlacementChanged -= new EventHandler(this.OnPlacementChanged);
    this._prototypeManager.PrototypesReloaded -= new Action<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded);
    this.UserInterfaceManager.ClearWindows();
    this._loadController.UnloadScreen();
    this.UserInterfaceManager.UnloadScreen();
    IInputCmdContext context = this._input.Contexts.GetContext("common");
    context.RemoveFunction(ContentKeyFunctions.MappingUnselect);
    context.RemoveFunction(ContentKeyFunctions.SaveMap);
    context.RemoveFunction(ContentKeyFunctions.MappingEnablePick);
    context.RemoveFunction(ContentKeyFunctions.MappingEnableDelete);
    context.RemoveFunction(ContentKeyFunctions.MappingPick);
    context.RemoveFunction(ContentKeyFunctions.MappingRemoveDecal);
    context.RemoveFunction(ContentKeyFunctions.MappingCancelEraseDecal);
    context.RemoveFunction(ContentKeyFunctions.MappingOpenContextMenu);
    this._overlays.RemoveOverlay<MappingOverlay>();
    base.Shutdown();
  }

  private void EnsureSetup()
  {
    if (this._setup)
      return;
    this._setup = true;
    this._entityMenuController = this.UserInterfaceManager.GetUIController<EntityMenuUIController>();
    this._decal = this._entityManager.System<DecalPlacementSystem>();
    this._sprite = this._entityManager.System<SpriteSystem>();
    this._transform = this._entityManager.System<TransformSystem>();
    this._verbs = this._entityManager.System<VerbSystem>();
    this.ReloadPrototypes();
  }

  private void ReloadPrototypes()
  {
    MappingPrototype topLevel1 = new MappingPrototype((IPrototype) null, Loc.GetString("mapping-entities"))
    {
      Children = new List<MappingPrototype>()
    };
    this._prototypes.Add(topLevel1);
    Dictionary<string, MappingPrototype> prototypes = new Dictionary<string, MappingPrototype>();
    foreach (EntityPrototype enumeratePrototype in this._prototypeManager.EnumeratePrototypes<EntityPrototype>())
      this.Register<EntityPrototype>(enumeratePrototype, enumeratePrototype.ID, topLevel1);
    this.Sort(prototypes, topLevel1);
    prototypes.Clear();
    MappingPrototype topLevel2 = new MappingPrototype((IPrototype) null, Loc.GetString("mapping-tiles"))
    {
      Children = new List<MappingPrototype>()
    };
    this._prototypes.Add(topLevel2);
    foreach (ContentTileDefinition enumeratePrototype in this._prototypeManager.EnumeratePrototypes<ContentTileDefinition>())
      this.Register<ContentTileDefinition>(enumeratePrototype, enumeratePrototype.ID, topLevel2);
    this.Sort(prototypes, topLevel2);
    prototypes.Clear();
    MappingPrototype topLevel3 = new MappingPrototype((IPrototype) null, Loc.GetString("mapping-decals"))
    {
      Children = new List<MappingPrototype>()
    };
    this._prototypes.Add(topLevel3);
    foreach (DecalPrototype enumeratePrototype in this._prototypeManager.EnumeratePrototypes<DecalPrototype>())
      this.Register<DecalPrototype>(enumeratePrototype, enumeratePrototype.ID, topLevel3);
    this.Sort(prototypes, topLevel3);
    prototypes.Clear();
  }

  private void Sort(Dictionary<string, MappingPrototype> prototypes, MappingPrototype topLevel)
  {
    MappingPrototype mappingPrototype1 = topLevel;
    if (mappingPrototype1.Children == null)
      mappingPrototype1.Children = new List<MappingPrototype>();
    foreach (MappingPrototype mappingPrototype2 in prototypes.Values)
    {
      if (mappingPrototype2.Parents == null && mappingPrototype2 != topLevel)
      {
        mappingPrototype2.Parents = new List<MappingPrototype>()
        {
          topLevel
        };
        topLevel.Children.Add(mappingPrototype2);
      }
      mappingPrototype2.Parents?.Sort(new Comparison<MappingPrototype>(Compare));
      mappingPrototype2.Children?.Sort(new Comparison<MappingPrototype>(Compare));
    }
    topLevel.Children.Sort(new Comparison<MappingPrototype>(Compare));

    static int Compare(MappingPrototype a, MappingPrototype b)
    {
      return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
    }
  }

  private MappingPrototype? Register<T>(T? prototype, string id, MappingPrototype topLevel) where T : class, IPrototype, IInheritingPrototype
  {
    if ((object) prototype == null && this._prototypeManager.TryIndex<T>(id, ref prototype) && prototype is EntityPrototype entityPrototype1 && (entityPrototype1.HideSpawnMenu || entityPrototype1.Abstract))
      prototype = default (T);
    if ((object) prototype == null)
    {
      MappingDataNode mappingDataNode;
      if (!this._prototypeManager.TryGetMapping(typeof (T), id, ref mappingDataNode))
      {
        this._sawmill.Error("No T found with id " + id);
        return (MappingPrototype) null;
      }
      Dictionary<string, MappingPrototype> orNew = Extensions.GetOrNew<Type, Dictionary<string, MappingPrototype>>(this._idDict, typeof (T));
      MappingPrototype mappingPrototype1;
      if (orNew.TryGetValue(id, out mappingPrototype1))
        return mappingPrototype1;
      ValueDataNode valueDataNode1;
      string name = mappingDataNode.TryGet<ValueDataNode>("name", ref valueDataNode1) ? valueDataNode1.Value : id;
      ValueDataNode valueDataNode2;
      if (mappingDataNode.TryGet<ValueDataNode>("suffix", ref valueDataNode2))
        name = $"{name} [{valueDataNode2.Value}]";
      mappingPrototype1 = new MappingPrototype((IPrototype) prototype, name);
      this._allPrototypes.Add(mappingPrototype1);
      orNew.Add(id, mappingPrototype1);
      ValueDataNode valueDataNode3;
      if (mappingDataNode.TryGet<ValueDataNode>("parent", ref valueDataNode3))
      {
        MappingPrototype mappingPrototype2 = this.Register<T>(default (T), valueDataNode3.Value, topLevel);
        if (mappingPrototype2 != null)
        {
          MappingPrototype mappingPrototype3 = mappingPrototype1;
          if (mappingPrototype3.Parents == null)
            mappingPrototype3.Parents = new List<MappingPrototype>();
          mappingPrototype1.Parents.Add(mappingPrototype2);
          MappingPrototype mappingPrototype4 = mappingPrototype2;
          if (mappingPrototype4.Children == null)
            mappingPrototype4.Children = new List<MappingPrototype>();
          mappingPrototype2.Children.Add(mappingPrototype1);
        }
      }
      else
      {
        SequenceDataNode source;
        if (mappingDataNode.TryGet<SequenceDataNode>("parent", ref source))
        {
          foreach (ValueDataNode valueDataNode4 in ((IEnumerable) source).Cast<ValueDataNode>())
          {
            MappingPrototype mappingPrototype5 = this.Register<T>(default (T), valueDataNode4.Value, topLevel);
            if (mappingPrototype5 != null)
            {
              MappingPrototype mappingPrototype6 = mappingPrototype1;
              if (mappingPrototype6.Parents == null)
                mappingPrototype6.Parents = new List<MappingPrototype>();
              mappingPrototype1.Parents.Add(mappingPrototype5);
              MappingPrototype mappingPrototype7 = mappingPrototype5;
              if (mappingPrototype7.Children == null)
                mappingPrototype7.Children = new List<MappingPrototype>();
              mappingPrototype5.Children.Add(mappingPrototype1);
            }
          }
        }
        else
        {
          MappingPrototype mappingPrototype8 = topLevel;
          if (mappingPrototype8.Children == null)
            mappingPrototype8.Children = new List<MappingPrototype>();
          topLevel.Children.Add(mappingPrototype1);
          MappingPrototype mappingPrototype9 = mappingPrototype1;
          if (mappingPrototype9.Parents == null)
            mappingPrototype9.Parents = new List<MappingPrototype>();
          mappingPrototype1.Parents.Add(topLevel);
        }
      }
      return mappingPrototype1;
    }
    Dictionary<string, MappingPrototype> orNew1 = Extensions.GetOrNew<Type, Dictionary<string, MappingPrototype>>(this._idDict, typeof (T));
    MappingPrototype mappingPrototype10;
    if (orNew1.TryGetValue(id, out mappingPrototype10))
      return mappingPrototype10;
    string name1 = (prototype is EntityPrototype entityPrototype2 ? entityPrototype2.Name : (string) null) ?? prototype.ID;
    if (!string.IsNullOrWhiteSpace(entityPrototype2?.EditorSuffix))
      name1 = $"{name1} [{entityPrototype2.EditorSuffix}]";
    mappingPrototype10 = new MappingPrototype((IPrototype) prototype, name1);
    this._allPrototypes.Add(mappingPrototype10);
    this._allPrototypesDict.Add((IPrototype) prototype, mappingPrototype10);
    orNew1.Add(prototype.ID, mappingPrototype10);
    if (prototype.Parents == null)
    {
      MappingPrototype mappingPrototype11 = topLevel;
      if (mappingPrototype11.Children == null)
        mappingPrototype11.Children = new List<MappingPrototype>();
      topLevel.Children.Add(mappingPrototype10);
      MappingPrototype mappingPrototype12 = mappingPrototype10;
      if (mappingPrototype12.Parents == null)
        mappingPrototype12.Parents = new List<MappingPrototype>();
      mappingPrototype10.Parents.Add(topLevel);
      return mappingPrototype10;
    }
    foreach (string parent in prototype.Parents)
    {
      MappingPrototype mappingPrototype13 = this.Register<T>(default (T), parent, topLevel);
      if (mappingPrototype13 != null)
      {
        MappingPrototype mappingPrototype14 = mappingPrototype10;
        if (mappingPrototype14.Parents == null)
          mappingPrototype14.Parents = new List<MappingPrototype>();
        mappingPrototype10.Parents.Add(mappingPrototype13);
        MappingPrototype mappingPrototype15 = mappingPrototype13;
        if (mappingPrototype15.Children == null)
          mappingPrototype15.Children = new List<MappingPrototype>();
        mappingPrototype13.Children.Add(mappingPrototype10);
      }
    }
    return mappingPrototype10;
  }

  private void OnPlacementChanged(object? sender, EventArgs e) => this._updatePlacement = true;

  protected override void OnKeyBindStateChanged(ViewportBoundKeyEventArgs args)
  {
    if (args.Viewport == null)
      base.OnKeyBindStateChanged(new ViewportBoundKeyEventArgs(args.KeyEventArgs, (Control) this.Viewport.Viewport));
    else
      base.OnKeyBindStateChanged(args);
  }

  private void OnSearch(LineEdit.LineEditEventArgs args)
  {
    if (string.IsNullOrEmpty(args.Text))
    {
      ((Control) this.Screen.Prototypes.PrototypeList).Visible = true;
      ((Control) this.Screen.Prototypes.SearchList).Visible = false;
    }
    else
    {
      List<MappingPrototype> prototypes = new List<MappingPrototype>();
      foreach (MappingPrototype allPrototype in this._allPrototypes)
      {
        if (allPrototype.Name.Contains(args.Text, StringComparison.OrdinalIgnoreCase))
          prototypes.Add(allPrototype);
      }
      prototypes.Sort((Comparison<MappingPrototype>) ((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase)));
      ((Control) this.Screen.Prototypes.PrototypeList).Visible = false;
      ((Control) this.Screen.Prototypes.SearchList).Visible = true;
      this.Screen.Prototypes.Search(prototypes);
    }
  }

  private void OnCollapseAll(BaseButton.ButtonEventArgs args)
  {
    foreach (Control child in ((Control) this.Screen.Prototypes.PrototypeList).Children)
    {
      if (child is MappingSpawnButton button)
        this.Collapse(button);
    }
    this.Screen.Prototypes.ScrollContainer.SetScrollValue(new Vector2(0.0f, 0.0f));
  }

  private void OnClearSearch(BaseButton.ButtonEventArgs obj)
  {
    this.Screen.Prototypes.SearchBar.Text = string.Empty;
    this.OnSearch(new LineEdit.LineEditEventArgs(this.Screen.Prototypes.SearchBar, string.Empty));
  }

  private void OnGetData(IPrototype prototype, List<Texture> textures)
  {
    switch (prototype)
    {
      case EntityPrototype entityPrototype:
        textures.AddRange(this._sprite.GetPrototypeTextures(entityPrototype).Select<IDirectionalTextureProvider, Texture>((Func<IDirectionalTextureProvider, Texture>) (t => t.Default)));
        break;
      case DecalPrototype decalPrototype:
        textures.Add(this._sprite.Frame0(decalPrototype.Sprite));
        break;
      case ContentTileDefinition contentTileDefinition:
        ResPath? sprite = contentTileDefinition.Sprite;
        ref ResPath? local = ref sprite;
        string str = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
        if (str == null)
          break;
        textures.Add(this._resources.GetResource<TextureResource>(str, true).Texture);
        break;
    }
  }

  private void OnSelected(MappingPrototype mapping)
  {
    if (mapping.Prototype == null)
      return;
    Stack<MappingPrototype> mappingPrototypeStack = new Stack<MappingPrototype>();
    mappingPrototypeStack.Push(mapping);
    List<MappingPrototype> parents1 = mapping.Parents;
    List<MappingPrototype> parents2;
    for (MappingPrototype mappingPrototype = parents1 != null ? parents1.FirstOrDefault<MappingPrototype>() : (MappingPrototype) null; mappingPrototype != null; mappingPrototype = parents2 != null ? parents2.FirstOrDefault<MappingPrototype>() : (MappingPrototype) null)
    {
      mappingPrototypeStack.Push(mappingPrototype);
      parents2 = mappingPrototype.Parents;
    }
    this._lastClicked = new (TimeSpan, MappingSpawnButton)?();
    Control control1 = (Control) null;
    Control.OrderedChildCollection children = ((Control) this.Screen.Prototypes.PrototypeList).Children;
    foreach (MappingPrototype mappingPrototype in mappingPrototypeStack)
    {
      foreach (Control control2 in children)
      {
        if (control2 is MappingSpawnButton button && button.Prototype == mappingPrototype)
        {
          this.UnCollapse(button);
          this.OnSelected(button, mappingPrototype.Prototype);
          children = ((Control) button.ChildrenPrototypes).Children;
          control1 = control2;
          break;
        }
      }
    }
    if (control1 == null || !((Control) this.Screen.Prototypes.PrototypeList).Visible)
      return;
    this._scrollTo = control1;
  }

  private void OnSelected(MappingSpawnButton button, IPrototype? prototype)
  {
    TimeSpan curTime = this._timing.CurTime;
    if (prototype is DecalPrototype)
      this.Screen.SelectDecal(prototype.ID);
    (TimeSpan At, MappingSpawnButton Button)? lastClicked = this._lastClicked;
    if (lastClicked.HasValue)
    {
      (TimeSpan At, MappingSpawnButton Button) valueOrDefault = lastClicked.GetValueOrDefault();
      if (valueOrDefault.Button == button && valueOrDefault.At > curTime - TimeSpan.FromSeconds(0.333) && string.IsNullOrEmpty(this.Screen.Prototypes.SearchBar.Text) && ((Control) button.CollapseButton).Visible)
      {
        ((BaseButton) button.CollapseButton).Pressed = !((BaseButton) button.CollapseButton).Pressed;
        this.ToggleCollapse(button);
        ((BaseButton) button.Button).Pressed = true;
        this.Screen.Prototypes.Selected = button;
        this._lastClicked = new (TimeSpan, MappingSpawnButton)?();
        return;
      }
    }
    if (!((BaseButton) button.Button).Pressed && button.Prototype?.Prototype != null)
    {
      ref (TimeSpan, MappingSpawnButton)? local = ref this._lastClicked;
      if ((local.HasValue ? local.GetValueOrDefault().Item2 : (MappingSpawnButton) null) == button)
      {
        this._lastClicked = new (TimeSpan, MappingSpawnButton)?();
        this.Deselect();
        return;
      }
    }
    this._lastClicked = new (TimeSpan, MappingSpawnButton)?((curTime, button));
    if (button.Prototype == null)
      return;
    MappingSpawnButton selected = this.Screen.Prototypes.Selected;
    if (selected != null && selected != button)
      this.Deselect();
    ((Control) this.Screen.EntityContainer).Visible = false;
    ((Control) this.Screen.DecalContainer).Visible = false;
    switch (prototype)
    {
      case EntityPrototype entityPrototype:
        int selectedId = this.Screen.EntityPlacementMode.SelectedId;
        PlacementInformation placementInformation1 = new PlacementInformation()
        {
          PlacementOption = selectedId > 0 ? this._placement.AllModeNames[selectedId] : entityPrototype.PlacementMode,
          EntityType = entityPrototype.ID,
          IsTile = false
        };
        ((Control) this.Screen.EntityContainer).Visible = true;
        this._decal.SetActive(false);
        this._placement.BeginPlacing(placementInformation1, (PlacementHijack) null);
        break;
      case DecalPrototype decalPrototype:
        this._placement.Clear();
        this._decal.SetActive(true);
        this._decal.UpdateDecalInfo(decalPrototype.ID, Color.White, 0.0f, true, 0, false);
        ((Control) this.Screen.DecalContainer).Visible = true;
        break;
      case ContentTileDefinition contentTileDefinition:
        PlacementInformation placementInformation2 = new PlacementInformation()
        {
          PlacementOption = "AlignTileAny",
          TileType = (int) contentTileDefinition.TileId,
          IsTile = true
        };
        this._decal.SetActive(false);
        this._placement.BeginPlacing(placementInformation2, (PlacementHijack) null);
        break;
      default:
        this._placement.Clear();
        break;
    }
    this.Screen.Prototypes.Selected = button;
    ((BaseButton) button.Button).Pressed = true;
  }

  private void Deselect()
  {
    MappingSpawnButton selected = this.Screen.Prototypes.Selected;
    if (selected == null)
      return;
    ((BaseButton) selected.Button).Pressed = false;
    this.Screen.Prototypes.Selected = (MappingSpawnButton) null;
    if (selected.Prototype?.Prototype is DecalPrototype)
    {
      this._decal.SetActive(false);
      ((Control) this.Screen.DecalContainer).Visible = false;
    }
    if (selected.Prototype?.Prototype is EntityPrototype)
      this._placement.Clear();
    if (!(selected.Prototype?.Prototype is ContentTileDefinition))
      return;
    this._placement.Clear();
  }

  private void OnCollapseToggled(MappingSpawnButton button, BaseButton.ButtonToggledEventArgs args)
  {
    this.ToggleCollapse(button);
  }

  private void OnPickPressed(BaseButton.ButtonEventArgs args)
  {
    if (args.Button.Pressed)
      this.EnablePick();
    else
      this.DisablePick();
  }

  private void OnDeletePressed(BaseButton.ButtonEventArgs obj)
  {
    if (obj.Button.Pressed)
      this.EnableDelete();
    else
      this.DisableDelete();
  }

  private void OnEntityReplacePressed(BaseButton.ButtonToggledEventArgs args)
  {
    this._placement.Replacement = args.Pressed;
  }

  private void OnEntityPlacementSelected(OptionButton.ItemSelectedEventArgs args)
  {
    this.Screen.EntityPlacementMode.SelectId(args.Id);
    if (this._placement.CurrentMode == null)
      return;
    this._placement.BeginPlacing(new PlacementInformation()
    {
      PlacementOption = this._placement.AllModeNames[args.Id],
      EntityType = this._placement.CurrentPermission.EntityType,
      TileType = this._placement.CurrentPermission.TileType,
      Range = 2,
      IsTile = this._placement.CurrentPermission.IsTile
    }, (PlacementHijack) null);
  }

  private void OnEraseEntityPressed(BaseButton.ButtonEventArgs args)
  {
    if (args.Button.Pressed == this._placement.Eraser)
      return;
    if (args.Button.Pressed)
      this.EnableEraser();
    else
      this.DisableEraser();
  }

  private void OnEraseDecalPressed(BaseButton.ButtonToggledEventArgs args)
  {
    this._placement.Clear();
    this.Deselect();
    ((BaseButton) this.Screen.EraseEntityButton).Pressed = false;
    this._updatePlacement = true;
    this._updateEraseDecal = args.Pressed;
  }

  private void EnableEraser()
  {
    if (this._placement.Eraser)
      return;
    this._placement.Clear();
    this._placement.ToggleEraser();
    ((BaseButton) this.Screen.EntityPlacementMode).Disabled = true;
    ((BaseButton) this.Screen.EraseDecalButton).Pressed = false;
    this.Deselect();
  }

  private void DisableEraser()
  {
    if (!this._placement.Eraser)
      return;
    this._placement.ToggleEraser();
    ((BaseButton) this.Screen.EntityPlacementMode).Disabled = false;
  }

  private void EnablePick()
  {
    this.Screen.UnPressActionsExcept((Control) this.Screen.Pick);
    this.State = MappingState.CursorState.Pick;
  }

  private void DisablePick()
  {
    ((BaseButton) this.Screen.Pick).Pressed = false;
    this.State = MappingState.CursorState.None;
  }

  private void EnableDelete()
  {
    this.Screen.UnPressActionsExcept((Control) this.Screen.Delete);
    this.State = MappingState.CursorState.Delete;
    this.EnableEraser();
  }

  private void DisableDelete()
  {
    ((BaseButton) this.Screen.Delete).Pressed = false;
    this.State = MappingState.CursorState.None;
    this.DisableEraser();
  }

  private bool HandleMappingUnselect(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    MappingSpawnButton selected = this.Screen.Prototypes.Selected;
    if (selected != null)
    {
      MappingPrototype prototype = selected.Prototype;
      if (prototype != null && prototype.Prototype is DecalPrototype)
      {
        this.Deselect();
        return true;
      }
    }
    return false;
  }

  private bool HandleSaveMap(in PointerInputCmdHandler.PointerInputCmdArgs args) => false;

  private bool HandleEnablePick(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
  {
    this.EnablePick();
    return true;
  }

  private bool HandleDisablePick(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
  {
    this.DisablePick();
    return true;
  }

  private bool HandleEnableDelete(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
  {
    this.EnableDelete();
    return true;
  }

  private bool HandleDisableDelete(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
  {
    this.DisableDelete();
    return true;
  }

  private bool HandlePick(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
  {
    if (this.State != MappingState.CursorState.Pick)
      return false;
    MappingPrototype mapping = (MappingPrototype) null;
    EntityUid entityUid;
    MapGridComponent mapGridComponent;
    TileRef tile;
    if (!((EntityUid) ref uid).IsValid() && this._mapMan.TryFindGridAt(((SharedTransformSystem) this._transform).ToMapCoordinates(coords, true), ref entityUid, ref mapGridComponent) && this._entityManager.System<SharedMapSystem>().TryGetTileRef(entityUid, mapGridComponent, coords, ref tile) && this._allPrototypesDict.TryGetValue((IPrototype) this._entityManager.System<TurfSystem>().GetContentTileDefinition(tile), out mapping))
    {
      this.OnSelected(mapping);
      return true;
    }
    if (mapping == null && !EntityUid.op_Equality(uid, EntityUid.Invalid))
    {
      MetaDataComponent componentOrNull = EntityManagerExt.GetComponentOrNull<MetaDataComponent>(this._entityManager, uid);
      if (componentOrNull != null)
      {
        EntityPrototype entityPrototype = componentOrNull.EntityPrototype;
        if (entityPrototype != null && this._allPrototypesDict.TryGetValue((IPrototype) entityPrototype, out mapping))
        {
          this.OnSelected(mapping);
          IPlacementManager placement = this._placement;
          Angle localRotation = this._entityManager.GetComponent<TransformComponent>(uid).LocalRotation;
          Direction dir = ((Angle) ref localRotation).GetDir();
          placement.Direction = dir;
          goto label_9;
        }
      }
      return true;
    }
label_9:
    return true;
  }

  private bool HandleEditorCancelPlace(
    ICommonSession? session,
    EntityCoordinates coords,
    EntityUid uid)
  {
    if (!((BaseButton) this.Screen.EraseDecalButton).Pressed)
      return false;
    this._entityNetwork.SendSystemNetworkMessage((EntityEventArgs) new RequestDecalRemovalEvent(this._entityManager.GetNetCoordinates(coords, (MetaDataComponent) null)), true);
    return true;
  }

  private bool HandleCancelEraseDecal(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (!((BaseButton) this.Screen.EraseDecalButton).Pressed)
      return false;
    ((BaseButton) this.Screen.EraseDecalButton).Pressed = false;
    return true;
  }

  private async void SaveMap() => await this._mapping.SaveMap();

  private void ToggleCollapse(MappingSpawnButton button)
  {
    if (((BaseButton) button.CollapseButton).Pressed)
    {
      if (button.Prototype?.Children != null)
      {
        foreach (MappingPrototype child in button.Prototype.Children)
          this.Screen.Prototypes.Insert((Container) button.ChildrenPrototypes, child, true);
      }
      button.CollapseButton.Label.Text = "▼";
    }
    else
    {
      ((Control) button.ChildrenPrototypes).DisposeAllChildren();
      button.CollapseButton.Label.Text = "▶";
    }
  }

  private void Collapse(MappingSpawnButton button)
  {
    if (!((BaseButton) button.CollapseButton).Pressed)
      return;
    ((BaseButton) button.CollapseButton).Pressed = false;
    this.ToggleCollapse(button);
  }

  private void UnCollapse(MappingSpawnButton button)
  {
    if (((BaseButton) button.CollapseButton).Pressed)
      return;
    ((BaseButton) button.CollapseButton).Pressed = true;
    this.ToggleCollapse(button);
  }

  public EntityUid? GetHoveredEntity()
  {
    if (this.UserInterfaceManager.CurrentlyHovered is IViewportControl currentlyHovered)
    {
      ScreenCoordinates mouseScreenPosition = this._input.MouseScreenPosition;
      if (((ScreenCoordinates) ref mouseScreenPosition).IsValid)
        return this.GetClickedEntity(currentlyHovered.PixelToMap(mouseScreenPosition.Position));
    }
    return new EntityUid?();
  }

  public virtual void FrameUpdate(FrameEventArgs e)
  {
    if (this._updatePlacement)
    {
      this._updatePlacement = false;
      if (!this._placement.IsActive && this._decal.GetActiveDecal().Decal == null)
        this.Deselect();
      ((BaseButton) this.Screen.EraseEntityButton).Pressed = this._placement.Eraser;
      ((BaseButton) this.Screen.EraseDecalButton).Pressed = this._updateEraseDecal;
      ((BaseButton) this.Screen.EntityPlacementMode).Disabled = this._placement.Eraser;
    }
    Control scrollTo = this._scrollTo;
    if (scrollTo == null || (double) scrollTo.Height <= 0.0 || !((Control) this.Screen.Prototypes.PrototypeList).Visible)
      return;
    float y = scrollTo.GlobalPosition.Y - ((Control) this.Screen.Prototypes.ScrollContainer).Height / 2f + scrollTo.Height;
    ScrollContainer scrollContainer = this.Screen.Prototypes.ScrollContainer;
    scrollContainer.SetScrollValue(scrollContainer.GetScrollValue(false) + new Vector2(0.0f, y));
    this._scrollTo = (Control) null;
  }

  public enum CursorState
  {
    None,
    Pick,
    Delete,
  }
}
