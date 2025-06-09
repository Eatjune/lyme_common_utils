using System;
using System.Collections.Generic;

namespace LymeGame.Utils.Common
{
    /// <summary>
    /// 这段代码实现了一个泛型数据结构 MultiMap<T, K>，是一个「多值映射表」，它的作用可以理解为：
    //  “一个键（T）可以对应多个值（K）”，也就是 Dictionary<T, List<K>> 的封装，提供了一些更方便的操作方法。
    //  它继承自 SortedDictionary<T, List<K>>，所以键是自动排序的（按 T 的默认比较器）。
    /// </summary>
    public class MultiMap<T, K>: SortedDictionary<T, List<K>>
    {
        private readonly List<K> m_Empty = new List<K>();

        public void Add(T t, K k)
        {
            TryGetValue(t, out var list);
            if (list == null)
            {
                list = new List<K>();
                Add(t, list);
            }
            list.Add(k);
        }

        public bool Remove(T t, K k)
        {
            TryGetValue(t, out var list);
            if (list == null)
            {
                return false;
            }
            if (!list.Remove(k))
            {
                return false;
            }
            if (list.Count == 0)
            {
                Remove(t);
            }
            return true;
        }

        /// <summary>
        /// 不返回内部的list,copy一份出来
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public K[] GetAll(T t)
        {
            TryGetValue(t, out var list);
            if (list == null)
            {
                return Array.Empty<K>();
            }
            return list.ToArray();
        }

        /// <summary>
        /// 返回内部的list
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public new List<K> this[T t]
        {
            get
            {
                TryGetValue(t, out List<K> list);
                return list ?? m_Empty;
            }
        }

        public K GetOne(T t)
        {
            TryGetValue(t, out var list);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return default;
        }

        public bool Contains(T t, K k)
        {
            TryGetValue(t, out var list);
            if (list == null)
            {
                return false;
            }
            return list.Contains(k);
        }
    }
}