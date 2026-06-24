// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.DoAfterMobCollisionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Movement.Systems;

public sealed class DoAfterMobCollisionSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ActiveDoAfterComponent, AttemptMobCollideEvent>(new ComponentEventRefHandler<ActiveDoAfterComponent, AttemptMobCollideEvent>(this.OnAttemptMobCollide));
  }

  private void OnAttemptMobCollide(
    EntityUid uid,
    ActiveDoAfterComponent component,
    ref AttemptMobCollideEvent args)
  {
    DoAfterComponent comp;
    if (!this.TryComp<DoAfterComponent>(uid, out comp))
      return;
    foreach (Content.Shared.DoAfter.DoAfter doAfter in comp.DoAfters.Values)
    {
      if (!doAfter.Cancelled && !doAfter.Completed && doAfter.Args.RootEntity)
      {
        args.Cancelled = true;
        break;
      }
    }
  }
}
