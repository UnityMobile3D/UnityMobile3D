using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public static class InitializeInput
{
    // �� �ε� ���� ���� ���� ����
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        // 1) ������/PC���� ���� ��ġ ����̽��� ������ ����
        if (Touchscreen.current == null)
            InputSystem.AddDevice<Touchscreen>(); 

        // 2) EnhancedTouch ���������� + ���콺����ġ �ù� ON
        EnhancedTouchSupport.Enable();

#if UNITY_EDITOR || UNITY_STANDALONE
        TouchSimulation.Enable();        // ������/PC: ���콺=�̱���ġ
#endif
    }
}