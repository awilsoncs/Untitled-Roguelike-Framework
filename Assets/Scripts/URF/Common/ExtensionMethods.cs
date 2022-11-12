using System.Collections.Generic;

namespace URF.Common {
  internal static class ExtensionMethods {

    public static TValue GetValueOrDefault<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      TKey key,
      TValue defaultValue
    ) {
      return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
    }

  }
}
