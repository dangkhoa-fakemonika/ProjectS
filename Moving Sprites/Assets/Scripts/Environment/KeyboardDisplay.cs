using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardDisplay : MonoBehaviour
{
    // Start is called before the first frame update

    public Image leftKey;
    public Image rightKey;
    public Image spaceKey;
    public Image controlKey;
    public Image shiftKey;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if (Input.GetKey(KeyCode.A))
            leftKey.color = Color.red;
        else leftKey.color = Color.green;
        if (Input.GetKey(KeyCode.D))
            rightKey.color = Color.red;
        else rightKey.color = Color.green;
        if (Input.GetKey(KeyCode.Space))
            spaceKey.color = Color.red;
        else spaceKey.color = Color.green;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            controlKey.color = Color.red;
        else controlKey.color = Color.green;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            shiftKey.color = Color.red;
        else shiftKey.color = Color.green;
        
    }
}
