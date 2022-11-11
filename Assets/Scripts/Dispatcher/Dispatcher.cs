using System;
using System.Collections.Generic;

public class Dispatcher : IDispatcher
{
    private static Dispatcher instance;

    public static Dispatcher Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Dispatcher();
            }

            return instance;
        }
    }

    private Queue<Action> _pending = new Queue<Action>();

    // Schedule code for execution in the main-thread.
    public void Invoke(Action fn)
    {
        //pending.Add(fn);
        _pending.Enqueue(fn);
    }

    // Execute pending actions.
    public void InvokePending()
    {
        lock (_pending)
        {
            // foreach (var action in pending)
            // {
            //     action(); // Invoke the action.
            // } 

            while (_pending.Count > 0) _pending.Dequeue().Invoke();
            
        }

        _pending.Clear(); // Clear the pending list.
    }
}