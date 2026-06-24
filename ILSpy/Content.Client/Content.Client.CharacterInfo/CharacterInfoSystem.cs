using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.CharacterInfo;
using Content.Shared.Objectives;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.CharacterInfo;

public sealed class CharacterInfoSystem : EntitySystem
{
	public readonly record struct CharacterData(EntityUid Entity, string Job, Dictionary<string, List<ObjectiveInfo>> Objectives, string? Briefing, string EntityName);

	[ByRefEvent]
	public readonly record struct GetCharacterInfoControlsEvent(EntityUid Entity)
	{
		public readonly List<Control> Controls = new List<Control>();

		public readonly EntityUid Entity = Entity;

		[CompilerGenerated]
		public void Deconstruct(out EntityUid Entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Entity = this.Entity;
		}
	}

	[Dependency]
	private IPlayerManager _players;

	public event Action<CharacterData>? OnCharacterUpdate;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<CharacterInfoEvent>((EntitySessionEventHandler<CharacterInfoEvent>)OnCharacterInfoEvent, (Type[])null, (Type[])null);
	}

	public void RequestCharacterInfo()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_players).LocalEntity;
		if (localEntity.HasValue)
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RequestCharacterInfoEvent(((EntitySystem)this).GetNetEntity(localEntity.Value, (MetaDataComponent)null)));
		}
	}

	private void OnCharacterInfoEvent(CharacterInfoEvent msg, EntitySessionEventArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity = ((EntitySystem)this).GetEntity(msg.NetEntity);
		CharacterData obj = new CharacterData(entity, msg.JobTitle, msg.Objectives, msg.Briefing, ((EntitySystem)this).Name(entity, (MetaDataComponent)null));
		this.OnCharacterUpdate?.Invoke(obj);
	}

	public List<Control> GetCharacterInfoControls(EntityUid uid)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		GetCharacterInfoControlsEvent getCharacterInfoControlsEvent = new GetCharacterInfoControlsEvent(uid);
		((EntitySystem)this).RaiseLocalEvent<GetCharacterInfoControlsEvent>(uid, ref getCharacterInfoControlsEvent, true);
		return getCharacterInfoControlsEvent.Controls;
	}
}
