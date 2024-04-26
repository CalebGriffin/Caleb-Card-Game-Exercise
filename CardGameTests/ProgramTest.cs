namespace CardGameTests;

[TestClass]
public class ProgramTest
{
    static CardGame.Program _program = new CardGame.Program();

    // This data is dynamic so that it changes with the program's values
    public static IEnumerable<object[]> TestRunData
    {
        get
        {
            return new[]
            {
                new object[] { "", _program.InvalidInputFormat },
                new object[] { "asdf", _program.InvalidInputFormat },
                new object[] { "as,df", _program.InvalidInputFormat },
                new object[] { "ASD,F", _program.InvalidInputFormat },
                new object[] { "AS,DF", _program.InvalidCard },
                new object[] { "1S", _program.InvalidCard },
                new object[] { "2S,1S", _program.InvalidCard },
                new object[] { "3H,3H", _program.DuplicateCards },
                new object[] { "JK,JK,JK", _program.TooManyJokers }
            };
        }
    }

    [TestMethod]
    [DataRow("AC", "Your score is: 14")]
    [DataRow("3C,4C", "Your score is: 7")]
    [DataRow("TC,TD,TH,TS", "Your score is: 500")]
    [DataRow("JK", "Your score is: 0")]
    [DataRow("2C,2D,JK", "Your score is: 12")]
    [DataRow("2H,2S,JK,JK", "Your score is: 56")]
    [DataRow("TC,TD,TH,TS", "Your score is: 500")]
    [DataRow("TC,TD,TH,TS,2C,2D,2H,2S", "Your score is: 920")]
    [DynamicData(nameof(TestRunData))]
    public void TestRun(string input, string expected)
    {
        // StringReader is used to simulate user input
        using (var sr = new StringReader(input))
        {
            // Arrange
            Console.SetIn(sr);

            // Act
            _program.Run();
            string actual = _program.ResultMessage;

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }

    [TestMethod]
    [DataRow("", false)]
    [DataRow("asdf", false)]
    [DataRow("ASDF", false)]
    [DataRow("AS,DF", true)]
    [DataRow("as,df", false)]
    [DataRow("ASD,F", false)]
    [DataRow("AS,D", false)]
    [DataRow("AS,DF,GH", true)]
    [DataRow("AS,DF,GH,JK,L;", false)]
    public void TestVerifyInputFormat(string input, bool expected)
    {
        // Arrange
        _program.UserInput = input;

        // Act
        bool actual = _program.VerifyInputFormat();

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow("AS", true)]
    [DataRow("1S", false)]
    [DataRow("2B", false)]
    [DataRow("11S", false)]
    [DataRow("A", false)]
    [DataRow("0C", false)]
    public void TestVerifyValidCard(string input, bool expected)
    {
        // Arrange and Act
        bool actual = _program.VerifyValidCard(input);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow("2C", false)]
    [DataRow("2C,2C", true)]
    [DataRow("2C,2D", false)]
    [DataRow("2C,2D,2C", true)]
    [DataRow("JK,JK", false)]
    [DataRow("JK,2C,JK", false)]
    [DataRow("JK,2C,JK,2D,JK", true)]
    public void TestCheckForDuplicates(string input, bool expected)
    {
        // Arrange
        _program.UserInput = input;
        _program.VerifyInputFormat();

        // Act
        bool actual = _program.CheckForDuplicates().found;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow("2C", 2)]
    [DataRow("TC", 10)]
    [DataRow("JD", 22)]
    [DataRow("QH", 36)]
    [DataRow("KS", 52)]
    [DataRow("AC", 14)]
    [DataRow("3C,4C", 7)]
    [DataRow("TC,TD,TH,TS", 500)]
    [DataRow("JK", 0)]
    [DataRow("JK,JK", 0)]
    [DataRow("2C,JK", 4)]
    [DataRow("JK,2C,JK", 8)]
    [DataRow("2C,2D,JK", 12)]
    [DataRow("2H,2S,JK,JK", 56)]
    [DataRow("TC,TD,TH,TS", 500)]
    [DataRow("TC,TD,TH,TS,2C,2D,2H,2S", 920)]
    public void TestCalculateScore(string input, int expected)
    {
        // Arrange
        _program.Reset();
        _program.UserInput = input;
        _program.VerifyInputFormat();
        _program.VerifyAllCards();
        _program.CheckForDuplicates();
        
        // Act
        _program.CalculateScore();
        int actual = _program.Score;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestReset()
    {
        // Arrange and Act
        _program.Reset();

        // Assert
        Assert.AreEqual("", _program.UserInput);
        Assert.IsTrue(_program.SplitUserInput.Length == 0);
        Assert.AreEqual(0, _program.Score);
        Assert.AreEqual("", _program.ResultMessage);
        Assert.AreEqual(0, _program.NoOfJokers);
    }
}
