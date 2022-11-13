namespace Sorter.Logic
{
    using System;

    public sealed class MinHeap<T> where T : IComparable<T>
    {
        private int count;
        private readonly T[] heap;

        public MinHeap(int size)
        {
            heap = new T[size];
        }

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

            Fix(0);

            return result;
        }

        private int GetLeftChildIndex(int index) => 2 * index + 1;

        private int GetRightChildIndex(int index) => 2 * index + 2;

        private int GetParentIndex(int index) => (index - 1) / 2;

        private void UpNode(int index)
        {
            var startIndex = index;

            while (startIndex != 0 && heap[GetParentIndex(startIndex)].CompareTo(heap[startIndex]) > 0)
            {
                var parentIndex = GetParentIndex(startIndex);
                Swap(parentIndex, startIndex);

                startIndex = parentIndex;
            }
        }

        private void Fix(int index)
        {
            var startIndex = index;
            var leftChildIndex = GetLeftChildIndex(startIndex);
            var rightChildIndex = GetRightChildIndex(startIndex);

            while ((leftChildIndex < count && Get(leftChildIndex).CompareTo(heap[startIndex]) < 0) ||
                   (rightChildIndex < count && Get(rightChildIndex).CompareTo(heap[startIndex]) < 0))
            {
                if (rightChildIndex < count && Get(rightChildIndex).CompareTo(Get(leftChildIndex)) < 0)
                {
                    Swap(startIndex, rightChildIndex);
                    startIndex = rightChildIndex;
                }
                else
                {
                    Swap(startIndex, leftChildIndex);
                    startIndex = leftChildIndex;
                }


                leftChildIndex = GetLeftChildIndex(startIndex);
                rightChildIndex = GetRightChildIndex(startIndex);
            }
        }

        private T Get(int index)
        {
            return heap[index];
        }

        private void Swap(int index1, int index2)
        {
            (heap[index1], heap[index2]) = (heap[index2], heap[index1]);
        }
    }
}