using System;

namespace TuringMachineLibrary;

/// <summary>
/// Represents a transition rule in a Turing Machine
/// </summary>
/// <remarks>
/// A transition rule defines how the machine behaves when in a specific state
/// and reading a specific symbol from the tape.
/// </remarks>
public class TuringRule
{
    /// <summary>
    /// Gets the current state required for this rule to apply
    /// </summary>
    /// <value>The current state identifier</value>
    public string CurrentState { get; }

    /// <summary>
    /// Gets the symbol that must be read for this rule to apply
    /// </summary>
    /// <value>The symbol to read from tape</value>
    public char ReadSymbol { get; }

    /// <summary>
    /// Gets the new state to transition to
    /// </summary>
    /// <value>The new state identifier</value>
    public string NewState { get; }

    /// <summary>
    /// Gets the symbol to write to the tape
    /// </summary>
    /// <value>The symbol to write</value>
    public char WriteSymbol { get; }

    /// <summary>
    /// Gets the direction to move the tape head
    /// </summary>
    /// <value>'L' for left, 'R' for right, 'N' for no move</value>
    public char Direction { get; }

    /// <summary>
    /// Initializes a new instance of the TuringRule class
    /// </summary>
    /// <param name="currentState">The current state required for this rule</param>
    /// <param name="readSymbol">The symbol that must be read</param>
    /// <param name="newState">The new state to transition to</param>
    /// <param name="writeSymbol">The symbol to write to the tape</param>
    /// <param name="direction">The direction to move the tape head</param>
    /// <exception cref="System.ArgumentException">
    /// Thrown when states are null/empty or direction is invalid
    /// </exception>
    public TuringRule(string currentState, char readSymbol, string newState, char writeSymbol, char direction)
    {
        if (string.IsNullOrWhiteSpace(currentState))
            throw new ArgumentException("Current state cannot be null or empty");

        if (string.IsNullOrWhiteSpace(newState))
            throw new ArgumentException("New state cannot be null or empty");

        if (direction != 'L' && direction != 'R' && direction != 'N')
            throw new ArgumentException("Direction must be L, R, or N");

        CurrentState = currentState;
        ReadSymbol = readSymbol;
        NewState = newState;
        WriteSymbol = writeSymbol;
        Direction = direction;
    }

    /// <summary>
    /// Determines whether this rule is applicable for the given state and symbol
    /// </summary>
    /// <param name="state">The current state to check</param>
    /// <param name="symbol">The symbol being read to check</param>
    /// <returns>true if this rule applies; otherwise, false</returns>
    public bool IsApplicable(string state, char symbol)
    {
        return state == CurrentState && symbol == ReadSymbol;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current rule
    /// </summary>
    /// <param name="obj">The object to compare with the current rule</param>
    /// <returns>true if the objects are equal; otherwise, false</returns>
    /// <remarks>
    /// Two rules are considered equal if they have the same CurrentState and ReadSymbol.
    /// This ensures that no two rules conflict for the same state-symbol combination.
    /// </remarks>
    public override bool Equals(object? obj)
    {
        return obj is TuringRule rule &&
               CurrentState == rule.CurrentState &&
               ReadSymbol == rule.ReadSymbol;
    }

    /// <summary>
    /// Serves as the default hash function
    /// </summary>
    /// <returns>A hash code for the current rule</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(CurrentState, ReadSymbol);
    }

    /// <summary>
    /// Returns a string that represents the current rule
    /// </summary>
    /// <returns>A string in the format "(state, symbol) → (newState, writeSymbol, direction)"</returns>
    public override string ToString()
    {
        return $"({CurrentState}, {ReadSymbol}) → ({NewState}, {WriteSymbol}, {Direction})";
    }
}