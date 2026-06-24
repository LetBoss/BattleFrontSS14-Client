using System;
using System.Numerics;
using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Weapons.Ranged.ItemStatus;

public sealed class BulletRender : BaseBulletRenderer
{
	public enum BulletType
	{
		Normal,
		Tiny
	}

	public const int MinCountPerRow = 7;

	public const int BulletHeight = 12;

	public const int VerticalSeparation = 2;

	private static readonly LayoutParameters LayoutNormal = new LayoutParameters
	{
		ItemHeight = 12,
		ItemSeparation = 3,
		ItemWidth = 5,
		VerticalSeparation = 2,
		MinCountPerRow = 7
	};

	private static readonly LayoutParameters LayoutTiny = new LayoutParameters
	{
		ItemHeight = 12,
		ItemSeparation = 2,
		ItemWidth = 2,
		VerticalSeparation = 2,
		MinCountPerRow = 7
	};

	private static readonly Color ColorA = Color.FromHex((ReadOnlySpan<char>)"#b68f0e", (Color?)null);

	private static readonly Color ColorB = Color.FromHex((ReadOnlySpan<char>)"#d7df60", (Color?)null);

	private static readonly Color ColorGoneA = Color.FromHex((ReadOnlySpan<char>)"#000000", (Color?)null);

	private static readonly Color ColorGoneB = Color.FromHex((ReadOnlySpan<char>)"#222222", (Color?)null);

	private readonly Texture _bulletTiny;

	private readonly Texture _bulletNormal;

	private BulletType _type;

	public BulletType Type
	{
		get
		{
			return _type;
		}
		set
		{
			if (_type != value)
			{
				base.Parameters = _type switch
				{
					BulletType.Normal => LayoutNormal, 
					BulletType.Tiny => LayoutTiny, 
					_ => throw new ArgumentOutOfRangeException(), 
				};
				_type = value;
			}
		}
	}

	public BulletRender()
	{
		IResourceCache cache = IoCManager.Resolve<IResourceCache>();
		_bulletTiny = cache.GetTexture("/Textures/Interface/ItemStatus/Bullets/tiny.png");
		_bulletNormal = cache.GetTexture("/Textures/Interface/ItemStatus/Bullets/normal.png");
		base.Parameters = LayoutNormal;
	}

	protected override void DrawItem(DrawingHandleScreen handle, Vector2 renderPos, bool spent, bool altColor)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Color value = ((!spent) ? (altColor ? ColorA : ColorB) : (altColor ? ColorGoneA : ColorGoneB));
		Texture val = ((_type == BulletType.Tiny) ? _bulletTiny : _bulletNormal);
		((DrawingHandleBase)handle).DrawTexture(val, renderPos, (Color?)value);
	}
}
