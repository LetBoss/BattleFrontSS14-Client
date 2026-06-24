// Decompiled with JetBrains decompiler
// Type: Content.Shared.Power.EntitySystems.SharedPowerReceiverSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Power.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Power.EntitySystems;

public abstract class SharedPowerReceiverSystem : EntitySystem
{
  [Dependency]
  private INetManager _netMan;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPowerNetSystem _net;

  public abstract bool ResolveApc(EntityUid entity, [NotNullWhen(true)] ref SharedApcPowerReceiverComponent? component);

  public void SetNeedsPower(EntityUid uid, bool value, SharedApcPowerReceiverComponent? receiver = null)
  {
    if (!this.ResolveApc(uid, ref receiver) || receiver.NeedsPower == value)
      return;
    receiver.NeedsPower = value;
    this.Dirty(uid, (IComponent) receiver);
  }

  public void SetPowerDisabled(EntityUid uid, bool value, SharedApcPowerReceiverComponent? receiver = null)
  {
    if (!this.ResolveApc(uid, ref receiver) || receiver.PowerDisabled == value)
      return;
    receiver.PowerDisabled = value;
    this.Dirty(uid, (IComponent) receiver);
  }

  public bool TogglePower(
    EntityUid uid,
    bool playSwitchSound = true,
    SharedApcPowerReceiverComponent? receiver = null,
    EntityUid? user = null)
  {
    if (!this.ResolveApc(uid, ref receiver))
      return true;
    if (!receiver.NeedsPower)
    {
      bool flag = this._net.IsPoweredCalculate(receiver);
      if (receiver.Powered != flag)
        this.RaisePower((Entity<SharedApcPowerReceiverComponent>) (uid, receiver));
      this.SetPowerDisabled(uid, false, receiver);
      return true;
    }
    this.SetPowerDisabled(uid, !receiver.PowerDisabled, receiver);
    if (user.HasValue)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(32 /*0x20*/, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user.Value), "player", "ToPrettyString(user.Value)");
      logStringHandler.AppendLiteral(" hit power button on ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "ToPrettyString(uid)");
      logStringHandler.AppendLiteral(", it's now ");
      logStringHandler.AppendFormatted(!receiver.PowerDisabled ? "on" : "off");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
    }
    if (playSwitchSound)
      this._audio.PlayPredicted((SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg"), uid, user, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
    if (this._netMan.IsClient && receiver.PowerDisabled)
    {
      bool flag = this._net.IsPoweredCalculate(receiver);
      if (receiver.Powered != flag)
      {
        receiver.Powered = flag;
        this.RaisePower((Entity<SharedApcPowerReceiverComponent>) (uid, receiver));
      }
    }
    return !receiver.PowerDisabled;
  }

  protected virtual void RaisePower(Entity<SharedApcPowerReceiverComponent> entity)
  {
  }

  public bool IsPowered(Entity<SharedApcPowerReceiverComponent?> entity)
  {
    return !this.ResolveApc(entity.Owner, ref entity.Comp) || entity.Comp.Powered;
  }

  protected string GetExamineText(bool powered)
  {
    return this.Loc.GetString("power-receiver-component-on-examine-main", ("stateText", (object) this.Loc.GetString(powered ? "power-receiver-component-on-examine-powered" : "power-receiver-component-on-examine-unpowered")));
  }
}
