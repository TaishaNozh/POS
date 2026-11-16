using Pos.Core.Domain;
namespace Pos.Tests.Domain;

public class ScoreTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_Negative_Throws(int value)
    {
        Assert.Throws<DomainException>(() => new Score(value));
    }

    [Fact]
    public void Scores_WithSameValue_AreEqual()
    {
        Assert.Equal(new Score(7), new Score(7));
        Assert.NotEqual(new Score(7), new Score(8));
    }
}
