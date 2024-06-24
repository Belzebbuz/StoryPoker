namespace StoryPoker.Server.Grains.Constants;

public static class Fibonacci
{
    public static int[] Sequence { get; } = GenerateFibonacciSequence(89);
    private static int[] GenerateFibonacciSequence(float maxNumber)
    {
        var fibonacci = new List<int> { 0, 1 };
        while (true)
        {
            var next = fibonacci[^1] + fibonacci[^2];
            if (next > maxNumber)
            {
                fibonacci.Add(next);
                break;
            }
            fibonacci.Add(next);
        }
        return fibonacci.ToArray();
    }
}
