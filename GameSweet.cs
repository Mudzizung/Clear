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

public class GameSweet : MonoBehaviour
{
    public bool isSpecial;
    private int x;
    public int X
    {
        get
        {
            return x;
        }
        
        set
        {
            if (CanMove())
            {
                x = value;
            }
        }
    }

    private int y;
    public int Y
    {
        get
        {
            return y;
        }

        set
        {
            if (CanMove())
            {
                y = value;
            }
        }
    }

    private GameManager.SweetType type;
    public GameManager.SweetType Type
    {
        get
        {
            return type;
        }
    }

    [HideInInspector]
    public GameManager gameManager;

    private ClearSweet clearComponent;
    public ClearSweet ClearComponent
    {
        get
        {
            return clearComponent;
        }

    }
    private MoveSweet moveComponent;
    public MoveSweet MoveComponent
    {
        get
        {
            return moveComponent;
        }
    }

    private ColorSweet colorComponent;
    public ColorSweet ColorComponent
    {
        get
        {
            return colorComponent;
        }
    }

   

    public bool CanMove()
    {
        return moveComponent != null;
    }


    public bool CanColor()
    {
        return colorComponent != null;
    }

    public bool CanClear()
    {
        return clearComponent != null;
    }
    private void Awake()
    {
        moveComponent = GetComponent<MoveSweet>();
        colorComponent = GetComponent<ColorSweet>();
        clearComponent = GetComponent<ClearSweet>();
    }




    public void Init(int _x,int _y,GameManager _gameManager, GameManager.SweetType _type)
    {
        x = _x;
        y = _y;
        gameManager = _gameManager;
        type = _type;
    }
    /// <summary>
    /// 鼠标检测
    /// </summary>
    private void OnMouseEnter()
    {
        gameManager.EnterSweet(this);
    }

    private void OnMouseDown()
    {
        gameManager.PressSweet(this);
       
    }

    private void OnMouseUp()
    {
        gameManager.ReleassSweet();
    }

}

