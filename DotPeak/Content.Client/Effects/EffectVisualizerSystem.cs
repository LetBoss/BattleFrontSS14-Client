// Decompiled with JetBrains decompiler
// Type: Content.Client.Effects.EffectVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Effects;

public sealed class EffectVisualizerSystem : EntitySystem
{
  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EffectVisualsComponent, AnimationCompletedEvent>(new ComponentEventHandler<EffectVisualsComponent, AnimationCompletedEvent>((object) this, __methodptr(OnEffectAnimComplete)), (Type[]) null, (Type[]) null);
  }

  private void OnEffectAnimComplete(
    EntityUid uid,
    EffectVisualsComponent component,
    AnimationCompletedEvent args)
  {
    this.QueueDel(new EntityUid?(uid));
  }
}
