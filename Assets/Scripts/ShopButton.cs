using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    private Button button;

    // Start is called before the first frame update
    private void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = .1f;
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {

    }
}
