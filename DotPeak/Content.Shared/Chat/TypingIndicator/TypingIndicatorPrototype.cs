// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.TypingIndicator.TypingIndicatorPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System.Numerics;

#nullable enable
namespace Content.Shared.Chat.TypingIndicator;

[Prototype(null, 1)]
public sealed class TypingIndicatorPrototype : IPrototype
{
  [DataField("spritePath", false, 1, false, false, null)]
  public ResPath SpritePath = new ResPath("/Textures/Effects/speech.rsi");
  [DataField("typingState", false, 1, true, false, null)]
  public string TypingState;
  [DataField("idleState", false, 1, true, false, null)]
  public string IdleState;
  [DataField("offset", false, 1, false, false, null)]
  public Vector2 Offset = new Vector2(0.0f, 0.0f);
  [DataField("shader", false, 1, false, false, null)]
  public string Shader = "shaded";

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
