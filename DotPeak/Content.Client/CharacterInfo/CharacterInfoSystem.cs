// Decompiled with JetBrains decompiler
// Type: Content.Client.CharacterInfo.CharacterInfoSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CharacterInfo;
using Content.Shared.Objectives;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.CharacterInfo;

public sealed class CharacterInfoSystem : EntitySystem
{
  [Dependency]
  private IPlayerManager _players;

  public event Action<CharacterInfoSystem.CharacterData>? OnCharacterUpdate;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<CharacterInfoEvent>(new EntitySessionEventHandler<CharacterInfoEvent>(this.OnCharacterInfoEvent), (Type[]) null, (Type[]) null);
  }

  public void RequestCharacterInfo()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._players).LocalEntity;
    if (!localEntity.HasValue)
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new RequestCharacterInfoEvent(this.GetNetEntity(localEntity.Value, (MetaDataComponent) null)));
  }

  private void OnCharacterInfoEvent(CharacterInfoEvent msg, EntitySessionEventArgs args)
  {
    EntityUid entity = this.GetEntity(msg.NetEntity);
    CharacterInfoSystem.CharacterData characterData = new CharacterInfoSystem.CharacterData(entity, msg.JobTitle, msg.Objectives, msg.Briefing, this.Name(entity, (MetaDataComponent) null));
    Action<CharacterInfoSystem.CharacterData> onCharacterUpdate = this.OnCharacterUpdate;
    if (onCharacterUpdate == null)
      return;
    onCharacterUpdate(characterData);
  }

  public List<Control> GetCharacterInfoControls(EntityUid uid)
  {
    CharacterInfoSystem.GetCharacterInfoControlsEvent infoControlsEvent = new CharacterInfoSystem.GetCharacterInfoControlsEvent(uid);
    this.RaiseLocalEvent<CharacterInfoSystem.GetCharacterInfoControlsEvent>(uid, ref infoControlsEvent, true);
    return infoControlsEvent.Controls;
  }

  public readonly record struct CharacterData(
    EntityUid Entity,
    string Job,
    Dictionary<string, List<ObjectiveInfo>> Objectives,
    string? Briefing,
    string EntityName)
  ;

  [ByRefEvent]
  public readonly record struct GetCharacterInfoControlsEvent(EntityUid Entity)
  {
    public readonly List<Control> Controls = new List<Control>();
    public readonly EntityUid Entity = Entity;

    [CompilerGenerated]
    public override int GetHashCode()
    {
      return EqualityComparer<List<Control>>.Default.GetHashCode(this.Controls) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.Entity);
    }

    [CompilerGenerated]
    public bool Equals(
      CharacterInfoSystem.GetCharacterInfoControlsEvent other)
    {
      return EqualityComparer<List<Control>>.Default.Equals(this.Controls, other.Controls) && EqualityComparer<EntityUid>.Default.Equals(this.Entity, other.Entity);
    }

    [CompilerGenerated]
    public void Deconstruct(out EntityUid Entity) => Entity = this.Entity;
  }
}
