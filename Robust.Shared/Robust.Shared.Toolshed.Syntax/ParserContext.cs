using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Robust.Shared.Collections;
using Robust.Shared.Console;
using Robust.Shared.Log;
using Robust.Shared.Player;
using Robust.Shared.Toolshed.Errors;

namespace Robust.Shared.Toolshed.Syntax;

public sealed class ParserContext
{
	public bool NoMultilineExprs;

	public readonly ToolshedManager Toolshed;

	public readonly ToolshedEnvironment Environment;

	public IVariableParser VariableParser;

	public CommandArgumentBundle Bundle;

	public bool GenerateCompletions;

	public CompletionResult? Completions;

	public IConError? Error;

	public readonly string Input;

	public readonly ICommonSession? Session;

	private Stack<Rune> _blockStack = new Stack<Rune>();

	public int MaxIndex { get; }

	public int Index { get; private set; }

	public bool OutOfInput => Index > MaxIndex;

	public static bool IsToken(Rune c)
	{
		if (!Rune.IsLetterOrDigit(c))
		{
			return c == new Rune('_');
		}
		return true;
	}

	public static bool IsCommandToken(Rune c)
	{
		if (Rune.IsLetterOrDigit(c))
		{
			return true;
		}
		if (Rune.IsWhiteSpace(c))
		{
			return false;
		}
		if (c != new Rune('{') && c != new Rune('}') && c != new Rune('[') && c != new Rune(']') && c != new Rune('(') && c != new Rune(')') && c != new Rune('"') && c != new Rune('\'') && c != new Rune(':') && c != new Rune(';') && c != new Rune('|') && c != new Rune('$'))
		{
			return !Rune.IsControl(c);
		}
		return false;
	}

	public static bool IsNumeric(Rune c)
	{
		if (!IsToken(c) && !(c == new Rune('+')) && !(c == new Rune('-')) && !(c == new Rune('.')))
		{
			return c == new Rune('%');
		}
		return true;
	}

	public ParserContext(string input, ToolshedManager toolshed, ToolshedEnvironment environment, IVariableParser parser, ICommonSession? session)
	{
		Toolshed = toolshed;
		Environment = environment;
		Input = input;
		MaxIndex = input.Length - 1;
		VariableParser = parser;
		Session = session;
	}

	public ParserContext(string input, ToolshedManager toolshed)
		: this(input, toolshed, toolshed.DefaultEnvironment, IVariableParser.Empty, null)
	{
	}

	public ParserContext(string input, ToolshedManager toolshed, IInvocationContext ctx)
		: this(input, toolshed, ctx.Environment, new InvocationCtxVarParser(ctx), ctx.Session)
	{
	}

	private ParserContext(ParserContext parserContext, int sliceSize, int? index)
	{
		Toolshed = parserContext.Toolshed;
		Environment = parserContext.Environment;
		Input = parserContext.Input;
		Index = index ?? parserContext.Index;
		MaxIndex = Math.Min(parserContext.MaxIndex, Index + sliceSize - 1);
		VariableParser = parserContext.VariableParser;
		Session = parserContext.Session;
	}

	public bool EatMatch(char c)
	{
		return EatMatch(new Rune(c));
	}

	public bool EatMatch(Rune c)
	{
		Rune? rune = PeekRune();
		if (rune.HasValue)
		{
			Rune valueOrDefault = rune.GetValueOrDefault();
			if (!(valueOrDefault != c))
			{
				Index += c.Utf16SequenceLength;
				return true;
			}
		}
		return false;
	}

	public bool EatMatch(string c)
	{
		if (PeekWord() != c)
		{
			return false;
		}
		GetWord();
		return true;
	}

	public char? PeekChar()
	{
		Rune? rune = PeekRune();
		if (rune.HasValue)
		{
			Rune valueOrDefault = rune.GetValueOrDefault();
			if (valueOrDefault.Utf16SequenceLength > 1)
			{
				return '\u0001';
			}
			Span<char> destination = stackalloc char[2];
			valueOrDefault.EncodeToUtf16(destination);
			return destination[0];
		}
		return null;
	}

	public Rune? PeekRune()
	{
		if (MaxIndex < Index)
		{
			return null;
		}
		return Rune.GetRuneAt(Input, Index);
	}

	public Rune? GetRune()
	{
		Rune? rune = PeekRune();
		if (rune.HasValue)
		{
			Rune valueOrDefault = rune.GetValueOrDefault();
			Index += valueOrDefault.Utf16SequenceLength;
			return valueOrDefault;
		}
		return null;
	}

	public char? GetChar()
	{
		Rune? rune = PeekRune();
		if (rune.HasValue)
		{
			Rune valueOrDefault = rune.GetValueOrDefault();
			Index += valueOrDefault.Utf16SequenceLength;
			if (valueOrDefault.Utf16SequenceLength > 1)
			{
				return '\u0001';
			}
			Span<char> destination = stackalloc char[2];
			valueOrDefault.EncodeToUtf16(destination);
			return destination[0];
		}
		return null;
	}

	public void DebugPrint()
	{
		Logger.DebugS("parser", string.Join(", ", _blockStack));
		Logger.DebugS("parser", Input);
		MakeDebugPointer(Index);
		MakeDebugPointer(MaxIndex, '|');
	}

	private void MakeDebugPointer(int pointAt, char pointer = '^')
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(' ', pointAt);
		stringBuilder.Append(pointer);
		Logger.DebugS("parser", stringBuilder.ToString());
	}

	private string? MaybeGetWord(bool advanceIndex, Func<Rune, bool>? test)
	{
		int index = Index;
		if (test == null)
		{
			test = (Rune c) => !Rune.IsWhiteSpace(c);
		}
		StringBuilder stringBuilder = new StringBuilder();
		ConsumeWhitespace();
		while (true)
		{
			Rune? rune = PeekRune();
			if (!rune.HasValue)
			{
				break;
			}
			Rune valueOrDefault = rune.GetValueOrDefault();
			if (!test(valueOrDefault))
			{
				break;
			}
			stringBuilder.Append(GetRune());
		}
		if (index == Index)
		{
			return null;
		}
		if (!advanceIndex)
		{
			Index = index;
		}
		return stringBuilder.ToString();
	}

	public string? PeekWord(Func<Rune, bool>? test = null)
	{
		return MaybeGetWord(advanceIndex: false, test);
	}

	public string? GetWord(Func<Rune, bool>? test = null)
	{
		return MaybeGetWord(advanceIndex: true, test);
	}

	public bool TryMatch(Regex match, int max = int.MaxValue)
	{
		ValueList<char> valueList = new ValueList<char>(8);
		Span<char> destination = stackalloc char[4];
		do
		{
			Rune? rune = PeekRune();
			if (rune.HasValue)
			{
				Rune valueOrDefault = rune.GetValueOrDefault();
				if (max == 0)
				{
					return false;
				}
				max--;
				int num = valueOrDefault.EncodeToUtf16(destination);
				for (int i = 0; i < num; i++)
				{
					valueList.Add(destination[i]);
				}
				continue;
			}
			return false;
		}
		while (!match.IsMatch(valueList.Span));
		return true;
	}

	public bool TryMatch(string match)
	{
		ValueList<char> valueList = new ValueList<char>(8);
		Span<char> destination = stackalloc char[4];
		int index = Index;
		int num = match.Length;
		do
		{
			Rune? rune = GetRune();
			if (rune.HasValue)
			{
				Rune valueOrDefault = rune.GetValueOrDefault();
				if (num == 0)
				{
					Index = index;
					return false;
				}
				num--;
				int num2 = valueOrDefault.EncodeToUtf16(destination);
				for (int i = 0; i < num2; i++)
				{
					valueList.Add(destination[i]);
				}
				continue;
			}
			Index = index;
			return false;
		}
		while (!((ReadOnlySpan<char>)valueList.Span).SequenceEqual(match.AsSpan()));
		return true;
	}

	public ParserRestorePoint Save()
	{
		return new ParserRestorePoint(Index, new Stack<Rune>(_blockStack), Bundle, VariableParser);
	}

	public void Restore(ParserRestorePoint point)
	{
		Index = point.Index;
		_blockStack = point.TerminatorStack;
		Bundle = point.Bundle;
		VariableParser = point.VariableParser;
	}

	public int ConsumeWhitespace()
	{
		if (NoMultilineExprs)
		{
			return Consume((Rune x) => Rune.IsWhiteSpace(x) && x != new Rune('\n'));
		}
		return Consume(Rune.IsWhiteSpace);
	}

	public void PushBlockTerminator(Rune term)
	{
		_blockStack.Push(term);
	}

	public void PushBlockTerminator(char term)
	{
		PushBlockTerminator(new Rune(term));
	}

	public bool PeekBlockTerminator()
	{
		if (_blockStack.Count == 0)
		{
			return false;
		}
		return PeekRune() == _blockStack.Peek();
	}

	public bool EatBlockTerminator()
	{
		if (_blockStack.Count == 0)
		{
			return false;
		}
		if (!EatMatch(_blockStack.Peek()))
		{
			return false;
		}
		_blockStack.Pop();
		return true;
	}

	public bool CheckEndLine()
	{
		if (NoMultilineExprs)
		{
			return EatMatch('\n');
		}
		return false;
	}

	public int Consume(Func<Rune, bool> control)
	{
		int num = 0;
		while (true)
		{
			Rune? rune = PeekRune();
			if (!rune.HasValue)
			{
				break;
			}
			Rune valueOrDefault = rune.GetValueOrDefault();
			if (!control(valueOrDefault))
			{
				break;
			}
			Index += valueOrDefault.Utf16SequenceLength;
			num++;
		}
		return num;
	}

	public ParserContext? SliceBlock(Rune startDelim, Rune endDelim)
	{
		ParserRestorePoint point = Save();
		ConsumeWhitespace();
		if (GetRune() != startDelim)
		{
			Restore(point);
			return null;
		}
		int index = Index;
		int num = 1;
		while (num > 0)
		{
			Rune? rune = GetRune();
			if (rune == startDelim)
			{
				num++;
			}
			Rune? rune2 = rune;
			Rune rune3 = endDelim;
			if (rune2.HasValue && rune2.GetValueOrDefault() == rune3 && --num == 0)
			{
				break;
			}
			if (!rune.HasValue)
			{
				Restore(point);
				return null;
			}
		}
		return new ParserContext(this, Index - index, index);
	}

	public bool CheckInvokable(CommandSpec cmd)
	{
		return Toolshed.CheckInvokable(cmd, Session, out Error);
	}

	public bool CheckInvokable<T>() where T : ToolshedCommand
	{
		if (!Environment.TryGetCommands<T>(out IReadOnlyList<CommandSpec> commands))
		{
			return false;
		}
		foreach (CommandSpec item in commands)
		{
			if (!CheckInvokable(item))
			{
				return false;
			}
		}
		return true;
	}

	public bool PeekCommandOrBlockTerminated()
	{
		Rune? rune = PeekRune();
		if (rune.HasValue)
		{
			Rune valueOrDefault = rune.GetValueOrDefault();
			if (valueOrDefault == new Rune(';'))
			{
				return true;
			}
			if (valueOrDefault == new Rune('|'))
			{
				return true;
			}
			if (NoMultilineExprs && valueOrDefault == new Rune('\n'))
			{
				return true;
			}
			if (_blockStack.Count == 0)
			{
				return false;
			}
			return valueOrDefault == _blockStack.Peek();
		}
		return false;
	}

	public bool EatCommandTerminator(ref Type? pipedType, out bool commandExpected)
	{
		commandExpected = false;
		if (EatMatch(new Rune(';')))
		{
			pipedType = null;
			return true;
		}
		if (pipedType != null && pipedType != typeof(void) && EatMatch(new Rune('|')))
		{
			commandExpected = true;
			return true;
		}
		if (NoMultilineExprs && EatMatch(new Rune('\n')))
		{
			pipedType = null;
			return true;
		}
		return false;
	}

	public void EatCommandTerminators(ref Type? pipedType, out bool commandExpected)
	{
		if (EatCommandTerminator(ref pipedType, out commandExpected))
		{
			ConsumeWhitespace();
			while (!commandExpected && EatCommandTerminator(ref pipedType, out commandExpected))
			{
				ConsumeWhitespace();
			}
		}
	}
}
