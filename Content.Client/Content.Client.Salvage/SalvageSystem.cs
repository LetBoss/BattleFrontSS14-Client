using System;
using Content.Client.Audio;
using Content.Shared.Salvage;
using Content.Shared.Salvage.Expeditions;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.Salvage;

public sealed class SalvageSystem : SharedSalvageSystem
{
	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private ContentAudioSystem _audio;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PlayAmbientMusicEvent>((EntityEventRefHandler<PlayAmbientMusicEvent>)OnPlayAmbientMusic, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SalvageExpeditionComponent, ComponentHandleState>((ComponentEventRefHandler<SalvageExpeditionComponent, ComponentHandleState>)OnExpeditionHandleState, (Type[])null, (Type[])null);
	}

	private void OnExpeditionHandleState(EntityUid uid, SalvageExpeditionComponent component, ref ComponentHandleState args)
	{
		if (((ComponentHandleState)(ref args)).Current is SalvageExpeditionComponentState salvageExpeditionComponentState)
		{
			component.Stage = salvageExpeditionComponentState.Stage;
			if ((int)component.Stage >= 3)
			{
				_audio.DisableAmbientMusic();
			}
		}
	}

	private void OnPlayAmbientMusic(ref PlayAmbientMusicEvent ev)
	{
		if (!ev.Cancelled)
		{
			EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
			TransformComponent val = default(TransformComponent);
			SalvageExpeditionComponent salvageExpeditionComponent = default(SalvageExpeditionComponent);
			if (((EntitySystem)this).TryComp(localEntity, ref val) && ((EntitySystem)this).TryComp<SalvageExpeditionComponent>(val.MapUid, ref salvageExpeditionComponent) && (int)salvageExpeditionComponent.Stage >= 3)
			{
				ev.Cancelled = true;
			}
		}
	}
}
