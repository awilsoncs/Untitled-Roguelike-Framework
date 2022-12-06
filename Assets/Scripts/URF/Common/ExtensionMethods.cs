namespace URF.Common {
  using System.Collections.Generic;

  internal static class ExtensionMethods {

    public static TValue GetValueOrDefault<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      TKey key,
      TValue defaultValue
    ) {
      return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
    }

    public delegate TValue ElementGetter<TValue>();

    public static void Populate<TValue>(this TValue[,] array, ElementGetter<TValue> getter) {
      int width = array.GetLength(0);
      int height = array.GetLength(1);
      for (int x = 0; x < width; x++) {
        for (int y = 0; y < height; y++) {
          array[x, y] = getter();
        }
      }
    }

    public delegate TValue ElementGetterByPosition<TValue>(int x, int y);

    public static void Populate<TValue>(
      this TValue[,] array,
      ElementGetterByPosition<TValue> getter
    ) {
      int width = array.GetLength(0);
      int height = array.GetLength(1);
      for (int x = 0; x < width; x++) {
        for (int y = 0; y < height; y++) {
          array[x, y] = getter(x, y);
        }
      }
    }

  }
}
