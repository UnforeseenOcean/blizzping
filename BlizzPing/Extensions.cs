using System.Collections.Generic;

namespace BlizzPing
{
    public static class Extensions
    {
        public static int Sum(this List<int> list)
        {
            int result = 0;

            for (int i = 0; i < list.Count; i++)
            {
                result += list[i];
            }

            return result;
        }

        public static decimal Average(this List<int> list)
        {
            int sum = list.Sum();
            decimal result = (decimal)sum / list.Count;
            return result;
        }
    }
}
