using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

public class TransferMoves
{
    private Bottle _from, _to;
    
    string path = @"D:\UnitySaves\WaterSort - Copy\Solves\Logs.txt";

    private int _lastTransferAmount;

    public TransferMoves(Bottle from, Bottle to)
    {
        _from = from;
        _to = to;

        OpenTXT();
    }

    private void OpenTXT()
    {
        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("----------- START -----------");
            }
        }
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

        _lastTransferAmount = _from.GetTopColorAmount();

        WriteDoActionsToTxtFile(_from, _to,false);

        _to.CheckIsSorted();
        _to.CalculateTopColorAmount();

        _from.CheckIsSorted();
        _from.CalculateTopColorAmount();
    }

    public void UndoActions()
    {
        for (int i = 0; i < _lastTransferAmount; i++)
        {
            if (_to.NumberedBottleStack.Count > 0)
            {
                _from.NumberedBottleStack.Push(_to.NumberedBottleStack.Pop());
            }
        }

        WriteDoActionsToTxtFile(_to, _from,true);

        _to.CheckIsSorted();
        _to.CalculateTopColorAmount();

        _from.CheckIsSorted();
        _from.CalculateTopColorAmount();
    }

    private void WriteDoActionsToTxtFile(Bottle from, Bottle to,bool isUndo)
    {
        //path = AssetDatabase.GenerateUniqueAssetPath(path);
        if (isUndo)
        {
            List<string> lines = File.ReadAllLines(path).ToList();
            File.WriteAllLines(path, lines.GetRange(0, lines.Count - 1).ToArray());
        }
        else
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(from.GetBottleIndex() + " --> " + to.GetBottleIndex());
            }
        }
    }
}