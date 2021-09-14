using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Kookie.CodeAnalysis;
using Kookie.CodeAnalysis.Binding;
using Kookie.CodeAnalysis.Syntax;
using Kookie.CodeAnalysis.Text;

namespace Kookie.Compiler
{
    // 1 + 2 * 3
    // 
    // 
    // 
    //     +
    //    / \
    //   1   *
    //      / \
    //     2   3
    
    // 1 + 2 + 3
    //
    //
    //
    //       +
    //      / \
    //     +   3
    //    / \
    //   1   2

    internal static class Program
    {
        private static void Main()
        {
            var showTree = false;
            var variables = new Dictionary<VariableSymbol, object>();

            while (true)
            {
                Console.ForegroundColor =ConsoleColor.Yellow;
                Console.Write("» ");
                Console.ResetColor();
                
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }

                switch (line)
                {
                    case "#showTree":
                        showTree = !showTree;
                        Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees.");
                        continue;
                    case "#cls":
                        Console.Clear();
                        continue;
                }

                var syntaxTree = SyntaxTree.Parse(line);
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
                    var text = syntaxTree.Text;
                    
                    foreach (var diagnostic in diagnostics)
                    {
                        var lineIndex = text.GetLineIndex(diagnostic.Span.Start);
                        var lineNumber = lineIndex + 1;
                        var character = diagnostic.Span.Start - text.Lines[lineIndex].Start + 1;
                        
                        Console.WriteLine();
                        
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write($"({lineNumber}, {character}): ");
                        Console.WriteLine(diagnostic);
                        Console.ResetColor();
                        
                        var prefix = line[..diagnostic.Span.Start];
                        var error = line.Substring(diagnostic.Span.Start, diagnostic.Span.Length);
                        var suffix = line[diagnostic.Span.End..];
                        
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
            }
        }

        
    }
}