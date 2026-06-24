// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.ViewVariablesBlobMembers
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Robust.Shared.ViewVariables;

[NetSerializable]
[Serializable]
public sealed class ViewVariablesBlobMembers : ViewVariablesBlob
{
  public List<(string groupName, List<ViewVariablesBlobMembers.MemberData> groupMembers)> MemberGroups { get; set; } = new List<(string, List<ViewVariablesBlobMembers.MemberData>)>();

  [NetSerializable]
  [Virtual]
  [Serializable]
  public class ReferenceToken
  {
    public string Stringified { get; set; }

    public override string ToString() => this.Stringified;
  }

  [NetSerializable]
  [Serializable]
  public sealed class PrototypeReferenceToken : ViewVariablesBlobMembers.ReferenceToken
  {
    public string ID { get; set; }

    public string Variant { get; set; }

    public override string ToString()
    {
      return $"{this.Stringified} Prototype: {this.Variant} ID: {this.ID}";
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ServerValueTypeToken
  {
    public string Stringified { get; set; }

    public override string ToString() => this.Stringified;
  }

  [NetSerializable]
  [Serializable]
  public sealed class ServerKeyValuePairToken
  {
    public object Key { get; set; }

    public object Value { get; set; }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ServerTupleToken : ITuple
  {
    public object[] Items { get; set; }

    public object this[int index] => this.Items[index];

    public int Length => this.Items.Length;
  }

  [NetSerializable]
  [Serializable]
  public sealed class MemberData
  {
    public bool Editable { get; set; }

    public string Type { get; set; }

    public string TypePretty { get; set; }

    public string Name { get; set; }

    public int PropertyIndex { get; set; }

    public object Value { get; set; }
  }
}
