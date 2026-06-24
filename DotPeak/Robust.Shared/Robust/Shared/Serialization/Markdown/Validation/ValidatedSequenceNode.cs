// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Markdown.Validation.ValidatedSequenceNode
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Serialization.Markdown.Validation;

public sealed class ValidatedSequenceNode : ValidationNode
{
  public ValidatedSequenceNode(List<ValidationNode> sequence) => this.Sequence = sequence;

  public List<ValidationNode> Sequence { get; }

  public override bool Valid
  {
    get => this.Sequence.All<ValidationNode>((Func<ValidationNode, bool>) (p => p.Valid));
  }

  public override IEnumerable<ErrorNode> GetErrors()
  {
    foreach (ValidationNode validationNode in this.Sequence)
    {
      IEnumerator<ErrorNode> enumerator = validationNode.GetErrors().GetEnumerator();
      while (enumerator.MoveNext())
        yield return enumerator.Current;
      enumerator = (IEnumerator<ErrorNode>) null;
    }
  }
}
