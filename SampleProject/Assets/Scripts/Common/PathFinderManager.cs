using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PathDirection
{
    PathDirection_Left,
    PathDirection_Right,
    PathDirection_Up,
    PathDirection_Down,
    PathDirection_LeftUp,
    PathDirection_RightDown,
    PathDirection_LeftDown,
    PathDirection_RightUp
};

enum TileState
{
    TileState_None,
    TileState_Open,
    TileState_Close,
    TileState_Path
};

public class PathFinderManager : MonoBehaviour
{
    private static PathFinderManager m_instance;

    //싱글톤 접근
    public static PathFinderManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<PathFinderManager>();
            }
            return m_instance;
        }
    }

    //현재 사이즈
    public int sizeX, sizeY;
    //현재 검사타일 위치
    public int curX, curY;

    public Tile startTile, targetTile;
    public List<Tile> FinalTileList;
    public List<Tile> OpenList, ClosedList;
    public List<Tile> TileList;
    public bool[] tempBlock;

    private void Awake()
    {
        Init(45, 18);
    }

    public void Init(int x, int y)
    {
        FinalTileList.Clear();
        OpenList.Clear();
        ClosedList.Clear();
        TileList.Clear();

        //크기를 정해주고 값을 대입한다
        sizeX = x + 1;
        sizeY = y + 1;

        tempBlock = new bool[9];

        for (int i = 0; i < 9; i++)
        {
            tempBlock[i] = false;
        }

        for (int i = 0; i < sizeY; i++)
        {
            for (int j = 0; j < sizeX; j++)
            {
                bool isWall = false;

                foreach (Collider2D col in Physics2D.OverlapBoxAll(new Vector2(j, i), new Vector2(1, 1), 0.0f))
                {
                    if (col.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                    {
                        isWall = true;
                    }
                }

                TileList.Add(new Tile(isWall, i, j));
            }
        }

    }

    public List<Tile> PathFinding(Vector2 start, Vector2 end)
    {
        Tile curTile;
        bool isFind = false, noPath = false;
        // left, right, up, down, leftup, rightdown, leftdown, rightup
        int[] dx = { -1, 1, 0, 0, -1, 1, -1, 1 };
        int[] dy = { 0, 0, -1, 1, -1, 1, 1, -1 };

        //에이스타 알고리즘을 이용해 최소 도착 리스트를 검출합니다
        startTile = TileList[(int)start.x + ((int)start.y * sizeX)];
        targetTile = TileList[(int)end.x + ((int)end.y * sizeX)]; ;

        OpenList.Add(startTile);

        while (OpenList.Count > 0 || !isFind)
        {
            curTile = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
            {
                if (OpenList[i].F <= curTile.F && OpenList[i].h < curTile.h)
                    curTile = OpenList[i];
            }

            OpenList.Remove(curTile);
            ClosedList.Add(curTile);


            //8방향으로 이동 가능한 지역을 검출합니다.
            for (int i = 0; i < 8; i++)
            {
                int x = curX + dx[i];
                int y = curY + dy[i];
                tempBlock[i] = false;

                // 해당 방향으로 움직인 타일이 유효한 타일인지 확인
                if (0 <= x && x < sizeX && 0 <= y && y < sizeY)
                {
                    bool isOpen;
                    // 대각선 타일의 이동 문제로 (주변에 블락있으면 못감) 임시로 블락 상태 저장
                    if (TileList[y * sizeX + x].isWall) tempBlock[i] = true;
                    else
                    {
                        // check closeList z
                        bool isClose = false;
                        for (int j = 0; j < ClosedList.Count; j++)
                        {
                            if (ClosedList[j].y == y && ClosedList[j].x == x)
                            {
                                isClose = true;
                                break;
                            }
                        }
                        if (isClose) continue;

                        if (i < 4)
                        {
                            TileList[y * sizeX + x].g = 10;
                        }
                        else
                        {
                            // leftup인 경우 left나 up에 블락있으면 안됨
                            if (i == (int)PathDirection.PathDirection_LeftUp &&
                                tempBlock[(int)PathDirection.PathDirection_Left] || tempBlock[(int)PathDirection.PathDirection_Up]) continue;
                            // rightdown인 경우 right나 down에 블락있으면 안됨
                            if (i == (int)PathDirection.PathDirection_RightDown &&
                                tempBlock[(int)PathDirection.PathDirection_Right] || tempBlock[(int)PathDirection.PathDirection_Down]) continue;
                            // rightup인 경우 right나 up에 블락있으면 안됨
                            if (i == (int)PathDirection.PathDirection_RightUp &&
                                tempBlock[(int)PathDirection.PathDirection_Right] || tempBlock[(int)PathDirection.PathDirection_Up]) continue;
                            // leftdown인 경우 left나 down에 블락있으면 안됨
                            if (i == (int)PathDirection.PathDirection_LeftDown &&
                                tempBlock[(int)PathDirection.PathDirection_Left] || tempBlock[(int)PathDirection.PathDirection_Down]) continue;
                            TileList[y * sizeX + x].g = 14;

                        }
                        //abs절대값

                        TileList[y * sizeX + x].h = (int)(Mathf.Abs(end.x - x) + Mathf.Abs(end.y - y)) * 10;

                        // 오픈리스트에 있으면 g 비용 비교 후 처리
                        isOpen = false;
                        for (int j = 0; j < OpenList.Count; j++)
                        {
                            if (OpenList[j].y == y && OpenList[j].x == x)
                            {
                                isOpen = true;
                                if (TileList[OpenList[j].y * sizeX + OpenList[j].x].g > TileList[y * sizeX + x].g)
                                {
                                    TileList[OpenList[j].y * sizeX + OpenList[j].x].h = TileList[y * sizeX + x].h;
                                    TileList[OpenList[j].y * sizeX + OpenList[j].x].g = TileList[y * sizeX + x].g;
                                    TileList[OpenList[j].y * sizeX + OpenList[j].x].ParentTile = curTile;
                                }
                            }
                        }
                        // 없으면 그냥 넣고 부모 설정
                        if (!isOpen)
                        {
                            OpenList.Add(curTile);
                            TileList[y * sizeX + x].ParentTile = curTile;
                        }
                        

                        //마지막 노드로 검출을 완료 했을때
                        if (curTile == targetTile)
                        {
                            isFind = true;

                            Tile tileTemp = targetTile;
                            while (tileTemp != startTile)
                            {
                                FinalTileList.Add(tileTemp);
                                tileTemp = tileTemp.ParentTile;
                            }
                            FinalTileList.Add(startTile);
                            FinalTileList.Reverse();

                            return FinalTileList;
                        }

                    }
                }
            }
            
            // not Find
            if (OpenList.Count == 0)
            {
                noPath = true;
            }
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        //현재 클로즈리스트 가는 길에 그려지는 함수입니다. 
        if (FinalTileList.Count != 0)
        {
            for (int i = 0; i < FinalTileList.Count - 1; i++)
            {
                Gizmos.DrawLine(new Vector2(FinalTileList[i].x, FinalTileList[i].y), new Vector2(FinalTileList[i + 1].x, FinalTileList[i + 1].y));
            }
        }
    }
}

[System.Serializable]
public class Tile
{
    public Tile(bool _isWall, int _x, int _y) {
        isWall = _isWall;
        x = _x;
        y = _y;
    }

    public bool isWall;
    public Tile ParentTile;

    //g는 시작부터 이동 값, h는 여기부터 목표까지 값
    public int x, y, g, h;

    public int F { get { return g + h; } }
}


