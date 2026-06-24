// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.SerializationHookContext
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Threading.Channels;

#nullable enable
namespace Robust.Shared.Serialization;

public sealed class SerializationHookContext
{
  public static readonly SerializationHookContext DoSkipHooks = new SerializationHookContext((ChannelWriter<ISerializationHooks>) null, true);
  public static readonly SerializationHookContext DontSkipHooks = new SerializationHookContext((ChannelWriter<ISerializationHooks>) null, false);
  public readonly ChannelWriter<ISerializationHooks>? DeferQueue;
  public readonly bool SkipHooks;

  public SerializationHookContext(ChannelWriter<ISerializationHooks>? deferQueue, bool skipHooks)
  {
    this.DeferQueue = deferQueue;
    this.SkipHooks = skipHooks;
  }

  public static SerializationHookContext ForSkipHooks(bool skip)
  {
    return !skip ? SerializationHookContext.DontSkipHooks : SerializationHookContext.DoSkipHooks;
  }
}
