namespace UberDeployer.ConsoleCommander
{
  internal static class ArrayExtensions
  {
    public static void CopyTo<T>(this T[] array, T[] destArray, int startIndex, int maxCount)
    {
      int index = 0;

      for (int i = startIndex; i < array.Length; i++)
      {
        destArray[index++] = array[i];

        if (index > maxCount)
        {
          break;
        }
      }
    }
  }
}
