using System.Numerics;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.Decals.Overlays;

public sealed class DecalPlacementOverlay : Overlay
{
	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IMapManager _mapManager;

	private readonly DecalPlacementSystem _placement;

	private readonly SharedTransformSystem _transform;

	private readonly SpriteSystem _sprite;

	public override OverlaySpace Space => (OverlaySpace)16;

	public DecalPlacementOverlay(DecalPlacementSystem placement, SharedTransformSystem transform, SpriteSystem sprite)
	{
		IoCManager.InjectDependencies<DecalPlacementOverlay>(this);
		_placement = placement;
		_transform = transform;
		_sprite = sprite;
		((Overlay)this).ZIndex = 1000;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		var (decalPrototype, flag, val, value) = _placement.GetActiveDecal();
		if (decalPrototype == null)
		{
			return;
		}
		ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
		MapCoordinates val2 = _eyeManager.PixelToMap(mouseScreenPosition);
		EntityUid val3 = default(EntityUid);
		MapGridComponent val4 = default(MapGridComponent);
		if (!(val2.MapId != args.MapId) && _mapManager.TryFindGridAt(val2, ref val3, ref val4))
		{
			Matrix3x2 worldMatrix = _transform.GetWorldMatrix(val3);
			Matrix3x2 invWorldMatrix = _transform.GetInvWorldMatrix(val3);
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			((DrawingHandleBase)worldHandle).SetTransform(ref worldMatrix);
			Vector2 vector = Vector2.Transform(val2.Position, invWorldMatrix);
			if (flag)
			{
				vector = Vector2i.op_Implicit(Vector2Helpers.Floored(vector)) + val4.TileSizeHalfVector;
			}
			Box2 val5 = ((Box2)(ref Box2.UnitCentered)).Translated(vector);
			Box2Rotated val6 = default(Box2Rotated);
			((Box2Rotated)(ref val6))._002Ector(val5, val, vector);
			worldHandle.DrawTextureRect(_sprite.Frame0(decalPrototype.Sprite), ref val6, (Color?)value);
			Matrix3x2 identity = Matrix3x2.Identity;
			((DrawingHandleBase)worldHandle).SetTransform(ref identity);
		}
	}
}
