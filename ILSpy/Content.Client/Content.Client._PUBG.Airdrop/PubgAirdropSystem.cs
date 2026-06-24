using System;
using System.Numerics;
using Content.Shared._PUBG.Airdrop;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Client._PUBG.Airdrop;

public sealed class PubgAirdropSystem : EntitySystem
{
	public bool Active { get; private set; }

	public Vector2 Position { get; private set; }

	public int RemainingSeconds { get; private set; }

	public MapId MapId { get; private set; }

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgAirdropStateEvent>((EntitySessionEventHandler<PubgAirdropStateEvent>)OnAirdropState, (Type[])null, (Type[])null);
	}

	private void OnAirdropState(PubgAirdropStateEvent ev, EntitySessionEventArgs args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Active = ev.Active;
		Position = ev.Position;
		RemainingSeconds = ev.RemainingSeconds;
		MapId = ev.MapId;
	}
}
