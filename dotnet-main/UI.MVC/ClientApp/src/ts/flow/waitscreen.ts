let flowSessionIdElement = document.getElementById('flowsessionid') as HTMLSpanElement;
let flowSessionIdValue = flowSessionIdElement.textContent || flowSessionIdElement.innerText; 

let nextButtonWaitScreen = document.querySelector("#nextButtonWaitScreen") as HTMLButtonElement; 

nextButtonWaitScreen.addEventListener("click", function() {
    console.log("button clicked");
    window.location.href = `https://phygital.programmersinparis.net/FlowStep/Index/${flowSessionIdValue}`; 
});
