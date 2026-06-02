
using UnityEngine;

public class GlobalEventSystem : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
