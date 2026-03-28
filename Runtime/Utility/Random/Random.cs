using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace ACore
{
    [Serializable]
    public struct Random<T>
    {
        [SerializeReference]
        [HideLabel]
        [InlineProperty]
        public RandomBase value;

        public T Get()
        {
            return value != null ? value.Get() : default;
        }

        /// <summary>
        /// Base class untuk semua jenis random.
        /// Semua turunan wajib implement fungsi Get().
        /// </summary>
        [Serializable]
        public abstract class RandomBase
        {
            public abstract T Get();
        }

        /// <summary>
        /// Mengambil nilai secara random dari list.
        /// 
        /// Contoh:
        /// values = [10, 20, 30]
        /// Hasil: bisa 10 / 20 / 30 secara acak
        /// </summary>
        [Serializable]
        public class Fixed : RandomBase
        {
            public List<T> values;

            public override T Get()
            {
                if (values == null || values.Count == 0)
                    return default;

                return values[UnityEngine.Random.Range(0, values.Count)];
            }
        }

        /// <summary>
        /// Mengambil nilai berdasarkan weight (bobot).
        /// Semakin besar weight, semakin besar kemungkinan terpilih.
        /// 
        /// Contoh:
        /// A (70), B (30)
        /// → A punya peluang lebih besar dari B
        /// </summary>
        [Serializable]
        public class Weighted : RandomBase
        {
            [Serializable]
            public struct Entry
            {
                [LabelText("Weight (%)")]
                public float weight;
                public T value;
            }

            public List<Entry> values;

            public override T Get()
            {
                if (values == null || values.Count == 0)
                    return default;

                var _total = 0f;

                for (var _i = 0; _i < values.Count; _i++)
                    _total += values[_i].weight > 0f ? values[_i].weight : 0f;

                if (_total <= 0f)
                    return values[UnityEngine.Random.Range(0, values.Count)].value;

                var _random = UnityEngine.Random.Range(0f, _total);

                for (var _i = 0; _i < values.Count; _i++)
                {
                    var _w = values[_i].weight > 0f ? values[_i].weight : 0f;

                    if (_random < _w)
                        return values[_i].value;

                    _random -= _w;
                }

                return values[0].value;
            }
        }

        /// <summary>
        /// Menghasilkan nilai berdasarkan peluang (chance).
        /// 
        /// Contoh:
        /// chance = 25%
        /// → 25% trueValue, 75% falseValue
        /// 
        /// Cocok untuk:
        /// - Drop item
        /// - Critical hit
        /// - Proc skill
        /// </summary>
        [Serializable]
        public class Chance : RandomBase
        {
            [LabelText("Chance (%)")] [Range(0, 100)]
            public float chance = 50f;

            [LabelText("True")] public T trueValue;
            [LabelText("False")] public T falseValue;

            public override T Get()
            {
                return UnityEngine.Random.value * 100f <= chance ? trueValue : falseValue;
            }
        }

        /// <summary>
        /// Mengambil nilai secara berurutan (loop).
        /// Tidak random.
        /// 
        /// Contoh:
        /// values = [A, B, C]
        /// Hasil:
        /// A → B → C → A → B → ...
        /// 
        /// Cocok untuk:
        /// - Pattern musuh
        /// - Testing
        /// </summary>
        [Serializable]
        public class Sequence : RandomBase
        {
            public List<T> values;
            private int index;

            public override T Get()
            {
                if (values == null || values.Count == 0)
                    return default;

                if (index >= values.Count)
                    index = 0;

                return values[index++];
            }
        }

        /// <summary>
        /// Random tanpa pengulangan sampai semua nilai terpakai.
        /// Setelah habis, akan shuffle ulang.
        /// 
        /// Dijamin:
        /// - Tidak ada repeat dalam 1 cycle
        /// - Tidak ada repeat antara akhir cycle dan awal cycle berikutnya
        /// 
        /// Contoh:
        /// values = [A, B, C]
        /// Hasil:
        /// A, C, B
        /// lalu reset → (tidak boleh mulai dari B) → A, B, C
        /// 
        /// Cocok untuk:
        /// - Loot biar adil
        /// - Card system
        /// </summary>
        [Serializable]
        public class Shuffle : RandomBase
        {
            public List<T> values;

            private List<T> pool;
            private int cursor;

            private T lastValue;
            private bool hasLastValue;

            public override T Get()
            {
                if (values == null || values.Count == 0)
                    return default;

                if (pool == null || cursor >= values.Count)
                {
                    if (pool == null)
                        pool = new List<T>(values.Count);

                    pool.Clear();
                    pool.AddRange(values);

                    for (var _i = 0; _i < pool.Count; _i++)
                    {
                        var _rnd = UnityEngine.Random.Range(_i, pool.Count);
                        (pool[_i], pool[_rnd]) = (pool[_rnd], pool[_i]);
                    }

                    if (hasLastValue && pool.Count > 1 &&
                        EqualityComparer<T>.Default.Equals(pool[0], lastValue))
                    {
                        var _swapIndex = UnityEngine.Random.Range(1, pool.Count);
                        (pool[0], pool[_swapIndex]) = (pool[_swapIndex], pool[0]);
                    }

                    cursor = 0;
                }

                var _value = pool[cursor++];
                lastValue = _value;
                hasLastValue = true;

                return _value;
            }
        }
    }
}