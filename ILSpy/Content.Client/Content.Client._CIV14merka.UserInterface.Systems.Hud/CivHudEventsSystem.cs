using System;
using Content.Shared._CIV14merka;
using Robust.Shared.GameObjects;

namespace Content.Client._CIV14merka.UserInterface.Systems.Hud;

public sealed class CivHudEventsSystem : EntitySystem
{
	public CivHudStatusEvent? LastStatus { get; private set; }

	public event Action<CivHudStatusEvent>? OnStatusReceived;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<CivHudStatusEvent>((EntitySessionEventHandler<CivHudStatusEvent>)OnStatus, (Type[])null, (Type[])null);
	}

	private void OnStatus(CivHudStatusEvent msg, EntitySessionEventArgs args)
	{
		LastStatus = msg;
		this.OnStatusReceived?.Invoke(msg);
	}
}
