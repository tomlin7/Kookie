namespace Kookie.CodeAnalysis.Text
{
    public sealed class TextLine
    {
        public SourceText Text { get; }
        public int Start { get; }
        public int Length { get; }
        public int LengthIncludingLineBreak { get; }

        public TextLine(SourceText text, int start, int length, int lengthIncludingLineBreak)
        {
            Text = text;
            Start = start;
            Length = length;
            LengthIncludingLineBreak = lengthIncludingLineBreak;
        }

        public int End => Start + Length;
        public TextSpan Span => new TextSpan(Start, Length);
        public TextSpan SpanIncludingLineBreak => new(Start, LengthIncludingLineBreak);
        
        public override string ToString() => Text.ToString(Span);
    }
}