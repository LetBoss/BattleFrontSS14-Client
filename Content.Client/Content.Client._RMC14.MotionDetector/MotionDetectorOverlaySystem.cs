using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client.Hands.Systems;
using Content.Shared._RMC14.MotionDetector;
using Content.Shared.CCVar;
using Content.Shared.Hands.Components;
using Content.Shared.Inventory;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.MotionDetector;

public sealed class MotionDetectorOverlaySystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private IEntityManager _entity;

	[Dependency]
	private IEyeManager _eye;

	[Dependency]
	private IClientNetManager _net;

	[Dependency]
	private IOverlayManager _overlay;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		if (!_overlay.HasOverlay<MotionDetectorOverlay>())
		{
			_overlay.AddOverlay((Overlay)(object)new MotionDetectorOverlay());
		}
	}

	public override void Shutdown()
	{
		_overlay.RemoveOverlay<MotionDetectorOverlay>();
	}

	public void DrawBlips<T>(DrawingHandleWorld handle, ref TimeSpan last, List<(Vector2 Pos, bool QueenEye)> blips, Texture texture, Texture queenEyeTexture) where T : IComponent, IDetectorComponent
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Invalid comparison between Unknown and I4
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Invalid comparison between Unknown and I4
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		MapCoordinates mapCoordinates = ((SharedTransformSystem)_entity.System<TransformSystem>()).GetMapCoordinates(valueOrDefault, (TransformComponent)null);
		float num = 15f;
		float num2 = _config.GetCVar<int>(CCVars.ViewportWidth);
		IEye currentEye = _eye.CurrentEye;
		Vector2 zoom = currentEye.Zoom;
		Angle rotation = currentEye.Rotation;
		Direction cardinalDir = ((Angle)(ref rotation)).GetCardinalDir();
		if (((int)cardinalDir == 2 || (int)cardinalDir == 6) ? true : false)
		{
			float num3 = num;
			num = num2;
			num2 = num3;
		}
		HandsSystem handsSystem = _entity.System<HandsSystem>();
		InventorySystem inventorySystem = _entity.System<InventorySystem>();
		TimeSpan curTime = _timing.CurTime;
		List<EntityUid> list = handsSystem.EnumerateHeld(Entity<HandsComponent>.op_Implicit(valueOrDefault)).ToList();
		if (inventorySystem.TryGetContainerSlotEnumerator(Entity<InventoryComponent>.op_Implicit(valueOrDefault), out var containerSlotEnumerator))
		{
			EntityUid item;
			while (containerSlotEnumerator.NextItem(out item))
			{
				list.Add(item);
			}
		}
		T val = default(T);
		foreach (EntityUid item3 in list)
		{
			if (!_entity.TryGetComponent<T>(item3, ref val))
			{
				continue;
			}
			TimeSpan scanDuration = val.ScanDuration;
			INetChannel serverChannel = _net.ServerChannel;
			if (serverChannel != null)
			{
				scanDuration += TimeSpan.FromMilliseconds((float)serverChannel.Ping / 2f);
			}
			if (curTime > val.LastScan + scanDuration)
			{
				continue;
			}
			if (last != val.LastScan)
			{
				last = val.LastScan;
				blips.Clear();
				foreach (Blip blip in val.Blips)
				{
					if (!(mapCoordinates.MapId != blip.Coordinates.MapId))
					{
						num2 *= zoom.X;
						num *= zoom.Y;
						Vector2 item2 = blip.Coordinates.Position - new Vector2(0.5f, 0.5f) - mapCoordinates.Position;
						Cap(ref item2.X, num2);
						Cap(ref item2.Y, num);
						blips.Add((item2, blip.QueenEye));
					}
				}
			}
			foreach (var blip2 in blips)
			{
				((DrawingHandleBase)handle).DrawTexture(blip2.QueenEye ? queenEyeTexture : texture, mapCoordinates.Position + blip2.Pos, (Color?)null);
			}
		}
	}

	private void Cap(ref float i, float size)
	{
		float num = size / 2f - 0.5f;
		if (i > num)
		{
			i = num;
		}
		else if (i < 0f - num)
		{
			i = 0f - num;
		}
	}
}
