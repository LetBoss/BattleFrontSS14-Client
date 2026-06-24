using System;
using Content.Shared._PUBG.Sponsor;
using Robust.Shared.GameObjects;

namespace Content.Client._PUBG.Sponsor;

public sealed class SponsorSandboxSystem : EntitySystem
{
	public SponsorSandboxState State { get; private set; } = SponsorSandboxState.Disabled();

	public event Action<SponsorSandboxState>? OnStateUpdated;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<SponsorSandboxStateMessage>((EntityEventHandler<SponsorSandboxStateMessage>)OnState, (Type[])null, (Type[])null);
	}

	private void OnState(SponsorSandboxStateMessage msg)
	{
		State = new SponsorSandboxState(msg.AllowSpawnEntities, msg.AllowSpawnTiles, msg.AllowSpawnDecals, msg.AllowEraseEntities, msg.AllowEraseTiles, msg.AllowSponsorArena, msg.AllowSponsorAghost, msg.BlockEraseMinds, msg.IsMiniGameSandbox, msg.DisallowedEntityIds);
		this.OnStateUpdated?.Invoke(State);
	}

	public void RequestSponsorArenaTeleport()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new SponsorArenaTeleportRequestMessage());
	}

	public void RequestSponsorAghost()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new SponsorAghostRequestMessage());
	}
}
