using UnityEngine;

public class TestFramework : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            GameEvents.PlayerDeath();
        }


        if(Input.GetKeyDown(KeyCode.L))
        {
            GameEvents.PlayerVictory();
        }
    }
}