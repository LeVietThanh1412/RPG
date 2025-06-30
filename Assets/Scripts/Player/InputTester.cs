using UnityEngine;

/// <summary>
/// Simple input testing script
/// Shows input values in console for debugging
/// </summary>
public class InputTester : MonoBehaviour
{
    [Header("Input Test Settings")]
    public bool logInputToConsole = true;
    public float logInterval = 1f; // Log every 1 second

    private float lastLogTime;

    void Update()
    {
        if (!logInputToConsole) return;

        // Test input every interval
        if (Time.time - lastLogTime >= logInterval)
        {
            TestInputs();
            lastLogTime = Time.time;
        }
    }

    void TestInputs()
    {
        // Test movement inputs
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            Debug.Log($"🎮 Input Test - Horizontal: {horizontal}, Vertical: {vertical}");
        }

        // Test specific keys
        if (Input.GetKey(KeyCode.W)) Debug.Log("🎮 W key pressed");
        if (Input.GetKey(KeyCode.A)) Debug.Log("🎮 A key pressed");
        if (Input.GetKey(KeyCode.S)) Debug.Log("🎮 S key pressed");
        if (Input.GetKey(KeyCode.D)) Debug.Log("🎮 D key pressed");

        // Test run key
        if (Input.GetKey(KeyCode.LeftShift)) Debug.Log("🏃 Shift (Run) key pressed");
    }

    [ContextMenu("Test Input Now")]
    public void TestInputNow()
    {
        Debug.Log("🎮 Manual Input Test:");
        TestInputs();
    }
}
