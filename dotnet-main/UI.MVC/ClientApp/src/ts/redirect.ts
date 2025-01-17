import {flowid} from "./startsession";

let timeoutId: ReturnType<typeof setTimeout>;


function resetTimer() {
    clearTimeout(timeoutId);
    timeoutId = setTimeout(redirectToBaseURL, 100000);
}


function redirectToBaseURL() {
    var projectThemeElement = document.getElementById('projectTheme');
    if (projectThemeElement !== null) {
        var projectId = projectThemeElement.textContent; // Nu veilig om textContent te benaderen
  console.log(projectId)
        window.location.href = `/Home/Startup/${projectId}`;
    } else {
        console.error('Project theme element not found');
    }
}



document.addEventListener("mousemove", resetTimer);


document.addEventListener("keypress", resetTimer);

document.addEventListener("scroll", resetTimer);


window.addEventListener("load", resetTimer);


const backButton = document.getElementById("backButton");


if (backButton) {
    backButton.addEventListener("click", () => {
        redirectToBaseURL();
    });
}


