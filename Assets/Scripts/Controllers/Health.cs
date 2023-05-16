using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 100;
    public int armor = 0;

    public void Damage(int rawAmmount)
    {
        Debug.Log("Hit!");
        if (armor <= 50 && armor > 25)
        {
            health -= rawAmmount / 4;
        }
        else if (armor <= 25 && armor > 0)
        {
            health -= rawAmmount / 2;
        }
        else if (armor == 0)
        {
            health -= rawAmmount;
        }
            
        Debug.Log(health);

        armor -= rawAmmount / 2;
        armor = armor < 0 ? 0 : armor;
        health = health < 0 ? 0 : health;
    }
}
