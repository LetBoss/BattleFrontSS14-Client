// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.ComponentTypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class ComponentTypeParser : CustomTypeParser<Type>
{
  [Dependency]
  private readonly IComponentFactory _factory;

  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Type? result)
  {
    result = (Type) null;
    int index = ctx.Index;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    string word = ctx.GetWord(ComponentTypeParser.\u003C\u003EO.\u003C0\u003E__IsToken ?? (ComponentTypeParser.\u003C\u003EO.\u003C0\u003E__IsToken = new Func<Rune, bool>(ParserContext.IsToken)));
    if (word == null)
    {
      ctx.Error = (IConError) new OutOfInputError();
      return false;
    }
    ComponentRegistration registration;
    if (!this._factory.TryGetRegistration(word.ToLower(), out registration, true))
    {
      ctx.Error = (IConError) new UnknownComponentError(word);
      ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
      return false;
    }
    result = registration.Type;
    return true;
  }

  public override CompletionResult TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    return CompletionResult.FromHintOptions(this._factory.AllRegisteredTypes.Select<Type, string>(new Func<Type, string>(this._factory.GetComponentName)), this.GetArgHint(arg));
  }
}
