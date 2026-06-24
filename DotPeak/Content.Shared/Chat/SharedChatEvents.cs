// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.TransformSpeakerNameEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Content.Shared.Speech;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Shared.Chat;

public sealed class TransformSpeakerNameEvent : EntityEventArgs, IInventoryRelayEvent
{
  public EntityUid Sender;
  public string VoiceName;
  public ProtoId<SpeechVerbPrototype>? SpeechVerb;

  public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

  public TransformSpeakerNameEvent(EntityUid sender, string name)
  {
    this.Sender = sender;
    this.VoiceName = name;
    this.SpeechVerb = new ProtoId<SpeechVerbPrototype>?();
  }
}
