using System.Collections.Concurrent;

namespace FixedSizedQueue
{
    public class FixedSizedQueue<T>
    {
        ConcurrentQueue<T> q = new ConcurrentQueue<T>();
        private object lockObject = new object();

        public int Limit { get; set; }
        public void Enqueue(T obj)
        {
            q.Enqueue(obj);
            lock (lockObject)
            {
            T overflow;
            while (q.Count > Limit && q.TryDequeue(out overflow)) ;
            }
        }

        public T TryPeek()
        {
            T obj;
            q.TryPeek(out obj);
            return obj;
        }

        public T TryDequeue()
        {
            T obj;
            q.TryDequeue(out obj);
            return obj;
        }

        public int Count()
        {
            return q.Count;
        }
    }
}