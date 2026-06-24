using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class ComponentTypeParser : CustomTypeParser<Type>
{
	[Dependency]
	private readonly IComponentFactory _factory;

	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Type? result)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		result = null;
		int index = ctx.Index;
		string word = ctx.GetWord(ParserContext.IsToken);
		if (word == null)
		{
			ctx.Error = new OutOfInputError();
			return false;
		}
		if (!_factory.TryGetRegistration(word.ToLower(), out ComponentRegistration registration, ignoreCase: true))
		{
			ctx.Error = new UnknownComponentError(word);
			ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
			return false;
		}
		result = registration.Type;
		return true;
	}

	public override CompletionResult TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		return CompletionResult.FromHintOptions(_factory.AllRegisteredTypes.Select(_factory.GetComponentName), GetArgHint(arg));
	}
}
