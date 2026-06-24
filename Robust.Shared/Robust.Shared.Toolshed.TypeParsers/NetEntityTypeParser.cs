using System;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class NetEntityTypeParser : TypeParser<NetEntity>
{
	[Dependency]
	private readonly IEntityManager _entMan;

	public override bool TryParse(ParserContext ctx, out NetEntity result)
	{
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		int index = ctx.Index;
		string word;
		if (ctx.EatMatch('e'))
		{
			word = ctx.GetWord(ParserContext.IsToken);
			if (EntityUid.TryParse(word.AsSpan(), out var entityUid))
			{
				result = _entMan.GetNetEntity(entityUid);
				return true;
			}
			result = default(NetEntity);
			IConError error;
			if (word == null)
			{
				IConError conError = new OutOfInputError();
				error = conError;
			}
			else
			{
				IConError conError = new InvalidEntity("e" + word);
				error = conError;
			}
			ctx.Error = error;
			ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
			return false;
		}
		ctx.EatMatch('n');
		word = ctx.GetWord(ParserContext.IsToken);
		if (NetEntity.TryParse(word.AsSpan(), out result))
		{
			return true;
		}
		result = default(NetEntity);
		IConError error2;
		if (word == null)
		{
			IConError conError = new OutOfInputError();
			error2 = conError;
		}
		else
		{
			IConError conError = new InvalidEntity(word);
			error2 = conError;
		}
		ctx.Error = error2;
		ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
		return false;
	}

	public override CompletionResult TryAutocomplete(ParserContext ctx, CommandArgument? arg)
	{
		return CompletionResult.FromHint(ToolshedCommand.GetArgHint(arg, typeof(NetEntity)));
	}
}
