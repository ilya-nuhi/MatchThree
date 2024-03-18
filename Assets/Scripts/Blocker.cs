using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Scaler))]
public class Blocker : MonoBehaviour
{

    // x and y index used for determining position in the Board's array
    public int xIndex;
    public int yIndex;
    public Board board;

    public float delayTime = 0.5f;
    public float scaleTime = 0.3f;

    // initialze the Blockers's array index and cache a reference to the Board
    public void Init(int x, int y, Board board)
    {
        if (board == null)
            return;

        xIndex = x;
        yIndex = y;
        board.allBlockers[x, y] = this;
        gameObject.GetComponent<Scaler>().ScaleUp();
    }
    

    public void SelfDestruct()
    {
        StartCoroutine(SelfDestructRoutine());
    }

    IEnumerator SelfDestructRoutine()
    {
        gameObject.GetComponent<Scaler>().ScaleDown();
        yield return new WaitForSeconds(delayTime);
        Destroy(gameObject, delayTime);

    }


}
