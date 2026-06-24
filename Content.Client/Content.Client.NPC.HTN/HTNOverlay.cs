using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.NPC.HTN;

public sealed class HTNOverlay : Overlay
{
	private readonly IEntityManager _entManager;

	private readonly Font _font;

	private readonly SharedTransformSystem _transformSystem;

	public override OverlaySpace Space => (OverlaySpace)2;

	public HTNOverlay(IEntityManager entManager, IResourceCache resourceCache)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		_entManager = entManager;
		_font = (Font)new VectorFont(resourceCache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 10);
		_transformSystem = _entManager.System<SharedTransformSystem>();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if (args.ViewportControl == null)
		{
			return;
		}
		DrawingHandleScreen screenHandle = ((OverlayDrawArgs)(ref args)).ScreenHandle;
		foreach (var (hTNComponent, val) in _entManager.EntityQuery<HTNComponent, TransformComponent>(true))
		{
			if (!string.IsNullOrEmpty(hTNComponent.DebugText) && !(val.MapID != args.MapId))
			{
				Vector2 worldPosition = _transformSystem.GetWorldPosition(val);
				if (((Box2)(ref args.WorldAABB)).Contains(worldPosition, true))
				{
					Vector2 vector = args.ViewportControl.WorldToScreen(worldPosition);
					screenHandle.DrawString(_font, vector + new Vector2(0f, 10f), hTNComponent.DebugText, Color.White);
				}
			}
		}
	}
}
