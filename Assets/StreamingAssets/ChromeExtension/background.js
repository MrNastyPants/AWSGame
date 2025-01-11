chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
    if (message.action === "selectItem") {
        chrome.tabs.query({ active: true, currentWindow: true }, (tabs) => {
            const tab = tabs[0];
            chrome.scripting.executeScript(
                {
                    target: { tabId: tab.id },
                    func: () => {
                        // Extract product name and price
                        let productName = document.querySelector('#productTitle')?.textContent.trim();
                        let priceWhole = document.querySelector('.a-price-whole')?.textContent.trim();
                        let priceDecimal = document.querySelector('.a-price-fraction')?.textContent.trim();

                        let price = priceWhole && priceDecimal ? `${priceWhole}${priceDecimal}` : null;

                        if (productName && price) {
                            let productData = `Product Name: ${productName}\nPrice: ${price}`;
                            navigator.clipboard.writeText(productData)
                                .then(() => {
                                    alert('You have purchased this item in-game!');
                                })
                                .catch((err) => {
                                    console.error('Failed to copy to clipboard:', err);
                                    alert('Failed to complete in-game transaction.');
                                });
                        } else {
                            alert('Failed to find product name or price on this page.');
                        }
                    }
                },
                () => {
                    if (chrome.runtime.lastError) {
                        console.error('Error injecting script:', chrome.runtime.lastError.message);
                    } else {
                        console.log('Content script executed.');
                    }
                }
            );
        });
    }
});
