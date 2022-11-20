namespace Sorter.Logic.Structures
{
    using System.Collections.Generic;

    internal sealed class MinHeap<T>
    {
        private readonly IComparer<T> comparer;
        private readonly T[] heap;
        private int count;

        public MinHeap(int size, IComparer<T> comparer)
        {
            this.comparer = comparer;
            
            heap = new T[size];
        }
        
        public int Count => count;

        public void Add(T data)
        {
            heap[count++] = data;
            UpNode(count - 1);
        }

        public T RemoveMin()
        {
            if (count == 0)
            {
                return default;
            }

            var result = heap[0];
            Swap(0, count - 1);
            count--;

            FixDown(0);
            return result;
        }

        private void UpNode(int index)
        {
            while (index != 0 && comparer.Compare(heap[index], heap[GetParentIndex(index)]) < 0)
            {
                var parent = GetParentIndex(index);

                Swap(parent, index);
                index = parent;
            }
        }

        private void FixDown(int index)
        {
            while (true)
            {
                var smallest = index;

                var left = GetLeftChildIndex(index);
                var right = GetRightChildIndex(index);

                if (left < count && comparer.Compare(heap[left], heap[smallest]) < 0)
                {
                    smallest = left;
                }

                if (right < count && comparer.Compare(heap[right], heap[smallest]) < 0)
                {
                    smallest = right;
                }

                if (smallest == index)
                {
                    return;
                }

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int index1, int index2)
        {
            (heap[index1], heap[index2]) = (heap[index2], heap[index1]);
        }
        
        private int GetLeftChildIndex(int index) => 2 * index + 1;

        private int GetRightChildIndex(int index) => 2 * index + 2;

        private int GetParentIndex(int index) => (index - 1) / 2;
    }
}