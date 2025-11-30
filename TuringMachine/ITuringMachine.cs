using System.Collections.Generic;

namespace TuringMachineLibrary
{
    /// <summary>
    /// Interface defining the operations of a Turing Machine
    /// </summary>
    /// <remarks>
    /// A Turing Machine is a mathematical model of computation that consists of a tape, 
    /// a head that reads and writes symbols on the tape, and a set of rules that define 
    /// the machine's behavior based on its current state and the symbol being read.
    /// </remarks>
    public interface ITuringMachine
    {
        /// <summary>
        /// Gets the current state of the Turing Machine
        /// </summary>
        /// <value>The current state identifier</value>
        string CurrentState { get; }

        /// <summary>
        /// Gets the number of steps executed since initialization
        /// </summary>
        /// <value>The count of executed steps</value>
        int StepCount { get; }

        /// <summary>
        /// Gets the current content of the tape as a string
        /// </summary>
        /// <value>The tape content without position information</value>
        string TapeContent { get; }

        /// <summary>
        /// Gets a value indicating whether the machine has halted
        /// </summary>
        /// <value>true if the machine has halted; otherwise, false</value>
        /// <remarks>
        /// The machine halts when it reaches a final state or when no applicable rule exists.
        /// </remarks>
        bool IsHalted { get; }

        /// <summary>
        /// Adds a transition rule to the Turing Machine
        /// </summary>
        /// <param name="rule">The rule to add</param>
        /// <exception cref="System.InvalidOperationException">Thrown when rule already exists</exception>
        void AddRule(TuringRule rule);

        /// <summary>
        /// Removes a transition rule from the Turing Machine
        /// </summary>
        /// <param name="rule">The rule to remove</param>
        /// <returns>true if rule was successfully removed; otherwise, false</returns>
        bool RemoveRule(TuringRule rule);

        /// <summary>
        /// Gets all transition rules in the Turing Machine
        /// </summary>
        /// <returns>An enumerable collection of rules</returns>
        IEnumerable<TuringRule> GetRules();

        /// <summary>
        /// Sets the initial state of the Turing Machine
        /// </summary>
        /// <param name="state">The initial state identifier</param>
        /// <exception cref="System.ArgumentException">Thrown when state is null or empty</exception>
        void SetInitialState(string state);

        /// <summary>
        /// Adds a final (halting) state to the Turing Machine
        /// </summary>
        /// <param name="state">The final state identifier</param>
        /// <exception cref="System.ArgumentException">Thrown when state is null or empty</exception>
        void AddFinalState(string state);

        /// <summary>
        /// Initializes the Turing Machine with input on the tape
        /// </summary>
        /// <param name="input">The initial tape content</param>
        /// <remarks>
        /// Resets the step counter and sets the current state to the initial state.
        /// </remarks>
        void Initialize(string input);

        /// <summary>
        /// Executes a single step of the Turing Machine
        /// </summary>
        /// <returns>true if a step was executed; false if machine has halted</returns>
        bool Step();

        /// <summary>
        /// Runs the Turing Machine until it halts
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when maximum step count is exceeded (possible infinite loop)
        /// </exception>
        void Run();

        /// <summary>
        /// Determines whether the machine is in a final state
        /// </summary>
        /// <returns>true if current state is a final state; otherwise, false</returns>
        bool IsInFinalState();

        /// <summary>
        /// Loads a Turing Machine program from a collection of strings
        /// </summary>
        /// <param name="programLines">Lines containing program definitions</param>
        /// <exception cref="System.ArgumentNullException">Thrown when programLines is null</exception>
        /// <exception cref="System.ArgumentException">Thrown when program format is invalid</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when initial state is not specified</exception>
        /// <remarks>
        /// Program format:
        /// - initial state: "initial q0"
        /// - final states: "final q1", "final q2"
        /// - rules: "q0 0 q1 1 R", "q0 1 q0 0 R"
        /// - comments: lines starting with "//"
        /// </remarks>
        void LoadProgramFromStrings(IEnumerable<string> programLines);

        /// <summary>
        /// Sets the maximum number of steps before halting (infinite loop protection)
        /// </summary>
        /// <param name="maxSteps">Maximum number of steps allowed</param>
        void SetMaxSteps(int maxSteps);
    }
}