// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Manager.Exceptions.InvalidValidationNodeReturnedException`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Markdown.Validation;
using System;

#nullable enable
namespace Robust.Shared.Serialization.Manager.Exceptions;

public sealed class InvalidValidationNodeReturnedException<T> : Exception where T : ValidationNode
{
  public Type ActualType;

  public InvalidValidationNodeReturnedException(ValidationNode validationNode)
  {
    this.ActualType = validationNode.GetType();
  }

  public override string Message
  {
    get => $"{"ValidationNode"} of type {this.ActualType} provided, but {typeof (T)} expected.";
  }
}
