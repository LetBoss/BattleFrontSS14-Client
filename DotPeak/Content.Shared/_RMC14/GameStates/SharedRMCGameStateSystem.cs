// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.GameStates.SharedRMCGameStateSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

#nullable enable
namespace Content.Shared._RMC14.GameStates;

public sealed class SharedRMCGameStateSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private INetManager _net;

  public override void Initialize()
  {
    this.SubscribeNetworkEvent<RMCSetPredictionEvent>(new EntityEventHandler<RMCSetPredictionEvent>(this.OnDisablePrediction));
  }

  private void OnDisablePrediction(RMCSetPredictionEvent ev)
  {
    if (this._net.IsServer)
      return;
    this._config.SetCVar<bool>(CVars.NetPredict, ev.Enable);
  }
}
