using System;
using System.Numerics;
using Content.Shared._RMC14.Vehicle;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._PUBG.UserInterface.Systems.Minimap;

public sealed class PubgPartyPingOverlay : Overlay
{
	private readonly IPlayerManager _player;

	private readonly SharedTransformSystem _transform;

	private readonly PubgPartyPingClientSystem _pingSystem;

	private readonly VehicleSystem _vehicles;

	public override OverlaySpace Space => (OverlaySpace)2;

	public PubgPartyPingOverlay(IPlayerManager player, SharedTransformSystem transform, PubgPartyPingClientSystem pingSystem, VehicleSystem vehicles)
	{
		_player = player;
		_transform = transform;
		_pingSystem = pingSystem;
		_vehicles = vehicles;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		if (args.ViewportControl == null)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		PubgActivePingState? latestPing = _pingSystem.LatestPing;
		if (!latestPing.HasValue)
		{
			return;
		}
		PubgActivePingState value = latestPing.Value;
		if (_vehicles.TryGetDisplayMapId(localEntity.Value, out var mapId) && !(mapId != value.MapId))
		{
			Vector2 vector = new Vector2((float)((UIBox2i)(ref args.ViewportBounds)).Width / 2f, (float)((UIBox2i)(ref args.ViewportBounds)).Height / 2f);
			Vector2 vector2 = args.ViewportControl.WorldToScreen(value.Position) - vector;
			float num = vector2.Length();
			if (!(num < 1f))
			{
				Vector2 vector3 = vector2 / num;
				float num2 = MathF.Min(MathF.Min(((UIBox2i)(ref args.ViewportBounds)).Width, ((UIBox2i)(ref args.ViewportBounds)).Height) * 0.42f, MathF.Max(34f, num - 20f));
				Vector2 center = vector + vector3 * num2;
				DrawArrow(((OverlayDrawArgs)(ref args)).ScreenHandle, center, vector3, PubgPartyPingColorResolver.GetColor(value.Source));
			}
		}
	}

	private static void DrawArrow(DrawingHandleScreen handle, Vector2 center, Vector2 direction, Color color)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = center + direction * 14f;
		Vector2 vector2 = new Vector2(0f - direction.Y, direction.X);
		Vector2 vector3 = center - direction * 7f + vector2 * 6f;
		Vector2 vector4 = center - direction * 7f - vector2 * 6f;
		((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)1, (ReadOnlySpan<Vector2>)new Vector2[3] { vector, vector3, vector4 }, color);
		((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)5, (ReadOnlySpan<Vector2>)new Vector2[4] { vector, vector3, vector4, vector }, Color.Black);
	}
}
