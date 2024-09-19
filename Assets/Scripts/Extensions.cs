using System.Collections;

public static class Extensions
{
    public static void Shuffle(this IList list) {
        var rand = new System.Random();
        for(int i = list.Count - 1; i > 0; --i) {
            var j = rand.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}