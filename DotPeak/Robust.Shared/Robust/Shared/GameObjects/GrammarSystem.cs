// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.GrammarSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Enums;
using Robust.Shared.GameObjects.Components.Localization;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.GameObjects;

public sealed class GrammarSystem : EntitySystem
{
  public void Clear(Entity<GrammarComponent> grammar)
  {
    grammar.Comp.Attributes.Clear();
    this.Dirty<GrammarComponent>(grammar);
  }

  public bool TryGet(Entity<GrammarComponent> grammar, string key, [NotNullWhen(true)] out string? value)
  {
    return grammar.Comp.Attributes.TryGetValue(key, out value);
  }

  public void Set(Entity<GrammarComponent> grammar, string key, string? value)
  {
    if (value == null)
      grammar.Comp.Attributes.Remove(key);
    else
      grammar.Comp.Attributes[key] = value;
    this.Dirty<GrammarComponent>(grammar);
  }

  public void SetGender(Entity<GrammarComponent> grammar, Gender? gender)
  {
    this.Set(grammar, nameof (gender), gender?.ToString());
  }

  public void SetProperNoun(Entity<GrammarComponent> grammar, bool? proper)
  {
    this.Set(grammar, nameof (proper), proper?.ToString());
  }
}
