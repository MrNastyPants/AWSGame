const button = document.getElementById("select-item");

// Function to check if an item exists on the current Amazon page
function checkItemDetection() {
    chrome.tabs.query({ active: true, currentWindow: true }, (tabs) => {
        const tab = tabs[0];
        chrome.scripting.executeScript(
            {
                target: { tabId: tab.id },
                func: () => {
                    const productName = document.querySelector('#productTitle')?.textContent.trim();
                    return Boolean(productName); // Return true if a product name is found
                },
            },
            (results) => {
                const itemDetected = results[0]?.result;

                // Update button color based on detection result
                if (itemDetected) {
                    button.style.backgroundColor = "#4caf50"; // Green
                    button.textContent = "Item Detected! Select Item";
                } else {
                    button.style.backgroundColor = "#f44336"; // Red
                    button.textContent = "No Item Found";
                }
            }
        );
    });
}

// Add a click event listener to the button
button.addEventListener("click", () => {
    chrome.runtime.sendMessage({ action: "selectItem" });
});

// Check item detection when the popup is opened
checkItemDetection();
