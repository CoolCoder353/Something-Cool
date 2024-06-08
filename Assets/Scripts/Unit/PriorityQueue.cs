using System;

// A generic struct for priority queue
public struct PriorityQueue<T>
{
    // A private array to store the elements
    private T[] elements;

    // A private field to store the number of elements in the queue
    private int count;

    // A public property to get the number of elements in the queue
    public int Count
    {
        get { return count; }
    }

    // A private field to store the comparer delegate for comparing the elements
    private Comparison<T> comparer;

    // A constructor that creates an empty queue with a given capacity and comparer
    /// <summary>
    /// Initializes a new instance of the PriorityQueue<T> struct with a specified capacity and comparer.
    /// </summary>
    /// <param name="capacity">The initial number of elements that the PriorityQueue<T> can contain.</param>
    /// <param name="comparer">The Comparison<T> delegate to use when comparing elements.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">capacity is less than zero.</exception>
    /// <exception cref="System.ArgumentNullException">comparer is null.</exception>
    public PriorityQueue(int capacity, Comparison<T> comparer)
    {
        // Check if the capacity is valid, throw an exception if not
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "The capacity must be non-negative");
        }

        // Check if the comparer is null, throw an exception if so
        if (comparer == null)
        {
            throw new ArgumentNullException(nameof(comparer), "The comparer cannot be null");
        }

        // Initialize the array with the given capacity
        elements = new T[capacity];

        // Initialize the count to zero
        count = 0;

        // Initialize the comparer with the given delegate
        this.comparer = comparer;
    }


    public PriorityQueue(PriorityQueue<T> other)
    {
        elements = new T[other.elements.Length];
        Array.Copy(other.elements, elements, other.elements.Length);
        count = other.count;
        comparer = other.comparer;
    }
    // A method that adds an element to the queue
    /// <summary>
    /// Adds an element to the priority queue.
    /// </summary>
    /// <param name="element">The element to add.</param>
    public void Enqueue(T element)
    {
        // Check if the array is full, double its size if so
        if (count == elements.Length)
        {
            Array.Resize(ref elements, count * 2);
        }

        // Add the element to the end of the array
        elements[count] = element;

        // Bubble up the element to its correct position based on its priority
        int index = count;
        while (index > 0)
        {
            int parentIndex = (index - 1) >> 1;
            if (comparer(elements[index], elements[parentIndex]) > 0)
            {
                // Swap the element with its parent
                T temp = elements[index];
                elements[index] = elements[parentIndex];
                elements[parentIndex] = temp;

                // Update the index to the parent's index
                index = parentIndex;
            }
            else
            {
                // The element is in the right position, break the loop
                break;
            }
        }

        // Increment the count by one
        count++;
    }

    // A method that removes and returns the element with the highest priority from the queue
    /// <summary>
    /// Removes and returns the element with the highest priority from the priority queue.
    /// </summary>
    /// <returns>The element with the highest priority.</returns>
    /// <exception cref="System.InvalidOperationException">The PriorityQueue<T> is empty.</exception>
    public T Dequeue()
    {
        // Check if the queue is empty, throw an exception if so
        if (count == 0)
        {
            throw new InvalidOperationException("The queue is empty");
        }

        // Get the element at the root of the heap, which is the highest priority element
        T element = elements[0];

        // Decrement the count by one
        count--;

        // Move the last element to the root position
        elements[0] = elements[count];

        // Sink down the element at the root to its correct position based on its priority
        int index = 0;
        while (index < count)
        {
            int leftChildIndex = (index << 1) + 1;
            int rightChildIndex = (index << 1) + 2;

            // Find the child with the highest priority, if any
            int maxChildIndex = -1;
            if (leftChildIndex < count && rightChildIndex < count)
            {
                maxChildIndex = (comparer(elements[leftChildIndex], elements[rightChildIndex]) > 0) ? leftChildIndex : rightChildIndex;
            }
            else if (leftChildIndex < count)
            {
                maxChildIndex = leftChildIndex;
            }
            else if (rightChildIndex < count)
            {
                maxChildIndex = rightChildIndex;
            }

            // If there is a child with higher priority than the element, swap them
            if (maxChildIndex != -1 && comparer(elements[maxChildIndex], elements[index]) > 0)
            {
                // Swap elements using tuple assignment
                (elements[index], elements[maxChildIndex]) = (elements[maxChildIndex], elements[index]);

                // Update the index to the child's index
                index = maxChildIndex;
            }
            else
            {
                // The element is in the right position, break the loop
                break;
            }
        }

        // Return the highest priority element
        return element;
    }

    /// <summary>
    /// Checks if the priority queue contains a specific element or an element of a specific type.
    /// </summary>
    /// <typeparam name="ElementType">The type to check for in the priority queue.</typeparam>
    /// <param name="element">The element to check for in the priority queue.</param>
    /// <returns>True if the element or an element of type ElementType is found in the queue, false otherwise.</returns>
    public bool Contains<ElementType>(ElementType element = default) where ElementType : T
    {
        // Iterate over all elements in the queue
        for (int i = 0; i < count; i++)
        {
            // If the current element is equal to the input element or is of type ElementType
            if (elements[i].Equals(element))
            {
                // The element or an element of type ElementType is found in the queue, return true
                return true;
            }
        }
        // If the loop completes without finding the element or an element of type ElementType, return false
        return false;
    }
}