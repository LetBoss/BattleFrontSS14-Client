// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.ComponentRegistration
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Frozen;

#nullable enable
namespace Robust.Shared.GameObjects;

public sealed class ComponentRegistration
{
  public string[] NetworkedFields = Array.Empty<string>();
  public FrozenDictionary<string, int> NetworkedFieldLookup = FrozenDictionary<string, int>.Empty;

  public string Name { get; }

  public CompIdx Idx { get; }

  public bool Unsaved { get; }

  public ushort? NetID { get; internal set; }

  public Type Type { get; }

  internal ComponentRegistration(string name, Type type, CompIdx idx, bool unsaved = false)
  {
    this.Name = name;
    this.Type = type;
    this.Idx = idx;
    this.Unsaved = unsaved;
  }

  public override string ToString() => $"ComponentRegistration({this.Name}: {this.Type})";
}
