using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client.Examine;
using Content.Client.Hands.Systems;
using Content.Client.Interaction;
using Content.Client.Storage;
using Content.Client.Storage.Systems;
using Content.Client.UserInterface.Systems.Hotbar.Widgets;
using Content.Client.UserInterface.Systems.Info;
using Content.Client.UserInterface.Systems.Storage.Controls;
using Content.Client.Verbs.UI;
using Content.Shared.CCVar;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.Storage;

public sealed class StorageUIController : UIController, IOnSystemChanged<StorageSystem>, IOnSystemLoaded<StorageSystem>, IOnSystemUnloaded<StorageSystem>
{
	[Dependency]
	private IConfigurationManager _configuration;

	[Dependency]
	private IInputManager _input;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private CloseRecentWindowUIController _closeRecentWindowUIController;

	[UISystemDependency]
	private readonly StorageSystem _storage;

	[UISystemDependency]
	private readonly UserInterfaceSystem _ui;

	private readonly DragDropHelper<ItemGridPiece> _menuDragHelper;

	public Angle DraggingRotation = Angle.Zero;

	public bool StaticStorageUIEnabled;

	public bool OpaqueStorageWindow;

	private int _openStorageLimit = -1;

	public ItemGridPiece? DraggingGhost => _menuDragHelper.Dragged;

	public bool IsDragging => _menuDragHelper.IsDragging;

	public ItemGridPiece? CurrentlyDragging => _menuDragHelper.Dragged;

	public bool WindowTitle { get; private set; }

	public StorageUIController()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_menuDragHelper = new DragDropHelper<ItemGridPiece>(OnMenuBeginDrag, OnMenuContinueDrag, OnMenuEndDrag);
	}

	public override void Initialize()
	{
		((UIController)this).Initialize();
		_configuration.OnValueChanged<bool>(CCVars.StaticStorageUI, (Action<bool>)OnStaticStorageChanged, true);
		_configuration.OnValueChanged<bool>(CCVars.OpaqueStorageWindow, (Action<bool>)OnOpaqueWindowChanged, true);
		_configuration.OnValueChanged<bool>(CCVars.StorageWindowTitle, (Action<bool>)OnStorageWindowTitle, true);
		_configuration.OnValueChanged<int>(CCVars.StorageLimit, (Action<int>)OnStorageLimitChanged, true);
	}

	private void OnStorageLimitChanged(int obj)
	{
		_openStorageLimit = obj;
	}

	private void OnStorageWindowTitle(bool obj)
	{
		WindowTitle = obj;
	}

	private void OnOpaqueWindowChanged(bool obj)
	{
		OpaqueStorageWindow = obj;
	}

	private void OnStaticStorageChanged(bool obj)
	{
		StaticStorageUIEnabled = obj;
	}

	public StorageWindow CreateStorageWindow(StorageBoundUserInterface sBui)
	{
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		StorageWindow window = new StorageWindow();
		((Control)window).MouseFilter = (MouseFilterMode)1;
		window.OnPiecePressed += delegate(GUIBoundKeyEventArgs args, ItemGridPiece piece)
		{
			OnPiecePressed(args, window, piece);
		};
		window.OnPieceUnpressed += delegate(GUIBoundKeyEventArgs args, ItemGridPiece piece)
		{
			OnPieceUnpressed(args, window, piece);
		};
		StorageBoundUserInterface storageBoundUserInterface = default(StorageBoundUserInterface);
		Vector2 vector = default(Vector2);
		if (StaticStorageUIEnabled)
		{
			HotbarGui activeUIWidgetOrNull = base.UIManager.GetActiveUIWidgetOrNull<HotbarGui>();
			Action<Control, Control> action = delegate(Control? parent, Control child)
			{
				if (parent != null)
				{
					int num = ((IEnumerable<Control>)parent.Children).ToList().FindIndex((Control c) => !c.Visible);
					if (num != -1)
					{
						child.SetPositionInParent(num);
					}
				}
			};
			if (activeUIWidgetOrNull != null)
			{
				((Control)activeUIWidgetOrNull.DoubleStorageContainer).Visible = _openStorageLimit == 2;
				((Control)activeUIWidgetOrNull.SingleStorageContainer).Visible = _openStorageLimit != 2;
			}
			if (_openStorageLimit == 2)
			{
				if (activeUIWidgetOrNull != null && !((IEnumerable<Control>)((Control)activeUIWidgetOrNull.LeftStorageContainer).Children).Any((Control c) => c.Visible))
				{
					if (activeUIWidgetOrNull != null)
					{
						((Control)activeUIWidgetOrNull.LeftStorageContainer).AddChild((Control)(object)window);
					}
					action((Control)(object)activeUIWidgetOrNull?.LeftStorageContainer, (Control)(object)window);
				}
				else
				{
					if (activeUIWidgetOrNull != null)
					{
						((Control)activeUIWidgetOrNull.RightStorageContainer).AddChild((Control)(object)window);
					}
					action((Control)(object)activeUIWidgetOrNull?.RightStorageContainer, (Control)(object)window);
				}
			}
			else
			{
				if (activeUIWidgetOrNull != null)
				{
					((Control)activeUIWidgetOrNull.SingleStorageContainer).AddChild((Control)(object)window);
				}
				action((Control)(object)activeUIWidgetOrNull?.SingleStorageContainer, (Control)(object)window);
			}
			_closeRecentWindowUIController.SetMostRecentlyInteractedWindow((BaseWindow)(object)window);
		}
		else if (((SharedUserInterfaceSystem)_ui).TryGetOpenUi<StorageBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(base.EntityManager.GetComponent<TransformComponent>(((BoundUserInterface)sBui).Owner).ParentUid), (Enum)StorageComponent.StorageUiKey.Key, ref storageBoundUserInterface) && storageBoundUserInterface.Position.HasValue)
		{
			((BaseWindow)window).Open(storageBoundUserInterface.Position.Value);
		}
		else if (((SharedUserInterfaceSystem)_ui).TryGetPosition(Entity<UserInterfaceComponent>.op_Implicit(((BoundUserInterface)sBui).Owner), (Enum)StorageComponent.StorageUiKey.Key, ref vector))
		{
			((BaseWindow)window).Open(vector);
		}
		else
		{
			((BaseWindow)window).OpenCenteredLeft();
		}
		_ui.RegisterControl((BoundUserInterface)(object)sBui, (Control)(object)window);
		return window;
	}

	public void OnSystemLoaded(StorageSystem system)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		_input.FirstChanceOnKeyEvent += new KeyEventAction(OnMiddleMouse);
	}

	public void OnSystemUnloaded(StorageSystem system)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		_input.FirstChanceOnKeyEvent -= new KeyEventAction(OnMiddleMouse);
	}

	private void OnMiddleMouse(KeyEventArgs keyEvent, KeyEventType type)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Invalid comparison between Unknown and I4
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Invalid comparison between Unknown and I4
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Invalid comparison between Unknown and I4
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Invalid comparison between Unknown and I4
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Invalid comparison between Unknown and I4
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Invalid comparison between Unknown and I4
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Invalid comparison between Unknown and I4
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Invalid comparison between Unknown and I4
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Invalid comparison between Unknown and I4
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		IKeyBinding val = default(IKeyBinding);
		if (!((InputEventArgs)keyEvent).Handled && (int)type == 0 && _input.TryGetKeyBinding(ContentKeyFunctions.RotateStoredItem, ref val) && val.BaseKey == keyEvent.Key && (!((ModifierInputEventArgs)keyEvent).Shift || (int)val.Mod1 == 58 || (int)val.Mod2 == 58 || (int)val.Mod3 == 58) && (!((ModifierInputEventArgs)keyEvent).Alt || (int)val.Mod1 == 59 || (int)val.Mod2 == 59 || (int)val.Mod3 == 59) && (!((ModifierInputEventArgs)keyEvent).Control || (int)val.Mod1 == 57 || (int)val.Mod2 == 57 || (int)val.Mod3 == 57) && (IsDragging || base.EntityManager.System<HandsSystem>().GetActiveHandEntity().HasValue) && (DraggingGhost != null || base.UIManager.CurrentlyHovered is StorageWindow))
		{
			Angle val2 = DraggingRotation + Angle.op_Implicit(Math.PI / 2.0);
			DraggingRotation = DirectionExtensions.ToAngle(((Angle)(ref val2)).GetCardinalDir());
			if (DraggingGhost != null)
			{
				DraggingGhost.Location.Rotation = DraggingRotation;
			}
			if (IsDragging || base.UIManager.CurrentlyHovered is StorageWindow)
			{
				((InputEventArgs)keyEvent).Handle();
			}
		}
	}

	private void OnPiecePressed(GUIBoundKeyEventArgs args, StorageWindow window, ItemGridPiece control)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		if (IsDragging || !((BaseWindow)window).IsOpen)
		{
			return;
		}
		if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.MoveStoredItem)
		{
			((BoundKeyEventArgs)args).Handle();
		}
		else if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.SaveItemLocation)
		{
			EntityUid? storageEntity = window.StorageEntity;
			if (!storageEntity.HasValue)
			{
				return;
			}
			EntityUid valueOrDefault = storageEntity.GetValueOrDefault();
			base.EntityManager.RaisePredictiveEvent<StorageSaveItemLocationEvent>(new StorageSaveItemLocationEvent(base.EntityManager.GetNetEntity(control.Entity, (MetaDataComponent)null), base.EntityManager.GetNetEntity(valueOrDefault, (MetaDataComponent)null)));
			((BoundKeyEventArgs)args).Handle();
		}
		else if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.ExamineEntity)
		{
			base.EntityManager.System<ExamineSystem>().DoExamine(control.Entity);
			((BoundKeyEventArgs)args).Handle();
		}
		else if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UseSecondary)
		{
			base.UIManager.GetUIController<VerbMenuUIController>().OpenVerbMenu(control.Entity);
			((BoundKeyEventArgs)args).Handle();
		}
		else if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.ActivateItemInWorld)
		{
			base.EntityManager.RaisePredictiveEvent<InteractInventorySlotEvent>(new InteractInventorySlotEvent(base.EntityManager.GetNetEntity(control.Entity, (MetaDataComponent)null)));
			((BoundKeyEventArgs)args).Handle();
		}
		else if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.AltActivateItemInWorld)
		{
			base.EntityManager.RaisePredictiveEvent<InteractInventorySlotEvent>(new InteractInventorySlotEvent(base.EntityManager.GetNetEntity(control.Entity, (MetaDataComponent)null), altInteract: true));
			((BoundKeyEventArgs)args).Handle();
		}
		window.FlagDirty();
	}

	private void OnPieceUnpressed(GUIBoundKeyEventArgs args, StorageWindow window, ItemGridPiece control)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		if (((BoundKeyEventArgs)args).Function != ContentKeyFunctions.MoveStoredItem)
		{
			return;
		}
		((Control)control).MouseFilter = (MouseFilterMode)2;
		Control obj = base.UIManager.MouseGetControl(((BoundKeyEventArgs)args).PointerLocation);
		StorageWindow storageWindow = obj as StorageWindow;
		((Control)control).MouseFilter = (MouseFilterMode)1;
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		window.RemoveGrid(control);
		window.FlagDirty();
		if (!(obj is ItemGridPiece))
		{
			EntityUid? storageEntity = window.StorageEntity;
			if (storageEntity.HasValue)
			{
				EntityUid valueOrDefault = storageEntity.GetValueOrDefault();
				if (localEntity.HasValue)
				{
					if (_menuDragHelper.IsDragging)
					{
						ItemGridPiece draggingGhost = DraggingGhost;
						if (draggingGhost != null)
						{
							EntityUid entity = draggingGhost.Entity;
							ItemStorageLocation location = draggingGhost.Location;
							if (storageWindow == window)
							{
								Vector2i mouseGridPieceLocation = storageWindow.GetMouseGridPieceLocation(Entity<ItemComponent>.op_Implicit(entity), location);
								ItemStorageLocation location2 = new ItemStorageLocation(DraggingRotation, mouseGridPieceLocation);
								if (!_storage.ItemFitsInGridLocation(Entity<ItemComponent>.op_Implicit(entity), Entity<StorageComponent>.op_Implicit(valueOrDefault), location2))
								{
									window.Reclaim(control.Location, control);
								}
								else
								{
									base.EntityManager.RaisePredictiveEvent<StorageSetItemLocationEvent>(new StorageSetItemLocationEvent(base.EntityManager.GetNetEntity(draggingGhost.Entity, (MetaDataComponent)null), base.EntityManager.GetNetEntity(valueOrDefault, (MetaDataComponent)null), location2));
									window.Reclaim(location2, control);
								}
							}
							else if (storageWindow != null && storageWindow.StorageEntity.HasValue && storageWindow != window)
							{
								Vector2i mouseGridPieceLocation2 = storageWindow.GetMouseGridPieceLocation(Entity<ItemComponent>.op_Implicit(entity), location);
								ItemStorageLocation location3 = new ItemStorageLocation(DraggingRotation, mouseGridPieceLocation2);
								if (_storage.ItemFitsInGridLocation(Entity<ItemComponent>.op_Implicit((ValueTuple<EntityUid, ItemComponent>)(entity, null)), Entity<StorageComponent>.op_Implicit((ValueTuple<EntityUid, StorageComponent>)(storageWindow.StorageEntity.Value, null)), location3))
								{
									base.EntityManager.RaisePredictiveEvent<StorageTransferItemEvent>(new StorageTransferItemEvent(base.EntityManager.GetNetEntity(entity, (MetaDataComponent)null), base.EntityManager.GetNetEntity(storageWindow.StorageEntity.Value, (MetaDataComponent)null), location3));
									storageWindow.Reclaim(location3, control);
									DraggingRotation = Angle.Zero;
								}
								else
								{
									window.Reclaim(location, control);
								}
							}
							storageWindow?.FlagDirty();
							goto IL_0267;
						}
					}
					window.Reclaim(control.Location, control);
					base.EntityManager.RaisePredictiveEvent<StorageInteractWithItemEvent>(new StorageInteractWithItemEvent(base.EntityManager.GetNetEntity(control.Entity, (MetaDataComponent)null), base.EntityManager.GetNetEntity(valueOrDefault, (MetaDataComponent)null)));
					goto IL_0267;
				}
			}
		}
		window.Reclaim(control.Location, control);
		((BoundKeyEventArgs)args).Handle();
		_menuDragHelper.EndDrag();
		return;
		IL_0267:
		_menuDragHelper.EndDrag();
		((BoundKeyEventArgs)args).Handle();
	}

	private bool OnMenuBeginDrag()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ItemGridPiece dragged = _menuDragHelper.Dragged;
		if (dragged == null)
		{
			return false;
		}
		((Control)DraggingGhost).Orphan();
		DraggingRotation = dragged.Location.Rotation;
		((Control)base.UIManager.PopupRoot).AddChild((Control)(object)DraggingGhost);
		SetDraggingRotation();
		return true;
	}

	private bool OnMenuContinueDrag(float frameTime)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (DraggingGhost == null)
		{
			return false;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue || !_storage.TryGetStorageLocation(Entity<ItemComponent>.op_Implicit(DraggingGhost.Entity), out BaseContainer container, out StorageComponent _, out ItemStorageLocation _) || !((SharedUserInterfaceSystem)_ui).IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(container.Owner), (Enum)StorageComponent.StorageUiKey.Key, localEntity.Value))
		{
			((Control)DraggingGhost).Orphan();
			return false;
		}
		SetDraggingRotation();
		return true;
	}

	private void SetDraggingRotation()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer val = default(BaseContainer);
		StorageComponent item = default(StorageComponent);
		if (DraggingGhost != null && ((SharedContainerSystem)base.EntityManager.System<ContainerSystem>()).TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(DraggingGhost.Entity, null)), ref val) && base.EntityManager.TryGetComponent<StorageComponent>(val.Owner, ref item))
		{
			Vector2 centerOffset = ItemGridPiece.GetCenterOffset(Entity<StorageComponent>.op_Implicit((val.Owner, item)), Entity<ItemComponent>.op_Implicit((ValueTuple<EntityUid, ItemComponent>)(DraggingGhost.Entity, null)), new ItemStorageLocation(DraggingRotation, Vector2i.Zero), base.EntityManager);
			LayoutContainer.SetPosition((Control)(object)DraggingGhost, base.UIManager.MousePositionScaled.Position / 2f - centerOffset);
		}
	}

	private void OnMenuEndDrag()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (DraggingGhost != null)
		{
			DraggingRotation = Angle.Zero;
		}
	}

	public override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((UIController)this).FrameUpdate(args);
		_menuDragHelper.Update(((FrameEventArgs)(ref args)).DeltaSeconds);
	}
}
