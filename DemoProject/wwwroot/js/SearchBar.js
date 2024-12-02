document.addEventListener("DOMContentLoaded", async () => {
    const searchResults = document.getElementById("searchResults");
    const searchForm = document.getElementById("searchForm");

    const results = await Search();
    searchResults.innerHTML = "";
    if (results && Array.isArray(results)) {
        results.forEach(result => {
            console.log(result)
            const option = document.createElement("option");
            option.value = result.path;
            option.text = result.title;
            searchResults.appendChild(option);
        });
    }

    searchForm.addEventListener("submit", (e) => {
        e.preventDefault();
        window.location.href = `/${searchBar.value}`;
    })
});

async function Search() {
    try {
        const response = await fetch(`/search`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
        });

        const data = await response.json();
        const results = data?.results || null;
        return results;
    } catch (error) {
        console.error("Fetch error:", error);
        return null;
    }
}
