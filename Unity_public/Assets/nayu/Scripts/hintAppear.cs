using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class hintAppear : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);

        NetworkManager.instance.isShowHint.Subscribe(x =>
        {
            activateHint();
        }).AddTo(this);
    }

    private void activateHint()
    {
        Debug.Log("hint activated");
        gameObject.SetActive(true);
    }
}
