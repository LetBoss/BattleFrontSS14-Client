// Decompiled with JetBrains decompiler
// Type: Content.Client.Salvage.SalvageSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Audio;
using Content.Shared.Salvage;
using Content.Shared.Salvage.Expeditions;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client.Salvage;

public sealed class SalvageSystem : SharedSalvageSystem
{
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private ContentAudioSystem _audio;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PlayAmbientMusicEvent>(new EntityEventRefHandler<PlayAmbientMusicEvent>((object) this, __methodptr(OnPlayAmbientMusic)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SalvageExpeditionComponent, ComponentHandleState>(new ComponentEventRefHandler<SalvageExpeditionComponent, ComponentHandleState>((object) this, __methodptr(OnExpeditionHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnExpeditionHandleState(
    EntityUid uid,
    SalvageExpeditionComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is SalvageExpeditionComponentState current))
      return;
    component.Stage = current.Stage;
    if (component.Stage < ExpeditionStage.MusicCountdown)
      return;
    this._audio.DisableAmbientMusic();
  }

  private void OnPlayAmbientMusic(ref PlayAmbientMusicEvent ev)
  {
    TransformComponent transformComponent;
    SalvageExpeditionComponent expeditionComponent;
    if (ev.Cancelled || !this.TryComp(((ISharedPlayerManager) this._playerManager).LocalEntity, ref transformComponent) || !this.TryComp<SalvageExpeditionComponent>(transformComponent.MapUid, ref expeditionComponent) || expeditionComponent.Stage < ExpeditionStage.MusicCountdown)
      return;
    ev.Cancelled = true;
  }
}
