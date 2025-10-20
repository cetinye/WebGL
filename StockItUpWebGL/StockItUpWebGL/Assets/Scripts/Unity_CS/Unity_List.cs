using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Unity_CS
{
    public static class Unity_List
    {
        #region EACH

        public delegate void _EachAction<T>(T value, ref bool doBreak);

        public delegate void _EachActionIterate<T>(T value, int i, ref bool doBreak);

        public static void _Each<T>(this IList<T> source, Action<T> action)
        {
            for (int i = 0; i < source.Count; i++)
                action(source[i]);
        }
        
        public static void _Each<T>(this IList<T> source, _EachAction<T> action)
        {
            var doBreak = false;

            for (int i = 0; i < source.Count; i++)
            {
                action(source[i], ref doBreak);
                if (doBreak)
                    break;
            }
        }

        public static void _Each<T>(this IList<T> source, Action<T, int> action)
        {
            for (int i = 0; i < source.Count; i++)
                action(source[i], i);
        }

        public static void _Each<T>(this IList<T> source, _EachActionIterate<T> action)
        {
            var doBreak = false;

            for (int i = 0; i < source.Count; i++)
            {
                action(source[i], i, ref doBreak);
                if (doBreak)
                    break;
            }
        }

        #endregion

        #region WHERE

        public static IList<T> _Where<T>(this IList<T> source, Func<T, bool> predicate)
        {
            List<T> ret = new List<T>();
            for (int i = 0; i < source.Count; i++)
            {
                if (predicate(source[i]))
                    ret.Add(source[i]);
            }
            return ret;
        }

        #endregion

        #region TAKE

        public static List<T> _Take<T>(this IList<T> source, int count)
        {
            List<T> ret = new List<T>();

            if (count > source.Count)
                count = source.Count;

            for (int i = 0; i < count; i++)
                ret.Add(source[i]);

            return ret;
        }

        public static List<T> _TakeWhile<T>(this IList<T> source, Func<T, bool> predicate)
        {
            List<T> ret = new List<T>();
            for (int i = 0; i < source.Count; i++)
            {
                if (predicate(source[i]))
                    ret.Add(source[i]);
                else
                    break;
            }

            return ret;
        }

        #endregion

        #region FIRSTORDEFAULT

        public static T _First<T>(this IList<T> list)
        {
            return list[0];
        }

        public static T _FirstOrDefault<T>(this IList<T> list)
        {
            if (list != null)
                return list.Count > 0 ? list[0] : default(T);

            return default(T);
        }

        #endregion

        #region LASTORDEFAULT

        public static T _Last<T>(this IList<T> list)
        {
            return list[list.Count - 1];
        }

        public static T _LastOrDefault<T>(this IList<T> list)
        {
            return list.Count > 0 ? list[list.Count - 1] : default(T);
        }

        #endregion

        #region REVERSE

        public static IList<T> _Reverse<T>(this IList<T> source)
        {
            List<T> ret = new List<T>();

            for (int i = source.Count - 1; i >= 0; i--)
                ret.Add(source[i]);

            return ret;
        }

        #endregion

        #region COUNT

        public static int _Count<T>(this IList<T> source)
        {
            return source.Count;
        }

        #endregion

        #region SUM

        public static int _Sum<T>(this IList<int> source)
        {
            int i = 0;
            for (int j = 0; j < source.Count; j++)
                i += Convert.ToInt32(source[j]);

            return i;
        }

        public static float _Sum<T>(this IList<float> source)
        {
            float i = 0;

            for (int j = 0; j < source.Count; j++)
            {
                i += Convert.ToSingle(source[j]);
            }

            return i;
        }

        public static int _Sum<T>(this IList<T> source, Func<T, int> selector)
        {
            int i = 0;

            for (int j = 0; j < source.Count; j++)
            {
                i += selector(source[j]);
            }
            return i;
        }

        public static float _Sum<T>(this IList<T> source, Func<T, float> selector)
        {
            float i = 0;

            for (int j = 0; j < source.Count; j++)
            {
                i += selector(source[j]);
            }
            return i;
        }

        public static int _Sum<T>(this IEnumerable<int> source)
        {
            int i = 0;
            foreach (var item in source)
                i += Convert.ToInt32(item);

            return i;
        }

        public static float _Sum<T>(this IEnumerable<float> source)
        {
            float i = 0f;
            foreach (var item in source)
                i += Convert.ToSingle(item);
            return i;
        }

        public static int _Sum<T>(this IEnumerable<T> source, Func<T, int> selector)
        {
            int i = 0;
            foreach (var item in source)
                i += selector(item);
            return i;
        }

        public static float _Sum<T>(this IEnumerable<T> source, Func<T, float> selector)
        {
            float f = 0;
            foreach (var item in source)
                f += selector(item);
            return f;
        }

        #endregion

        #region SHUFFLE

        public static void _Shuffle<T>(this IList<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        #endregion

        #region JOIN

        public static string _Join<T>(this List<T> list, string separator)
        {
            return string.Join(separator, list.Select(x => x.ToString()).ToArray());
        }

        #endregion

        #region SKIP

        public static IEnumerable<T> _Skip<T>(this IEnumerable<T> source, int count)
        {
            return source._SkipWhile((item, i) => i < count);
        }

        public static IEnumerable<T> _SkipWhile<T>(this IEnumerable<T> source, Func<T, int, bool> predicate)
        {
            using (var e = source.GetEnumerator())
            {
                for (var i = 0; ; i++)
                {
                    if (!e.MoveNext())
                        yield break;

                    if (!predicate(e.Current, i))
                        break;
                }

                do
                {
                    yield return e.Current;
                } while (e.MoveNext());
            }
        }

        #endregion

        #region LASTORDEFAULT

        public static T _LastOrDefault<T>(this IEnumerable<T> source)
        {
            var list = source as IList<T>;
            return list.Count > 0 ? list[list.Count - 1] : default(T);
        }

        #endregion

        #region CONCAT

        public static IEnumerable<T> _Concat<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            foreach (var item in first)
                yield return item;

            foreach (var item in second)
                yield return item;
        }

        #endregion

        #region DISTINCT

        public static IEnumerable<T> _Distinct<T>(this IEnumerable<T> source)
        {
            var set = new Dictionary<T, object>();

            var gotNull = false;

            foreach (var item in source)
            {
                if (item == null)
                {
                    if (gotNull)
                        continue;
                    gotNull = true;
                }
                else
                {
                    if (set.ContainsKey(item))
                        continue;
                    set.Add(item, null);
                }

                yield return item;
            }
        }

        #endregion

        #region UNION

        public static IEnumerable<T> _Union<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            return first._Concat(second)._Distinct();
        }

        #endregion

        #region CAST

        public static IEnumerable<int> _Cast<T>(this IEnumerable<float> source)
        {
            foreach (var item in source)
                yield return Convert.ToInt32(item);
        }

        public static IEnumerable<float> _Cast<T>(this IEnumerable<int> source)
        {
            foreach (var item in source)
                yield return Convert.ToSingle(item);
        }

        #endregion

        #region RANDOM

        public static T _RandomItem<T>(this IList<T> thisList)
        {
            if (thisList.Count > 0)
            {
                int r = UnityEngine.Random.Range(0, thisList.Count);
                return thisList[r];
            }
            else
                return default(T);
        }

        public static List<T> _RandomItems<T>(this IList<T> thisList, int randomCount)
        {
            if (randomCount < thisList.Count && thisList.Count > 0)
            {
                List<T> randomList = new List<T>();
                List<int> randomNumbers = new List<int>();

                while (randomList.Count < randomCount)
                {
                    int r = UnityEngine.Random.Range(0, thisList.Count);

                    if (!randomNumbers.Contains(r))
                    {
                        randomList.Add(thisList[r]);
                        randomNumbers.Add(r);
                    }
                }

                return randomList;
            }
            else
            {
                List<T> self = new List<T>();
                self.AddRange(thisList);
                return self;
            }
        }

        #endregion

        #region IMPLODE

        public static string _Implode<T>(this IList<T> thisList, string separator)
        {
            string retValue = string.Empty;
            for (int i = 0; i < thisList.Count; i++)
            {
                if (i < (thisList.Count - 1))
                    retValue += separator;
            }
            return retValue;
        }

        #endregion

        #region TOLIST

        public static List<T> _ToList<T>(this IEnumerable<T> source)
        {
            List<T> list = new List<T>();
            list.AddRange(source);
            return list;
        }

        #endregion

        #region CONTAINS

        public static bool _Contains<T>(this IEnumerable<T> source, T value)
        {
            var collection = source as ICollection<T>;
            return collection.Contains(value);
        }

        #endregion

        #region AddAsFirst

        public static void _AddAsFirst<T>(this IList<T> list, T value)
        {
            list.Insert(0, value);
        }

        #endregion

        #region ToStr

        public static string _ToStr<T>(this IList<T> thisList, string separator)
        {
            string retValue = string.Empty;
            for (int i = 0; i < thisList.Count; i++)
            {
                retValue += thisList[i];
                if (i < (thisList.Count - 1))
                    retValue += separator;
            }

            return retValue;
        }

        #endregion

        #region _GetRandomItemExcludeValues

        public static int _GetRandomItemExcludeValues(this IEnumerable<int> source, params int[] ext)
        {
            var pList = ext._ToList();
            var yList = new List<int>();

            foreach (var item in source)
            {
                if (!pList.Contains(item))
                    yList.Add(item);
            }

            var r = UnityEngine.Random.Range(0, yList.Count);
            return yList[r];
        }

        public static int _GetRandomItemExcludeIndex(this IList<int> source, params int[] ext)
        {
            var pList = ext._ToList();
            var yList = new List<int>();

            int i = 0;
            foreach (var item in source)
            {
                if (!pList.Contains(i))
                    yList.Add(i);
            }

            var r = UnityEngine.Random.Range(0, yList.Count);
            return source[yList[r]];
        }

        #endregion

        #region Save Text File

        public static void _Save(this IList<string> list, string path)
        {
            File.WriteAllLines(path, list.ToArray());
        }

        #endregion
    }
}