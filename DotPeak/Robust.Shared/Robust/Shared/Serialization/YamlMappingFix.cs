// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.YamlMappingFix
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using YamlDotNet.Core;
using YamlDotNet.Core.Events;

#nullable enable
namespace Robust.Shared.Serialization;

public sealed class YamlMappingFix : IEmitter
{
  private readonly IEmitter _next;

  public YamlMappingFix(IEmitter next) => this._next = next;

  public void Emit(ParsingEvent @event)
  {
    if (@event is MappingStart mappingStart)
      @event = (ParsingEvent) new MappingStart(((NodeEvent) mappingStart).Anchor, ((NodeEvent) mappingStart).Tag, false, mappingStart.Style, ((ParsingEvent) mappingStart).Start, ((ParsingEvent) mappingStart).End);
    this._next.Emit(@event);
  }
}
