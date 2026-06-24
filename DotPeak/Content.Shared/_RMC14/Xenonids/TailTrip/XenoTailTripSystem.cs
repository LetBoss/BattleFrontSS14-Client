// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.TailTrip.XenoTailTripSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Finesse;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared.Actions;
using Content.Shared.Coordinates;
using Content.Shared.Stunnable;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.TailTrip;

public sealed class XenoTailTripSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private RMCDazedSystem _daze;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private RMCSizeStunSystem _size;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoTailTripComponent, XenoTailTripActionEvent>(new EntityEventRefHandler<XenoTailTripComponent, XenoTailTripActionEvent>(this.OnXenoTailTripAction));
  }

  private void OnXenoTailTripAction(
    Entity<XenoTailTripComponent> xeno,
    ref XenoTailTripActionEvent args)
  {
    if (args.Handled || !this._rmcActions.TryUseAction((EntityTargetActionEvent) args))
      return;
    args.Handled = true;
    if (this._net.IsServer)
      this.SpawnAttachedTo((string) xeno.Comp.TailEffect, args.Target.ToCoordinates(), rotation: new Angle());
    this.EnsureComp<XenoSweepingComponent>((EntityUid) xeno);
    this._audio.PlayPredicted(xeno.Comp.Sound, (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    if (this.HasComp<XenoMarkedComponent>(args.Target))
    {
      RMCSizes size;
      if (!this._size.TryGetSize(args.Target, out size) || size < RMCSizes.Big)
        this._stun.TryParalyze(args.Target, xeno.Comp.MarkedStunTime, true);
      this._daze.TryDaze(args.Target, xeno.Comp.MarkedDazeTime, true, stutter: true);
      this.RemCompDeferred<XenoMarkedComponent>(args.Target);
    }
    else
    {
      RMCSizes size;
      if (!this._size.TryGetSize(args.Target, out size) || size < RMCSizes.Big)
        this._stun.TryParalyze(args.Target, xeno.Comp.StunTime, true);
      this._slow.TrySlowdown(args.Target, xeno.Comp.SlowTime, ignoreDurationModifier: true);
    }
  }
}
