using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG;

[Serializable]
[NetSerializable]
public sealed class PubgZoneStateEvent : EntityEventArgs
{
	public Vector2 CurrentCenter;

	public float CurrentRadius;

	public Vector2 NextCenter;

	public float NextRadius;

	public int CurrentPhase;

	public ZoneState State;

	public float TimeRemaining;

	public bool Active;

	public bool Visible;

	public NetEntity ZoneMapEntity;

	public PubgZoneStateEvent(Vector2 currentCenter, float currentRadius, Vector2 nextCenter, float nextRadius, int currentPhase, ZoneState state, float timeRemaining, bool active, bool visible, NetEntity zoneMapEntity)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		CurrentCenter = currentCenter;
		CurrentRadius = currentRadius;
		NextCenter = nextCenter;
		NextRadius = nextRadius;
		CurrentPhase = currentPhase;
		State = state;
		TimeRemaining = timeRemaining;
		Active = active;
		Visible = visible;
		ZoneMapEntity = zoneMapEntity;
	}
}
