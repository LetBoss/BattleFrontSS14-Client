// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.Prototypes.AutoEmotePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;

#nullable enable
namespace Content.Shared.Chat.Prototypes;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class AutoEmotePrototype : IPrototype
{
  [DataField("emote", false, 1, true, false, typeof (PrototypeIdSerializer<EmotePrototype>))]
  public string EmoteId = string.Empty;
  [DataField("interval", false, 1, true, false, null)]
  public TimeSpan Interval;
  [DataField("chance", false, 1, false, false, null)]
  public float Chance = 1f;
  [DataField("withChat", false, 1, false, false, null)]
  public bool WithChat = true;
  [DataField("hiddenFromChatWindow", false, 1, false, false, null)]
  public bool HiddenFromChatWindow;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
