using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomArrangeShips : MonoBehaviour
{
    private GameController GC;
    public Ship[] ships;
    private int orientation;

    private void Update()
    {

    }

    public void DisableAllShips()
    {
        foreach (Ship ship in ships)
        {
            BoxCollider2D col = ship.GetComponent<BoxCollider2D>();
            col.enabled = false;
        }
    }

    public void RandomArrangeShip()
    {
        foreach (Ship ship in ships)
        {
            float randX = Random.Range(0f, 9f);
            float randY = Random.Range(0f, 9f);
            orientation = Mathf.RoundToInt(randX * randY) % 2;
            int length = ship.GetLenght();
            //Debug.Log("L: " + length);

            while (!ship.OnValidPosition(randX, randY, orientation))
            {
                orientation = Mathf.RoundToInt(randX * randY) % 2;

                if (orientation == 1)
                {
                    randX = Random.Range(0f, 9f - length);
                    randY = Random.Range(0f, 9f);
                }
                else
                {
                    randX = Random.Range(0f, 9f);
                    randY = Random.Range(0f, 9f - length);
                }
            }
            ship.ActivateShip();
            ship.SetShipByRandom(randX, randY, orientation);
        }
    }
}
