
using UnityEngine;
using UnityEngine.UI;


public class KeyDownToClickButton : MonoBehaviour
{
    public KeyCode _Key;
    private Button _button;

    // Start is called before the first frame update
    void Awake()
    {
        _button = GetComponent<Button>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(_Key))
        {
            _button.onClick.Invoke();
            Debug.Log("button clicked");
        }
    }

    public void OnButtonPress(Button button)
    {
        Debug.Log($"button {button} clicked");
    }
}
