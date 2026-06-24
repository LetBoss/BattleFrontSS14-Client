using System;
using System.Collections.Generic;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Commands.Entities;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class EntityTypeParser : TypeParser<EntityUid>
{
	[Dependency]
	private readonly IEntityManager _entMan;

	public static bool TryParseEntity(IEntityManager entMan, ParserContext ctx, out EntityUid result)
	{
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		int index = ctx.Index;
		string word;
		if (ctx.EatMatch('e'))
		{
			word = ctx.GetWord(ParserContext.IsToken);
			if (EntityUid.TryParse(word.AsSpan(), out result))
			{
				return true;
			}
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
		if (NetEntity.TryParse(word.AsSpan(), out var entity))
		{
			result = entMan.GetEntity(entity);
			return true;
		}
		result = default(EntityUid);
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

	public override bool TryParse(ParserContext parser, out EntityUid result)
	{
		return TryParseEntity(_entMan, parser, out result);
	}

	public override CompletionResult TryAutocomplete(ParserContext ctx, CommandArgument? arg)
	{
		return CompletionResult.FromHint(ToolshedCommand.GetArgHint(arg, typeof(NetEntity)));
	}
}
internal sealed class EntityTypeParser<T> : TypeParser<Entity<T>> where T : IComponent
{
	[Dependency]
	private readonly IEntityManager _entMan;

	public override bool TryParse(ParserContext parser, out Entity<T> result)
	{
		result = default(Entity<T>);
		if (!EntityTypeParser.TryParseEntity(_entMan, parser, out var result2))
		{
			return false;
		}
		if (!_entMan.TryGetComponent<T>(result2, out T component))
		{
			return false;
		}
		result = new Entity<T>(result2, component);
		return true;
	}

	public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
	{
		if (!ctx.CheckInvokable<EntitiesCommand>())
		{
			return null;
		}
		string argHint = ToolshedCommand.GetArgHint(arg, typeof(NetEntity));
		if (_entMan.Count<T>() > 128)
		{
			return CompletionResult.FromHint(argHint);
		}
		AllEntityQueryEnumerator<T, MetaDataComponent> allEntityQueryEnumerator = _entMan.AllEntityQueryEnumerator<T, MetaDataComponent>();
		List<CompletionOption> list = new List<CompletionOption>();
		T comp;
		MetaDataComponent comp2;
		while (allEntityQueryEnumerator.MoveNext(out comp, out comp2))
		{
			list.Add(new CompletionOption(comp2.NetEntity.ToString(), comp2.EntityName));
		}
		return CompletionResult.FromHintOptions(list, argHint);
	}
}
internal sealed class EntityTypeParser<T1, T2> : TypeParser<Entity<T1, T2>> where T1 : IComponent where T2 : IComponent
{
	[Dependency]
	private readonly IEntityManager _entMan;

	public override bool TryParse(ParserContext parser, out Entity<T1, T2> result)
	{
		result = default(Entity<T1, T2>);
		if (!EntityTypeParser.TryParseEntity(_entMan, parser, out var result2))
		{
			return false;
		}
		if (!_entMan.TryGetComponent<T1>(result2, out T1 component))
		{
			return false;
		}
		if (!_entMan.TryGetComponent<T2>(result2, out T2 component2))
		{
			return false;
		}
		result = new Entity<T1, T2>(result2, component, component2);
		return true;
	}

	public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
	{
		if (!ctx.CheckInvokable<EntitiesCommand>())
		{
			return null;
		}
		string argHint = ToolshedCommand.GetArgHint(arg, typeof(NetEntity));
		if (_entMan.Count<T1>() > 128)
		{
			return CompletionResult.FromHint(argHint);
		}
		AllEntityQueryEnumerator<T1, T2, MetaDataComponent> allEntityQueryEnumerator = _entMan.AllEntityQueryEnumerator<T1, T2, MetaDataComponent>();
		List<CompletionOption> list = new List<CompletionOption>();
		T1 comp;
		T2 comp2;
		MetaDataComponent comp3;
		while (allEntityQueryEnumerator.MoveNext(out comp, out comp2, out comp3))
		{
			list.Add(new CompletionOption(comp3.NetEntity.ToString(), comp3.EntityName));
		}
		return CompletionResult.FromHintOptions(list, argHint);
	}
}
internal sealed class EntityTypeParser<T1, T2, T3> : TypeParser<Entity<T1, T2, T3>> where T1 : IComponent where T2 : IComponent where T3 : IComponent
{
	[Dependency]
	private readonly IEntityManager _entMan;

	public override bool TryParse(ParserContext parser, out Entity<T1, T2, T3> result)
	{
		result = default(Entity<T1, T2, T3>);
		if (!EntityTypeParser.TryParseEntity(_entMan, parser, out var result2))
		{
			return false;
		}
		if (!_entMan.TryGetComponent<T1>(result2, out T1 component))
		{
			return false;
		}
		if (!_entMan.TryGetComponent<T2>(result2, out T2 component2))
		{
			return false;
		}
		if (!_entMan.TryGetComponent<T3>(result2, out T3 component3))
		{
			return false;
		}
		result = new Entity<T1, T2, T3>(result2, component, component2, component3);
		return true;
	}

	public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
	{
		if (!ctx.CheckInvokable<EntitiesCommand>())
		{
			return null;
		}
		string argHint = ToolshedCommand.GetArgHint(arg, typeof(NetEntity));
		if (_entMan.Count<T1>() > 128)
		{
			return CompletionResult.FromHint(argHint);
		}
		AllEntityQueryEnumerator<T1, T2, T3, MetaDataComponent> allEntityQueryEnumerator = _entMan.AllEntityQueryEnumerator<T1, T2, T3, MetaDataComponent>();
		List<CompletionOption> list = new List<CompletionOption>();
		T1 comp;
		T2 comp2;
		T3 comp3;
		MetaDataComponent comp4;
		while (allEntityQueryEnumerator.MoveNext(out comp, out comp2, out comp3, out comp4))
		{
			list.Add(new CompletionOption(comp4.NetEntity.ToString(), comp4.EntityName));
		}
		return CompletionResult.FromHintOptions(list, argHint);
	}
}
