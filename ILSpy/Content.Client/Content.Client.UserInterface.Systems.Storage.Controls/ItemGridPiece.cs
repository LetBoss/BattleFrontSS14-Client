using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client._RMC14.Storage;
using Content.Client.Items.Systems;
using Content.Shared.Item;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Systems.Storage.Controls;

public sealed class ItemGridPiece : Control, IEntityControl
{
	private readonly IEntityManager _entityManager;

	private readonly StorageUIController _storageController;

	private readonly List<(Texture, Vector2)> _texturesPositions = new List<(Texture, Vector2)>();

	public readonly EntityUid Entity;

	public ItemStorageLocation Location;

	public ItemGridPieceMarks? Marked;

	private readonly string _centerTexturePath = "Storage/piece_center";

	private Texture? _centerTexture;

	private readonly string _topTexturePath = "Storage/piece_top";

	private Texture? _topTexture;

	private readonly string _bottomTexturePath = "Storage/piece_bottom";

	private Texture? _bottomTexture;

	private readonly string _leftTexturePath = "Storage/piece_left";

	private Texture? _leftTexture;

	private readonly string _rightTexturePath = "Storage/piece_right";

	private Texture? _rightTexture;

	private readonly string _topLeftTexturePath = "Storage/piece_topLeft";

	private Texture? _topLeftTexture;

	private readonly string _topRightTexturePath = "Storage/piece_topRight";

	private Texture? _topRightTexture;

	private readonly string _bottomLeftTexturePath = "Storage/piece_bottomLeft";

	private Texture? _bottomLeftTexture;

	private readonly string _bottomRightTexturePath = "Storage/piece_bottomRight";

	private Texture? _bottomRightTexture;

	private readonly string _markedFirstTexturePath = "Storage/marked_first";

	private Texture? _markedFirstTexture;

	private readonly string _markedSecondTexturePath = "Storage/marked_second";

	private Texture? _markedSecondTexture;

	public EntityUid? UiEntity => Entity;

	public event Action<GUIBoundKeyEventArgs, ItemGridPiece>? OnPiecePressed;

	public event Action<GUIBoundKeyEventArgs, ItemGridPiece>? OnPieceUnpressed;

	public ItemGridPiece(Entity<ItemComponent> entity, ItemStorageLocation location, IEntityManager entityManager)
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		IoCManager.InjectDependencies<ItemGridPiece>(this);
		_entityManager = entityManager;
		_storageController = ((Control)this).UserInterfaceManager.GetUIController<StorageUIController>();
		Entity = entity.Owner;
		Location = location;
		((Control)this).Visible = true;
		((Control)this).MouseFilter = (MouseFilterMode)0;
		((Control)this).TooltipSupplier = new TooltipSupplier(SupplyTooltip);
		((Control)this).OnThemeUpdated();
	}

	private Control? SupplyTooltip(Control sender)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		if (_storageController.IsDragging)
		{
			return null;
		}
		return (Control?)new Tooltip
		{
			Text = _entityManager.GetComponent<MetaDataComponent>(Entity).EntityName
		};
	}

	protected override void OnThemeUpdated()
	{
		((Control)this).OnThemeUpdated();
		TextureResource obj = ((Control)this).Theme.ResolveTextureOrNull(_centerTexturePath);
		_centerTexture = ((obj != null) ? obj.Texture : null);
		TextureResource obj2 = ((Control)this).Theme.ResolveTextureOrNull(_topTexturePath);
		_topTexture = ((obj2 != null) ? obj2.Texture : null);
		TextureResource obj3 = ((Control)this).Theme.ResolveTextureOrNull(_bottomTexturePath);
		_bottomTexture = ((obj3 != null) ? obj3.Texture : null);
		TextureResource obj4 = ((Control)this).Theme.ResolveTextureOrNull(_leftTexturePath);
		_leftTexture = ((obj4 != null) ? obj4.Texture : null);
		TextureResource obj5 = ((Control)this).Theme.ResolveTextureOrNull(_rightTexturePath);
		_rightTexture = ((obj5 != null) ? obj5.Texture : null);
		TextureResource obj6 = ((Control)this).Theme.ResolveTextureOrNull(_topLeftTexturePath);
		_topLeftTexture = ((obj6 != null) ? obj6.Texture : null);
		TextureResource obj7 = ((Control)this).Theme.ResolveTextureOrNull(_topRightTexturePath);
		_topRightTexture = ((obj7 != null) ? obj7.Texture : null);
		TextureResource obj8 = ((Control)this).Theme.ResolveTextureOrNull(_bottomLeftTexturePath);
		_bottomLeftTexture = ((obj8 != null) ? obj8.Texture : null);
		TextureResource obj9 = ((Control)this).Theme.ResolveTextureOrNull(_bottomRightTexturePath);
		_bottomRightTexture = ((obj9 != null) ? obj9.Texture : null);
		TextureResource obj10 = ((Control)this).Theme.ResolveTextureOrNull(_markedFirstTexturePath);
		_markedFirstTexture = ((obj10 != null) ? obj10.Texture : null);
		TextureResource obj11 = ((Control)this).Theme.ResolveTextureOrNull(_markedSecondTexturePath);
		_markedSecondTexture = ((obj11 != null) ? obj11.Texture : null);
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).Draw(handle);
		ItemComponent itemComponent = default(ItemComponent);
		if (!_entityManager.EntityExists(Entity) || !_entityManager.TryGetComponent<ItemComponent>(Entity, ref itemComponent))
		{
			((Control)this).Orphan();
			return;
		}
		if (_storageController.IsDragging)
		{
			EntityUid? val = _storageController.DraggingGhost?.Entity;
			EntityUid entity = Entity;
			if (val.HasValue && val.GetValueOrDefault() == entity && _storageController.DraggingGhost != this)
			{
				return;
			}
		}
		BaseContainer val2 = default(BaseContainer);
		StorageComponent item = default(StorageComponent);
		if (!((SharedContainerSystem)_entityManager.System<ContainerSystem>()).TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(Entity, null)), ref val2) || !_entityManager.TryGetComponent<StorageComponent>(val2.Owner, ref item))
		{
			return;
		}
		IReadOnlyList<Box2i> adjustedItemShape = _entityManager.System<ItemSystem>().GetAdjustedItemShape(Entity<StorageComponent>.op_Implicit((val2.Owner, item)), Entity<ItemComponent>.op_Implicit((Entity, itemComponent)), Location.Rotation, Vector2i.Zero);
		Box2i boundingBox = adjustedItemShape.GetBoundingBox();
		Vector2 vector = _centerTexture.Size * 2 * ((Control)this).UIScale;
		Color? val3 = ((!_storageController.IsDragging && (object)((Control)this).UserInterfaceManager.CurrentlyHovered == this) ? ((Color?)null) : new Color?(Color.FromHex((ReadOnlySpan<char>)"#a8a8a8", (Color?)null)));
		bool flag = Marked.HasValue;
		Vector2i? val4 = null;
		_texturesPositions.Clear();
		for (int i = boundingBox.Bottom; i <= boundingBox.Top; i++)
		{
			for (int j = boundingBox.Left; j <= boundingBox.Right; j++)
			{
				if (!adjustedItemShape.Contains(j, i))
				{
					continue;
				}
				Vector2 vector2 = vector * 2f * new Vector2(j - boundingBox.Left, i - boundingBox.Bottom);
				Vector2i val5 = ((Control)this).PixelPosition + Vector2Helpers.Floored(vector2);
				Texture texture = GetTexture(adjustedItemShape, new Vector2i(j, i), (Direction)3);
				if (texture != null)
				{
					Vector2 vector3 = new Vector2(vector.X, 0f);
					handle.DrawTextureRect(texture, new UIBox2(Vector2i.op_Implicit(val5) + vector3, Vector2i.op_Implicit(val5) + vector3 + vector), val3);
				}
				Texture texture2 = GetTexture(adjustedItemShape, new Vector2i(j, i), (Direction)5);
				if (texture2 != null)
				{
					_texturesPositions.Add((texture2, ((Control)this).Position + vector2 / ((Control)this).UIScale));
					handle.DrawTextureRect(texture2, new UIBox2(Vector2i.op_Implicit(val5), Vector2i.op_Implicit(val5) + vector), val3);
					if (flag && texture2 == _topLeftTexture)
					{
						val4 = val5;
						flag = false;
					}
				}
				Texture texture3 = GetTexture(adjustedItemShape, new Vector2i(j, i), (Direction)1);
				if (texture3 != null)
				{
					Vector2 vector4 = vector;
					handle.DrawTextureRect(texture3, new UIBox2(Vector2i.op_Implicit(val5) + vector4, Vector2i.op_Implicit(val5) + vector4 + vector), val3);
				}
				Texture texture4 = GetTexture(adjustedItemShape, new Vector2i(j, i), (Direction)7);
				if (texture4 != null)
				{
					Vector2 vector5 = new Vector2(0f, vector.Y);
					handle.DrawTextureRect(texture4, new UIBox2(Vector2i.op_Implicit(val5) + vector5, Vector2i.op_Implicit(val5) + vector5 + vector), val3);
				}
			}
		}
		Angle val6 = Location.Rotation;
		Vector2 vector6 = Vector2i.op_Implicit(itemComponent.StoredOffset);
		Vector2 vector7 = ((Angle)(ref val6)).RotateVec(ref vector6) * 2f * ((Control)this).UIScale;
		Vector2 vector8 = new Vector2((float)(((Box2i)(ref boundingBox)).Width + 1) * vector.X + vector7.X, (float)(((Box2i)(ref boundingBox)).Height + 1) * vector.Y + vector7.Y);
		Angle val7 = Location.Rotation + Angle.FromDegrees((double)itemComponent.StoredRotation);
		SpriteSpecifier storedSprite = itemComponent.StoredSprite;
		if (storedSprite != null)
		{
			float num = 2f * ((Control)this).UIScale;
			Texture val8 = _entityManager.System<SpriteSystem>().Frame0(storedSprite);
			Vector2 vector9 = ((((Box2i)(ref boundingBox)).Size + Vector2i.One) * _centerTexture.Size * 2 - val8.Size) * ((Control)this).UIScale;
			Box2Rotated val9 = default(Box2Rotated);
			((Box2Rotated)(ref val9))._002Ector(new Box2(0f, (float)val8.Height * num, (float)val8.Width * num, 0f), -val7, Vector2.Zero);
			Vector2 bottomLeft = ((Box2Rotated)(ref val9)).CalcBoundingBox().BottomLeft;
			Vector2 vector10 = Vector2i.op_Implicit(((Control)this).PixelPosition * 2);
			Control parent = ((Control)this).Parent;
			Vector2i? val10 = ((parent != null) ? new Vector2i?(parent.GlobalPixelPosition) : ((Vector2i?)null));
			Vector2 vector11 = vector10 + (val10.HasValue ? Vector2i.op_Implicit(val10.GetValueOrDefault()) : Vector2.Zero) + vector9 + vector7;
			((DrawingHandleBase)handle).SetTransform(ref vector11, ref val7);
			UIBox2 val11 = default(UIBox2);
			((UIBox2)(ref val11))._002Ector(bottomLeft, bottomLeft + val8.Size * num);
			handle.DrawTextureRect(val8, val11, (Color?)null);
			vector6 = Vector2i.op_Implicit(((Control)this).GlobalPixelPosition);
			val6 = Angle.Zero;
			((DrawingHandleBase)handle).SetTransform(ref vector6, ref val6);
		}
		else
		{
			_entityManager.System<SpriteSystem>().ForceUpdate(Entity);
			handle.DrawEntity(Entity, Vector2i.op_Implicit(((Control)this).PixelPosition) + vector8, Vector2.One * 2f * ((Control)this).UIScale, (Angle?)Angle.Zero, val7, (Direction?)(Direction)0, (SpriteComponent)null, (TransformComponent)null, (SharedTransformSystem)null);
		}
		if (val4.HasValue)
		{
			Vector2i valueOrDefault = val4.GetValueOrDefault();
			Texture val12 = (Texture)(Marked switch
			{
				ItemGridPieceMarks.First => _markedFirstTexture, 
				ItemGridPieceMarks.Second => _markedSecondTexture, 
				_ => null, 
			});
			if (val12 != null)
			{
				handle.DrawTextureRect(val12, new UIBox2(Vector2i.op_Implicit(valueOrDefault), Vector2i.op_Implicit(valueOrDefault) + vector), (Color?)null);
			}
		}
		_entityManager.System<RMCIconLabelSystem>().DrawStorage(Entity, ((Control)this).UIScale, vector8, handle);
	}

	protected override bool HasPoint(Vector2 point)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		foreach (var texturesPosition in _texturesPositions)
		{
			Texture item = texturesPosition.Item1;
			Vector2 item2 = texturesPosition.Item2;
			Box2 val = new Box2(item2, item2 + Vector2i.op_Implicit(item.Size * 4));
			if (((Box2)(ref val)).Contains(point, true))
			{
				return true;
			}
		}
		return false;
	}

	public bool Has(Vector2 point)
	{
		return ((Control)this).HasPoint(point);
	}

	protected override void KeyBindDown(GUIBoundKeyEventArgs args)
	{
		((Control)this).KeyBindDown(args);
		this.OnPiecePressed?.Invoke(args, this);
	}

	protected override void KeyBindUp(GUIBoundKeyEventArgs args)
	{
		((Control)this).KeyBindUp(args);
		this.OnPieceUnpressed?.Invoke(args, this);
	}

	private Texture? GetTexture(IReadOnlyList<Box2i> boxes, Vector2i position, Direction corner)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected I4, but got Unknown
		bool flag = !boxes.Contains(position - Vector2i.Up);
		bool flag2 = !boxes.Contains(position - Vector2i.Down);
		bool flag3 = !boxes.Contains(position + Vector2i.Left);
		bool flag4 = !boxes.Contains(position + Vector2i.Right);
		switch (corner - 1)
		{
		case 2:
			if (flag && flag4)
			{
				return _topRightTexture;
			}
			if (flag)
			{
				return _topTexture;
			}
			if (flag4)
			{
				return _rightTexture;
			}
			return _centerTexture;
		case 4:
			if (flag && flag3)
			{
				return _topLeftTexture;
			}
			if (flag)
			{
				return _topTexture;
			}
			if (flag3)
			{
				return _leftTexture;
			}
			return _centerTexture;
		case 0:
			if (flag2 && flag4)
			{
				return _bottomRightTexture;
			}
			if (flag2)
			{
				return _bottomTexture;
			}
			if (flag4)
			{
				return _rightTexture;
			}
			return _centerTexture;
		case 6:
			if (flag2 && flag3)
			{
				return _bottomLeftTexture;
			}
			if (flag2)
			{
				return _bottomTexture;
			}
			if (flag3)
			{
				return _leftTexture;
			}
			return _centerTexture;
		default:
			return null;
		}
	}

	public static Vector2 GetCenterOffset(Entity<StorageComponent?> storage, Entity<ItemComponent?> entity, ItemStorageLocation location, IEntityManager entMan)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Box2i boundingBox = entMan.System<ItemSystem>().GetAdjustedItemShape(storage, entity, location).GetBoundingBox();
		Vector2i size = ((Box2i)(ref boundingBox)).Size;
		return new Vector2(size.X + 1, size.Y + 1) * Vector2i.op_Implicit(new Vector2i(8, 8));
	}
}
