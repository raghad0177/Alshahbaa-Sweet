document.addEventListener("DOMContentLoaded", function () {
    // Check if the user is logged in when the page loads
    const userId = localStorage.getItem("userId");

    if (userId) {
        // Fetch user details to get the name
        fetchUserDetails(userId);
    } else {
        // If no user is logged in, show the registration tab and hide the profile tab
        document.getElementById("registerTab").style.display = "block";
        document.getElementById("profileTab").style.display = "none";
    }
});

// Function to fetch user details and update the UI
function fetchUserDetails(userId) {
    fetch(`https://localhost:7011/api/Users/GetUserById/${userId}`)
        .then(response => response.json())
        .then(user => {
            if (user && user.name) {
                // Update the profile tab with the user's name
                document.getElementById("userNameLink").textContent = user.name;

                // Show the profile tab and hide the registration tab
                document.getElementById("profileTab").style.display = "block";
                document.getElementById("registerTab").style.display = "none";
            } else {
                console.error("Failed to fetch user details or user name is missing.");
            }
        })
        .catch(error => {
            console.error("Error fetching user details:", error);
        });
}





document.addEventListener("DOMContentLoaded", function () {
    // Function to update the navbar based on the user login status
    function updateNavbar() {
        const userId = localStorage.getItem("userId");
        const userName = localStorage.getItem("userName"); // Assuming you store the user's name in localStorage

        const userProfileTab = document.getElementById("userProfileTab");
        const userProfileLink = document.getElementById("userProfileLink");
        const registerTab = document.getElementById("registerTab");

        if (userId && userName) {
            // User is logged in
            userProfileLink.textContent = userName; // Change "الصفحة الشخصية" to the user's name
            userProfileTab.style.display = "block"; // Show the user profile tab
            registerTab.style.display = "none"; // Hide the registration tab
        } else {
            // User is not logged in
            userProfileTab.style.display = "none"; // Hide the user profile tab
            registerTab.style.display = "block"; // Show the registration tab
        }
    }

    // Call the function to update the navbar on page load
    updateNavbar();
});


document.addEventListener("DOMContentLoaded", function () {
    // Function to update the cart count
    function updateCartCount() {
        const cart = JSON.parse(localStorage.getItem("cart")) || [];
        const totalItems = cart.reduce((total, item) => total + item.quantity, 0);

        const cartCountLabel = document.querySelector(".module-label");
        if (cartCountLabel) {
            cartCountLabel.textContent = totalItems;
        }
    }

    // Redirect to cart page when the cart icon is clicked
    const cartIcon = document.querySelector(".module-icon.cart-icon");
    if (cartIcon) {
        cartIcon.addEventListener("click", () => {
            window.location.href = "/customer/Cart"; // Updated URL     
        });
    }

    // Load visible products and update cart count on page load
    loadVisibleProducts();
    updateCartCount();
});
