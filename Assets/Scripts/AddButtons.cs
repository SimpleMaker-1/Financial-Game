using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddButtons : MonoBehaviour
{
    [SerializeField]
    private Transform _PuzzleField;

    [SerializeField]
    private GameObject Button;

    private void Awake()
    {
        for (int i = 0; i < 20; i++) {
            GameObject btn = Instantiate(Button);
            btn.name = "" + i;
            btn.transform.SetParent(_PuzzleField , false);
        }
    }
}
