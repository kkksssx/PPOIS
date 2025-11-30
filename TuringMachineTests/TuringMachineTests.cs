using TuringMachineLibrary;
using Xunit;

namespace TuringMachineTests;

public class TuringMachineTests
{
    #region Tape Tests

    [Fact]
    public void Tape_Initialize_WithValidInput_ShouldSetCells()
    {
        var tape = new Tape();
        tape.Initialize("101");

        Assert.Equal('1', tape.Read());
        tape.Move('R');
        Assert.Equal('0', tape.Read());
        tape.Move('R');
        Assert.Equal('1', tape.Read());
    }

    [Fact]
    public void Tape_Initialize_WithEmptyString_ShouldWork()
    {
        var tape = new Tape();
        tape.Initialize("");

        Assert.Equal('_', tape.Read());
    }

    [Fact]
    public void Tape_Initialize_WithNull_ShouldWork()
    {
        var tape = new Tape();
        tape.Initialize(null!);

        Assert.Equal('_', tape.Read());
    }

    [Fact]
    public void Tape_ReadWrite_ShouldWorkCorrectly()
    {
        var tape = new Tape();
        tape.Initialize("1");

        Assert.Equal('1', tape.Read());
        tape.Write('0');
        Assert.Equal('0', tape.Read());
    }

    [Fact]
    public void Tape_WriteBlankSymbol_ShouldRemoveCell()
    {
        var tape = new Tape();
        tape.Initialize("1");

        Assert.Equal('1', tape.Read());
        tape.Write('_');
        Assert.Equal('_', tape.Read());
    }

    [Fact]
    public void Tape_Move_ShouldChangePosition()
    {
        var tape = new Tape();
        tape.Initialize("101");

        Assert.Equal(0, tape.Position);
        tape.Move('R');
        Assert.Equal(1, tape.Position);
        tape.Move('L');
        Assert.Equal(0, tape.Position);
        tape.Move('N');
        Assert.Equal(0, tape.Position);
    }

    [Fact]
    public void Tape_Move_WithInvalidDirection_ShouldThrowException()
    {
        var tape = new Tape();
        tape.Initialize("1");

        var exception = Assert.Throws<ArgumentException>(() => tape.Move('X'));
        Assert.Contains("Invalid direction", exception.Message);
    }

    [Fact]
    public void Tape_GetTapeContent_ShouldReturnCorrectString()
    {
        var tape = new Tape();
        tape.Initialize("101");

        Assert.Equal("101", tape.GetTapeContent());
    }

    [Fact]
    public void Tape_GetTapeContent_WithEmptyTape_ShouldReturnBlankSymbol()
    {
        var tape = new Tape();
        tape.Initialize("");

        Assert.Equal("_", tape.GetTapeContent());
    }

    [Fact]
    public void Tape_GetTapeWithPointer_ShouldFormatCorrectly()
    {
        var tape = new Tape();
        tape.Initialize("101");

        string result = tape.GetTapeWithPointer();
        Assert.Contains("1 0 1", result);
        Assert.Contains("^", result);
    }

    [Fact]
    public void Tape_BlankSymbol_ShouldBeUsedForEmptyCells()
    {
        var tape = new Tape('B');
        tape.Initialize("");

        Assert.Equal('B', tape.Read());
    }

    [Fact]
    public void Tape_ReadCell_OutsideInitializedRange_ShouldReturnBlank()
    {
        var tape = new Tape();
        tape.Initialize("1");

        tape.Move('R');
        tape.Move('R');
        Assert.Equal('_', tape.Read());
    }

    #endregion

    #region TuringRule Tests

    [Fact]
    public void TuringRule_Constructor_WithValidParameters_ShouldCreateRule()
    {
        var rule = new TuringRule("q0", '1', "q1", '0', 'R');

        Assert.Equal("q0", rule.CurrentState);
        Assert.Equal('1', rule.ReadSymbol);
        Assert.Equal("q1", rule.NewState);
        Assert.Equal('0', rule.WriteSymbol);
        Assert.Equal('R', rule.Direction);
    }

    [Fact]
    public void TuringRule_Constructor_WithNullCurrentState_ShouldThrowException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new TuringRule(null!, '1', "q1", '0', 'R'));
        Assert.Contains("Current state cannot be null or empty", exception.Message);
    }

    [Fact]
    public void TuringRule_Constructor_WithEmptyCurrentState_ShouldThrowException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new TuringRule("", '1', "q1", '0', 'R'));
        Assert.Contains("Current state cannot be null or empty", exception.Message);
    }

    [Fact]
    public void TuringRule_Constructor_WithNullNewState_ShouldThrowException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new TuringRule("q0", '1', null!, '0', 'R'));
        Assert.Contains("New state cannot be null or empty", exception.Message);
    }

    [Fact]
    public void TuringRule_Constructor_WithInvalidDirection_ShouldThrowException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new TuringRule("q0", '1', "q1", '0', 'X'));
        Assert.Contains("Direction must be L, R, or N", exception.Message);
    }

    [Fact]
    public void TuringRule_IsApplicable_ShouldReturnTrueForMatchingStateAndSymbol()
    {
        var rule = new TuringRule("q0", '1', "q1", '0', 'R');

        Assert.True(rule.IsApplicable("q0", '1'));
        Assert.False(rule.IsApplicable("q1", '1'));
        Assert.False(rule.IsApplicable("q0", '0'));
    }

    [Fact]
    public void TuringRule_Equals_ShouldWorkCorrectly()
    {
        var rule1 = new TuringRule("q0", '1', "q1", '0', 'R');
        var rule2 = new TuringRule("q0", '1', "q2", '1', 'L');
        var rule3 = new TuringRule("q1", '1', "q2", '1', 'L');

        Assert.True(rule1.Equals(rule2));
        Assert.False(rule1.Equals(rule3));
        Assert.False(rule1.Equals(null));
    }

    [Fact]
    public void TuringRule_ToString_ShouldReturnFormattedString()
    {
        var rule = new TuringRule("q0", '1', "q1", '0', 'R');
        string result = rule.ToString();

        Assert.Contains("q0", result);
        Assert.Contains("1", result);
        Assert.Contains("q1", result);
        Assert.Contains("0", result);
        Assert.Contains("R", result);
    }

    #endregion

    #region TuringMachine Basic Tests

    [Fact]
    public void TuringMachine_Constructor_ShouldInitializeCorrectly()
    {
        var tm = new TuringMachine();

        Assert.Equal(string.Empty, tm.CurrentState);
        Assert.Equal(0, tm.StepCount);
        Assert.True(tm.IsHalted);
    }

    [Fact]
    public void TuringMachine_Constructor_WithRules_ShouldNotBeHalted()
    {
        var tm = new TuringMachine();
        tm.SetInitialState("q0");
        tm.AddRule(new TuringRule("q0", '1', "q1", '0', 'R'));
        tm.Initialize("1");

        Assert.Equal("q0", tm.CurrentState);
        Assert.Equal(0, tm.StepCount);
        Assert.False(tm.IsHalted);
    }

    [Fact]
    public void TuringMachine_Constructor_WithCustomBlankSymbol_ShouldWork()
    {
        var tm = new TuringMachine('B');
        tm.SetInitialState("q0");
        tm.Initialize("");

        Assert.Equal("B", tm.TapeContent);
    }

    [Fact]
    public void TuringMachine_AddRule_ShouldAddRule()
    {
        var tm = new TuringMachine();
        var rule = new TuringRule("q0", '1', "q1", '0', 'R');

        tm.AddRule(rule);

        Assert.Single(tm.GetRules());
        Assert.Contains(rule, tm.GetRules());
    }

    [Fact]
    public void TuringMachine_AddDuplicateRule_ShouldThrowException()
    {
        var tm = new TuringMachine();
        var rule = new TuringRule("q0", '1', "q1", '0', 'R');

        tm.AddRule(rule);
        var exception = Assert.Throws<InvalidOperationException>(() => tm.AddRule(rule));
        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public void TuringMachine_RemoveRule_ShouldRemoveRule()
    {
        var tm = new TuringMachine();
        var rule = new TuringRule("q0", '1', "q1", '0', 'R');

        tm.AddRule(rule);
        Assert.True(tm.RemoveRule(rule));
        Assert.Empty(tm.GetRules());
    }

    [Fact]
    public void TuringMachine_RemoveNonExistentRule_ShouldReturnFalse()
    {
        var tm = new TuringMachine();
        var rule = new TuringRule("q0", '1', "q1", '0', 'R');

        Assert.False(tm.RemoveRule(rule));
    }

    [Fact]
    public void TuringMachine_GetRules_ShouldReturnOrderedRules()
    {
        var tm = new TuringMachine();
        var rule1 = new TuringRule("q1", '0', "q2", '1', 'L');
        var rule2 = new TuringRule("q0", '1', "q1", '0', 'R');

        tm.AddRule(rule1);
        tm.AddRule(rule2);

        var rules = tm.GetRules().ToList();
        Assert.Equal(2, rules.Count);
        Assert.Equal("q0", rules[0].CurrentState);
        Assert.Equal("q1", rules[1].CurrentState);
    }

    [Fact]
    public void TuringMachine_SetInitialState_ShouldWork()
    {
        var tm = new TuringMachine();
        tm.SetInitialState("q1");

        Assert.Equal("q1", tm.CurrentState);
    }

    [Fact]
    public void TuringMachine_SetInitialState_WithNull_ShouldThrowException()
    {
        var tm = new TuringMachine();
        var exception = Assert.Throws<ArgumentException>(() => tm.SetInitialState(null!));
        Assert.Contains("State cannot be null or empty", exception.Message);
    }

    [Fact]
    public void TuringMachine_SetInitialState_WithEmpty_ShouldThrowException()
    {
        var tm = new TuringMachine();
        var exception = Assert.Throws<ArgumentException>(() => tm.SetInitialState(""));
        Assert.Contains("State cannot be null or empty", exception.Message);
    }

    [Fact]
    public void TuringMachine_AddFinalState_ShouldWork()
    {
        var tm = new TuringMachine();
        tm.AddFinalState("q_final");
        tm.SetInitialState("q_final");
        tm.Initialize("");

        Assert.True(tm.IsInFinalState());
    }

    [Fact]
    public void TuringMachine_AddFinalState_WithNull_ShouldThrowException()
    {
        var tm = new TuringMachine();
        var exception = Assert.Throws<ArgumentException>(() => tm.AddFinalState(null!));
        Assert.Contains("State cannot be null or empty", exception.Message);
    }

    [Fact]
    public void TuringMachine_AddFinalState_WithEmpty_ShouldThrowException()
    {
        var tm = new TuringMachine();
        var exception = Assert.Throws<ArgumentException>(() => tm.AddFinalState(""));
        Assert.Contains("State cannot be null or empty", exception.Message);
    }

    [Fact]
    public void TuringMachine_Initialize_ShouldResetStateAndTape()
    {
        var tm = new TuringMachine();
        tm.SetInitialState("q0");
        tm.AddRule(new TuringRule("q0", '1', "q1", '0', 'R'));
        tm.Initialize("101");
        tm.Step(); // Изменяем состояние

        tm.Initialize("010"); // Сбрасываем

        Assert.Equal("q0", tm.CurrentState);
        Assert.Equal(0, tm.StepCount);
        Assert.Equal("010", tm.TapeContent);
    }

    [Fact]
    public void TuringMachine_Initialize_WithNull_ShouldWork()
    {
        var tm = new TuringMachine();
        tm.SetInitialState("q0");
        tm.Initialize(null!);

        Assert.Equal("_", tm.TapeContent);
    }

    [Fact]
    public void TuringMachine_Step_WithApplicableRule_ShouldExecute()
    {
        var tm = new TuringMachine();
        tm.SetInitialState("q0");
        tm.AddRule(new TuringRule("q0", '1', "q1", '0', 'R'));
        tm.Initialize("1");

        bool result = tm.Step();

        Assert.True(result);
        Assert.Equal("q1", tm.CurrentState);
        Assert.Equal(1, tm.StepCount);
        Assert.Equal("0", tm.TapeContent);
    }

    [Fact]
    public void TuringMachine_Step_WithoutApplicableRule_ShouldReturnFalse()
    {
        var tm = new TuringMachine();
        tm.SetInitialState("q0");
        tm.Initialize("1");

        bool result = tm.Step();

        Assert.False(result);
        Assert.Equal(0, tm.StepCount);
    }

    [Fact]
    public void TuringMachine_Step_WhenInFinalState_ShouldReturnFalse()
    {
        var tm = new TuringMachine();
        tm.AddFinalState("q0");
        tm.SetInitialState("q0");
        tm.Initialize("");

        bool result = tm.Step();

        Assert.False(result);
    }

    [Fact]
    public void TuringMachine_Run_ShouldExecuteUntilHalted()
    {
        var tm = new TuringMachine();
        tm.SetInitialState("q0");
        tm.AddRule(new TuringRule("q0", '1', "q1", '0', 'R'));
        tm.AddRule(new TuringRule("q1", '0', "q2", '1', 'R'));
        tm.AddFinalState("q2");
        tm.Initialize("10");

        tm.Run();

        Assert.True(tm.IsInFinalState());
        Assert.Equal("q2", tm.CurrentState);
        Assert.Equal(2, tm.StepCount);
        Assert.Equal("01", tm.TapeContent);
    }

    [Fact]
    public void TuringMachine_Run_WithInfiniteLoop_ShouldThrowException()
    {
        var tm = new TuringMachine();
        tm.SetInitialState("q0");
        tm.SetMaxSteps(100); // Устанавливаем маленький лимит для теста

        // Создаем настоящий бесконечный цикл
        tm.AddRule(new TuringRule("q0", '1', "q0", '1', 'R'));
        tm.AddRule(new TuringRule("q0", '0', "q0", '0', 'R'));
        tm.AddRule(new TuringRule("q0", '_', "q0", '1', 'R'));

        tm.Initialize("11111111111111111111");

        var exception = Assert.Throws<InvalidOperationException>(() => tm.Run());
        Assert.Contains("Maximum step count", exception.Message);
    }

    [Fact]
    public void TuringMachine_IsInFinalState_ShouldReturnCorrectValue()
    {
        var tm = new TuringMachine();
        tm.AddFinalState("q0");
        tm.SetInitialState("q0");
        tm.Initialize("");

        Assert.True(tm.IsInFinalState());
    }

    [Fact]
    public void TuringMachine_IsHalted_WhenInFinalState_ShouldBeTrue()
    {
        var tm = new TuringMachine();
        tm.AddFinalState("q0");
        tm.SetInitialState("q0");
        tm.Initialize("");

        Assert.True(tm.IsHalted);
    }

    [Fact]
    public void TuringMachine_IsHalted_WhenNoApplicableRule_ShouldBeTrue()
    {
        var tm = new TuringMachine();
        tm.SetInitialState("q0");
        tm.Initialize("1");

        Assert.True(tm.IsHalted);
    }

    [Fact]
    public void TuringMachine_MultipleFinalStates_ShouldWork()
    {
        var tm = new TuringMachine();
        tm.AddFinalState("q1");
        tm.AddFinalState("q2");
        tm.SetInitialState("q1");
        tm.Initialize("");

        Assert.True(tm.IsInFinalState());
    }

    #endregion

    #region Program Loading Tests

    [Fact]
    public void LoadProgramFromStrings_ShouldParseCorrectly()
    {
        var program = new[]
        {
            "initial q0",
            "final q2",
            "q0 1 q1 0 R",
            "q1 0 q2 1 R"
        };

        var tm = new TuringMachine();
        tm.LoadProgramFromStrings(program);

        Assert.Equal("q0", tm.CurrentState);
        Assert.Equal(2, tm.GetRules().Count());

        var rulesList = tm.GetRules().ToList();
        Assert.Contains(rulesList, r => r.NewState == "q2");
    }

    [Fact]
    public void LoadProgramFromStrings_WithComments_ShouldIgnoreThem()
    {
        var program = new[]
        {
            "// This is a comment",
            "initial q0",
            "  // Another comment",
            "final q1",
            "q0 1 q1 0 R"
        };

        var tm = new TuringMachine();
        tm.LoadProgramFromStrings(program);

        Assert.Equal("q0", tm.CurrentState);
        Assert.Single(tm.GetRules());
    }

    [Fact]
    public void LoadProgramFromStrings_WithEmptyLines_ShouldIgnoreThem()
    {
        var program = new[]
        {
            "",
            "initial q0",
            "   ",
            "final q1",
            "q0 1 q1 0 R"
        };

        var tm = new TuringMachine();
        tm.LoadProgramFromStrings(program);

        Assert.Equal("q0", tm.CurrentState);
        Assert.Single(tm.GetRules());
    }

    [Fact]
    public void LoadProgramFromStrings_WithInvalidRuleFormat_ShouldThrowException()
    {
        var program = new[]
        {
            "initial q0",
            "q0 1 q1 0" // Не хватает direction
        };

        var tm = new TuringMachine();
        var exception = Assert.Throws<ArgumentException>(() => tm.LoadProgramFromStrings(program));
        Assert.Contains("Invalid line format", exception.Message);
    }

    [Fact]
    public void LoadProgramFromStrings_WithInvalidDirection_ShouldThrowException()
    {
        var program = new[]
        {
            "initial q0",
            "q0 1 q1 0 X" // Неправильное направление
        };

        var tm = new TuringMachine();
        var exception = Assert.Throws<ArgumentException>(() => tm.LoadProgramFromStrings(program));
        Assert.Contains("Invalid direction", exception.Message);
    }

    [Fact]
    public void LoadProgramFromStrings_WithInvalidCommand_ShouldThrowException()
    {
        var program = new[]
        {
            "unknown q0", // Неизвестная команда
            "q0 1 q1 0 R"
        };

        var tm = new TuringMachine();
        var exception = Assert.Throws<ArgumentException>(() => tm.LoadProgramFromStrings(program));
        Assert.Contains("Invalid command", exception.Message);
    }

    [Fact]
    public void LoadProgramFromStrings_WithoutInitialState_ShouldThrowException()
    {
        var program = new[]
        {
            "final q1",
            "q0 1 q1 0 R"
        };

        var tm = new TuringMachine();
        var exception = Assert.Throws<InvalidOperationException>(() => tm.LoadProgramFromStrings(program));
        Assert.Contains("Initial state not specified", exception.Message);
    }

    [Fact]
    public void LoadProgramFromStrings_WithNullProgram_ShouldThrowException()
    {
        var tm = new TuringMachine();
        var exception = Assert.Throws<ArgumentNullException>(() => tm.LoadProgramFromStrings(null!));
        Assert.Contains("programLines", exception.Message);
    }

    #endregion

    #region Complex Program Tests

    [Fact]
    public void TuringMachine_ComplexProgram_SimpleCopy()
    {
        // Простая программа для копирования
        var program = new[]
        {
            "initial start",
            "final end",
            "start 1 start 1 R",
            "start 0 start 0 R",
            "start _ end _ N"
        };

        var tm = new TuringMachine();
        tm.LoadProgramFromStrings(program);

        tm.Initialize("1010");
        tm.Run();

        Assert.Equal("1010", tm.TapeContent.Trim('_'));
        Assert.True(tm.IsInFinalState());
    }

    [Fact]
    public void TuringMachine_ComplexProgram_UnaryAddition()
    {
        // Сложение унарных чисел (1+1=11)
        var program = new[]
        {
            "initial q0",
            "final q2",
            "q0 1 q0 1 R",
            "q0 + q1 1 R",
            "q1 1 q1 1 R",
            "q1 _ q2 _ L"
        };

        var tm = new TuringMachine();
        tm.LoadProgramFromStrings(program);

        tm.Initialize("1+1");
        tm.Run();

        string result = tm.TapeContent.Replace("_", "").Replace("+", "");
        Assert.Equal("111", result);
        Assert.True(tm.IsInFinalState());
    }

    [Fact]
    public void TuringMachine_ComplexProgram_BinaryIncrement()
    {
        // Корректная программа для инкремента двоичного числа
        var program = new[]
        {
            "initial start",
            "final end",
            "start 1 start 1 R",
            "start 0 start 0 R",
            "start _ back _ L",
            "back 1 back 0 L",
            "back 0 end 1 N",
            "back _ end 1 N"
        };

        var tm = new TuringMachine();
        tm.LoadProgramFromStrings(program);

        // Тестируем инкремент "101" (5) -> "110" (6)
        tm.Initialize("101");
        tm.Run();

        string actual = tm.TapeContent.Replace("_", "").Replace(" ", "");
        Assert.Equal("110", actual);
        Assert.True(tm.IsInFinalState());
    }

    #endregion

    #region Theory Tests

    [Theory]
    [InlineData("q0", '1', true)]
    [InlineData("q1", '1', false)]
    [InlineData("q0", '0', false)]
    public void TuringRule_IsApplicable_Theory(string state, char symbol, bool expected)
    {
        var rule = new TuringRule("q0", '1', "q1", '0', 'R');
        bool result = rule.IsApplicable(state, symbol);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("", "_")]
    [InlineData("1", "1")]
    [InlineData("101", "101")]
    [InlineData("010", "010")]
    public void Tape_GetTapeContent_Theory(string input, string expected)
    {
        var tape = new Tape();
        tape.Initialize(input);

        Assert.Equal(expected, tape.GetTapeContent());
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void TuringMachine_EmptyInput_ShouldWork()
    {
        var tm = new TuringMachine();
        tm.SetInitialState("q0");
        tm.Initialize("");

        Assert.Equal("_", tm.TapeContent);
        Assert.Equal("q0", tm.CurrentState);
        Assert.True(tm.IsHalted);
    }

    [Fact]
    public void TuringMachine_SingleStepToFinalState_ShouldWork()
    {
        var tm = new TuringMachine();
        tm.SetInitialState("q0");
        tm.AddRule(new TuringRule("q0", '_', "q1", '_', 'N'));
        tm.AddFinalState("q1");
        tm.Initialize("");

        bool result = tm.Step();

        Assert.True(result);
        Assert.True(tm.IsInFinalState());
    }

    [Fact]
    public void TuringMachine_NoMoveDirection_ShouldStayInPlace()
    {
        var tm = new TuringMachine();
        tm.SetInitialState("q0");
        tm.AddRule(new TuringRule("q0", '1', "q1", '0', 'N'));
        tm.Initialize("1");

        tm.Step();

        Assert.Equal("0", tm.TapeContent);
        Assert.Equal("q1", tm.CurrentState);
    }

    [Fact]
    public void TuringMachine_MultipleRulesSameState_ShouldWork()
    {
        var tm = new TuringMachine();
        tm.SetInitialState("q0");
        tm.AddRule(new TuringRule("q0", '0', "q1", '1', 'R'));
        tm.AddRule(new TuringRule("q0", '1', "q2", '0', 'R'));
        tm.Initialize("0");

        tm.Step();

        Assert.Equal("1", tm.TapeContent);
        Assert.Equal("q1", tm.CurrentState);
    }

    [Fact]
    public void TuringMachine_StepCount_IncrementsCorrectly()
    {
        var tm = new TuringMachine();
        tm.SetInitialState("q0");
        tm.AddRule(new TuringRule("q0", '1', "q1", '0', 'R'));
        tm.AddRule(new TuringRule("q1", '0', "q2", '1', 'R'));
        tm.Initialize("10");

        tm.Step();
        Assert.Equal(1, tm.StepCount);

        tm.Step();
        Assert.Equal(2, tm.StepCount);

        tm.Step(); // Не должно выполняться
        Assert.Equal(2, tm.StepCount);
    }

    #endregion
}