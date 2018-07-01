/***
  *Title:""��Ŀ
  *Description:
  *		����:
  *Author:D
  *Data:2018.03.18
  *
  *
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialClear : ClearSweet
{
    
    public bool isRow;

    public  override void Clear()
    {
        base.Clear();
        if (isRow)
        {
            sweet.gameManager.ClearRowSweet(sweet.Y);
        }
        else
        {
            sweet.gameManager.ClearCloumSweet(sweet.X);
        }
    }
}

