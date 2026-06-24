// Decompiled with JetBrains decompiler
// Type: Content.Shared.IgnitionSource.EntitySystems.MatchstickSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.IgnitionSource.Components;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Smoking;
using Content.Shared.Temperature;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.IgnitionSource.EntitySystems;

public sealed class MatchstickSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedItemSystem _item;
  [Dependency]
  private SharedPointLightSystem _lights;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedIgnitionSourceSystem _ignition;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<MatchstickComponent, InteractUsingEvent>(new EntityEventRefHandler<MatchstickComponent, InteractUsingEvent>(this.OnInteractUsing));
  }

  private void OnInteractUsing(Entity<MatchstickComponent> ent, ref InteractUsingEvent args)
  {
    if (args.Handled)
      return;
    IsHotEvent args1 = new IsHotEvent();
    this.RaiseLocalEvent<IsHotEvent>(args.Used, args1);
    if (!args1.IsHot)
      return;
    args.Handled = this.TryIgnite(ent, new EntityUid?(args.User));
  }

  public bool TryIgnite(Entity<MatchstickComponent> matchstick, EntityUid? user)
  {
    if (matchstick.Comp.State != SmokableState.Unlit)
      return false;
    this._audio.PlayPredicted(matchstick.Comp.IgniteSound, (EntityUid) matchstick, user);
    this.SetState(matchstick, SmokableState.Lit);
    matchstick.Comp.TimeMatchWillBurnOut = new TimeSpan?(this._timing.CurTime + matchstick.Comp.Duration);
    this.Dirty<MatchstickComponent>(matchstick);
    return true;
  }

  private void SetState(Entity<MatchstickComponent> ent, SmokableState newState)
  {
    this._lights.SetEnabled((EntityUid) ent, newState == SmokableState.Lit);
    this._appearance.SetData((EntityUid) ent, (Enum) SmokingVisuals.Smoking, (object) newState);
    this._ignition.SetIgnited((Entity<IgnitionSourceComponent>) ent.Owner, newState == SmokableState.Lit);
    if (newState == SmokableState.Lit)
      this._item.SetHeldPrefix((EntityUid) ent, "lit");
    else
      this._item.SetHeldPrefix((EntityUid) ent, "unlit");
    ent.Comp.State = newState;
    this.Dirty<MatchstickComponent>(ent);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<MatchstickComponent> entityQueryEnumerator = this.EntityQueryEnumerator<MatchstickComponent>();
    EntityUid uid;
    MatchstickComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.State == SmokableState.Lit)
      {
        TimeSpan curTime = this._timing.CurTime;
        TimeSpan? matchWillBurnOut = comp1.TimeMatchWillBurnOut;
        if ((matchWillBurnOut.HasValue ? (curTime > matchWillBurnOut.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          this.SetState((Entity<MatchstickComponent>) (uid, comp1), SmokableState.Burnt);
      }
    }
  }
}
