using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.MapText;

public sealed class MapTextOverlay : Overlay
{
	private readonly IConfigurationManager _configManager;

	private readonly IEntityManager _entManager;

	private readonly IUserInterfaceManager _uiManager;

	private readonly SharedTransformSystem _transform;

	public override OverlaySpace Space => (OverlaySpace)2;

	public MapTextOverlay(IConfigurationManager configManager, IEntityManager entManager, IUserInterfaceManager uiManager, SharedTransformSystem transform, IResourceCache resourceCache, IPrototypeManager prototypeManager)
	{
		_configManager = configManager;
		_entManager = entManager;
		_uiManager = uiManager;
		_transform = transform;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (args.ViewportControl != null)
		{
			DrawingHandleBase drawingHandle = args.DrawingHandle;
			Matrix3x2 identity = Matrix3x2.Identity;
			drawingHandle.SetTransform(ref identity);
			float num = _configManager.GetCVar<float>(CVars.DisplayUIScale);
			if (num == 0f)
			{
				num = _uiManager.DefaultUIScale;
			}
			DrawWorld(((OverlayDrawArgs)(ref args)).ScreenHandle, args, num);
			args.DrawingHandle.UseShader((ShaderInstance)null);
		}
	}

	private void DrawWorld(DrawingHandleScreen handle, OverlayDrawArgs args, float scale)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		if (args.ViewportControl == null)
		{
			return;
		}
		Matrix3x2 worldToScreenMatrix = args.ViewportControl.GetWorldToScreenMatrix();
		AllEntityQueryEnumerator<MapTextComponent> val = _entManager.AllEntityQueryEnumerator<MapTextComponent>();
		Box2Rotated val2 = ((Box2Rotated)(ref args.WorldBounds)).Enlarged(2f);
		EntityUid val3 = default(EntityUid);
		MapTextComponent mapTextComponent = default(MapTextComponent);
		while (val.MoveNext(ref val3, ref mapTextComponent))
		{
			MapCoordinates mapCoordinates = _transform.GetMapCoordinates(val3, (TransformComponent)null);
			if (!(mapCoordinates.MapId != args.MapId) && ((Box2Rotated)(ref val2)).Contains(mapCoordinates.Position) && mapTextComponent.CachedFont != null)
			{
				Vector2 vector = Vector2.Transform(mapCoordinates.Position, worldToScreenMatrix) + mapTextComponent.Offset;
				Vector2 dimensions = handle.GetDimensions((Font)(object)mapTextComponent.CachedFont, (ReadOnlySpan<char>)mapTextComponent.CachedText, scale);
				handle.DrawString((Font)(object)mapTextComponent.CachedFont, vector - dimensions / 2f, (ReadOnlySpan<char>)mapTextComponent.CachedText, scale, mapTextComponent.Color);
			}
		}
	}
}
