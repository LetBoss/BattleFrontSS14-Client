// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Manager.Attributes.ValidatePrototypeIdAttribute`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Robust.Shared.Serialization.Manager.Attributes;

[Obsolete("Use a static readonly ProtoId<T> instead")]
[AttributeUsage(AttributeTargets.Field)]
public sealed class ValidatePrototypeIdAttribute<T> : Attribute where T : IPrototype
{
}
