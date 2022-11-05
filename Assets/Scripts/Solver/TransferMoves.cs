
public class TransferMoves
{
    private Bottle _from, _to;

    public TransferMoves(Bottle from, Bottle to)
    {
        _from = from;
        _to = to;
    }

    public bool CheckCanTransfer()
    {
        if (_from == _to) return false;

        if (_from.GetSorted() || _to.GetSorted()) return false;

        if (_from.NumberedBottleStack.Count <= 0) return false;

        if (_from.GetTopColorAmount() == _from.NumberedBottleStack.Count &&
            _to.NumberedBottleStack.Count == 0) return false;

        if (_to.NumberedBottleStack.Count + _from.GetTopColorAmount() > 4) return false;

        if (_from.GetTopColorID() != _to.GetTopColorID() && _to.NumberedBottleStack.Count > 0) return false;


        return true;
    }

    public void DoAction()
    {
        for (int i = 0; i < _from.GetTopColorAmount(); i++)
        {
            _to.NumberedBottleStack.Push(_from.NumberedBottleStack.Pop());
        }

        _to.CheckIsSorted();
        _from.CheckIsSorted();
    }

    public void UndoActions()
    {
        for (int i = 0; i < _to.GetTopColorAmount(); i++)
        {
            _from.NumberedBottleStack.Push(_to.NumberedBottleStack.Pop());
        }
        
        _to.CheckIsSorted();
        _from.CheckIsSorted();
    }
}

