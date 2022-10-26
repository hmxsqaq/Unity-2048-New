using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Controller : MonoBehaviour
{
    public GameObject[] GridPrefabs;
    public Button RestartButton;
    public GameObject GameOverIMG;
    
    private List<MyGrid> GridList = new List<MyGrid>();
    private bool pause = false;

    private Action GameOver;
    private void Awake()
    {
        for (int i = 0; i < 16; i++)
        {
            GridList.Add(new MyGrid(0,i/4,i%4));
        }
    }

    private void Start()
    {
        GameInit();
        RestartButton.onClick.AddListener((() =>
        {
            GameInit();
        }));

        GameOver += () =>
        {
            GameOverIMG.SetActive(true);
            pause = true;
        };
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveRight();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveUp();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveDown();
        }
        GraphUptade();
    }

    private void GameInit()
    {
        pause = false;
        GameOverIMG.SetActive(false);
        for (int i = 0; i < 16; i++)
        {
            GridList[i].Number = 0;
        }
        RandomGenerate();
        RandomGenerate();
    }
    
    private void GraphUptade()
    {
        if(!pause)
        {
            bool noempty = true;
            bool noequal = true;
            foreach (MyGrid _grid in GridList)
            {
                if (_grid.GameObject is null && _grid.Number != 0)
                {
                    _grid.GameObject = Instantiate(GridPrefabs[_grid.NumberIndex],
                        new Vector3(Data.XPos[_grid.Column], Data.YPos[_grid.Row]),
                        Quaternion.identity);
                }
                else if (!(_grid.GameObject is null) && _grid.Number == 0)
                {
                    GridDestory(_grid);
                }
                else if (!(_grid.GameObject is null) && _grid.Number != 0)
                {
                    GridDestory(_grid);
                    _grid.GameObject = Instantiate(GridPrefabs[_grid.NumberIndex],
                        new Vector3(Data.XPos[_grid.Column], Data.YPos[_grid.Row]),
                        Quaternion.identity);
                }

                if (_grid.Number > 131000)
                {
                    GameOver();
                }

                if (noempty && _grid.Number == 0)
                    noempty = false;
                if (noequal)
                {
                    if (_grid.Row <= 2 && _grid.Column <= 2)
                    {
                        if (_grid.Number == GridList[_grid.Index + 1].Number ||
                            _grid.Number == GridList[_grid.Index + 4].Number)
                            noequal = false;
                    }
                    else if (_grid.Index == 15) { }
                    else if (_grid.Row == 3)
                    {
                        if (_grid.Number == GridList[_grid.Index + 1].Number)
                            noequal = false;
                    }
                    else if (_grid.Column == 3)
                    {
                        if (_grid.Number == GridList[_grid.Index + 4].Number)
                            noequal = false;
                    }
                }
            }
            if (noempty && noequal)
                GameOver();
        }
    }

    private void RandomGenerate()
    {
        List<int> existIndex = new List<int>();
        List<int> emptyIndex = new List<int>();
        
        foreach (MyGrid _grid in GridList)
        {
            if(_grid.Number != 0)
                existIndex.Add(_grid.Index);
        }
        for (int i = 0; i < 16; i++)
        {
            if (!existIndex.Contains(i)) 
                emptyIndex.Add(i);
        }
        
        int index = emptyIndex[Random.Range(0, emptyIndex.Count)];
        int number = Random.Range(0, 5) == 0 ? 4 : 2;
        GridList[index].Number = number;
    }

    private void GridDestory(MyGrid myGrid)
    {
        Destroy(myGrid.GameObject);
        myGrid.GameObject = null;
    }
    
    private enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    private bool Merge(int index, bool qualification, Direction direction)
    {
        if (qualification)
        {
            switch (direction)
            {
                case Direction.Left:
                    if (GridList[index].Number == GridList[index-1].Number)
                    {
                        GridList[index - 1].Number *= 2;
                        GridList[index - 1].IsMerge = true;
                        GridList[index].Number = 0;
                        return true;
                    }
                    break;
                case Direction.Right:
                    if (GridList[index].Number == GridList[index+1].Number)
                    {
                        GridList[index + 1].Number *= 2;
                        GridList[index + 1].IsMerge = true;
                        GridList[index].Number = 0;
                        return true;
                    }
                    break;
                case Direction.Up:
                    if (GridList[index].Number == GridList[index-4].Number)
                    {
                        GridList[index - 4].Number *= 2;
                        GridList[index - 4].IsMerge = true;
                        GridList[index].Number = 0;
                        return true;
                    }
                    break;
                case Direction.Down:
                    if (GridList[index].Number == GridList[index+4].Number)
                    {
                        GridList[index + 4].Number *= 2;
                        GridList[index + 4].IsMerge = true;
                        GridList[index].Number = 0;
                        return true;
                    }
                    break;
            }
        }
        return false;
    }
    
    private void MoveLeft()
    {
        bool isMove = false;
        bool isMerge = false;
        foreach (MyGrid myGrid in GridList)
        {
            if (!(myGrid.Number == 0 || myGrid.Column == 0))
            {
                int targetIndex = myGrid.Index;
                for (int i = myGrid.Index-1; i >= myGrid.Row * 4 ; i--)
                {
                    if (GridList[i].Number == 0)
                    {
                        targetIndex = i;
                        isMove = true;
                    }
                }
                (myGrid.Number, GridList[targetIndex].Number) = (GridList[targetIndex].Number, myGrid.Number);
                if(GridList[targetIndex].Column != 0)
                {
                    if (Merge(targetIndex, !GridList[targetIndex - 1].IsMerge, Direction.Left))
                        isMerge = true;
                }
            }
        }
        if (isMerge || isMove)
            RandomGenerate();
        foreach (MyGrid myGrid in GridList)
            myGrid.IsMerge = false;
    }

    private void MoveRight()
    {
        bool isMove = false;
        bool isMerge = false;
        for(int index = 15;index >= 0;index--)
        {
            MyGrid myGrid = GridList[index];
            if (!(myGrid.Number == 0 || myGrid.Column == 3))
            {
                int targetIndex = myGrid.Index;
                for (int i = myGrid.Index+1; i <= myGrid.Row * 4+3 ; i++)
                {
                    if (GridList[i].Number == 0)
                    {
                        targetIndex = i;
                        isMove = true;
                    }
                }
                (myGrid.Number, GridList[targetIndex].Number) = (GridList[targetIndex].Number, myGrid.Number);
                if(GridList[targetIndex].Column != 3)
                {
                    if (Merge(targetIndex, !GridList[targetIndex + 1].IsMerge, Direction.Right))
                        isMerge = true;
                }
            }
        }
        if (isMerge || isMove)
            RandomGenerate();
        foreach (MyGrid myGrid in GridList)
            myGrid.IsMerge = false;
    }

    private void MoveUp()
    {
        bool isMove = false;
        bool isMerge = false;
        foreach (MyGrid myGrid in GridList)
        {
            if (!(myGrid.Number == 0 || myGrid.Row == 0))
            {
                int targetIndex = myGrid.Index;
                for (int i = myGrid.Index-4; i >= myGrid.Column ; i-=4)
                {
                    if (GridList[i].Number == 0)
                    {
                        targetIndex = i;
                        isMove = true;
                    }
                }
                (myGrid.Number, GridList[targetIndex].Number) = (GridList[targetIndex].Number, myGrid.Number);
                if(GridList[targetIndex].Row != 0)
                {
                    if (Merge(targetIndex, !GridList[targetIndex - 4].IsMerge, Direction.Up))
                        isMerge = true;
                }
            }
        }
        if (isMerge || isMove)
            RandomGenerate();
        foreach (MyGrid myGrid in GridList)
            myGrid.IsMerge = false;
    }

    private void MoveDown()
    {
        bool isMove = false;
        bool isMerge = false;
        for(int index = 15;index >= 0;index--)
        {
            MyGrid myGrid = GridList[index];
            if (!(myGrid.Number == 0 || myGrid.Row == 3))
            {
                int targetIndex = myGrid.Index;
                for (int i = myGrid.Index+4; i <= myGrid.Column+12 ; i+=4)
                {
                    if (GridList[i].Number == 0)
                    {
                        targetIndex = i;
                        isMove = true;
                    }
                }
                (myGrid.Number, GridList[targetIndex].Number) = (GridList[targetIndex].Number, myGrid.Number);
                if(GridList[targetIndex].Row != 3)
                {
                    if (Merge(targetIndex, !GridList[targetIndex + 4].IsMerge, Direction.Down))
                        isMerge = true;
                }
            }
        }
        if (isMerge || isMove)
            RandomGenerate();
        foreach (MyGrid myGrid in GridList)
            myGrid.IsMerge = false;
    }
}