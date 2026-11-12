using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Spine
{
	[DebuggerDisplay("Count={Count}")]
	public class ExposedList<T> : IEnumerable<T>, IEnumerable
	{
		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			private ExposedList<T> l;

			private int next;

			private int ver;

			private T current;

			public T Current => current;

			object IEnumerator.Current
			{
				get
				{
					VerifyState();
					if (next <= 0)
					{
						throw new InvalidOperationException();
					}
					return current;
				}
			}

			internal Enumerator(ExposedList<T> l)
			{
				this = default(Enumerator);
				this.l = l;
				ver = l.version;
			}

			public void Dispose()
			{
				l = null;
			}

			private void VerifyState()
			{
				if (l == null)
				{
					throw new ObjectDisposedException(GetType().FullName);
				}
				if (ver != l.version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
			}

			public bool MoveNext()
			{
				VerifyState();
				if (next < 0)
				{
					return false;
				}
				if (next < l.Count)
				{
					current = l.Items[next++];
					return true;
				}
				next = -1;
				return false;
			}

			void IEnumerator.Reset()
			{
				VerifyState();
				next = 0;
			}
		}

		public T[] Items;

		public int Count;

		private const int DefaultCapacity = 4;

		private static readonly T[] EmptyArray = new T[0];

		private int version;

		public int Capacity
		{
			get
			{
				return Items.Length;
			}
			set
			{
				if ((uint)value < (uint)Count)
				{
					throw new ArgumentOutOfRangeException();
				}
				Array.Resize(ref Items, value);
			}
		}

		public ExposedList()
		{
			Items = EmptyArray;
		}

		public ExposedList(IEnumerable<T> collection)
		{
			CheckCollection(collection);
			if (!(collection is ICollection<T> c))
			{
				Items = EmptyArray;
				AddEnumerable(collection);
			}
			else
			{
				Items = new T[c.Count];
				AddCollection(c);
			}
		}

		public ExposedList(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			Items = new T[capacity];
		}

		internal ExposedList(T[] data, int size)
		{
			Items = data;
			Count = size;
		}

		public void Add(T item)
		{
			if (Count == Items.Length)
			{
				GrowIfNeeded(1);
			}
			Items[Count++] = item;
			version++;
		}

		public void GrowIfNeeded(int addedCount)
		{
			int minimumSize = Count + addedCount;
			if (minimumSize > Items.Length)
			{
				Capacity = Math.Max(Math.Max(Capacity * 2, 4), minimumSize);
			}
		}

		public ExposedList<T> Resize(int newSize)
		{
			int itemsLength = Items.Length;
			T[] oldItems = Items;
			if (newSize > itemsLength)
			{
				Array.Resize(ref Items, newSize);
			}
			else if (newSize < itemsLength)
			{
				for (int i = newSize; i < itemsLength; i++)
				{
					oldItems[i] = default(T);
				}
			}
			Count = newSize;
			return this;
		}

		public void EnsureCapacity(int min)
		{
			if (Items.Length < min)
			{
				int newCapacity = ((Items.Length == 0) ? 4 : (Items.Length * 2));
				if (newCapacity < min)
				{
					newCapacity = min;
				}
				Capacity = newCapacity;
			}
		}

		private void CheckRange(int index, int count)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if ((uint)(index + count) > (uint)Count)
			{
				throw new ArgumentException("index and count exceed length of list");
			}
		}

		private void AddCollection(ICollection<T> collection)
		{
			int collectionCount = collection.Count;
			if (collectionCount != 0)
			{
				GrowIfNeeded(collectionCount);
				collection.CopyTo(Items, Count);
				Count += collectionCount;
			}
		}

		private void AddEnumerable(IEnumerable<T> enumerable)
		{
			foreach (T t in enumerable)
			{
				Add(t);
			}
		}

		public void AddRange(ExposedList<T> list)
		{
			CheckCollection(list);
			int collectionCount = list.Count;
			if (collectionCount != 0)
			{
				GrowIfNeeded(collectionCount);
				list.CopyTo(Items, Count);
				Count += collectionCount;
				version++;
			}
		}

		public void AddRange(IEnumerable<T> collection)
		{
			CheckCollection(collection);
			if (collection is ICollection<T> c)
			{
				AddCollection(c);
			}
			else
			{
				AddEnumerable(collection);
			}
			version++;
		}

		public int BinarySearch(T item)
		{
			return Array.BinarySearch(Items, 0, Count, item);
		}

		public int BinarySearch(T item, IComparer<T> comparer)
		{
			return Array.BinarySearch(Items, 0, Count, item, comparer);
		}

		public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
		{
			CheckRange(index, count);
			return Array.BinarySearch(Items, index, count, item, comparer);
		}

		public void Clear(bool clearArray = true)
		{
			if (clearArray)
			{
				Array.Clear(Items, 0, Items.Length);
			}
			Count = 0;
			version++;
		}

		public bool Contains(T item)
		{
			return Array.IndexOf(Items, item, 0, Count) != -1;
		}

		public ExposedList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
		{
			if (converter == null)
			{
				throw new ArgumentNullException("converter");
			}
			ExposedList<TOutput> u = new ExposedList<TOutput>(Count);
			for (int i = 0; i < Count; i++)
			{
				u.Items[i] = converter(Items[i]);
			}
			u.Count = Count;
			return u;
		}

		public void CopyTo(T[] array)
		{
			Array.Copy(Items, 0, array, 0, Count);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			Array.Copy(Items, 0, array, arrayIndex, Count);
		}

		public void CopyTo(int index, T[] array, int arrayIndex, int count)
		{
			CheckRange(index, count);
			Array.Copy(Items, index, array, arrayIndex, count);
		}

		public bool Exists(Predicate<T> match)
		{
			CheckMatch(match);
			return GetIndex(0, Count, match) != -1;
		}

		public T Find(Predicate<T> match)
		{
			CheckMatch(match);
			int i = GetIndex(0, Count, match);
			return (i != -1) ? Items[i] : default(T);
		}

		private static void CheckMatch(Predicate<T> match)
		{
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
		}

		public ExposedList<T> FindAll(Predicate<T> match)
		{
			CheckMatch(match);
			return FindAllList(match);
		}

		private ExposedList<T> FindAllList(Predicate<T> match)
		{
			ExposedList<T> results = new ExposedList<T>();
			for (int i = 0; i < Count; i++)
			{
				if (match(Items[i]))
				{
					results.Add(Items[i]);
				}
			}
			return results;
		}

		public int FindIndex(Predicate<T> match)
		{
			CheckMatch(match);
			return GetIndex(0, Count, match);
		}

		public int FindIndex(int startIndex, Predicate<T> match)
		{
			CheckMatch(match);
			CheckIndex(startIndex);
			return GetIndex(startIndex, Count - startIndex, match);
		}

		public int FindIndex(int startIndex, int count, Predicate<T> match)
		{
			CheckMatch(match);
			CheckRange(startIndex, count);
			return GetIndex(startIndex, count, match);
		}

		private int GetIndex(int startIndex, int count, Predicate<T> match)
		{
			int end = startIndex + count;
			for (int i = startIndex; i < end; i++)
			{
				if (match(Items[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public T FindLast(Predicate<T> match)
		{
			CheckMatch(match);
			int i = GetLastIndex(0, Count, match);
			return (i == -1) ? default(T) : Items[i];
		}

		public int FindLastIndex(Predicate<T> match)
		{
			CheckMatch(match);
			return GetLastIndex(0, Count, match);
		}

		public int FindLastIndex(int startIndex, Predicate<T> match)
		{
			CheckMatch(match);
			CheckIndex(startIndex);
			return GetLastIndex(0, startIndex + 1, match);
		}

		public int FindLastIndex(int startIndex, int count, Predicate<T> match)
		{
			CheckMatch(match);
			int start = startIndex - count + 1;
			CheckRange(start, count);
			return GetLastIndex(start, count, match);
		}

		private int GetLastIndex(int startIndex, int count, Predicate<T> match)
		{
			int i = startIndex + count;
			while (i != startIndex)
			{
				if (match(Items[--i]))
				{
					return i;
				}
			}
			return -1;
		}

		public void ForEach(Action<T> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			for (int i = 0; i < Count; i++)
			{
				action(Items[i]);
			}
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		public ExposedList<T> GetRange(int index, int count)
		{
			CheckRange(index, count);
			T[] tmpArray = new T[count];
			Array.Copy(Items, index, tmpArray, 0, count);
			return new ExposedList<T>(tmpArray, count);
		}

		public int IndexOf(T item)
		{
			return Array.IndexOf(Items, item, 0, Count);
		}

		public int IndexOf(T item, int index)
		{
			CheckIndex(index);
			return Array.IndexOf(Items, item, index, Count - index);
		}

		public int IndexOf(T item, int index, int count)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if ((uint)(index + count) > (uint)Count)
			{
				throw new ArgumentOutOfRangeException("index and count exceed length of list");
			}
			return Array.IndexOf(Items, item, index, count);
		}

		private void Shift(int start, int delta)
		{
			if (delta < 0)
			{
				start -= delta;
			}
			if (start < Count)
			{
				Array.Copy(Items, start, Items, start + delta, Count - start);
			}
			Count += delta;
			if (delta < 0)
			{
				Array.Clear(Items, Count, -delta);
			}
		}

		private void CheckIndex(int index)
		{
			if (index < 0 || (uint)index > (uint)Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
		}

		public void Insert(int index, T item)
		{
			CheckIndex(index);
			if (Count == Items.Length)
			{
				GrowIfNeeded(1);
			}
			Shift(index, 1);
			Items[index] = item;
			version++;
		}

		private void CheckCollection(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
		}

		public void InsertRange(int index, IEnumerable<T> collection)
		{
			CheckCollection(collection);
			CheckIndex(index);
			if (collection == this)
			{
				T[] buffer = new T[Count];
				CopyTo(buffer, 0);
				GrowIfNeeded(Count);
				Shift(index, buffer.Length);
				Array.Copy(buffer, 0, Items, index, buffer.Length);
			}
			else if (collection is ICollection<T> c)
			{
				InsertCollection(index, c);
			}
			else
			{
				InsertEnumeration(index, collection);
			}
			version++;
		}

		private void InsertCollection(int index, ICollection<T> collection)
		{
			int collectionCount = collection.Count;
			GrowIfNeeded(collectionCount);
			Shift(index, collectionCount);
			collection.CopyTo(Items, index);
		}

		private void InsertEnumeration(int index, IEnumerable<T> enumerable)
		{
			foreach (T t in enumerable)
			{
				Insert(index++, t);
			}
		}

		public int LastIndexOf(T item)
		{
			return Array.LastIndexOf(Items, item, Count - 1, Count);
		}

		public int LastIndexOf(T item, int index)
		{
			CheckIndex(index);
			return Array.LastIndexOf(Items, item, index, index + 1);
		}

		public int LastIndexOf(T item, int index, int count)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", index, "index is negative");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", count, "count is negative");
			}
			if (index - count + 1 < 0)
			{
				throw new ArgumentOutOfRangeException("count", count, "count is too large");
			}
			return Array.LastIndexOf(Items, item, index, count);
		}

		public bool Remove(T item)
		{
			int loc = IndexOf(item);
			if (loc != -1)
			{
				RemoveAt(loc);
			}
			return loc != -1;
		}

		public int RemoveAll(Predicate<T> match)
		{
			CheckMatch(match);
			int i = 0;
			int j = 0;
			for (i = 0; i < Count && !match(Items[i]); i++)
			{
			}
			if (i == Count)
			{
				return 0;
			}
			version++;
			for (j = i + 1; j < Count; j++)
			{
				if (!match(Items[j]))
				{
					Items[i++] = Items[j];
				}
			}
			if (j - i > 0)
			{
				Array.Clear(Items, i, j - i);
			}
			Count = i;
			return j - i;
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || (uint)index >= (uint)Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			Shift(index, -1);
			Array.Clear(Items, Count, 1);
			version++;
		}

		public T Pop()
		{
			if (Count == 0)
			{
				throw new InvalidOperationException("List is empty. Nothing to pop.");
			}
			int i = Count - 1;
			T item = Items[i];
			Items[i] = default(T);
			Count--;
			version++;
			return item;
		}

		public void RemoveRange(int index, int count)
		{
			CheckRange(index, count);
			if (count > 0)
			{
				Shift(index, -count);
				Array.Clear(Items, Count, count);
				version++;
			}
		}

		public void Reverse()
		{
			Array.Reverse(Items, 0, Count);
			version++;
		}

		public void Reverse(int index, int count)
		{
			CheckRange(index, count);
			Array.Reverse(Items, index, count);
			version++;
		}

		public void Sort()
		{
			Array.Sort(Items, 0, Count, Comparer<T>.Default);
			version++;
		}

		public void Sort(IComparer<T> comparer)
		{
			Array.Sort(Items, 0, Count, comparer);
			version++;
		}

		public void Sort(Comparison<T> comparison)
		{
			Array.Sort(Items, comparison);
			version++;
		}

		public void Sort(int index, int count, IComparer<T> comparer)
		{
			CheckRange(index, count);
			Array.Sort(Items, index, count, comparer);
			version++;
		}

		public T[] ToArray()
		{
			T[] t = new T[Count];
			Array.Copy(Items, t, Count);
			return t;
		}

		public void TrimExcess()
		{
			Capacity = Count;
		}

		public bool TrueForAll(Predicate<T> match)
		{
			CheckMatch(match);
			for (int i = 0; i < Count; i++)
			{
				if (!match(Items[i]))
				{
					return false;
				}
			}
			return true;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
