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

public class ColorSweet : MonoBehaviour
{
    //一种颜色对应一种精灵
    public enum ColorType
    {
        YELLOW,
        PURPLE,
        RED,
        BLUE,
        GREEN,
        PINK,
        ANY,
        COUNT,
    }
    //
    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;
    }

    public ColorSprite[] colorSprite;
    //根据颜色找到精灵的字典

    private Dictionary<ColorType, Sprite> colorSpriteDict;

    private SpriteRenderer spriteRenderer;

    public int GetNumColor
    {
        get { return colorSprite.Length; }
    }

    private ColorType color;
    public ColorType Color
    {
        get
        {
            return color;
        }

        set
        {
            SetColor(value);
        }
    }


    private void Awake()
    {
        //Debug.Log("colorType_3="+(ColorType)3);
        spriteRenderer = transform.Find("Sweet").GetComponent<SpriteRenderer>();
        colorSpriteDict = new Dictionary<ColorType, Sprite>();

        for (int i = 0; i < colorSprite.Length; i++)
        {
            if (!colorSpriteDict.ContainsKey(colorSprite[i].color))
            {
                colorSpriteDict.Add(colorSprite[i].color, colorSprite[i].sprite);
            }
        }
    }

    public void SetColor(ColorType newColor)
    {
        color = newColor;
        if (colorSpriteDict.ContainsKey(newColor))
        {
            spriteRenderer.sprite = colorSpriteDict[newColor];
        }
    }
}

