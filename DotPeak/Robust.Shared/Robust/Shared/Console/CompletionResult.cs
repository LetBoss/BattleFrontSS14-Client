// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.CompletionResult
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Console;

public sealed record CompletionResult(CompletionOption[] Options, string? Hint)
{
  public static readonly CompletionResult Empty = new CompletionResult(Array.Empty<CompletionOption>(), (string) null);

  public CompletionOption[] Options { get; init; } = Options;

  public string? Hint { get; set; } = Hint;

  public static CompletionResult FromHintOptions(IEnumerable<string> options, string? hint)
  {
    return new CompletionResult(CompletionResult.ConvertOptions(options), hint);
  }

  public static CompletionResult FromHintOptions(IEnumerable<CompletionOption> options, string? hint)
  {
    return new CompletionResult(options.ToArray<CompletionOption>(), hint);
  }

  public static CompletionResult FromOptions(IEnumerable<string> options)
  {
    return new CompletionResult(CompletionResult.ConvertOptions(options), (string) null);
  }

  public static CompletionResult FromOptions(IEnumerable<CompletionOption> options)
  {
    return new CompletionResult(options.ToArray<CompletionOption>(), (string) null);
  }

  public static CompletionResult FromHint(string hint)
  {
    return new CompletionResult(Array.Empty<CompletionOption>(), hint);
  }

  private static CompletionOption[] ConvertOptions(IEnumerable<string> stringOpts)
  {
    return stringOpts.Select<string, CompletionOption>((Func<string, CompletionOption>) (c => new CompletionOption(c))).ToArray<CompletionOption>();
  }

  [CompilerGenerated]
  public void Deconstruct(out CompletionOption[] Options, out string? Hint)
  {
    Options = this.Options;
    Hint = this.Hint;
  }
}
