using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Content.Client.Hands.Systems;
using Content.Client.Items.Systems;
using Content.Client.Storage;
using Content.Client.Storage.Systems;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Item;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Item;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Collections;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.Storage.Controls;

public sealed class StorageWindow : BaseWindow
{
	[Dependency]
	private IEntityManager _entity;

	[Dependency]
	private IPlayerManager _player;

	private readonly StorageUIController _storageController;

	public EntityUid? StorageEntity;

	private readonly GridContainer _pieceGrid;

	private readonly GridContainer _backgroundGrid;

	private readonly GridContainer _sidebar;

	private Control _titleContainer;

	private Label _titleLabel;

	private readonly Dictionary<EntityUid, (ItemStorageLocation? Loc, ItemGridPiece Control)> _pieces = new Dictionary<EntityUid, (ItemStorageLocation?, ItemGridPiece)>();

	private readonly List<Control> _controlGrid = new List<Control>();

	private ValueList<EntityUid> _contained;

	private ValueList<EntityUid> _toRemove;

	private Vector2i _pieceGridSize;

	private TextureButton? _backButton;

	private bool _isDirty;

	private readonly string _emptyTexturePath = "Storage/cm_tile_empty";

	private Texture? _emptyTexture;

	private readonly string _blockedTexturePath = "Storage/tile_blocked";

	private Texture? _blockedTexture;

	private readonly string _emptyOpaqueTexturePath = "Storage/cm_tile_empty_opaque";

	private Texture? _emptyOpaqueTexture;

	private readonly string _blockedOpaqueTexturePath = "Storage/tile_blocked_opaque";

	private Texture? _blockedOpaqueTexture;

	private readonly string _exitTexturePath = "Storage/exit";

	private Texture? _exitTexture;

	private readonly string _backTexturePath = "Storage/back";

	private Texture? _backTexture;

	private readonly string _sidebarTopTexturePath = "Storage/sidebar_top";

	private Texture? _sidebarTopTexture;

	private readonly string _sidebarMiddleTexturePath = "Storage/sidebar_mid";

	private Texture? _sidebarMiddleTexture;

	private readonly string _sidebarBottomTexturePath = "Storage/sidebar_bottom";

	private Texture? _sidebarBottomTexture;

	private readonly string _sidebarFatTexturePath = "Storage/sidebar_fat";

	private Texture? _sidebarFatTexture;

	private (Box2i Grid, EntityUid[] Contained, Dictionary<EntityUid, ItemStorageLocation> Stored) _lastUpdate = (Grid: default(Box2i), Contained: Array.Empty<EntityUid>(), Stored: new Dictionary<EntityUid, ItemStorageLocation>());

	public event Action<GUIBoundKeyEventArgs, ItemGridPiece>? OnPiecePressed;

	public event Action<GUIBoundKeyEventArgs, ItemGridPiece>? OnPieceUnpressed;

	public StorageWindow()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Expected O, but got Unknown
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Expected O, but got Unknown
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Expected O, but got Unknown
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Expected O, but got Unknown
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Expected O, but got Unknown
		//IL_027f: Expected O, but got Unknown
		//IL_0280: Expected O, but got Unknown
		IoCManager.InjectDependencies<StorageWindow>(this);
		((BaseWindow)this).Resizable = false;
		_storageController = ((Control)this).UserInterfaceManager.GetUIController<StorageUIController>();
		((Control)this).OnThemeUpdated();
		((Control)this).MouseFilter = (MouseFilterMode)0;
		_sidebar = new GridContainer
		{
			Name = "SideBar",
			HSeparationOverride = 0,
			VSeparationOverride = 0,
			Columns = 1
		};
		_pieceGrid = new GridContainer
		{
			Name = "PieceGrid",
			HSeparationOverride = 0,
			VSeparationOverride = 0
		};
		_backgroundGrid = new GridContainer
		{
			Name = "BackgroundGrid",
			HSeparationOverride = 0,
			VSeparationOverride = 0
		};
		_titleLabel = new Label
		{
			HorizontalExpand = true,
			Name = "StorageLabel",
			ClipText = true,
			Text = "Dummy",
			StyleClasses = { "FancyWindowTitle" }
		};
		PanelContainer val = new PanelContainer
		{
			StyleClasses = { "WindowHeadingBackground" }
		};
		((Control)val).Children.Add((Control)(object)_titleLabel);
		_titleContainer = (Control)val;
		PanelContainer val2 = new PanelContainer();
		val2.PanelOverride = (StyleBox)new StyleBoxFlat
		{
			BorderColor = Color.Black,
			BorderThickness = new Thickness(2f)
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		((Control)val3).Children.Add(_titleContainer);
		OrderedChildCollection children = ((Control)val3).Children;
		BoxContainer val4 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0
		};
		((Control)val4).Children.Add((Control)(object)_sidebar);
		OrderedChildCollection children2 = ((Control)val4).Children;
		Control val5 = new Control();
		val5.Children.Add((Control)(object)_backgroundGrid);
		val5.Children.Add((Control)(object)_pieceGrid);
		val5.Children.Add((Control)(object)val2);
		children2.Add(val5);
		children.Add((Control)val4);
		BoxContainer val6 = val3;
		((Control)this).AddChild((Control)(object)val6);
	}

	protected override void OnThemeUpdated()
	{
		((Control)this).OnThemeUpdated();
		TextureResource obj = ((Control)this).Theme.ResolveTextureOrNull(_emptyTexturePath);
		_emptyTexture = ((obj != null) ? obj.Texture : null);
		TextureResource obj2 = ((Control)this).Theme.ResolveTextureOrNull(_blockedTexturePath);
		_blockedTexture = ((obj2 != null) ? obj2.Texture : null);
		TextureResource obj3 = ((Control)this).Theme.ResolveTextureOrNull(_emptyOpaqueTexturePath);
		_emptyOpaqueTexture = ((obj3 != null) ? obj3.Texture : null);
		TextureResource obj4 = ((Control)this).Theme.ResolveTextureOrNull(_blockedOpaqueTexturePath);
		_blockedOpaqueTexture = ((obj4 != null) ? obj4.Texture : null);
		TextureResource obj5 = ((Control)this).Theme.ResolveTextureOrNull(_exitTexturePath);
		_exitTexture = ((obj5 != null) ? obj5.Texture : null);
		TextureResource obj6 = ((Control)this).Theme.ResolveTextureOrNull(_backTexturePath);
		_backTexture = ((obj6 != null) ? obj6.Texture : null);
		TextureResource obj7 = ((Control)this).Theme.ResolveTextureOrNull(_sidebarTopTexturePath);
		_sidebarTopTexture = ((obj7 != null) ? obj7.Texture : null);
		TextureResource obj8 = ((Control)this).Theme.ResolveTextureOrNull(_sidebarMiddleTexturePath);
		_sidebarMiddleTexture = ((obj8 != null) ? obj8.Texture : null);
		TextureResource obj9 = ((Control)this).Theme.ResolveTextureOrNull(_sidebarBottomTexturePath);
		_sidebarBottomTexture = ((obj9 != null) ? obj9.Texture : null);
		TextureResource obj10 = ((Control)this).Theme.ResolveTextureOrNull(_sidebarFatTexturePath);
		_sidebarFatTexture = ((obj10 != null) ? obj10.Texture : null);
	}

	public void UpdateContainer(Entity<StorageComponent>? entity)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).Visible = entity.HasValue;
		Entity<StorageComponent>? val = entity;
		StorageEntity = (val.HasValue ? new EntityUid?(Entity<StorageComponent>.op_Implicit(val.GetValueOrDefault())) : ((EntityUid?)null));
		if (entity.HasValue)
		{
			if (((Control)this).UserInterfaceManager.GetUIController<StorageUIController>().WindowTitle)
			{
				_titleLabel.Text = Identity.Name(Entity<StorageComponent>.op_Implicit(entity.Value), _entity);
				_titleContainer.Visible = true;
			}
			else
			{
				_titleContainer.Visible = false;
			}
			BuildGridRepresentation();
		}
	}

	private void CloseParent()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (StorageEntity.HasValue)
		{
			SharedContainerSystem obj = _entity.System<SharedContainerSystem>();
			UserInterfaceSystem val = _entity.System<UserInterfaceSystem>();
			BaseContainer val2 = default(BaseContainer);
			StorageComponent storageComponent = default(StorageComponent);
			StorageBoundUserInterface storageBoundUserInterface = default(StorageBoundUserInterface);
			if (obj.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(StorageEntity.Value), ref val2) && _entity.TryGetComponent<StorageComponent>(val2.Owner, ref storageComponent) && ((BaseContainer)storageComponent.Container).Contains(StorageEntity.Value) && ((SharedUserInterfaceSystem)val).TryGetOpenUi<StorageBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(val2.Owner), (Enum)StorageComponent.StorageUiKey.Key, ref storageBoundUserInterface))
			{
				storageBoundUserInterface.CloseWindow(((Control)this).Position);
			}
		}
	}

	private void BuildGridRepresentation()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_0111: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Expected O, but got Unknown
		//IL_01e0: Expected O, but got Unknown
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Expected O, but got Unknown
		StorageComponent storageComponent = default(StorageComponent);
		if (!_entity.TryGetComponent<StorageComponent>(StorageEntity, ref storageComponent) || storageComponent.Grid.Count == 0)
		{
			return;
		}
		Box2i boundingBox = storageComponent.Grid.GetBoundingBox();
		BuildBackground();
		((Control)_sidebar).Children.Clear();
		int num = ((Box2i)(ref boundingBox)).Height + 1;
		_sidebar.Rows = num;
		TextureButton val = new TextureButton
		{
			Name = "ExitButton",
			TextureNormal = _exitTexture,
			Scale = new Vector2(2f, 2f)
		};
		((BaseButton)val).OnPressed += delegate
		{
			((BaseWindow)this).Close();
			CloseParent();
		};
		((Control)val).OnKeyBindDown += delegate(GUIBoundKeyEventArgs args)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (!((BoundKeyEventArgs)args).Handled && ((BoundKeyEventArgs)args).Function == ContentKeyFunctions.ActivateItemInWorld)
			{
				((BaseWindow)this).Close();
				CloseParent();
				((BoundKeyEventArgs)args).Handle();
			}
		};
		BoxContainer val2 = new BoxContainer
		{
			Name = "ExitContainer"
		};
		OrderedChildCollection children = ((Control)val2).Children;
		TextureRect val3 = new TextureRect
		{
			Texture = ((((Box2i)(ref boundingBox)).Height != 0) ? _sidebarTopTexture : _sidebarFatTexture),
			TextureScale = new Vector2(2f, 2f)
		};
		((Control)val3).Children.Add((Control)(object)val);
		children.Add((Control)val3);
		BoxContainer val4 = val2;
		((Control)_sidebar).AddChild((Control)(object)val4);
		int num2 = 2;
		if (_entity.System<StorageSystem>().NestedStorage && num > 0)
		{
			_backButton = new TextureButton
			{
				TextureNormal = _backTexture,
				Scale = new Vector2(2f, 2f)
			};
			((BaseButton)_backButton).OnPressed += delegate
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				BaseContainer val8 = default(BaseContainer);
				StorageComponent storageComponent2 = default(StorageComponent);
				if (_entity.System<SharedContainerSystem>().TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(StorageEntity.Value), ref val8) && _entity.TryGetComponent<StorageComponent>(val8.Owner, ref storageComponent2) && ((BaseContainer)storageComponent2.Container).Contains(StorageEntity.Value))
				{
					((BaseWindow)this).Close();
					StorageBoundUserInterface storageBoundUserInterface = default(StorageBoundUserInterface);
					if (_entity.System<SharedUserInterfaceSystem>().TryGetOpenUi<StorageBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(val8.Owner), (Enum)StorageComponent.StorageUiKey.Key, ref storageBoundUserInterface))
					{
						storageBoundUserInterface.Show(((Control)this).Position);
					}
				}
			};
			BoxContainer val5 = new BoxContainer
			{
				Name = "ExitContainer"
			};
			OrderedChildCollection children2 = ((Control)val5).Children;
			TextureRect val6 = new TextureRect
			{
				Texture = ((num > 2) ? _sidebarMiddleTexture : _sidebarBottomTexture),
				TextureScale = new Vector2(2f, 2f)
			};
			((Control)val6).Children.Add((Control)(object)_backButton);
			children2.Add((Control)val6);
			BoxContainer val7 = val5;
			((Control)_sidebar).AddChild((Control)(object)val7);
		}
		int num3 = num - num2;
		for (int num4 = 0; num4 < num3; num4++)
		{
			((Control)_sidebar).AddChild((Control)new TextureRect
			{
				Texture = ((num4 != num3 - 1) ? _sidebarMiddleTexture : _sidebarBottomTexture),
				TextureScale = new Vector2(2f, 2f)
			});
		}
		FlagDirty();
	}

	public void BuildBackground()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		StorageComponent storageComponent = default(StorageComponent);
		if (!_entity.TryGetComponent<StorageComponent>(StorageEntity, ref storageComponent) || !storageComponent.Grid.Any())
		{
			return;
		}
		Box2i boundingBox = storageComponent.Grid.GetBoundingBox();
		Texture val = (_storageController.OpaqueStorageWindow ? _emptyOpaqueTexture : _emptyTexture);
		Texture val2 = (_storageController.OpaqueStorageWindow ? _blockedOpaqueTexture : _blockedTexture);
		((Control)_backgroundGrid).Children.Clear();
		_backgroundGrid.Rows = ((Box2i)(ref boundingBox)).Height + 1;
		_backgroundGrid.Columns = ((Box2i)(ref boundingBox)).Width + 1;
		int? fixedSizeX = EntityManagerExt.GetComponentOrNull<FixedItemSizeStorageComponent>(_entity, StorageEntity)?.Size.X;
		for (int i = boundingBox.Bottom; i <= boundingBox.Top; i++)
		{
			for (int j = boundingBox.Left; j <= boundingBox.Right; j++)
			{
				Texture texture = (storageComponent.Grid.Contains(j, i) ? val : val2);
				TextureRect val3 = new TextureRect
				{
					Texture = texture,
					TextureScale = new Vector2(2f, 2f)
				};
				PanelContainer val4 = WrapBorders((Control)(object)val3, fixedSizeX, j, boundingBox.Right);
				if (val4 != null)
				{
					((Control)_backgroundGrid).AddChild((Control)(object)val4);
				}
				else
				{
					((Control)_backgroundGrid).AddChild((Control)(object)val3);
				}
			}
		}
	}

	public void Reclaim(ItemStorageLocation location, ItemGridPiece draggingGhost)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		draggingGhost.OnPiecePressed += this.OnPiecePressed;
		draggingGhost.OnPieceUnpressed += this.OnPieceUnpressed;
		_pieces[draggingGhost.Entity] = (location, draggingGhost);
		draggingGhost.Location = location;
		int gridIndex = GetGridIndex(draggingGhost);
		_controlGrid[gridIndex].AddChild((Control)(object)draggingGhost);
	}

	private int GetGridIndex(ItemGridPiece piece)
	{
		return piece.Location.Position.X + piece.Location.Position.Y * _pieceGrid.Columns;
	}

	public void FlagDirty()
	{
		_isDirty = true;
	}

	public void RemoveGrid(ItemGridPiece control)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((Control)control).Orphan();
		_pieces.Remove(control.Entity);
		control.OnPiecePressed -= this.OnPiecePressed;
		control.OnPieceUnpressed -= this.OnPieceUnpressed;
	}

	public void BuildItemPieces()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Expected O, but got Unknown
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		StorageComponent storageComp = default(StorageComponent);
		if (!_entity.TryGetComponent<StorageComponent>(StorageEntity, ref storageComp) || storageComp.Grid.Count == 0)
		{
			return;
		}
		Box2i boundingBox = storageComp.Grid.GetBoundingBox();
		Vector2i val = _emptyTexture.Size * 2;
		EntityUid[] array = ((BaseContainer)storageComp.Container).ContainedEntities.Reverse().ToArray();
		if (((Box2i)(ref _lastUpdate.Grid)).Equals(boundingBox) && Enumerable.SequenceEqual(_lastUpdate.Contained, array) && _lastUpdate.Stored.Count == storageComp.StoredItems.Count && _lastUpdate.Stored.All((KeyValuePair<EntityUid, ItemStorageLocation> kvp) => storageComp.StoredItems.TryGetValue(kvp.Key, out var value6) && kvp.Value == value6))
		{
			return;
		}
		_lastUpdate = (Grid: boundingBox, Contained: array, Stored: storageComp.StoredItems);
		_contained.Clear();
		_contained.AddRange(((BaseContainer)storageComp.Container).ContainedEntities.Reverse());
		int num = ((Box2i)(ref boundingBox)).Width + 1;
		int num2 = ((Box2i)(ref boundingBox)).Height + 1;
		if (_pieceGrid.Rows != _pieceGridSize.Y || _pieceGrid.Columns != _pieceGridSize.X)
		{
			_pieceGrid.Rows = num2;
			_pieceGrid.Columns = num;
			_controlGrid.Clear();
			int? fixedSizeX = EntityManagerExt.GetComponentOrNull<FixedItemSizeStorageComponent>(_entity, StorageEntity)?.Size.X;
			for (int num3 = boundingBox.Bottom; num3 <= boundingBox.Top; num3++)
			{
				for (int num4 = boundingBox.Left; num4 <= boundingBox.Right; num4++)
				{
					Control val2 = new Control
					{
						MinSize = Vector2i.op_Implicit(val)
					};
					PanelContainer val3 = WrapBorders(val2, fixedSizeX, num4, boundingBox.Right);
					if (val3 != null)
					{
						((Control)_pieceGrid).AddChild((Control)(object)val3);
					}
					else
					{
						((Control)_pieceGrid).AddChild(val2);
					}
					_controlGrid.Add(val2);
				}
			}
		}
		_pieceGridSize = new Vector2i(num, num2);
		_toRemove.Clear();
		EntityUid key;
		foreach (KeyValuePair<EntityUid, (ItemStorageLocation?, ItemGridPiece)> piece in _pieces)
		{
			piece.Deconstruct(out key, out var value);
			EntityUid val4 = key;
			(ItemStorageLocation?, ItemGridPiece) tuple = value;
			if (storageComp.StoredItems.TryGetValue(val4, out var value2))
			{
				tuple.Item2.Marked = IsMarked(val4);
				var (itemStorageLocation, _) = tuple;
				if (!itemStorageLocation.Equals(value2))
				{
					tuple.Item2.Location = value2;
					int gridIndex = GetGridIndex(tuple.Item2);
					((Control)tuple.Item2).Orphan();
					_controlGrid[gridIndex].AddChild((Control)(object)tuple.Item2);
					_pieces[val4] = (value2, tuple.Item2);
				}
			}
			else
			{
				_toRemove.Add(val4);
			}
		}
		Enumerator<EntityUid> enumerator2 = _toRemove.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				EntityUid current = enumerator2.Current;
				_pieces.Remove(current, out (ItemStorageLocation?, ItemGridPiece) value3);
				((Control)value3.Item2).Orphan();
			}
		}
		finally
		{
			((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
		}
		ItemComponent item = default(ItemComponent);
		foreach (KeyValuePair<EntityUid, ItemStorageLocation> storedItem in storageComp.StoredItems)
		{
			storedItem.Deconstruct(out key, out var value4);
			EntityUid val5 = key;
			ItemStorageLocation itemStorageLocation2 = value4;
			if (!_pieces.TryGetValue(val5, out (ItemStorageLocation?, ItemGridPiece) _) && _entity.TryGetComponent<ItemComponent>(val5, ref item))
			{
				ItemGridPiece itemGridPiece = new ItemGridPiece(Entity<ItemComponent>.op_Implicit((val5, item)), itemStorageLocation2, _entity);
				((Control)itemGridPiece).MinSize = Vector2i.op_Implicit(val);
				itemGridPiece.Marked = IsMarked(val5);
				ItemGridPiece itemGridPiece2 = itemGridPiece;
				itemGridPiece2.OnPiecePressed += this.OnPiecePressed;
				itemGridPiece2.OnPieceUnpressed += this.OnPieceUnpressed;
				int index = itemStorageLocation2.Position.X + itemStorageLocation2.Position.Y * (((Box2i)(ref boundingBox)).Width + 1);
				_controlGrid[index].AddChild((Control)(object)itemGridPiece2);
				_pieces[val5] = (itemStorageLocation2, itemGridPiece2);
			}
		}
	}

	private ItemGridPieceMarks? IsMarked(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _contained.IndexOf(uid) switch
		{
			0 => ItemGridPieceMarks.First, 
			1 => ItemGridPieceMarks.Second, 
			_ => null, 
		};
	}

	protected unsafe override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		if (!((BaseWindow)this).IsOpen)
		{
			return;
		}
		if (_isDirty)
		{
			_isDirty = false;
			BuildItemPieces();
		}
		SharedContainerSystem val = _entity.System<SharedContainerSystem>();
		if (_backButton != null)
		{
			if (StorageEntity.HasValue && _entity.System<StorageSystem>().NestedStorage)
			{
				BaseContainer val2 = default(BaseContainer);
				StorageComponent storageComponent = default(StorageComponent);
				if (val.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(StorageEntity.Value), ref val2) && _entity.TryGetComponent<StorageComponent>(val2.Owner, ref storageComponent) && ((BaseContainer)storageComponent.Container).Contains(StorageEntity.Value))
				{
					((Control)_backButton).Visible = true;
				}
				else
				{
					((Control)_backButton).Visible = false;
				}
			}
			else
			{
				((Control)_backButton).Visible = false;
			}
		}
		ItemSystem itemSystem = _entity.System<ItemSystem>();
		StorageSystem storageSystem = _entity.System<StorageSystem>();
		HandsSystem handsSystem = _entity.System<HandsSystem>();
		Enumerator enumerator = ((Control)_backgroundGrid).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				((Enumerator)(ref enumerator)).Current.ModulateSelfOverride = null;
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		StorageComponent storageComponent2 = default(StorageComponent);
		if ((((Control)this).UserInterfaceManager.CurrentlyHovered is StorageWindow storageWindow && storageWindow != this) || !_entity.TryGetComponent<StorageComponent>(StorageEntity, ref storageComponent2))
		{
			return;
		}
		bool flag = false;
		EntityUid val3;
		ItemStorageLocation location;
		if (_storageController.IsDragging)
		{
			ItemGridPiece draggingGhost = _storageController.DraggingGhost;
			if (draggingGhost != null)
			{
				val3 = draggingGhost.Entity;
				location = draggingGhost.Location;
				goto IL_01fc;
			}
		}
		EntityUid? activeHandEntity = handsSystem.GetActiveHandEntity();
		if (activeHandEntity.HasValue)
		{
			EntityUid valueOrDefault = activeHandEntity.GetValueOrDefault();
			if (storageSystem.CanInsert(StorageEntity.Value, valueOrDefault, ((ISharedPlayerManager)_player).LocalEntity, out string _, storageComponent2, null, ignoreStacks: false, ignoreLocation: true))
			{
				val3 = valueOrDefault;
				location = new ItemStorageLocation(_storageController.DraggingRotation, Vector2i.Zero);
				flag = true;
				goto IL_01fc;
			}
			return;
		}
		return;
		IL_01fc:
		ItemComponent item = default(ItemComponent);
		if (!_entity.TryGetComponent<ItemComponent>(val3, ref item))
		{
			return;
		}
		Vector2i mouseGridPieceLocation = GetMouseGridPieceLocation(Entity<ItemComponent>.op_Implicit((val3, item)), location);
		IReadOnlyList<Box2i> adjustedItemShape = itemSystem.GetAdjustedItemShape(Entity<StorageComponent>.op_Implicit((StorageEntity.Value, storageComponent2)), Entity<ItemComponent>.op_Implicit((val3, item)), location.Rotation, mouseGridPieceLocation);
		Box2i boundingBox = adjustedItemShape.GetBoundingBox();
		storageSystem.ItemFitsInGridLocation(Entity<ItemComponent>.op_Implicit((val3, item)), Entity<StorageComponent>.op_Implicit((StorageEntity.Value, storageComponent2)), mouseGridPieceLocation, location.Rotation);
		MetaDataComponent val4 = default(MetaDataComponent);
		foreach (KeyValuePair<string, List<ItemStorageLocation>> savedLocation in storageComponent2.SavedLocations)
		{
			if (!_entity.TryGetComponent<MetaDataComponent>(val3, ref val4) || val4.EntityName != savedLocation.Key)
			{
				continue;
			}
			float num = 0f;
			ValueList<Control> val5 = default(ValueList<Control>);
			foreach (ItemStorageLocation item2 in savedLocation.Value)
			{
				IReadOnlyList<Box2i> adjustedItemShape2 = itemSystem.GetAdjustedItemShape(Entity<StorageComponent>.op_Implicit((StorageEntity.Value, storageComponent2)), Entity<ItemComponent>.op_Implicit(val3), item2);
				Box2i boundingBox2 = adjustedItemShape2.GetBoundingBox();
				bool flag2 = storageSystem.ItemFitsInGridLocation(Entity<ItemComponent>.op_Implicit(val3), Entity<StorageComponent>.op_Implicit(StorageEntity.Value), item2);
				if (flag2)
				{
					num += 1f;
				}
				for (int i = boundingBox2.Bottom; i <= boundingBox2.Top; i++)
				{
					for (int j = boundingBox2.Left; j <= boundingBox2.Right; j++)
					{
						if (TryGetBackgroundCell(j, i, out Control cell) && adjustedItemShape2.Contains(j, i) && !val5.Contains(cell))
						{
							val5.Add(cell);
							cell.ModulateSelfOverride = (flag2 ? Color.FromHsv(new Vector4(0.18f, 1f / num, 0.5f / num + 0.5f, 1f)) : Color.FromHex((ReadOnlySpan<char>)"#2222CC", (Color?)null));
						}
					}
				}
			}
		}
		if (!flag)
		{
			Color.FromHex((ReadOnlySpan<char>)"#1E8000", (Color?)null);
		}
		else
		{
			_ = Color.Goldenrod;
		}
		for (int k = boundingBox.Bottom; k <= boundingBox.Top; k++)
		{
			for (int l = boundingBox.Left; l <= boundingBox.Right; l++)
			{
				if (TryGetBackgroundCell(l, k, out Control _))
				{
					adjustedItemShape.Contains(l, k);
				}
			}
		}
	}

	protected override DragMode GetDragModeFor(Vector2 relativeMousePos)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!_storageController.StaticStorageUIEnabled)
		{
			UIBox2 sizeBox = ((Control)_sidebar).SizeBox;
			if (!((UIBox2)(ref sizeBox)).Contains(relativeMousePos - ((Control)_sidebar).Position, true))
			{
				return (DragMode)0;
			}
			return (DragMode)1;
		}
		return (DragMode)0;
	}

	public Vector2i GetMouseGridPieceLocation(Entity<ItemComponent?> entity, ItemStorageLocation location)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Vector2i zero = Vector2i.Zero;
		StorageComponent storageComponent = default(StorageComponent);
		if (!_entity.TryGetComponent<StorageComponent>(StorageEntity, ref storageComponent))
		{
			return zero;
		}
		zero = storageComponent.Grid.GetBoundingBox().BottomLeft;
		Vector2 vector = Vector2i.op_Implicit(_emptyTexture.Size) * 2f;
		return Vector2Helpers.Floored((((Control)this).UserInterfaceManager.MousePositionScaled.Position - ((Control)_backgroundGrid).GlobalPosition - ItemGridPiece.GetCenterOffset(Entity<StorageComponent>.op_Implicit((StorageEntity.Value, storageComponent)), entity, location, _entity) * 2f + vector / 2f) / vector) + zero;
	}

	public bool TryGetBackgroundCell(int x, int y, [NotNullWhen(true)] out Control? cell)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		cell = null;
		StorageComponent storageComponent = default(StorageComponent);
		if (!_entity.TryGetComponent<StorageComponent>(StorageEntity, ref storageComponent))
		{
			return false;
		}
		Box2i boundingBox = storageComponent.Grid.GetBoundingBox();
		x -= boundingBox.Left;
		y -= boundingBox.Bottom;
		if (x < 0 || x >= _backgroundGrid.Columns || y < 0 || y >= _backgroundGrid.Rows)
		{
			return false;
		}
		cell = ((Control)_backgroundGrid).GetChild(y * _backgroundGrid.Columns + x);
		return true;
	}

	protected override void KeyBindDown(GUIBoundKeyEventArgs args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		((BaseWindow)this).KeyBindDown(args);
		if (!((BaseWindow)this).IsOpen)
		{
			return;
		}
		StorageSystem storageSystem = _entity.System<StorageSystem>();
		HandsSystem handsSystem = _entity.System<HandsSystem>();
		if (!(((BoundKeyEventArgs)args).Function == ContentKeyFunctions.MoveStoredItem) || !StorageEntity.HasValue)
		{
			return;
		}
		EntityUid? activeHandEntity = handsSystem.GetActiveHandEntity();
		if (activeHandEntity.HasValue)
		{
			EntityUid valueOrDefault = activeHandEntity.GetValueOrDefault();
			if (storageSystem.CanInsert(StorageEntity.Value, valueOrDefault, ((ISharedPlayerManager)_player).LocalEntity, out string _) && CMInventoryExtensions.TryGetFirst(StorageEntity.Value, valueOrDefault, out var location) && storageSystem.ItemFitsInGridLocation(Entity<ItemComponent>.op_Implicit((ValueTuple<EntityUid, ItemComponent>)(valueOrDefault, null)), Entity<StorageComponent>.op_Implicit((ValueTuple<EntityUid, StorageComponent>)(StorageEntity.Value, null)), location))
			{
				_entity.RaisePredictiveEvent<StorageInsertItemIntoLocationEvent>(new StorageInsertItemIntoLocationEvent(_entity.GetNetEntity(valueOrDefault, (MetaDataComponent)null), _entity.GetNetEntity(StorageEntity.Value, (MetaDataComponent)null), location));
				_storageController.DraggingRotation = Angle.Zero;
				((BoundKeyEventArgs)args).Handle();
			}
		}
	}

	private PanelContainer? WrapBorders(Control control, int? fixedSizeX, int x, int right)
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		if (fixedSizeX.HasValue && x != 0 && (x != right || x % fixedSizeX == 0))
		{
			Thickness borderThickness = ((x % fixedSizeX == 0) ? new Thickness(1f, 0f, 0f, 0f) : new Thickness(0f, 0f, 1f, 0f));
			PanelContainer val = new PanelContainer
			{
				PanelOverride = (StyleBox)new StyleBoxFlat
				{
					BackgroundColor = Color.Transparent,
					BorderColor = Color.Black,
					BorderThickness = borderThickness
				}
			};
			((Control)val).AddChild(control);
			return val;
		}
		return null;
	}
}
