using UnityEngine;

[System.Serializable]
class Move
{
    private BottleController _firstBottle;
    private BottleController _secondBottle;

    private int _transferColorAmount;

    private Color _color;

    public Move(BottleController first, BottleController second, int colorAmountAmount,Color color)
    {
        _firstBottle = first;
        _secondBottle = second;
        _transferColorAmount = colorAmountAmount;
        _color = color;
    }

    public void UndoMove()
    {
        _firstBottle.NumberOfColorsInBottle += _transferColorAmount;
        _secondBottle.NumberOfColorsInBottle -= _transferColorAmount;


        int firstStartIndex = _firstBottle.NumberOfColorsInBottle - _transferColorAmount;
        firstStartIndex = (int)Mathf.Clamp(firstStartIndex,0, Mathf.Infinity);
        
        for (int i = firstStartIndex; i < _firstBottle.NumberOfColorsInBottle; i++)
        {
            _firstBottle.BottleColors[i] = _color;
        }

        _firstBottle.UpdateAfterUndo();
        _secondBottle.UpdateAfterUndo();
    }
}
