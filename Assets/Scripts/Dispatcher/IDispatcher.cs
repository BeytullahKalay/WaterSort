
using System;

public interface IDispatcher
{
    void Invoke(Action fn);
}
