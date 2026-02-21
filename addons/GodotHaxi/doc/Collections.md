# Collection Util

## Usage
```cs
// Associate enumerable items into Dictionary
CollectionUtil.Assoc(list, i => i.GetId());

// Make set of values from enumerable items
CollectionUtil.SetOf(list, i => i.GetId());

// Group items into Dictionary<K, IEnumerable<V>>
CollectionUtil.Group(list, i => i.GetId());
```