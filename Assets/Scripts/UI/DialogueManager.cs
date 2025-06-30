using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueText;
    public Button nextButton;
    public Button closeButton;

    [Header("Typing Effect")]
    public float typingSpeed = 0.05f;
    public bool useTypingEffect = true;

    private string[] currentDialogue;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Hide dialogue panel initially
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // Setup button listeners
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(NextLine);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseDialogue);
        }
    }

    private void Update()
    {
        // Allow player to advance dialogue with space or enter
        if (dialoguePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                if (isTyping)
                {
                    // Skip typing animation
                    SkipTyping();
                }
                else
                {
                    // Advance to next line
                    NextLine();
                }
            }

            // Close dialogue with ESC
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseDialogue();
            }
        }
    }

    public void StartDialogue(string npcName, string[] dialogueLines)
    {
        if (dialogueLines == null || dialogueLines.Length == 0) return;

        currentDialogue = dialogueLines;
        currentLineIndex = 0;

        // Show dialogue panel
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        // Set NPC name
        if (npcNameText != null)
        {
            npcNameText.text = npcName;
        }

        // Display first line
        DisplayCurrentLine();

        // Pause game time (optional)
        // Time.timeScale = 0f;
    }

    private void DisplayCurrentLine()
    {
        if (currentDialogue == null || currentLineIndex >= currentDialogue.Length) return;

        string line = currentDialogue[currentLineIndex];

        if (useTypingEffect)
        {
            StartTyping(line);
        }
        else
        {
            if (dialogueText != null)
            {
                dialogueText.text = line;
            }
        }

        // Update button visibility
        UpdateButtonStates();
    }

    private void StartTyping(string line)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;

        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        foreach (char character in line)
        {
            if (dialogueText != null)
            {
                dialogueText.text += character;
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        UpdateButtonStates();
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;

        if (dialogueText != null && currentDialogue != null && currentLineIndex < currentDialogue.Length)
        {
            dialogueText.text = currentDialogue[currentLineIndex];
        }

        UpdateButtonStates();
    }

    public void NextLine()
    {
        if (isTyping)
        {
            SkipTyping();
            return;
        }

        currentLineIndex++;

        if (currentLineIndex < currentDialogue.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            CloseDialogue();
        }
    }

    public void CloseDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        currentDialogue = null;
        currentLineIndex = 0;

        // Resume game time
        // Time.timeScale = 1f;
    }

    private void UpdateButtonStates()
    {
        if (nextButton != null)
        {
            // Show next button if not typing and there are more lines
            bool showNext = !isTyping && (currentLineIndex < currentDialogue.Length - 1);
            nextButton.gameObject.SetActive(showNext);
        }

        if (closeButton != null)
        {
            // Show close button if not typing and this is the last line
            bool showClose = !isTyping && (currentLineIndex >= currentDialogue.Length - 1);
            closeButton.gameObject.SetActive(showClose);
        }
    }

    public bool IsDialogueActive()
    {
        return dialoguePanel != null && dialoguePanel.activeSelf;
    }
}
