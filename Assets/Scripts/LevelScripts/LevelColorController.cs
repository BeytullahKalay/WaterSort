using System;
using System.Collections.Generic;
using System.Threading;
using DataRepo;
using Solver;
using UnityEngine;
using Random = System.Random;

namespace LevelScripts
{
    public class LevelColorController : MonoBehaviour
    {
        [SerializeField] private Colors _colorsdb;
        public int NumberOfColorsToCreate = 2;
        public List<Color> SelectedColors = new List<Color>();


        private List<MyColors> _myColorsList = new List<MyColors>();


        public void SelectColorsToCreate(Data data)
        {
            while (SelectedColors.Count < NumberOfColorsToCreate)
            {
                var selectedColor = _colorsdb.GetRandomColor(data.GetBottleColorRandomIndex());
                if (!SelectedColors.Contains(selectedColor))
                    SelectedColors.Add(selectedColor);
            }
        }

        public void CreateColorObjects()
        {
            foreach (var color in SelectedColors)
            {
                MyColors colorObj = new MyColors(color);
                _myColorsList.Add(colorObj);
            }
        }

        public void GetRandomColorForBottle(Bottle tempBottle, bool matchState, bool rainbowBottle, Data data)
        {
            for (int j = 0; j < tempBottle.BottleColorsHashCodes.Length; j++)
            {
                var color = GetColorFromList(matchState, rainbowBottle, tempBottle, j - 1, data);
                tempBottle.BottleColorsHashCodes[j] = color.GetHashCode();
                tempBottle.BottleColors[j] = color;
            }

            InitializeBottleNumberedStack(tempBottle);
        }

        private Color GetColorFromList(bool matchState, bool rainbowBottle, Bottle tempBottle, int checkIndex,
            Data data)
        {
            if (_myColorsList.Count <= 0) return Color.black;

            var randomColorIndex = GetRandomColorIndex(data);
            var color = _myColorsList[randomColorIndex].Color;

            if (checkIndex >= 0)
            {
                if (rainbowBottle)
                {
                    var colorMatched = false;

                    var iteration = 0;

                    do
                    {
                        if (iteration > 200) break;

                        iteration++;

                        colorMatched = false;

                        if (_myColorsList.Count < 2) break;

                        for (int i = 0; i <= checkIndex; i++)
                        {
                            if (color.GetHashCode() != tempBottle.GetColorHashCodeAtPosition(i)) continue;

                            randomColorIndex = GetRandomColorIndex(data);
                            color = _myColorsList[randomColorIndex].Color;

                            colorMatched = true;
                            break;
                        }
                    } while (colorMatched);
                }
                else
                {
                    while (matchState && color.GetHashCode() == tempBottle.GetColorHashCodeAtPosition(checkIndex))
                    {
                        if (_myColorsList.Count < 2) break;

                        randomColorIndex = GetRandomColorIndex(data);
                        color = _myColorsList[randomColorIndex].Color;
                    }
                }
            }

            _myColorsList[randomColorIndex].Amount++;

            if (_myColorsList[randomColorIndex].MoreThan4())
                _myColorsList.RemoveAt(randomColorIndex);

            return color;
        }


        private void InitializeBottleNumberedStack(Bottle comingBottle)
        {
            foreach (var colorHashCode in comingBottle.BottleColorsHashCodes)
            {
                var emptyColorHashCode = 532676608;
                if (colorHashCode != emptyColorHashCode)
                    comingBottle.NumberedBottleStack.Push(colorHashCode);
            }

            comingBottle.CheckIsSorted();
            comingBottle.CalculateTopColorAmount();
        }

        private int GetRandomColorIndex(Data data)
        {
            var hashString = "GetRandomColor " + data.GetColorPickerRandomIndex().ToString();
            var rand = new Unity.Mathematics.Random((uint)hashString.GetHashCode());
            var randomColorIndex = rand.NextInt(0, _myColorsList.Count);

            return randomColorIndex;
        }
    }
}