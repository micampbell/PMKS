﻿
using System;
using System.Collections;
using System.Collections.Generic;

namespace PlanarMechanismSimulator
{
    public class TimeSortedList : IList<KeyValuePair<double, double[,]>>
    {
        private int lastSpot = -1;
        private readonly List<double[,]> parameterValues = new List<double[,]>();
        private readonly List<double> timeKeys = new List<double>();

        public int Size
        {
            get { return lastSpot + 1; }
        }

        public List<double> Times
        {
            get { return timeKeys; }
        }

        public List<double[,]> Parameters
        {
            get { return parameterValues; }
        }

        internal void Add(double time, double[,] parameters)
        {
            if (Size == 0 || time > Times[lastSpot])
            {
                Times.Add(time);
                Parameters.Add(parameters);
            }
            else
            {
                int ub = lastSpot;
                int lb = 0;
                int i;
                do
                {
                    i = (ub - lb)/2;
                    if (Times[i] > time)
                        ub = i;
                    else lb = i;

                } while (ub - lb > 1);
                Times.Insert(i, time);
                Parameters.Insert(i, parameters);
            }
            lastSpot++;
        }

        internal void AddNearEnd(double time, double[,] parameters)
        {
            if (Size == 0 || time > Times[lastSpot])
            {
                Times.Add(time);
                Parameters.Add(parameters);
            }
            else
            {
                int i = lastSpot;
                while (Times[i] > time) i--;
                Times.Insert(i, time);
                Parameters.Insert(i, parameters);
            }
            lastSpot++;
        }

        internal void AddNearBegin(double time, double[,] parameters)
        {
            if (Size == 0 || time > Times[lastSpot])
            {
                Times.Add(time);
                Parameters.Add(parameters);
            }
            else
            {
                int i = 0;
                while (Times[i] < time) i++;
                Times.Insert(i, time);
                Parameters.Insert(i, parameters);
            }
            lastSpot++;
        }

        public void Add(KeyValuePair<double, double[,]> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<double, double[,]> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<double, double[,]>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return timeKeys.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(KeyValuePair<double, double[,]> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<double, double[,]>> GetEnumerator()
        {
            return new TimeKeyValueEnumerator(Times.ToArray(),Parameters.ToArray());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public double[,] this[double t]
        {
            get { return Parameters[Times.IndexOf(t)]; }
        }
        
        public int IndexOf(KeyValuePair<double, double[,]> item)
        {
            var index = Times.IndexOf(item.Key);
            if (index == -1) return -1;
            if (item.Value != Parameters[index])
                return -1;
            return index;
        }

        public void Insert(int index, KeyValuePair<double, double[,]> item)
        {
            Times.Insert(index,item.Key);
        Parameters.Insert(index,item.Value);
        }

        public void RemoveAt(int index)
        {
            Times.RemoveAt(index);
            Parameters.RemoveAt(index);
        }

        public KeyValuePair<double, double[,]> this[int index]
        {
            get { return new KeyValuePair<double, double[,]>(Times[index], Parameters[index]); }
            set
            {
                throw new InvalidOperationException();
            }
        }
    }

    class TimeKeyValueEnumerator : IEnumerator<KeyValuePair<double, double[,]>>
    {
        private readonly double[][,] parameterValues;
        private readonly double[] timeKeys;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;
        private readonly int length;
        public TimeKeyValueEnumerator(double[] timeKeys, double[][,] parameterValues)
        {
            this.timeKeys = timeKeys;
            this.parameterValues = parameterValues;
            length = timeKeys.GetLength(0);
        }

        public bool MoveNext()
        {
            position++;
            return (position < length);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public KeyValuePair<double, double[,]> Current
        {
            get
            {
                try
                {
                    return new KeyValuePair<double, double[,]>(timeKeys[position],parameterValues[position]);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        #endregion
    }

}