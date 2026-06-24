using System;
using System.IO;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Utility;

internal sealed class TextParser
{
	[Virtual]
	public class ParserException : Exception
	{
		public ParserException(string message)
			: base(message)
		{
		}

		public ParserException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	private readonly TextReader _reader;

	private string? _currentLine;

	public int CurrentLine { get; private set; }

	public int CurrentIndex { get; private set; }

	public TextParser(TextReader reader)
	{
		_reader = reader;
	}

	public void NextLine()
	{
		_currentLine = _reader.ReadLine();
		CurrentIndex = 0;
		CurrentLine++;
	}

	public bool TryParse(string str)
	{
		if (IsEOL())
		{
			return false;
		}
		bool num = _currentLine.IndexOf(str, CurrentIndex, StringComparison.Ordinal) == CurrentIndex;
		if (num)
		{
			Advance(str.Length);
		}
		return num;
	}

	public bool TryParse(char chr)
	{
		if (IsEOL())
		{
			return false;
		}
		bool num = _currentLine[CurrentIndex] == chr;
		if (num)
		{
			Advance();
		}
		return num;
	}

	public void Parse(char chr)
	{
		if (IsEOL())
		{
			throw new ParserException($"Expected '{chr}', got EOL");
		}
		if (_currentLine[CurrentIndex] != chr)
		{
			throw new ParserException($"Expected '{chr}'.");
		}
		Advance();
	}

	public bool IsEOL()
	{
		if (_currentLine != null)
		{
			return _currentLine.Length <= CurrentIndex;
		}
		return true;
	}

	public bool IsEOF()
	{
		if (_reader.Peek() == -1)
		{
			return IsEOL();
		}
		return false;
	}

	public void EnsureEOL()
	{
		if (!IsEOL())
		{
			throw new ParserException("Expected EOL");
		}
	}

	public void EnsureNoEOL()
	{
		if (IsEOL())
		{
			throw new ParserException("Unexpected EOL");
		}
	}

	public bool EatWhitespace()
	{
		bool result = false;
		while (!IsEOL() && char.IsWhiteSpace(_currentLine, CurrentIndex))
		{
			Advance();
			result = true;
		}
		return result;
	}

	public string EatUntilWhitespace()
	{
		int currentIndex = CurrentIndex;
		while (!IsEOL() && !char.IsWhiteSpace(_currentLine, CurrentIndex))
		{
			Advance();
		}
		return _currentLine.Substring(currentIndex, CurrentIndex - currentIndex);
	}

	public string EatUntilEOL()
	{
		int currentIndex = CurrentIndex;
		while (!IsEOL())
		{
			Advance();
		}
		return _currentLine.Substring(currentIndex, CurrentIndex - currentIndex);
	}

	public char Peek()
	{
		if (CurrentIndex >= _currentLine.Length)
		{
			return '\0';
		}
		return _currentLine[CurrentIndex];
	}

	public char Take()
	{
		return _currentLine[CurrentIndex++];
	}

	public void Advance(int amount = 1)
	{
		CurrentIndex += amount;
	}

	public bool PeekIsDigit()
	{
		return char.IsDigit(_currentLine, CurrentIndex);
	}
}
