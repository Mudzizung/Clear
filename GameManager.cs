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
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum SweetType//甜品的类型
    {
        EMPTY,//空的
        NORMAL,//正常的
        BARRIER,//障碍
        ROW_CLEAR,//行消除
        COLUMN_CLEAR,//列消除
        RAINBOWCANDY,//彩虹堂
        COUNT //标记类型
    }

    //甜品的预制体字典,可以方便查找
    public Dictionary<SweetType, GameObject> sweetPrefabDict;
    //定义结构体
    [System.Serializable]
    public struct SweetPrefab
    {
        public SweetType sweetType;
        public GameObject sweetPrefab;
    }

    public SweetPrefab[] sweetPrefabs;
    //单例
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }

        set
        {
            _instance = value;
        }
    }
    //网格行列
    public int xColumn;//行
    public int yRow;//列

    public GameObject gridPrefab;//格子
    //存储已经实例化的甜品的
    private GameSweet[,] sweets;


    public float time;

    //需要交换的两个甜品对象
    private GameSweet pressSweet;
    private GameSweet enterSweet;

    //UI Part
    public Text timeText;
    private float gameTime = 60;

    public Text score;
    //角色实际要加上去的分数
    public int playerScore;
    //当前显示的分数
    private int currentScore;
    //增加分数,慢慢加上去
    private float addScoreTime ;
    private bool gameOver = false;


    public GameObject gameOverPanel;
    //结束时候的分数
    public Text finalPanelScore;

    //最高纪录分数
    public Text record;
    private int maxScore;

    private void Awake()
    {
        _instance = this;
        gameOverPanel.SetActive(false);  //将结束面板影藏
        maxScore = PlayerPrefs.GetInt("max");
        record.text = PlayerPrefs.GetInt("max").ToString();
    }
    private void Update()
    {
        //if (gameOver)
        //{
        //    return;
        //}
        gameTime -= Time.deltaTime;
        if (gameTime <= 0)
        {
            gameTime = 0;
            //显示`结束界面
            gameOverPanel.SetActive(true);
            finalPanelScore.text = playerScore.ToString();
            //播放结束动画
            gameOver = true;
            return;
        }
        if (addScoreTime <=.1f)
        {
            addScoreTime += Time.deltaTime;
        }
        else
        {
            if (currentScore<playerScore)
            {
                currentScore++;
                score.text = currentScore.ToString();
                addScoreTime = 0;
            }
        }
        timeText.text = gameTime.ToString("00");
    }
    private void Start()
    {
        sweetPrefabDict = new Dictionary<SweetType, GameObject>();
        for (int i = 0; i < sweetPrefabs.Length; i++)
        {
            //如果字典当中没有包含预制体,将其添加进去
            if (!sweetPrefabDict.ContainsKey(sweetPrefabs[i].sweetType))
            {
                sweetPrefabDict.Add(sweetPrefabs[i].sweetType, sweetPrefabs[i].sweetPrefab);
            }
        }
        //界面生成grid存放sweet
        for (int x = 0; x < xColumn; x++)
        {
            for (int y = 0; y < yRow; y++)
            {
                GameObject chocolate = Instantiate(gridPrefab, CorrectPosition(x,y), Quaternion.identity);
                chocolate.transform.SetParent(transform);
            }
        }
        //初始化存放sweet的数组
        sweets = new GameSweet[xColumn, yRow];
        //
        for (int x = 0; x < xColumn; x++)
        {
            for (int y = 0; y < yRow; y++)
            {
                CreatNewSweet(x, y, SweetType.EMPTY);
            }
        }

        Destroy(sweets[4, 4].gameObject);
        CreatNewSweet(4, 4, SweetType.BARRIER);
        StartCoroutine(AllFill());
    }

    //回主界面
    public void ReturnToMain()
    {

    }

    //重新开始
    public void RePlay()
    {

    }
    //矫正坐标
    public Vector3 CorrectPosition(int x, int y)
    {
        //移动panel的位置,左上角变成(0,0)坐标了
        return new Vector3(transform.position.x - xColumn / 2f + x, transform.position.y + yRow / 2 - y);
    }

    //产生甜品的方法
    public GameSweet CreatNewSweet(int x, int y,SweetType type)
    {
        GameObject newSweet= Instantiate(sweetPrefabDict[type], CorrectPosition(x, y), Quaternion.identity);
        //设置父节点
        newSweet.transform.parent = transform;
        //将其添加进存储的数组里面
        sweets[x, y] = newSweet.GetComponent<GameSweet>();
        //初始化
        sweets[x, y].Init(x, y, this, type);
        return sweets[x, y];
    }


    //全部填充的方法
    public IEnumerator AllFill()
    {

        bool needReFill = true;
        while (needReFill)
        {
            yield return new WaitForSeconds(time);
            while (Fill())
            {
                yield return new WaitForSeconds(time);
            }
            //清除删除列表的元素
            needReFill = ClearAllMatchSweet();
        }

    }

    //分步填充的方法
    public bool Fill()
    {
        bool filledNotFinished = false;////判断本次填充是否完成
        //最后一行不遍历,因为它不需要往下填充
        for (int y = yRow - 2; y >= 0; y--)
        {
            for (int x = 0; x < xColumn; x++)
            {
                //取到当前元素
                GameSweet sweet = sweets[x, y];
                //如果无法移动则无法向下填充
                if (sweet.CanMove())
                {
                   
                    //获取到当前元素下方的元素
                    GameSweet sweetBelow = sweets[x, y + 1];
                    //如果下方元素为空,上方元素向下填充
                    if (sweetBelow.Type == SweetType.EMPTY)
                    {
                        //垂直填充
                        sweet.MoveComponent.Move(x, y + 1, time);
                        //将其保存在数组当中
                        sweets[x, y + 1] = sweet;
                        //将当前元素的位置滞空
                        CreatNewSweet(x, y, SweetType.EMPTY);
                        filledNotFinished = true;
                    }
                    else
                    {
                        //斜向填充
                        for (int down = -1; down <= 1; down++)
                        {
                            if (down != 0)
                            {
                                int downX = x + down;
                                if (downX > 0 && downX < xColumn)
                                {
                                    GameSweet downSweet = sweets[downX, y + 1];
                                    if (downSweet.Type == SweetType.EMPTY)
                                    {
                                        bool canFill = true;
                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GameSweet sweetAbove = sweets[downX, aboveY];
                                            if (sweetAbove.CanMove())
                                            {
                                                break;
                                            }
                                            else if (!sweetAbove.CanMove() && sweetAbove.Type != SweetType.EMPTY)
                                            {
                                                canFill = false;
                                                break;
                                            }
                                        }
                                        if (!canFill)
                                        {
                                            Destroy(downSweet.gameObject);
                                            sweet.MoveComponent.Move(downX, y + 1, time);
                                            sweets[downX, y + 1] = sweet;
                                            CreatNewSweet(x, y, SweetType.EMPTY);
                                            filledNotFinished = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //最上层的特殊情况
        for (int x = 0; x < xColumn; x++)
        {
            //获取到最上层元素对象
            GameSweet sweet = sweets[x, 0];
            //如果当前的元素为空
            if (sweet.Type == SweetType.EMPTY)
            {
                GameObject newSweet= Instantiate(sweetPrefabDict[SweetType.NORMAL], CorrectPosition(x, -1), Quaternion.identity);
                newSweet.transform.SetParent(transform);
                sweets[x, 0] = newSweet.GetComponent<GameSweet>();
                sweets[x, 0].Init(x, -1, this, SweetType.NORMAL);
                sweets[x, 0].MoveComponent.Move(x, 0,time);
                sweets[x, 0].ColorComponent.SetColor((ColorSweet.ColorType)Random.Range(0, sweets[x, 0].ColorComponent.GetNumColor));
                filledNotFinished = true;
            }
        }
        return filledNotFinished;
    }

    //判断是否是相邻元素
    private bool IsFrend(GameSweet sweet1,GameSweet sweet2)
    {
        return (sweet1.X == sweet2.X && Mathf.Abs(sweet1.Y - sweet2.Y) == 1)
            || (sweet1.Y == sweet2.Y && Mathf.Abs(sweet1.X - sweet2.X) == 1);
    }

    //交换甜品的方法

    private void ExchangeSweets(GameSweet sweet1, GameSweet sweet2)
    {
        if (sweet1.CanMove() && sweet2.CanMove())
        {
            sweets[sweet1.X, sweet1.Y] = sweet2;
            sweets[sweet2.X, sweet2.Y] = sweet1;
            //有返回的删除列表
            if (MatchSweets(sweet1, sweet2.X, sweet2.Y) != null || MatchSweets(sweet2, sweet1.X, sweet1.Y) != null)
            {
                //先交换两个元素
                int tempX = sweet1.X;
                int tempY = sweet1.Y;

                sweet1.MoveComponent.Move(sweet2.X, sweet2.Y, time);
                sweet2.MoveComponent.Move(tempX, tempY, time);
                //清除
                ClearAllMatchSweet();
                //填充
                StartCoroutine(AllFill());
            }
            else
            {
                //不做处理
                sweets[sweet1.X, sweet1.Y] = sweet1;
                sweets[sweet2.X, sweet2.Y] = sweet2;
            }


        }
    }

    public void PressSweet(GameSweet sweet)
    {
        if (gameOver)
            return;
        pressSweet = sweet;
        if (sweet.isSpecial == true)
        {
            sweet.ClearComponent.Clear();
            CreatNewSweet(sweet.X, sweet.Y, SweetType.EMPTY);   
            StartCoroutine(AllFill());
        }
    }

    public void EnterSweet(GameSweet sweet)
    {
        if (gameOver)
            return;
        enterSweet = sweet;
    }

    public void ReleassSweet()
    {
         if (gameOver)
            return;
        if (IsFrend(pressSweet, enterSweet))
        {
            ExchangeSweets(pressSweet, enterSweet);
        }
    }


    //匹配方法
    //参数
    public List<GameSweet> MatchSweets(GameSweet sweet,int newX,int newY)
    {
       
        ColorSweet.ColorType type = sweet.ColorComponent.Color;//获取到当前元素的颜色

        List<GameSweet> matchRowSweet = new List<GameSweet>();//横向匹配列表
        List<GameSweet> matchLineSweet = new List<GameSweet>();//纵向匹配列表
        List<GameSweet> finishMatchSweet = new List<GameSweet>();//删除列表

        if (sweet.X > newX) //这种情况水平方向只需要向左遍历以及向上下遍历
        {
            for (int i = 1; i < xColumn; i++)
            {
                int x = newX - i;  //向左前进
                if (x < 0)   //防止数组越界
                {
                    break;
                }
                //判断交换元素左边
                //如果可以换色并且其颜色和标准一样
                if (sweets[x, newY].CanColor() && sweets[x, newY].ColorComponent.Color == type)
                {
                    //将其添加进横向列表当中
                    matchRowSweet.Add(sweets[x, newY]);
                }
                else
                {
                    break;
                }
            }

            //上下遍历
            for (int i = 1; i < yRow; i++)
            {
                //向y轴正方向遍历
                int y = newY + i;
                if (y >= yRow)
                {
                    break;
                }

                if (sweets[newX,y].CanColor() && sweets[newX,y].ColorComponent.Color == type)
                {
                    //将其添加进横向列表当中
                    matchLineSweet.Add(sweets[newX, y]);
                }
                else
                {
                    break;
                }
            }

            //
            for (int i = 1; i < yRow; i++)
            {
                //向y轴负方向遍历
                int y = newY - i;
                if (y < 0)
                {
                    break;
                }

                if (sweets[newX, y].CanColor() && sweets[newX, y].ColorComponent.Color == type)
                {
                    //将其添加进横向列表当中
                    matchLineSweet.Add(sweets[newX, y]);
                }
                else
                {
                    break;
                }
            }




        }
        else//这种情况水平方向只需要向右遍历以及向上下遍历
        {
            for (int i = 1; i < xColumn; i++)
            {
                int x = newX + i;  //向右前进
                if (x >=xColumn)   //防止数组越界
                {
                    break;
                }
                //判断交换元素
                //如果可以换色并且其颜色和标准一样
                if (sweets[x, newY].CanColor() && sweets[x, newY].ColorComponent.Color == type)
                {
                    //将其添加进横向列表当中
                    matchRowSweet.Add(sweets[x, newY]);
                }
                else
                {
                    break;
                }
            }

            //上下遍历
            for (int i = 1; i < yRow; i++)
            {
                //向y轴正方向遍历
                int upY = newY + i;
                if (upY >= yRow)
                {
                    break;
                }

                if (sweets[newX, upY].CanColor() && sweets[newX, upY].ColorComponent.Color == type)
                {
                    //将其添加进横向列表当中
                    matchLineSweet.Add(sweets[newX, upY]);
                }
                else
                {
                    break;
                }
            }

            //
            for (int i = 1; i < yRow; i++)
            {
                //向y轴负方向遍历
                int upY = newY - i;
                if (upY < 0)
                {
                    break;
                }

                if (sweets[newX, upY].CanColor() && sweets[newX, upY].ColorComponent.Color == type)
                {
                    //将其添加进横向列表当中
                    matchLineSweet.Add(sweets[newX, upY]);
                }
                else
                {
                    break;
                }
            }

        }

        //当元素在上方时

        if (sweet.Y > newY) //这种情况水平方向需要向下遍历以及左右遍历
        {
            for (int i = 1; i < yRow; i++)
            {
                int y = newY - i;  //向下方前进
                if (y < 0)   //防止数组越界
                {
                    break;
                }
                //判断交换元素左边
                //如果可以换色并且其颜色和标准一样
                if (sweets[newX, y].CanColor() && sweets[newX, y].ColorComponent.Color == type)
                {
                    //将其添加进横向列表当中
                    matchLineSweet.Add(sweets[newX, y]);
                }
                else
                {
                    break;
                }
            }
            //向左右遍历

            for (int i = 1; i < xColumn; i++)
            {
                //向x轴正方向遍历
                int x = newX + i;
                if (x >= xColumn)
                {
                    break;
                }

                if (sweets[x, newY].CanColor() && sweets[x, newY].ColorComponent.Color == type)
                {
                    //将其添加进横向列表当中
                    matchRowSweet.Add(sweets[x, newY]);
                }
                else
                {
                    break;
                }
            }

            //
            for (int i = 1; i < xColumn; i++)
            {
                //向y轴负方向遍历
                int x = newX - i;
                if (x < 0)
                {
                    break;
                }

                if (sweets[x, newY].CanColor() && sweets[x, newY].ColorComponent.Color == type)
                {
                    //将其添加进横向列表当中
                    matchRowSweet.Add(sweets[x, newY]);
                }
                else
                {
                    break;
                }
            }

        }
        else//
        {
            for (int i = 1; i < yRow; i++)
            {
                int y = newY + i;  //向右前进
                if (y >= yRow)   //防止数组越界
                {
                    break;
                }
                //判断交换元素
                //如果可以换色并且其颜色和标准一样
                if (sweets[newX, y].CanColor() && sweets[newX, y].ColorComponent.Color == type)
                {
                    //将其添加进横向列表当中
                    matchRowSweet.Add(sweets[newX, y]);
                }
                else
                {
                    break;
                }
            }


            for (int i = 1; i < xColumn; i++)
            {
                //向x轴付方向遍历
                int x = newX - i;
                if (x <0)
                {
                    break;
                }

                if (sweets[x, newY].CanColor() && sweets[x, newY].ColorComponent.Color == type)
                {
                    //将其添加进横向列表当中
                    matchLineSweet.Add(sweets[x, newY]);
                }
                else
                {
                    break;
                }
            }

            //
            for (int i = 1; i < xColumn; i++)
            {
                //向y轴正方向遍历
                int x = newX + i;
                if (x >= xColumn)
                {
                    break;
                }

                if (sweets[x, newY].CanColor() && sweets[x, newY].ColorComponent.Color == type)
                {
                    //将其添加进横向列表当中
                    matchLineSweet.Add(sweets[x, newY]);
                }
                else
                {
                    break;
                }
            }
        }


        if (matchRowSweet.Count >= 2)//判断如果有一边连续两个
        {
            for (int i = 0; i < matchRowSweet.Count; i++)
            {
                finishMatchSweet.Add(matchRowSweet[i]);//加入删除列表
            }
            if (!finishMatchSweet.Contains(sweet))
            {
                finishMatchSweet.Add(sweet);//在将本元素添加进删除列表
            }
            if (finishMatchSweet.Count >= 3)
            {
                return finishMatchSweet;
            }
        }
        else if (matchLineSweet.Count >= 2)//
        {
            for (int i = 0; i < matchLineSweet.Count; i++)
            {
                  finishMatchSweet.Add(matchLineSweet[i]);//加入删除列表
            }
            if (!finishMatchSweet.Contains(sweet))
            {
                finishMatchSweet.Add(sweet);//在将本元素添加进删除列表
            }
            if (finishMatchSweet.Count >= 3)
            {
                return finishMatchSweet;
            }
        }
        matchLineSweet.Clear();
        matchRowSweet.Clear();
        return null;
    }

    //清除方法
    public bool ClearSweet(int x, int y)
    {
        if (sweets[x, y].CanClear()&&!sweets[x,y].ClearComponent.IsClearing&&sweets[x,y].isSpecial==false)
        {
            sweets[x, y].ClearComponent.Clear();//删除元素
            CreatNewSweet(x, y, SweetType.EMPTY);//将格子滞空
            return true;
        }
        return false;
    }
    //清除完成匹配的元素
    public bool ClearAllMatchSweet()
    {
        bool needReFill = false;
        for (int y = 0; y < yRow; y++)
        {
            for (int x = 0; x < xColumn; x++)
            {
                if (sweets[x, y].CanClear())
                {
                    // 返回的需要删除的元素列表
                    List<GameSweet> matchList= MatchSweets(sweets[x, y], x, y);
                    if (matchList != null)
                    {
                       // Debug.Log("消除列表长度="+matchList.Count);
                        SweetType specialSweetType = SweetType.COUNT;
                        //在随机一个消除位置生成一个行消除或者列消除
                        GameSweet birthSpecialSweet = matchList[Random.Range(0, matchList.Count)];
                        int specialX = birthSpecialSweet.X;
                        int specialY = birthSpecialSweet.Y;
                        if (matchList.Count == 4)
                        {
                           // Debug.Log("开始产生特殊物品!!!!!!!!!!!!!!!");
                            //随机一个行消除或者列消除
                            specialSweetType = (SweetType)Random.Range((int)SweetType.ROW_CLEAR, (int)SweetType.RAINBOWCANDY);
                        }

                        for (int i = 0; i < matchList.Count; i++)
                        {
                            //if (matchList[i] == null)
                            //    continue;
                            if (ClearSweet(matchList[i].X, matchList[i].Y))
                            {
                                needReFill = true;
                            }
                        }

                        if (specialSweetType != SweetType.COUNT)
                        {
                            //Debug.Log("开始产生特殊物品");
                            Destroy(sweets[specialX, specialY]);
                            GameSweet newSweet = CreatNewSweet(specialX, specialY, specialSweetType);
                            if (specialSweetType == SweetType.ROW_CLEAR || specialSweetType == SweetType.COLUMN_CLEAR && newSweet.CanMove() && matchList[0].CanColor())
                            {
                                //newSweet.ColorComponent.SetColor(matchList[0].ColorComponent.Color);
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }
        }
        return needReFill;
    }


    //行消除
    public void ClearRowSweet(int row)
    {
        for (int x = 0; x < xColumn; x++)
        {
            ClearSweet( x,row);
        }
    }
    //列消除
    public void ClearCloumSweet(int colum)
    {
        for (int y = 0; y < yRow; y++)
        {
            ClearSweet(colum, y);
        }
    }
    public void ReStart()
    {
        if(maxScore<= playerScore)
            PlayerPrefs.SetInt("max", playerScore);
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        if (maxScore <= playerScore)
            PlayerPrefs.SetInt("max", playerScore);
        Application.Quit();
    }
}

