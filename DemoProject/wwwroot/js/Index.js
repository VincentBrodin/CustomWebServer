document.addEventListener("DOMContentLoaded", async () => {
    Bind("folder", "i-folder");
    Bind("file", "i-file");
})

async function FetchIcon(iconName) {
    const response = await fetch(`/wwwroot/icons/${iconName}.svg`);
    if (!response.ok) {
        return null;
    }
    const text = await response.text();
    return text;
}

async function Bind(iconName, iconClass) {
    const icon = await FetchIcon(iconName);
    const iconElements = document.getElementsByClassName(iconClass)

    for (let i = 0; i < iconElements.length; i++) {
        iconElements[i].innerHTML = icon;
    }
}
