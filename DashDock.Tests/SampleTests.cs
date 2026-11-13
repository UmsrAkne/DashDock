namespace DashDock.Tests;

/// <summary>
/// ユニットテストの基本的な書き方・アサーションのサンプル集です。
/// 実際のテストを作成する際の参考・テンプレートとして利用してください。
/// </summary>
[TestFixture]
public class SampleTests
{
    private SampleCalculator calculator;

    // 各テストメソッドの実行前に毎回呼ばれる初期化処理
    [SetUp]
    public void SetUp()
    {
        calculator = new SampleCalculator();
    }

    // 各テストメソッドの実行後に毎回呼ばれる後片付け処理（必要な場合のみ）
    [TearDown]
    public void TearDown()
    {
        calculator.Dispose();
    }

    [Test]
    public void Add_2つの数値を渡したとき_合計値が返されること()
    {
        // --------------------------------------------------
        // Arrange (準備)
        // --------------------------------------------------
        int a = 10;
        int b = 20;

        // --------------------------------------------------
        // Act (実行)
        // --------------------------------------------------
        int actual = calculator.Add(a, b);

        // --------------------------------------------------
        // Assert (検証) - NUnit4 では Assert.That を推奨
        // --------------------------------------------------
        Assert.That(actual, Is.EqualTo(30));
    }

    // [TestCase] を使うと、複数の入力値・期待値をまとめてテストできます
    [TestCase(1, 2, 3)]
    [TestCase(0, 0, 0)]
    [TestCase(-5, 5, 0)]
    public void Add_パラメタライズドテスト(int a, int b, int expected)
    {
        int actual = calculator.Add(a, b);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Divide_0で除算したとき_ArgumentExceptionがスローされること()
    {
        // 例外のテスト: Assert.Throws または Throws.TypeOf を使用
        Assert.That(
            () => calculator.Divide(10, 0),
            Throws.TypeOf<ArgumentException>()
                .With.Message.Contains("0で割ることはできません"));
    }

    [Test]
    public async Task FetchDataAsync_非同期処理が完了したとき_正しいデータが返されること()
    {
        // 非同期メソッドのテストは async Task を戻り値にします
        string result = await calculator.FetchDataAsync();

        Assert.That(result, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void Collection_コレクションの検証パターン()
    {
        var items = calculator.GetSupportedOperations();

        // コレクションに対する代表的なアサーション
        Assert.Multiple(() =>
        {
            Assert.That(items, Has.Count.EqualTo(4));
            Assert.That(items, Does.Contain("Add"));
            Assert.That(items, Is.EquivalentTo(new[] { "Add", "Subtract", "Multiply", "Divide", }));
        });
    }
}

#region テスト対象のダミー実装（確認用 / テンプレートから削除可）

public class SampleCalculator : IDisposable
{
    public int Add(int a, int b)
    {
        return a + b;
    }

    public int Divide(int a, int b)
    {
        if (b == 0)
        {
            throw new ArgumentException("0で割ることはできません", nameof(b));
        }

        return a / b;
    }

    public async Task<string> FetchDataAsync()
    {
        await Task.Delay(10);
        return "Loaded Data";
    }

    public List<string> GetSupportedOperations()
    {
        return new List<string> { "Add", "Subtract", "Multiply", "Divide", };
    }

    public void Dispose()
    {
        // 破棄処理
    }
}

#endregion