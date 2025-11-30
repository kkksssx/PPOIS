using System;
using System.Collections.Generic;
using System.Linq;

namespace TuringMachineLibrary;

/// <summary>
/// Implementation of a Turing Machine with tape, states, and transition rules
/// </summary>
/// <remarks>
/// <para>
/// A Turing Machine consists of:
/// - A tape divided into cells, each containing a symbol
/// - A head that reads and writes symbols on the tape and moves left or right
/// - A state register storing the current state
/// - A finite set of transition rules defining the machine's behavior
/// </para>
/// <para>
/// The machine halts when it reaches a final state or when no applicable rule exists.
/// </para>
/// </remarks>
public class TuringMachine : ITuringMachine
{
    private readonly HashSet<TuringRule> _rules;
    private readonly Tape _tape;
    private readonly HashSet<string> _finalStates;

    private string _currentState;
    private string _initialState;
    private int _stepCount;
    private int _maxSteps;

    /// <inheritdoc/>
    public string CurrentState => _currentState ?? string.Empty;

    /// <inheritdoc/>
    public int StepCount => _stepCount;

    /// <inheritdoc/>
    public string TapeContent => _tape.GetTapeContent();

    /// <inheritdoc/>
    public bool IsHalted
    {
        get
        {
            if (string.IsNullOrEmpty(_currentState))
                return true;

            if (IsInFinalState())
                return true;

            // Проверяем, есть ли применимое правило
            char currentSymbol = _tape.Read();
            return !_rules.Any(r => r.IsApplicable(_currentState, currentSymbol));
        }
    }

    /// <summary>
    /// Initializes a new instance of the TuringMachine class
    /// </summary>
    /// <param name="blankSymbol">The symbol representing blank/empty cells</param>
    public TuringMachine(char blankSymbol = '_')
    {
        _rules = new HashSet<TuringRule>();
        _tape = new Tape(blankSymbol);
        _finalStates = new HashSet<string>();
        _stepCount = 0;
        _currentState = string.Empty;
        _initialState = string.Empty;
        _maxSteps = 10000;
    }

    /// <inheritdoc/>
    public void AddRule(TuringRule rule)
    {
        if (_rules.Contains(rule))
            throw new InvalidOperationException($"Rule already exists: {rule}");

        _rules.Add(rule);
    }

    /// <inheritdoc/>
    public bool RemoveRule(TuringRule rule)
    {
        return _rules.Remove(rule);
    }

    /// <inheritdoc/>
    public IEnumerable<TuringRule> GetRules()
    {
        return _rules.OrderBy(r => r.CurrentState).ThenBy(r => r.ReadSymbol);
    }

    /// <inheritdoc/>
    public void SetInitialState(string state)
    {
        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("State cannot be null or empty");

        _initialState = state;
        _currentState = state;
    }

    /// <inheritdoc/>
    public void AddFinalState(string state)
    {
        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("State cannot be null or empty");

        _finalStates.Add(state);
    }

    /// <inheritdoc/>
    public void Initialize(string input)
    {
        _tape.Initialize(input ?? "");
        _currentState = _initialState;
        _stepCount = 0;
    }

    /// <inheritdoc/>
    public bool Step()
    {
        if (IsHalted)
            return false;

        char currentSymbol = _tape.Read();
        TuringRule? applicableRule = _rules.FirstOrDefault(r => r.IsApplicable(_currentState, currentSymbol));

        if (applicableRule == null)
            return false;

        _tape.Write(applicableRule.WriteSymbol);
        _tape.Move(applicableRule.Direction);
        _currentState = applicableRule.NewState;
        _stepCount++;

        return true;
    }

    /// <inheritdoc/>
    public void Run()
    {
        while (Step())
        {
            if (_stepCount > _maxSteps)
            {
                throw new InvalidOperationException($"Maximum step count ({_maxSteps}) exceeded. Machine may be in infinite loop.");
            }
        }
    }

    /// <inheritdoc/>
    public bool IsInFinalState()
    {
        return !string.IsNullOrEmpty(_currentState) && _finalStates.Contains(_currentState);
    }

    /// <inheritdoc/>
    public void LoadProgramFromStrings(IEnumerable<string> programLines)
    {
        if (programLines == null)
            throw new ArgumentNullException(nameof(programLines));

        _rules.Clear();
        _finalStates.Clear();
        _initialState = string.Empty;
        _currentState = string.Empty;

        bool hasInitialState = false;

        foreach (string line in programLines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("//"))
                continue;

            string trimmedLine = line.Trim();
            string[] parts = trimmedLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 2)
            {
                if (parts[0] == "initial")
                {
                    SetInitialState(parts[1]);
                    hasInitialState = true;
                }
                else if (parts[0] == "final")
                {
                    AddFinalState(parts[1]);
                }
                else
                {
                    throw new ArgumentException($"Invalid command: {trimmedLine}");
                }
            }
            else if (parts.Length == 5)
            {
                if (parts[1].Length != 1 || parts[3].Length != 1 || parts[4].Length != 1)
                    throw new ArgumentException($"Invalid rule format: {trimmedLine}");

                if (!"LRN".Contains(parts[4]))
                    throw new ArgumentException($"Invalid direction in rule: {trimmedLine}");

                AddRule(new TuringRule(
                    parts[0],
                    parts[1][0],
                    parts[2],
                    parts[3][0],
                    parts[4][0]));
            }
            else if (parts.Length > 0)
            {
                throw new ArgumentException($"Invalid line format: {trimmedLine}");
            }
        }

        if (!hasInitialState)
        {
            throw new InvalidOperationException("Initial state not specified");
        }
    }

    /// <inheritdoc/>
    public void SetMaxSteps(int maxSteps)
    {
        _maxSteps = maxSteps;
    }
}