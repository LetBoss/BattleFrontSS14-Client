using System.Collections.Generic;
using System.Text;

namespace Robust.Shared.Toolshed.Syntax;

public readonly record struct ParserRestorePoint(int Index, Stack<Rune> TerminatorStack, CommandArgumentBundle Bundle, IVariableParser VariableParser);
