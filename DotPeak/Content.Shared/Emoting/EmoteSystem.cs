// Decompiled with JetBrains decompiler
// Type: Content.Shared.Emoting.EmoteSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Emoting;

public sealed class EmoteSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EmoteAttemptEvent>(new EntityEventHandler<EmoteAttemptEvent>(this.OnEmoteAttempt));
  }

  public void SetEmoting(EntityUid uid, bool value, EmotingComponent? component = null)
  {
    if (value && !this.Resolve<EmotingComponent>(uid, ref component))
      return;
    component = this.EnsureComp<EmotingComponent>(uid);
    if (component.Enabled == value)
      return;
    this.Dirty(uid, (IComponent) component);
  }

  private void OnEmoteAttempt(EmoteAttemptEvent args)
  {
    EmotingComponent comp;
    if (this.TryComp<EmotingComponent>(args.Uid, out comp) && comp.Enabled)
      return;
    args.Cancel();
  }
}
