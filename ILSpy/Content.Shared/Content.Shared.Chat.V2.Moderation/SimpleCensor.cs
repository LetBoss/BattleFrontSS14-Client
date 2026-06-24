using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;

namespace Content.Shared.Chat.V2.Moderation;

public sealed class SimpleCensor : IChatCensor
{
	private bool _shouldSanitizeLeetspeak;

	private FrozenDictionary<char, char> _leetspeakReplacements = FrozenDictionary<char, char>.Empty;

	private bool _shouldSanitizeSpecialCharacters;

	private HashSet<char> _specialCharacterReplacements = new HashSet<char>();

	private string[] _censoredWords = Array.Empty<string>();

	private string[] _falsePositives = Array.Empty<string>();

	private string[] _falseNegatives = Array.Empty<string>();

	private UnicodeRange[] _allowedUnicodeRanges = Array.Empty<UnicodeRange>();

	public bool Censor(string input, out string output, char replaceWith = '*')
	{
		output = Censor(input, replaceWith);
		return !string.Equals(input, output);
	}

	public string Censor(string input, char replaceWith = '*')
	{
		input = SanitizeOutBlockedUnicode(input);
		char[] originalInput = input.ToCharArray();
		input = SanitizeInput(input);
		List<char> censored = input.ToList();
		input = CheckProfanity(input, censored, _falseNegatives, replaceWith);
		char[] falsePositives = FindFalsePositives(censored, replaceWith);
		CheckProfanity(input, censored, _censoredWords, replaceWith);
		for (int i = 0; i < falsePositives.Length; i++)
		{
			if (falsePositives[i] != replaceWith)
			{
				censored[i] = falsePositives[i];
			}
		}
		for (int j = 0; j < originalInput.Length; j++)
		{
			if (originalInput[j] == ' ')
			{
				censored.Insert(j, ' ');
				continue;
			}
			if (_shouldSanitizeSpecialCharacters && _specialCharacterReplacements.Contains(originalInput[j]))
			{
				censored.Insert(j, originalInput[j]);
				continue;
			}
			if ((_shouldSanitizeLeetspeak || _shouldSanitizeSpecialCharacters) && originalInput[j] == '(' && j != originalInput.Length - 1 && originalInput[j + 1] == ')')
			{
				censored.Insert(j + 1, (censored[j] != replaceWith) ? ')' : replaceWith);
			}
			if (censored[j] != replaceWith)
			{
				censored[j] = originalInput[j];
			}
		}
		return string.Concat(censored);
	}

	public SimpleCensor WithSanitizeLeetSpeak()
	{
		_shouldSanitizeLeetspeak = true;
		return BuildCharacterReplacements();
	}

	public SimpleCensor WithSanitizeSpecialCharacters()
	{
		_shouldSanitizeSpecialCharacters = true;
		return BuildCharacterReplacements();
	}

	public SimpleCensor WithRanges(UnicodeRange[] ranges)
	{
		_allowedUnicodeRanges = ranges;
		return this;
	}

	public SimpleCensor WithCustomDictionary(string[] naughtyWords)
	{
		_censoredWords = naughtyWords;
		return this;
	}

	public SimpleCensor WithFalsePositives(string[] falsePositives)
	{
		_falsePositives = falsePositives;
		return this;
	}

	public SimpleCensor WithFalseNegatives(string[] falseNegatives)
	{
		_falseNegatives = falseNegatives;
		return this;
	}

	public SimpleCensor WithLeetspeakReplacements(Dictionary<char, char> replacements)
	{
		_leetspeakReplacements = replacements.ToFrozenDictionary();
		return this;
	}

	public SimpleCensor WithSpecialCharacterReplacements(Dictionary<char, char> replacements)
	{
		_leetspeakReplacements = replacements.ToFrozenDictionary();
		return this;
	}

	private string CheckProfanity(string input, List<char> censored, string[] words, char replaceWith = '*')
	{
		foreach (string word in words)
		{
			int wordLength = word.Length;
			int endOfFoundWord = 0;
			for (int foundIndex = input.IndexOf(word, endOfFoundWord, StringComparison.OrdinalIgnoreCase); foundIndex > -1; foundIndex = input.IndexOf(word, endOfFoundWord, StringComparison.OrdinalIgnoreCase))
			{
				endOfFoundWord = foundIndex + wordLength;
				for (int j = 0; j < wordLength; j++)
				{
					censored[foundIndex + j] = replaceWith;
				}
			}
		}
		return input;
	}

	private char[] FindFalsePositives(List<char> chars, char replaceWith = '*')
	{
		string input = string.Concat(chars);
		char[] output = Enumerable.Repeat(replaceWith, input.Length).ToArray();
		char[] inputAsARr = input.ToArray();
		string[] falsePositives = _falsePositives;
		foreach (string word in falsePositives)
		{
			int wordLength = word.Length;
			int endOfFoundWord = 0;
			for (int foundIndex = input.IndexOf(word, endOfFoundWord, StringComparison.OrdinalIgnoreCase); foundIndex > -1; foundIndex = input.IndexOf(word, endOfFoundWord, StringComparison.OrdinalIgnoreCase))
			{
				endOfFoundWord = foundIndex + wordLength;
				for (int j = foundIndex; j < endOfFoundWord; j++)
				{
					output[j] = inputAsARr[j];
				}
			}
		}
		return output;
	}

	private string SanitizeInput(string input)
	{
		if (_shouldSanitizeLeetspeak || _shouldSanitizeSpecialCharacters)
		{
			input = input.Replace("()", "o");
		}
		StringBuilder sb = new StringBuilder();
		string text = input;
		foreach (char character in text)
		{
			if (character != ' ' && (!_shouldSanitizeSpecialCharacters || !_specialCharacterReplacements.Contains(character)))
			{
				if (_shouldSanitizeLeetspeak && _leetspeakReplacements.TryGetValue(character, out var leetRepl))
				{
					sb.Append(leetRepl);
				}
				else
				{
					sb.Append(character);
				}
			}
		}
		return sb.ToString();
	}

	private string SanitizeOutBlockedUnicode(string input)
	{
		if (_allowedUnicodeRanges.Length == 0)
		{
			return input;
		}
		StringBuilder sb = new StringBuilder();
		foreach (Rune symbol in input.EnumerateRunes())
		{
			UnicodeRange[] allowedUnicodeRanges = _allowedUnicodeRanges;
			foreach (UnicodeRange range in allowedUnicodeRanges)
			{
				if (symbol.Value >= range.FirstCodePoint && symbol.Value < range.FirstCodePoint + range.Length)
				{
					sb.Append(symbol);
					break;
				}
			}
		}
		return sb.ToString();
	}

	private SimpleCensor BuildCharacterReplacements()
	{
		if (_shouldSanitizeSpecialCharacters)
		{
			_specialCharacterReplacements = new HashSet<char>
			{
				'-', '_', '|', '.', ',', '(', ')', '<', '>', '"',
				'`', '~', '*', '&', '%', '$', '#', '@', '!', '?',
				'+'
			};
		}
		if (_shouldSanitizeLeetspeak)
		{
			_leetspeakReplacements = new Dictionary<char, char>
			{
				['4'] = 'a',
				['$'] = 's',
				['!'] = 'i',
				['+'] = 't',
				['#'] = 'h',
				['@'] = 'a',
				['0'] = 'o',
				['1'] = 'i',
				['7'] = 'l',
				['3'] = 'e',
				['5'] = 's',
				['9'] = 'g',
				['<'] = 'c'
			}.ToFrozenDictionary();
		}
		return this;
	}
}
