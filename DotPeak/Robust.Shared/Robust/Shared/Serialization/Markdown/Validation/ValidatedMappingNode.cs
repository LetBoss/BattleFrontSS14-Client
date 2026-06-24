// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Markdown.Validation.ValidatedMappingNode
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Serialization.Markdown.Validation;

public sealed class ValidatedMappingNode : ValidationNode
{
  public ValidatedMappingNode(Dictionary<ValidationNode, ValidationNode> mapping)
  {
    this.Mapping = mapping;
  }

  public Dictionary<ValidationNode, ValidationNode> Mapping { get; }

  public override bool Valid
  {
    get
    {
      return this.Mapping.All<KeyValuePair<ValidationNode, ValidationNode>>((Func<KeyValuePair<ValidationNode, ValidationNode>, bool>) (p => p.Key.Valid && p.Value.Valid));
    }
  }

  public override IEnumerable<ErrorNode> GetErrors()
  {
    foreach ((ValidationNode key, ValidationNode validationNode) in this.Mapping.Where<KeyValuePair<ValidationNode, ValidationNode>>((Func<KeyValuePair<ValidationNode, ValidationNode>, bool>) (p => !p.Key.Valid || !p.Value.Valid)))
    {
      foreach (ErrorNode error in key.GetErrors())
        yield return error;
      foreach (ErrorNode error in validationNode.GetErrors())
        yield return error;
      validationNode = (ValidationNode) null;
    }
  }
}
