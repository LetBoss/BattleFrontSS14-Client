// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Markdown.DataNode
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.Serialization.Markdown;

public abstract class DataNode
{
  public string? Tag;
  public NodeMark Start;
  public NodeMark End;

  public DataNode(NodeMark start, NodeMark end)
  {
    this.Start = start;
    this.End = end;
  }

  public abstract bool IsEmpty { get; }

  public virtual bool IsNull { get; init; }

  public abstract DataNode Copy();

  public abstract DataNode? Except(DataNode node);

  [Obsolete("Use SerializationManager.PushComposition()")]
  public abstract DataNode PushInheritance(DataNode parent);

  public T CopyCast<T>() where T : DataNode => (T) this.Copy();

  public void Write(TextWriter writer)
  {
    YamlNode yamlNode = this.ToYamlNode();
    YamlStream yamlStream = new YamlStream();
    yamlStream.Add(new YamlDocument(yamlNode));
    yamlStream.Save((IEmitter) new YamlMappingFix((IEmitter) new Emitter(writer)), false);
  }

  public override string ToString()
  {
    StringWriter writer = new StringWriter();
    this.Write((TextWriter) writer);
    return writer.ToString();
  }
}
