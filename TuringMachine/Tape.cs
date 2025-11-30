using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TuringMachineLibrary;

/// <summary>
/// Represents the tape of a Turing Machine
/// </summary>
/// <remarks>
/// The tape is infinite in both directions and contains symbols at specific positions.
/// The tape head can read, write, and move left or right along the tape.
/// </remarks>
public class Tape
{
    private Dictionary<int, char> _cells;
    private readonly char _blankSymbol;

    /// <summary>
    /// Gets the current position of the tape head
    /// </summary>
    /// <value>The current zero-based position index</value>
    public int Position { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Tape class
    /// </summary>
    /// <param name="blankSymbol">The symbol representing blank/empty cells</param>
    public Tape(char blankSymbol = '_')
    {
        _cells = new Dictionary<int, char>();
        _blankSymbol = blankSymbol;
        Position = 0;
    }

    /// <summary>
    /// Initializes the tape with input data
    /// </summary>
    /// <param name="input">The initial tape content</param>
    /// <remarks>
    /// Clears existing tape content and positions the head at position 0.
    /// If input is null or empty, the tape will contain only blank symbols.
    /// </remarks>
    public void Initialize(string input)
    {
        _cells.Clear();
        Position = 0;

        if (!string.IsNullOrEmpty(input))
        {
            for (int i = 0; i < input.Length; i++)
            {
                _cells[i] = input[i];
            }
        }
    }

    /// <summary>
    /// Reads the symbol at the current tape position
    /// </summary>
    /// <returns>The symbol at current position, or blank symbol if position is empty</returns>
    public char Read()
    {
        return _cells.TryGetValue(Position, out char symbol) ? symbol : _blankSymbol;
    }

    /// <summary>
    /// Writes a symbol at the current tape position
    /// </summary>
    /// <param name="symbol">The symbol to write</param>
    /// <remarks>
    /// If the symbol is the blank symbol, the cell is effectively cleared.
    /// </remarks>
    public void Write(char symbol)
    {
        if (symbol != _blankSymbol)
            _cells[Position] = symbol;
        else
            _cells.Remove(Position);
    }

    /// <summary>
    /// Moves the tape head in the specified direction
    /// </summary>
    /// <param name="direction">The direction to move: 'L' (left), 'R' (right), or 'N' (no move)</param>
    /// <exception cref="System.ArgumentException">Thrown when direction is invalid</exception>
    public void Move(char direction)
    {
        switch (direction)
        {
            case 'L': Position--; break;
            case 'R': Position++; break;
            case 'N': break;
            default: throw new ArgumentException($"Invalid direction: {direction}. Valid values: L, R, N");
        }
    }

    /// <summary>
    /// Gets the tape content as a continuous string
    /// </summary>
    /// <returns>A string representing all non-blank symbols on the tape</returns>
    /// <remarks>
    /// The returned string includes only the segment between the leftmost and rightmost non-blank symbols.
    /// </remarks>
    public string GetTapeContent()
    {
        if (_cells.Count == 0)
            return _blankSymbol.ToString();

        int minPos = _cells.Keys.Min();
        int maxPos = _cells.Keys.Max();

        var result = new StringBuilder();
        for (int i = minPos; i <= maxPos; i++)
        {
            result.Append(_cells.TryGetValue(i, out char symbol) ? symbol : _blankSymbol);
        }

        return result.ToString();
    }

    /// <summary>
    /// Gets a visual representation of the tape with head position indicator
    /// </summary>
    /// <returns>
    /// A two-line string showing tape symbols on the first line and head position on the second line
    /// </returns>
    /// <example>
    /// <code>
    /// 0 1 0 1
    ///   ^
    /// </code>
    /// </example>
    public string GetTapeWithPointer()
    {
        if (_cells.Count == 0)
            return $"{_blankSymbol}\n^";

        int minPos = Math.Min(_cells.Keys.Min(), Position - 2);
        int maxPos = Math.Max(_cells.Keys.Max(), Position + 2);

        var tapeLine = new StringBuilder();
        var pointerLine = new StringBuilder();

        for (int i = minPos; i <= maxPos; i++)
        {
            char symbol = _cells.TryGetValue(i, out char s) ? s : _blankSymbol;
            tapeLine.Append(symbol);
            pointerLine.Append(i == Position ? '^' : ' ');

            if (i < maxPos)
            {
                tapeLine.Append(' ');
                pointerLine.Append(' ');
            }
        }

        return $"{tapeLine}\n{pointerLine}";
    }
}