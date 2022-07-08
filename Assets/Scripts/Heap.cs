using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for storing items in a min-heap datastructure.
/// </summary>
/// <typeparam name="T">
/// The datatype of the objects to be 
/// stored in the heap where it impelments the IHeapItem interface
/// </typeparam>
public class Heap<T> where T : IHeapItem<T> 
{
    private T[] items;

    private int currentItemCount;
    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    /// <summary>
    /// Create a new heap object with maximum capacity maxHeapSize.
    /// </summary>
    /// <param name="maxHeapSize">The maximum number of items the heap can store</param>
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    /// <summary>
    /// Add an item to the heap.
    /// </summary>
    /// <param name="item">The item to add</param>
    public void Push(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;

        SortUp(item);
        currentItemCount++;
    }

    /// <summary>
    /// Removes and returns the first item in the heap.
    /// </summary>
    /// <returns>The first item of type T in the heap</returns>
    public T Pop()
    {
        T firstItem = items[0];
        currentItemCount--;

        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;

        SortDown(items[0]);

        return firstItem;
    }

    /// <summary>
    /// Update the priority of an item.
    /// </summary>
    /// <param name="item">The updated item</param>
    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    /// <summary>
    /// Check if an item is contained in the heap.
    /// </summary>
    /// <param name="item">The item to search for</param>
    /// <returns>True if the item is in the heap, false otherwise</returns>
    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    /// <summary>
    /// Sorts an item down the heap to its proper location.
    /// </summary>
    /// <param name="item">The item to sort</param>
    private void SortDown(T item)
    {
        while (true)
        {
            int swapIndex;
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;

            // If the item has a child
            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    // If the left child has a higher priority than the right child
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        // Swap with the smaller one
                        swapIndex = childIndexRight;
                    }
                }

                // If the item has a higher priority than its child
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    // Otherwise we are done here
                    return;
                }
            }
            else
            {
                // Otherwise we are done here
                return;
            }
        }
    }

    /// <summary>
    /// Sorts an item up the heap to its proper location.
    /// </summary>
    /// <param name="item">The item to sort</param>
    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];

            // If the item's priority is smaller than the parent, swap them
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                // Otherwise we are done here.
                return;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    /// <summary>
    /// Swaps two items in the items array.
    /// </summary>
    /// <param name="itemA">The first item</param>
    /// <param name="itemB">The second item</param>
    private void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        int itemAIndex = itemA.HeapIndex;

        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

/// <summary>
/// Interface for items that will be added to a heap.
/// </summary>
/// <typeparam name="T">Type of the item being stored in a heap</typeparam>
public interface IHeapItem<T> : IComparable<T>
{
    public int HeapIndex
    {
        get;
        set;
    }
}
