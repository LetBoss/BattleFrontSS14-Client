// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.BaseParser`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public abstract class BaseParser<T> : ITypeParser, IPostInjectInit where T : notnull
{
  [Dependency]
  protected readonly ILocalizationManager Loc;
  [Dependency]
  private readonly ILogManager _log;
  [Dependency]
  protected readonly ToolshedManager Toolshed;
  protected ISawmill Log;

  public virtual bool EnableValueRef => true;

  public virtual bool ShowTypeArgSignature => true;

  public virtual void PostInject() => this.Log = this._log.GetSawmill(this.GetType().PrettyName());

  public abstract bool TryParse(ParserContext ctx, [NotNullWhen(true)] out T? result);

  public abstract CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg);

  protected string GetArgHint(CommandArgument? arg) => ToolshedCommand.GetArgHint(arg, typeof (T));

  public Type Parses => typeof (T);

  bool ITypeParser.TryParse(ParserContext ctx, [NotNullWhen(true)] out object? result)
  {
    T result1;
    if (!this.TryParse(ctx, out result1))
    {
      result = (object) null;
      return false;
    }
    result = (object) result1;
    return true;
  }
}
