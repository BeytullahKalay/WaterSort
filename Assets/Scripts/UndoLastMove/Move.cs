using UnityEngine;

[System.Serializable]
class Move
{
    private BottleController _firstBottle;
    private BottleController _secondBottle;

    private int _numberOfTopColorLayer;

    private Color _color;

    public Move(BottleController first, BottleController second, int colorLayerAmount,Color color)
    {
        _firstBottle = first;
        _secondBottle = second;
        _numberOfTopColorLayer = colorLayerAmount;
        _color = color;
    }

    public void UndoMove()
    {
        _firstBottle.NumberOfColorsInBottle += _numberOfTopColorLayer;
        _secondBottle.NumberOfColorsInBottle -= _numberOfTopColorLayer;


        int firstStartIndex = _firstBottle.NumberOfColorsInBottle - _numberOfTopColorLayer;
        firstStartIndex = (int)Mathf.Clamp(firstStartIndex,0, Mathf.Infinity);
        
        for (int i = firstStartIndex; i < _firstBottle.NumberOfColorsInBottle; i++)
        {
            _firstBottle.BottleColors[i] = _color;
        }

        _firstBottle.UpdateAfterUndo();
        _secondBottle.UpdateAfterUndo();
    }
}
