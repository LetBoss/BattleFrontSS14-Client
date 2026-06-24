using System;
using System.Numerics;
using Content.Client.Parallax.Data;
using Content.Client.Parallax.Managers;
using Content.Shared.Parallax;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Parallax;

public sealed class ParallaxSystem : SharedParallaxSystem
{
	[Dependency]
	private IOverlayManager _overlay;

	[Dependency]
	private IParallaxManager _parallax;

	[Dependency]
	private SharedMapSystem _map;

	private static readonly ProtoId<ParallaxPrototype> Fallback = ProtoId<ParallaxPrototype>.op_Implicit("Default");

	public const int ParallaxZIndex = 0;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_overlay.AddOverlay((Overlay)(object)new ParallaxOverlay());
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnReload, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParallaxComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<ParallaxComponent, AfterAutoHandleStateEvent>)OnAfterAutoHandleState, (Type[])null, (Type[])null);
	}

	private void OnReload(PrototypesReloadedEventArgs obj)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!obj.WasModified<ParallaxPrototype>())
		{
			return;
		}
		_parallax.UnloadParallax(ProtoId<ParallaxPrototype>.op_Implicit(Fallback));
		_parallax.LoadDefaultParallax();
		foreach (ParallaxComponent item in ((EntitySystem)this).EntityQuery<ParallaxComponent>(true))
		{
			_parallax.UnloadParallax(item.Parallax);
			_parallax.LoadParallaxByName(item.Parallax);
		}
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlay.RemoveOverlay<ParallaxOverlay>();
	}

	private void OnAfterAutoHandleState(EntityUid uid, ParallaxComponent component, ref AfterAutoHandleStateEvent args)
	{
		if (!_parallax.IsLoaded(component.Parallax))
		{
			_parallax.LoadParallaxByName(component.Parallax);
		}
	}

	public ParallaxLayerPrepared[] GetParallaxLayers(MapId mapId)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return _parallax.GetParallaxLayers(GetParallax(_map.GetMapOrInvalid((MapId?)mapId)));
	}

	public string GetParallax(MapId mapId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return GetParallax(_map.GetMapOrInvalid((MapId?)mapId));
	}

	public string GetParallax(EntityUid mapUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		ParallaxComponent parallaxComponent = default(ParallaxComponent);
		if (!((EntitySystem)this).TryComp<ParallaxComponent>(mapUid, ref parallaxComponent))
		{
			return ProtoId<ParallaxPrototype>.op_Implicit(Fallback);
		}
		return parallaxComponent.Parallax;
	}

	public void DrawParallax(DrawingHandleWorld worldHandle, Box2 worldAABB, Texture sprite, TimeSpan curTime, Vector2 position, Vector2 scrolling, float scale = 1f, float slowness = 0f, Color? modulate = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = sprite.Size / 32f * scale;
		Vector2 vector2 = scrolling * (float)curTime.TotalSeconds;
		Vector2 vector3 = position * slowness + vector2;
		vector3 -= vector / 2f;
		Vector2 vector4 = worldAABB.BottomLeft - vector3;
		vector4 = Vector2i.op_Implicit(Vector2Helpers.Floored(vector4 / vector)) * vector;
		vector4 += vector3;
		for (float num = vector4.X; num < worldAABB.Right; num += vector.X)
		{
			for (float num2 = vector4.Y; num2 < worldAABB.Top; num2 += vector.Y)
			{
				Box2 val = Box2.FromDimensions(new Vector2(num, num2), vector);
				worldHandle.DrawTextureRect(sprite, val, modulate);
			}
		}
	}
}
