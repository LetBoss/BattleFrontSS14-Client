// Decompiled with JetBrains decompiler
// Type: Content.Shared.Speech.SpeechSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Speech;

public sealed class SpeechSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SpeakAttemptEvent>(new EntityEventHandler<SpeakAttemptEvent>(this.OnSpeakAttempt));
  }

  public void SetSpeech(EntityUid uid, bool value, SpeechComponent? component = null)
  {
    if (value && !this.Resolve<SpeechComponent>(uid, ref component))
      return;
    component = this.EnsureComp<SpeechComponent>(uid);
    if (component.Enabled == value)
      return;
    component.Enabled = value;
    this.Dirty(uid, (IComponent) component);
  }

  private void OnSpeakAttempt(SpeakAttemptEvent args)
  {
    SpeechComponent comp;
    if (this.TryComp<SpeechComponent>(args.Uid, out comp) && comp.Enabled)
      return;
    args.Cancel();
  }
}
