// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Manager.Exceptions.RequiredFieldNotMappedException
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Serialization.Manager.Exceptions;

public sealed class RequiredFieldNotMappedException(Type type, string field, Type dataDef) : 
  Exception($"Required field {field} of type {type} in {dataDef} wasn't mapped.")
{
}
