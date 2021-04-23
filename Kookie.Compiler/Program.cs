using System;
using System.Diagnostics;
using System.Linq;
using Kookie.CodeAnalysis;
using Kookie.CodeAnalysis.Binding;
using Kookie.CodeAnalysis.Syntax;

namespace Kookie
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

            while (true)
            {
                Console.Write("> ");
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
                var result = compilation.Evaluate();
                
                var diagnostics = result.Diagnostics;
                
                if (showTree)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    PrettyPrint(syntaxTree.Root);
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
                        Console.WriteLine();
                        
                        Console.ForegroundColor = ConsoleColor.DarkRed;
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

        private static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            // ├──
            // │
            // └──

            var marker = isLast ? "└──" : "├──";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);

            if (node is SyntaxToken {Value: { }} token)
            {
                Console.Write(" ");
                Console.Write(token.Value);
            }

            Console.WriteLine();
            indent += isLast ? "   " : "│  ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
            {
                PrettyPrint(child, indent, child == lastChild);
            }
        }
    }
}