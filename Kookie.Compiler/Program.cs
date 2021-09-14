using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kookie.CodeAnalysis;
using Kookie.CodeAnalysis.Syntax;
using Kookie.CodeAnalysis.Text;

namespace Kookie.Compiler
{

    internal static class Program
    {
        private static void Main()
        {
            var showTree = false;
            var variables = new Dictionary<VariableSymbol, object>();
            var textBuilder = new StringBuilder();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                if (textBuilder.Length == 0)
                    Console.Write("» ");
                else
                    Console.Write("· ");
                Console.ResetColor();
                
                var input = Console.ReadLine();
                var isBlank = string.IsNullOrWhiteSpace(input);

                if(textBuilder.Length == 0)
                {
                    if (isBlank)
                    {
                        break;
                    }
                    else if (input == "#showTree")
                    {
                        showTree = !showTree;
                        Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees.");
                        continue;
                    }
                    else if (input == "#cls")
                    {
                        Console.Clear();
                        continue;
                    }
                }

                textBuilder.AppendLine(input);
                var text = textBuilder.ToString();
                
                var syntaxTree = SyntaxTree.Parse(text);

                if (!isBlank && syntaxTree.Diagnostics.Any())
                    continue;

                var compilation = new Compilation(syntaxTree);
                var result = compilation.Evaluate(variables);
                
                var diagnostics = result.Diagnostics;
                
                if (showTree)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    syntaxTree.Root.WriteTo(Console.Out);
                    Console.ResetColor();
                }

                
                if (!diagnostics.Any())
                {
                    Console.WriteLine(result.Value);
                }
                else
                {
                    foreach (var diagnostic in diagnostics)
                    {
                        var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
                        var line = syntaxTree.Text.Lines[lineIndex];
                        var lineNumber = lineIndex + 1;
                        var character = diagnostic.Span.Start - line.Start + 1;
                        
                        Console.WriteLine();
                        
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write($"({lineNumber}, {character}): ");
                        Console.WriteLine(diagnostic);
                        Console.ResetColor();

                        var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
                        var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);
                        
                        var prefix = syntaxTree.Text.ToString(prefixSpan);
                        var error = syntaxTree.Text.ToString(diagnostic.Span);
                        var suffix = syntaxTree.Text.ToString(suffixSpan);
                        
                        Console.Write("    ");
                        Console.Write(prefix);
                        
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(error);
                        Console.ResetColor();
                        
                        Console.Write(suffix);
                        
                        Console.WriteLine();
                    }

                    Console.WriteLine();
                }

                textBuilder.Clear();
            }
        }
    }
}