// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.ReflectionTypeParser`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Reflection;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class ReflectionTypeParser<TBase> : CustomTypeParser<Type> where TBase : class
{
  [Dependency]
  private readonly IReflectionManager _reflection;
  private Dictionary<string, Type>? _cache;
  private CompletionOption[]? _options;

  [MemberNotNull("_cache")]
  [MemberNotNull("_options")]
  private void InitCache()
  {
    if (this._cache != null && this._options != null)
      return;
    this._cache = this._reflection.GetAllChildren(typeof (TBase)).Where<Type>((Func<Type, bool>) (x => x.HasParameterlessConstructor())).ToDictionary<Type, string, Type>((Func<Type, string>) (x => x.Name), (Func<Type, Type>) (x => x));
    this._options = this._cache.Keys.Select<string, CompletionOption>((Func<string, CompletionOption>) (x => new CompletionOption(x))).ToArray<CompletionOption>();
  }

  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Type? result)
  {
    this.InitCache();
    string word = ctx.GetWord();
    if (word == null)
    {
      ctx.Error = (IConError) new OutOfInputError();
      result = (Type) null;
      return false;
    }
    if (this._cache.TryGetValue(word, out result))
      return true;
    ctx.Error = (IConError) new UnknownType(word);
    result = (Type) null;
    return false;
  }

  public override CompletionResult? TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    this.InitCache();
    return CompletionResult.FromHintOptions((IEnumerable<CompletionOption>) this._options, this.GetArgHint(arg));
  }
}
