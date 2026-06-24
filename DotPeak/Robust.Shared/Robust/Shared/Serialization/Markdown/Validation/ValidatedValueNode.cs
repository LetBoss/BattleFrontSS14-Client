// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Markdown.Validation.ValidatedValueNode
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Serialization.Markdown.Validation;

public sealed class ValidatedValueNode : ValidationNode
{
  public ValidatedValueNode(DataNode dataNode) => this.DataNode = dataNode;

  public DataNode DataNode { get; }

  public override bool Valid => true;

  public override IEnumerable<ErrorNode> GetErrors() => Enumerable.Empty<ErrorNode>();

  public override string? ToString() => this.DataNode.ToString();
}
