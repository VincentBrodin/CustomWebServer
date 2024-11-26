
const form = document.getElementById("root_form");
const item = document.getElementById("item");

form.addEventListener("submit", (e) => {
    e.preventDefault();
    window.location.href = "/wwwroot/"+item.value; 
})