namespace URF.Common {
  using System.Collections.Generic;

  internal static class ExtensionMethods {

    /// <summary>
    /// Return either a value corresponding to the given key, or the given default if the key is not
    /// present in the dictionary.
    /// </summary>
    /// <param name="dictionary">The operand dictionary</param>
    /// <param name="key">The key of type TKey to query</param>
    /// <param name="defaultValue">The default value of type TValue to return if the key is not
    /// found in the dictionary.
    /// </param>
    /// <typeparam name="TKey">The type of the key</typeparam>
    /// <typeparam name="TValue">The type of the value</typeparam>
    /// <returns></returns>
    public static TValue GetValueOrDefault<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      TKey key,
      TValue defaultValue
    ) {
      return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
    }

    /// <summary>
    /// A function that returns a value of type TValue unrelated to position.
    /// </summary>
    /// <typeparam name="TValue">The type of value to return.</typeparam>
    /// <returns>A value of type T</returns>
    public delegate TValue ElementGetter<out TValue>();

    /// <summary>
    /// A function that returns a value of type TValue as a function of position.
    /// </summary>
    /// <param name="x">The horizontal position</param>
    /// <param name="y">The vertical position</param>
    /// <typeparam name="TValue">The type to return</typeparam>
    /// <returns>A TValue related to position (x, y)</returns>
    public delegate TValue ElementGetterByPosition<out TValue>(int x, int y);

    /// <summary>
    /// Fill an array by repeatedly calling a delegate.
    /// </summary>
    /// <param name="array">The array to fill</param>
    /// <param name="getter">The delegate to repeatedly call</param>
    /// <typeparam name="TValue">The type of the array elements</typeparam>
    public static void Populate<TValue>(this TValue[,] array, ElementGetter<TValue> getter) {
      array.Populate((x, y) => getter());
    }

    /// <summary>
    /// Fill an array by repeatedly calling a delegate. This override uses an alternate delegate
    /// that accepts a position, allowing the value returned to be a function of its position in the
    /// array.
    /// </summary>
    /// <param name="array">The array to fill</param>
    /// <param name="getter">The delegate to repeatedly call</param>
    /// <typeparam name="TValue">The type of the array elements</typeparam>
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
