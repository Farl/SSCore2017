using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using TMPro;
using SS;

public class AsyncAwaitExample : MonoBehaviour
{
    [SerializeField] private Button loadDataButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button exceptionButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image progressBar;

    private bool isLoading = false;
    private bool isException = false;
    private System.Threading.CancellationTokenSource cancellationTokenSource;

    private void Start()
    {
        // Setup button listeners
        loadDataButton.onClick.AddListener(HandleLoadDataClick);
        cancelButton.onClick.AddListener(HandleCancelClick);
        exceptionButton.onClick.AddListener(() => isException = true);

        // Initially disable cancel button
        cancelButton.interactable = false;
        exceptionButton.interactable = false;
    }

    // Button click handler - this is your entry point from UI
    private void HandleLoadDataClick()
    {
        // Prevent multiple simultaneous operations
        if (isLoading) return;

        // Start the async operation
        ProcessAsyncOperation().Forget(); // Extension method defined below
    }

    private void HandleCancelClick()
    {
        cancellationTokenSource?.Cancel();
    }

    // Main async method
    private async Task ProcessAsyncOperation()
    {
        try
        {
            isLoading = true;
            loadDataButton.interactable = false;
            cancelButton.interactable = true;
            exceptionButton.interactable = true;

            // Create new cancellation token source
            cancellationTokenSource = new System.Threading.CancellationTokenSource();

            // Update UI to show loading state
            UpdateUI("Loading...", 0f);

            // Perform the async operation
            await LoadDataWithProgress(cancellationTokenSource.Token);

            // Update UI on success
            UpdateUI("Operation completed successfully!", 1f);
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation
            UpdateUI("Operation cancelled.", 0f);
        }
        catch (Exception ex)
        {
            // Handle any errors
            Debug.LogError($"Error during async operation: {ex}");
            UpdateUI($"Error: {ex.Message}", 0f);
        }
        finally
        {
            // Cleanup
            isLoading = false;
            isException = false;
            loadDataButton.interactable = true;
            cancelButton.interactable = false;
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }
    }

    // Example async operation with progress
    private async Task LoadDataWithProgress(System.Threading.CancellationToken cancellationToken)
    {
        for (int i = 0; i <= 100; i += 10)
        {
            // Check cancellation
            cancellationToken.ThrowIfCancellationRequested();

            // Simulate exception
            if (isException)
            {
                isException = false;
                throw new Exception("Simulated exception!");
            }

            // Simulate work
            await Task.Delay(500, cancellationToken);

            // Update progress
            float progress = i / 100f;
            UpdateUI($"Loading... {i}%", progress);
        }
    }

    // Helper method to update UI elements
    private void UpdateUI(string message, float progress)
    {
        // Ensure we're on the main thread
        if (!UnityThread.isMainThread)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() => UpdateUI(message, progress));
            return;
        }

        statusText.text = message;
        progressBar.fillAmount = progress;
    }
}
